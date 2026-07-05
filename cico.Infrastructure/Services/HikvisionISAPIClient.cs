using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using cico.Application.Interfaces;
using cico.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace cico.Infrastructure.Services;

public class HikvisionISAPIClient : IDeviceClientService
{
    private readonly ILogger<HikvisionISAPIClient> _logger;
    private readonly HttpClient? _httpClient;

    public HikvisionISAPIClient(ILogger<HikvisionISAPIClient> logger)
    {
        _logger = logger;
    }

    public HikvisionISAPIClient(
        ILogger<HikvisionISAPIClient> logger,
        HttpMessageHandler handler)
    {
        _logger = logger;
        _httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    public async Task<DeviceConnectionResult> TestConnectionAsync(
        string ip, int port, string username, string password)
    {
        var url = $"http://{ip}:{port}/ISAPI/System/status";
        var (success, body) = await DigestGetAsync(url, username, password);

        if (!success || body == null)
            return new DeviceConnectionResult
            {
                Success = false,
                Message = "Connection failed or no response",
                Status = DeviceStatus.Offline
            };

        try
        {
            var doc = XDocument.Parse(body);
            var status = doc.Descendants("statusString").FirstOrDefault()?.Value;
            var isOk = string.Equals(status, "OK", StringComparison.OrdinalIgnoreCase);

            return new DeviceConnectionResult
            {
                Success = isOk,
                Message = isOk ? "Device is online" : $"Unexpected status: {status}",
                Status = isOk ? DeviceStatus.Online : DeviceStatus.Offline
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse ISAPI status XML from {Url}", url);
            return new DeviceConnectionResult
            {
                Success = false,
                Message = $"Parse error: {ex.Message}",
                Status = DeviceStatus.Offline
            };
        }
    }

    public async Task<List<HikAttendanceRecord>> SearchAttendanceRecordsAsync(
        string ip, int port, string username, string password,
        DateTime from, DateTime to)
    {
        var urlBase = $"http://{ip}:{port}/ISAPI/AccessControl/AcsEvent";
        var query = $"?searchResultPosition=0&maxResults=100" +
            $"&major=0&minor=0" +
            $"&startTime={from:yyyyMMddHHmmss}&endTime={to:yyyyMMddHHmmss}";

        var (success, body) = await DigestGetAsync(urlBase + query, username, password);

        if (!success || body == null)
        {
            _logger.LogWarning("Search attendance returned {Success} from {Url}", success, urlBase);
            return [];
        }

        return ParseAcsEventXml(body);
    }

    public async Task<bool> EnrollFaceAsync(
        string ip, int port, string username, string password,
        string employeeCode, byte[] faceImage)
    {
        var url = $"http://{ip}:{port}/ISAPI/Intelligent/FDLib/FaceDataRecord";

        var xml = new XElement("FaceDataRecord",
            new XElement("employeeNo", employeeCode),
            new XElement("faceImageType", "JPEG"),
            new XElement("faceImage", Convert.ToBase64String(faceImage))
        ).ToString();

        var (success, _) = await DigestPostAsync(url, username, password, xml);
        return success;
    }

    public async Task<bool> DeleteFaceAsync(
        string ip, int port, string username, string password,
        string employeeCode)
    {
        var url = $"http://{ip}:{port}/ISAPI/Intelligent/FDLib/FaceDataRecordDelete";

        var xml = new XElement("FaceDataRecordDel",
            new XElement("employeeNo", employeeCode)
        ).ToString();

        var (success, _) = await DigestPostAsync(url, username, password, xml);
        return success;
    }

    private static List<HikAttendanceRecord> ParseAcsEventXml(string xml)
    {
        var records = new List<HikAttendanceRecord>();

        try
        {
            var doc = XDocument.Parse(xml);
            var entries = doc.Descendants("AcsEvent");

            foreach (var entry in entries)
            {
                var employeeNo = entry.Element("employeeNoString")?.Value
                    ?? entry.Element("employeeNo")?.Value;
                if (string.IsNullOrWhiteSpace(employeeNo)) continue;

                var timeStr = entry.Element("time")?.Value;
                if (!DateTime.TryParseExact(timeStr, "yyyy-MM-ddTHH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var scanTime))
                    continue;

                var method = ParseMethod(entry.Element("attendanceMethod")?.Value);
                var isSuccess = entry.Element("status")?.Value == "1" ||
                    entry.Element("status")?.Value == "0"; // 0=success on Hikvision

                records.Add(new HikAttendanceRecord
                {
                    EmployeeCode = employeeNo,
                    ScanTime = scanTime,
                    Method = method,
                    IsSuccess = isSuccess
                });
            }
        }
        catch
        {
            return records;
        }

        return records;
    }

    private static AttendanceMethod ParseMethod(string? value)
    {
        return value switch
        {
            "1" => AttendanceMethod.FaceRecognition,
            "2" => AttendanceMethod.Fingerprint,
            "3" => AttendanceMethod.Manual,
            _ => AttendanceMethod.FaceRecognition
        };
    }

    private async Task<(bool Success, string? Body)> DigestGetAsync(
        string url, string username, string password)
    {
        var ownsClient = _httpClient == null;
        var client = ownsClient ? CreateDefaultClient() : _httpClient!;

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await client.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var challenge = response.Headers.WwwAuthenticate
                .FirstOrDefault()?.Parameter;

            if (challenge == null)
            {
                if (ownsClient) client.Dispose();
                return (false, null);
            }

            var authHeader = BuildDigestHeader("GET", url, challenge, username, password);
            request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("Authorization", authHeader);
            response = await client.SendAsync(request);
        }

        if (!response.IsSuccessStatusCode)
        {
            if (ownsClient) client.Dispose();
            return (false, null);
        }

        var body = await response.Content.ReadAsStringAsync();
        if (ownsClient) client.Dispose();
        return (true, body);
    }

    private async Task<(bool Success, string? Body)> DigestPostAsync(
        string url, string username, string password, string xmlContent)
    {
        var ownsClient = _httpClient == null;
        var client = ownsClient ? CreateDefaultClient() : _httpClient!;

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(xmlContent, Encoding.UTF8, "application/xml")
        };

        var response = await client.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var challenge = response.Headers.WwwAuthenticate
                .FirstOrDefault()?.Parameter;

            if (challenge == null)
            {
                if (ownsClient) client.Dispose();
                return (false, null);
            }

            var bodyContent = await response.Content.ReadAsStringAsync();
            var authHeader = BuildDigestHeader("POST", url, challenge, username, password);
            request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(xmlContent, Encoding.UTF8, "application/xml")
            };
            request.Headers.TryAddWithoutValidation("Authorization", authHeader);
            response = await client.SendAsync(request);
        }

        if (!response.IsSuccessStatusCode)
        {
            if (ownsClient) client.Dispose();
            return (false, null);
        }

        var body = await response.Content.ReadAsStringAsync();
        if (ownsClient) client.Dispose();
        return (true, body);
    }

    private static string BuildDigestHeader(
        string method, string uri, string challenge, string username, string password)
    {
        var parts = ParseDigestChallenge(challenge);

        var realm = parts["realm"];
        var nonce = parts["nonce"];
        var opaque = parts.GetValueOrDefault("opaque", "");
        var qop = parts.GetValueOrDefault("qop", "auth");

        var nc = "00000001";
        var cnonce = Guid.NewGuid().ToString("N")[..16];

        var ha1 = Md5Hash($"{username}:{realm}:{password}");
        var ha2 = Md5Hash($"{method}:{uri}");
        var response = Md5Hash($"{ha1}:{nonce}:{nc}:{cnonce}:{qop}:{ha2}");

        return $"Digest username=\"{username}\", realm=\"{realm}\", " +
            $"nonce=\"{nonce}\", uri=\"{uri}\", " +
            $"qop={qop}, nc={nc}, cnonce=\"{cnonce}\", " +
            $"response=\"{response}\", opaque=\"{opaque}\"";
    }

    private static Dictionary<string, string> ParseDigestChallenge(string challenge)
    {
        var dict = new Dictionary<string, string>();
        var parts = challenge.Split(',');

        foreach (var part in parts)
        {
            var eq = part.IndexOf('=');
            if (eq < 0) continue;

            var key = part[..eq].Trim();
            var value = part[(eq + 1)..].Trim().Trim('"');
            dict[key] = value;
        }

        return dict;
    }

    private static HttpClient CreateDefaultClient()
    {
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = false,
            PreAuthenticate = false
        };

        return new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    private static string Md5Hash(string input)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
