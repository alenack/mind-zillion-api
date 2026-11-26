using EmployeeManagementApi.Service.BusinessLayer;
using EmployeeManagementApi.Service.DataLayer;
using EmployeeManagementApi.Service.DataLayer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<EmployeeDataRepository>();
builder.Services.AddScoped<EmployeeBusinessService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.MapControllers();

app.Run();