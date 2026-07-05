namespace cico.Application.DTOs.Employees;

public class EmployeeDto
{
    public Guid Id { get; set; }

    public string EmployeeCode { get; set; }
        = string.Empty;

    public string FullName { get; set; }
        = string.Empty;

    public string Email { get; set; }
        = string.Empty;

    public string PhoneNumber { get; set; }
        = string.Empty;

    public string DepartmentName { get; set; }
        = string.Empty;

    public string PositionName { get; set; }
        = string.Empty;

    public bool IsActive { get; set; }
}