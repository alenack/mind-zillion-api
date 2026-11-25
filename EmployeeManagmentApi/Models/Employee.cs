namespace EmployeeManagementApi.Models;

public class Employee
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public string? Email { get; set; }
}