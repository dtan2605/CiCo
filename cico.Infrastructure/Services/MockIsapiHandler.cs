using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using cico.Application.Interfaces;
using cico.Domain.Enums;

namespace cico.Infrastructure.Services;

public class MockIsapiHandler : HttpMessageHandler
{
    private static readonly string Realm = "Hikvision";
    private static readonly string Password = "admin";
    private static readonly Dictionary<string, string> ValidUsers = new()
    {
        ["admin"] = Password
    };

    private readonly ConcurrentDictionary<string, string> _nonces = new();

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authHeader = request.Headers.Authorization;

        if (authHeader == null || !IsValidDigest(authHeader.ToString(), request))
        {
            var nonce = Guid.NewGuid().ToString("N");
            _nonces[nonce] = nonce;

            var challenge = $"Digest realm=\"{Realm}\", nonce=\"{nonce}\", " +
                $"opaque=\"{nonce}\", qop=\"auth\", stale=FALSE";

            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.Headers.WwwAuthenticate.Add(
                new("Digest", challenge));
            return Task.FromResult(response);
        }

        var path = request.RequestUri?.AbsolutePath ?? "";
        var query = request.RequestUri?.Query ?? "";

        return Task.FromResult(path switch
        {
            "/ISAPI/System/status" => HandleStatus(),
            "/ISAPI/AccessControl/AcsEvent" => HandleAcsEvent(query),
            "/ISAPI/Intelligent/FDLib/FaceDataRecord" => HandleEnrollFace(request),
            "/ISAPI/Intelligent/FDLib/FaceDataRecordDelete" => HandleDeleteFace(request),
            _ => new HttpResponseMessage(HttpStatusCode.NotFound)
        });
    }

    private static HttpResponseMessage HandleStatus()
    {
        var xml = new XElement("DeviceStatus",
            new XElement("statusString", "OK"),
            new XElement("deviceID", "MOCK-DEVICE-001"),
            new XElement("deviceName", "Mock ISAPI Device")
        ).ToString();

        return XmlResponse(xml);
    }

    private static HttpResponseMessage HandleAcsEvent(string query)
    {
        var match = Regex.Match(query, @"startTime=(\d{14})&endTime=(\d{14})");
        if (!match.Success) return XmlResponse("<AcsEventList><AcsEvent></AcsEvent></AcsEventList>");

        var from = ParseHikTime(match.Groups[1].Value);
        var to = ParseHikTime(match.Groups[2].Value);

        var records = GenerateMockAcsEvents(from, to);

        var xml = new XElement("AcsEventList",
            new XAttribute("version", "2.0"),
            new XElement("searchResultPosition", 0),
            new XElement("numOfMatches", records.Count),
            records.Select(r => new XElement("AcsEvent",
                new XElement("employeeNoString", r.EmployeeCode),
                new XElement("time", r.ScanTime.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XElement("attendanceMethod", r.Method == AttendanceMethod.Fingerprint ? "2" : "1"),
                new XElement("status", r.IsSuccess ? "0" : "1"),
                new XElement("deviceSerial", "MOCK-DEVICE-001")
            ))
        );

        return XmlResponse(xml.ToString());
    }

    private static HttpResponseMessage HandleEnrollFace(HttpRequestMessage request)
    {
        return XmlResponse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
            "<FaceDataRecord><statusCode>1</statusCode><statusString>OK</statusString></FaceDataRecord>");
    }

    private static HttpResponseMessage HandleDeleteFace(HttpRequestMessage request)
    {
        return XmlResponse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
            "<FaceDataRecordDel><statusCode>1</statusCode><statusString>OK</statusString></FaceDataRecordDel>");
    }

    private bool IsValidDigest(string authValue, HttpRequestMessage request)
    {
        try
        {
            var dict = ParseDigest(authValue);
            if (!dict.TryGetValue("username", out var username) ||
                !dict.TryGetValue("realm", out var realm) ||
                !dict.TryGetValue("nonce", out var nonce) ||
                !dict.TryGetValue("uri", out var uri) ||
                !dict.TryGetValue("response", out var response) ||
                !ValidUsers.ContainsKey(username))
                return false;

            if (!_nonces.ContainsKey(nonce)) return false;

            var qop = dict.GetValueOrDefault("qop", "auth");
            var nc = dict.GetValueOrDefault("nc", "00000001");
            var cnonce = dict.GetValueOrDefault("cnonce", "");

            var password = ValidUsers[username];
            var method = request.Method.Method;

            var ha1 = Md5($"{username}:{realm}:{password}");
            var ha2 = Md5($"{method}:{uri}");
            var expected = Md5($"{ha1}:{nonce}:{nc}:{cnonce}:{qop}:{ha2}");

            return string.Equals(response, expected, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static Dictionary<string, string> ParseDigest(string authValue)
    {
        var dict = new Dictionary<string, string>();

        var afterDigest = authValue["Digest ".Length..];
        var parts = afterDigest.Split(',');

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

    private static List<HikAttendanceRecord> GenerateMockAcsEvents(DateTime from, DateTime to)
    {
        var employees = new[] { "ADM-001", "MGR-001", "EMP-001" };
        var rng = new Random();
        var records = new List<HikAttendanceRecord>();

        foreach (var emp in employees)
        {
            for (var day = from.Date; day <= to.Date; day = day.AddDays(1))
            {
                if (day > DateTime.Today) break;

                var morning = day.AddHours(7).AddMinutes(rng.Next(0, 90));
                if (morning >= from && morning <= to)
                {
                    records.Add(new HikAttendanceRecord
                    {
                        EmployeeCode = emp,
                        ScanTime = morning,
                        Method = rng.Next(2) == 0
                            ? AttendanceMethod.FaceRecognition
                            : AttendanceMethod.Fingerprint,
                        IsSuccess = true
                    });
                }

                var afternoon = day.AddHours(17).AddMinutes(rng.Next(0, 60));
                if (afternoon >= from && afternoon <= to)
                {
                    records.Add(new HikAttendanceRecord
                    {
                        EmployeeCode = emp,
                        ScanTime = afternoon,
                        Method = rng.Next(2) == 0
                            ? AttendanceMethod.FaceRecognition
                            : AttendanceMethod.Fingerprint,
                        IsSuccess = rng.NextDouble() > 0.1
                    });
                }
            }
        }

        return records.OrderBy(r => r.ScanTime).ToList();
    }

    private static DateTime ParseHikTime(string hikFormat)
    {
        return DateTime.ParseExact(hikFormat, "yyyyMMddHHmmss",
            CultureInfo.InvariantCulture);
    }

    private static HttpResponseMessage XmlResponse(string xml)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(xml, Encoding.UTF8, "application/xml")
        };
    }

    private static string Md5(string input)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
