using EmployeeManagementApi.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();
app.UseCors();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing DefaultConnection string.");

// GET all employees
app.MapGet("/api/employees", async () =>
{
    var employees = new List<Employee>();
    await using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();

    var sql = """
        SELECT employee_id, first_name, middle_name, last_name, mobile_number, email
        FROM public.employees
        ORDER BY employee_id;
        """;

    await using var cmd = new NpgsqlCommand(sql, conn);
    await using var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        employees.Add(new Employee
        {
            EmployeeId = reader.GetInt32(0),
            FirstName = reader.GetString(1),
            MiddleName = reader.IsDBNull(2) ? null : reader.GetString(2),
            LastName = reader.GetString(3),
            MobileNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
            Email = reader.IsDBNull(5) ? null : reader.GetString(5)
        });
    }

    return Results.Ok(employees);
});

// POST create a new employee
app.MapPost("/api/employees", async (Employee input) =>
{
    if (string.IsNullOrWhiteSpace(input.FirstName) || string.IsNullOrWhiteSpace(input.LastName))
        return Results.BadRequest("First Name and Last Name are required.");

    await using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();

    var sql = """
        INSERT INTO public.employees (first_name, middle_name, last_name, mobile_number, email)
        VALUES (@first_name, @middle_name, @last_name, @mobile_number, @email)
        RETURNING employee_id;
        """;

    await using var cmd = new NpgsqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("first_name", input.FirstName.Trim());
    cmd.Parameters.AddWithValue("middle_name", (object?)input.MiddleName?.Trim() ?? DBNull.Value);
    cmd.Parameters.AddWithValue("last_name", input.LastName.Trim());
    cmd.Parameters.AddWithValue("mobile_number", (object?)input.MobileNumber?.Trim() ?? DBNull.Value);
    cmd.Parameters.AddWithValue("email", (object?)input.Email?.Trim() ?? DBNull.Value);

    var newId = (int)(await cmd.ExecuteScalarAsync())!;
    input.EmployeeId = newId;

    return Results.Created($"/api/employees/{newId}", input);
});

// PUT update an existing employee
app.MapPut("/api/employees/{id:int}", async (int id, Employee input) =>
{
    if (string.IsNullOrWhiteSpace(input.FirstName) || string.IsNullOrWhiteSpace(input.LastName))
        return Results.BadRequest("First Name and Last Name are required.");

    await using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();

    var sql = """
        UPDATE public.employees
        SET first_name = @first_name,
            middle_name = @middle_name,
            last_name = @last_name,
            mobile_number = @mobile_number,
            email = @email
        WHERE employee_id = @id;
        """;

    await using var cmd = new NpgsqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("id", id);
    cmd.Parameters.AddWithValue("first_name", input.FirstName.Trim());
    cmd.Parameters.AddWithValue("middle_name", (object?)input.MiddleName?.Trim() ?? DBNull.Value);
    cmd.Parameters.AddWithValue("last_name", input.LastName.Trim());
    cmd.Parameters.AddWithValue("mobile_number", (object?)input.MobileNumber?.Trim() ?? DBNull.Value);
    cmd.Parameters.AddWithValue("email", (object?)input.Email?.Trim() ?? DBNull.Value);

    var rowsAffected = await cmd.ExecuteNonQueryAsync();
    return rowsAffected == 0 ? Results.NotFound() : Results.NoContent();
});

// DELETE an employee
app.MapDelete("/api/employees/{id:int}", async (int id) =>
{
    await using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();

    var sql = "DELETE FROM public.employees WHERE employee_id = @id;";
    await using var cmd = new NpgsqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("id", id);

    var rowsAffected = await cmd.ExecuteNonQueryAsync();
    return rowsAffected == 0 ? Results.NotFound() : Results.NoContent();
});

app.Run();