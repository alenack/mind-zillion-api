using EmployeeManagementApi.Models;
using Npgsql;

namespace EmployeeManagementApi.Service.DataLayer;

public sealed class EmployeeDataRepository
{
    private readonly string _connectionString;

    public EmployeeDataRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString(EmployeeConstants.DefaultConnectionStringName)
            ?? throw new InvalidOperationException("Missing DefaultConnection string.");
    }

    public async Task<List<EmployeeDto>> GetEmployeesAsync()
    {
        var employees = new List<EmployeeDto>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(EmployeeConstants.SelectAllEmployeesSql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            employees.Add(MapReaderToEmployee(reader));
        }

        return employees;
    }

    public async Task<EmployeeDto?> SaveEmployeeAsync(EmployeeSaveRequest request, int? routeId)
    {
        int? targetId = routeId ?? request.EmployeeId;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        if (targetId.HasValue)
        {
            await using var updateCmd = new NpgsqlCommand(EmployeeConstants.UpdateEmployeeSql, connection);
            updateCmd.Parameters.AddWithValue(EmployeeConstants.Params.EmployeeId, targetId.Value);
            AddEmployeeParameters(updateCmd, request);

            await using var reader = await updateCmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return MapReaderToEmployee(reader);
        }

        await using var insertCmd = new NpgsqlCommand(EmployeeConstants.InsertEmployeeSql, connection);
        AddEmployeeParameters(insertCmd, request);

        await using var insertReader = await insertCmd.ExecuteReaderAsync();
        await insertReader.ReadAsync();
        return MapReaderToEmployee(insertReader);
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(EmployeeConstants.DeleteEmployeeSql, connection);
        command.Parameters.AddWithValue(EmployeeConstants.Params.EmployeeId, id);

        int affectedRows = await command.ExecuteNonQueryAsync();
        return affectedRows > 0;
    }

    private static void AddEmployeeParameters(NpgsqlCommand command, EmployeeSaveRequest request)
    {
        command.Parameters.AddWithValue(EmployeeConstants.Params.FirstName, request.FirstName.Trim());
        command.Parameters.AddWithValue(
            EmployeeConstants.Params.MiddleName,
            string.IsNullOrWhiteSpace(request.MiddleName) ? DBNull.Value : request.MiddleName.Trim()
        );
        command.Parameters.AddWithValue(EmployeeConstants.Params.LastName, request.LastName.Trim());
        command.Parameters.AddWithValue(
            EmployeeConstants.Params.MobileNumber,
            string.IsNullOrWhiteSpace(request.MobileNumber) ? DBNull.Value : request.MobileNumber.Trim()
        );
        command.Parameters.AddWithValue(
            EmployeeConstants.Params.Email,
            string.IsNullOrWhiteSpace(request.Email) ? DBNull.Value : request.Email.Trim()
        );
    }

    private static EmployeeDto MapReaderToEmployee(NpgsqlDataReader reader)
    {
        return new EmployeeDto
        {
            EmployeeId = reader.GetInt32(reader.GetOrdinal(EmployeeConstants.Columns.EmployeeId)),
            FirstName = reader.GetString(reader.GetOrdinal(EmployeeConstants.Columns.FirstName)),
            MiddleName = reader.IsDBNull(reader.GetOrdinal(EmployeeConstants.Columns.MiddleName))
                ? null
                : reader.GetString(reader.GetOrdinal(EmployeeConstants.Columns.MiddleName)),
            LastName = reader.GetString(reader.GetOrdinal(EmployeeConstants.Columns.LastName)),
            MobileNumber = reader.IsDBNull(reader.GetOrdinal(EmployeeConstants.Columns.MobileNumber))
                ? null
                : reader.GetString(reader.GetOrdinal(EmployeeConstants.Columns.MobileNumber)),
            Email = reader.IsDBNull(reader.GetOrdinal(EmployeeConstants.Columns.Email))
                ? null
                : reader.GetString(reader.GetOrdinal(EmployeeConstants.Columns.Email)),
        };
    }
}