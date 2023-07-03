using System.Data.SqlClient;
using PayCheck.Business;
using PayCheck.Business.Interfaces;
using PayCheck.Infrastructure.UnitOfWork.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:SqlServer");

builder.Services.AddSingleton(provider => new SqlConnection(
    connectionString));

var unitOfWork = new UnitOfWorkSqlServer(
    builder.Configuration);

//var adapter = new UnitOfWorkSqlServerAdapter(
//    this._configuration);

//var repository = new UnitOfWorkSqlServerRepository(
//    (SqlConnection)adapter.Connection);

//var repository2 = new UnitOfWorkSqlServerRepository(
//    (SqlConnection)adapter.Connection,
//    (SqlTransaction)adapter.Transaction);

//services.AddSingleton(provider => unitOfWork);

//services.AddSingleton(provider => adapter);

//services.AddSingleton(provider => repository);
//services.AddSingleton(provider => repository2);

builder.Services.AddScoped<IMatriculaDemonstrativoPagamentoBusiness>(
    provider => new MatriculaDemonstrativoPagamentoBusiness(unitOfWork));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
