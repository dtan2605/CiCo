namespace cico.Application.Interfaces;

public class FingerprintVerifyResult
{
    public bool IsMatch { get; set; }
    public float Confidence { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? MatchedEmployeeId { get; set; }
}

public class FingerprintEnrollResult
{
    public bool Success { get; set; }
    public string Template { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public interface IFingerprintService
{
    Task<FingerprintVerifyResult> VerifyAsync(
        Guid employeeId,
        string scannedTemplate);

    Task<FingerprintEnrollResult> EnrollAsync(
        Guid employeeId,
        byte[] imageData);

    Task<FingerprintVerifyResult> IdentifyAsync(
        string scannedTemplate);

    Task<bool> TestConnectionAsync(string deviceIp);
}
