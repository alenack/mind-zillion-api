namespace EmployeeManagementApi;

public static class EmployeeConstants
{
    public const string DefaultConnectionStringName = "DefaultConnection";
    public const string EmployeesRoute = "api/employees";
    public const string EmployeeByIdRoute = "{id:int}";

    public const string SelectAllEmployeesSql = """
        SELECT employee_id, first_name, middle_name, last_name, mobile_number, email
        FROM public.employees
        ORDER BY employee_id;
        """;

    public const string InsertEmployeeSql = """
        INSERT INTO public.employees (first_name, middle_name, last_name, mobile_number, email)
        VALUES (@first_name, @middle_name, @last_name, @mobile_number, @email)
        RETURNING employee_id, first_name, middle_name, last_name, mobile_number, email;
        """;

    public const string UpdateEmployeeSql = """
        UPDATE public.employees
        SET first_name = @first_name,
            middle_name = @middle_name,
            last_name = @last_name,
            mobile_number = @mobile_number,
            email = @email
        WHERE employee_id = @employee_id
        RETURNING employee_id, first_name, middle_name, last_name, mobile_number, email;
        """;

    public const string DeleteEmployeeSql = """
        DELETE FROM public.employees
        WHERE employee_id = @employee_id;
        """;

    public static class Params
    {
        public const string EmployeeId = "@employee_id";
        public const string FirstName = "@first_name";
        public const string MiddleName = "@middle_name";
        public const string LastName = "@last_name";
        public const string MobileNumber = "@mobile_number";
        public const string Email = "@email";
    }

    public static class Columns
    {
        public const string EmployeeId = "employee_id";
        public const string FirstName = "first_name";
        public const string MiddleName = "middle_name";
        public const string LastName = "last_name";
        public const string MobileNumber = "mobile_number";
        public const string Email = "email";
    }
}