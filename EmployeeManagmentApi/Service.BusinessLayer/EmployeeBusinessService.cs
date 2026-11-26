using EmployeeManagementApi.Models;
using EmployeeManagementApi.Service.DataLayer;

namespace EmployeeManagementApi.Service.BusinessLayer;

public sealed class EmployeeBusinessService
{
    private readonly EmployeeDataRepository _dataRepository;

    public EmployeeBusinessService(EmployeeDataRepository dataRepository)
    {
        _dataRepository = dataRepository;
    }

    public Task<List<EmployeeDto>> GetEmployeesAsync()
    {
        return _dataRepository.GetEmployeesAsync();
    }

    public Task<EmployeeDto?> SaveEmployeeAsync(EmployeeSaveRequest request, int? routeId)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            throw new ArgumentException("First Name and Last Name are required.");
        }

        return _dataRepository.SaveEmployeeAsync(request, routeId);
    }

    public Task<bool> DeleteEmployeeAsync(int id)
    {
        return _dataRepository.DeleteEmployeeAsync(id);
    }
}