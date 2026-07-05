namespace cico.Application.DTOs.Reports;

public class MonthlyReportDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalEmployees { get; set; }
    public List<DailySummaryDto> DailySummaries { get; set; } = new();
    public MonthlySummaryDto Summary { get; set; } = new();
}

public class MonthlySummaryDto
{
    public int TotalWorkingDays { get; set; }
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalLate { get; set; }
    public double AttendanceRate { get; set; }
}
