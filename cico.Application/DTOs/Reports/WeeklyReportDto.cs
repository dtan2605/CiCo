namespace cico.Application.DTOs.Reports;

public class WeeklyReportDto
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public List<DailySummaryDto> DailySummaries { get; set; } = new();
}

public class DailySummaryDto
{
    public DateOnly Date { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
}
