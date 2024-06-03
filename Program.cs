using testSQLServer.Services;
using testSQLServer.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDapperDbContext, DapperDbContext>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<ISpecimensService, SpecimensService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

