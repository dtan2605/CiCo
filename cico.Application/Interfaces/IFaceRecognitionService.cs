namespace cico.Application.Interfaces;

public class FaceVerifyResult
{
    public bool IsMatch { get; set; }
    public float Confidence { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? MatchedEmployeeId { get; set; }
}

public class FaceEnrollResult
{
    public bool Success { get; set; }
    public string Template { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public interface IFaceRecognitionService
{
    Task<FaceVerifyResult> VerifyAsync(
        Guid employeeId,
        string scannedTemplate);

    Task<FaceEnrollResult> EnrollAsync(
        Guid employeeId,
        byte[] imageData);

    Task<FaceVerifyResult> IdentifyAsync(
        string scannedTemplate);

    Task<bool> TestConnectionAsync(string deviceIp);
}
