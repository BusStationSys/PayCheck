using System.Reflection;
using System.Text;
using ARVTech.DataAccess.Business.UniPayCheck;
using ARVTech.DataAccess.Business.UniPayCheck.Interfaces;
using ARVTech.DataAccess.Infrastructure.UnitOfWork.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PayCheck.Api;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IMatriculaEspelhoPontoBusiness>(
    provider => new MatriculaEspelhoPontoBusiness(unitOfWork));

builder.Services.AddScoped<IMatriculaDemonstrativoPagamentoBusiness>(
    provider => new MatriculaDemonstrativoPagamentoBusiness(unitOfWork));

builder.Services.AddScoped<IPessoaFisicaBusiness>(
    provider => new PessoaFisicaBusiness(unitOfWork));

builder.Services.AddScoped<IUsuarioBusiness>(
    provider => new UsuarioBusiness(unitOfWork));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  JWT
var appSettingsAuthenticationSection = builder.Configuration.GetSection("Authentication");    //  Section AppSettings/Authentication.
builder.Services.Configure<Authentication>(appSettingsAuthenticationSection);

var appSettingsAuthentication = appSettingsAuthenticationSection.Get<Authentication>();
var key = Encoding.ASCII.GetBytes(appSettingsAuthentication.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtBearerOption =>
{
    jwtBearerOption.RequireHttpsMetadata = true;
    jwtBearerOption.SaveToken = true;
    jwtBearerOption.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = appSettingsAuthentication.Audience,
        ValidIssuer = appSettingsAuthentication.Issuer,
    };
});

builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc(
            "V1",
            new OpenApiInfo
            {
                Title = "PayCheck.Api",
                Version = "V1",
            });

        c.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

        c.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        }
                    },

                    Array.Empty<string>()
                }
            });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

        var xmlPath = Path.Combine(
            AppContext.BaseDirectory,
            xmlFile);

        c.IncludeXmlComments(
            xmlPath);
    });

builder.Services.AddSingleton<IConfiguration>(
    builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
////if (app.Environment.IsDevelopment())
////{
//app.UseSwagger();
//app.UseSwaggerUI();
////}

//app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(
    c =>
    {
        c.SwaggerEndpoint(
            url: "V1/swagger.json",
            name: "PayCheck.Api V1");
    });

app.UseRouting();

app.UseAuthentication();    // Authentication DEVE ser executado antes do Authorization.
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

//using System.Data.SqlClient;
//using System.Reflection;
//using System.Text;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using PayCheck.Api;
//using PayCheck.Business;
//using PayCheck.Business.Interfaces;
//using PayCheck.Infrastructure.UnitOfWork.SqlServer;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//string connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:SqlServer");

//builder.Services.AddSingleton(provider => new SqlConnection(
//    connectionString));

//var unitOfWork = new UnitOfWorkSqlServer(
//    builder.Configuration);

////var adapter = new UnitOfWorkSqlServerAdapter(
////    this._configuration);

////var repository = new UnitOfWorkSqlServerRepository(
////    (SqlConnection)adapter.Connection);

////var repository2 = new UnitOfWorkSqlServerRepository(
////    (SqlConnection)adapter.Connection,
////    (SqlTransaction)adapter.Transaction);

////services.AddSingleton(provider => unitOfWork);

////services.AddSingleton(provider => adapter);

////services.AddSingleton(provider => repository);
////services.AddSingleton(provider => repository2);

//builder.Services.AddScoped<IMatriculaEspelhoPontoBusiness>(
//    provider => new MatriculaEspelhoPontoBusiness(unitOfWork));

//builder.Services.AddScoped<IMatriculaDemonstrativoPagamentoBusiness>(
//    provider => new MatriculaDemonstrativoPagamentoBusiness(unitOfWork));

//builder.Services.AddScoped<IPessoaFisicaBusiness>(
//    provider => new PessoaFisicaBusiness(unitOfWork));

//builder.Services.AddScoped<IUsuarioBusiness>(
//    provider => new UsuarioBusiness(unitOfWork));

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

////  JWT
//var appSettingsSection = builder.Configuration.GetSection("Authentication");    //  Section AppSettings/Authentication.
//builder.Services.Configure<AppSettings>(appSettingsSection);

//var appSettings = appSettingsSection.Get<AppSettings>();
//var key = Encoding.ASCII.GetBytes(appSettings.Secret);

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(jwtBearerOption =>
//{
//    jwtBearerOption.RequireHttpsMetadata = true;
//    jwtBearerOption.SaveToken = true;
//    jwtBearerOption.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidAudience = appSettings.Audience,
//        ValidIssuer = appSettings.Issuer,
//    };
//});

//builder.Services.AddSwaggerGen(
//    c =>
//    {
//        c.SwaggerDoc(
//            "V1",
//            new OpenApiInfo
//            {
//                Title = "PayCheck.Api",
//                Version = "V1",
//            });

//        c.AddSecurityDefinition(
//            "Bearer",
//            new OpenApiSecurityScheme
//            {
//                Name = "Authorization",
//                In = ParameterLocation.Header,
//                Type = SecuritySchemeType.ApiKey,
//                Scheme = "Bearer"
//            });

//        c.AddSecurityRequirement(
//            new OpenApiSecurityRequirement
//            {
//                {
//                    new OpenApiSecurityScheme
//                    {
//                        Reference = new OpenApiReference
//                        {
//                            Type = ReferenceType.SecurityScheme,
//                            Id = "Bearer",
//                        }
//                    },

//                    Array.Empty<string>()
//                }
//            });

//        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

//        var xmlPath = Path.Combine(
//            AppContext.BaseDirectory,
//            xmlFile);

//        c.IncludeXmlComments(
//            xmlPath);
//    });

//builder.Services.AddSingleton<IConfiguration>(
//    builder.Configuration);

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//////if (app.Environment.IsDevelopment())
//////{
////app.UseSwagger();
////app.UseSwaggerUI();
//////}

////app.UseHttpsRedirection();

////app.UseAuthentication();
////app.UseAuthorization();

////app.MapControllers();

//app.UseHttpsRedirection();

//app.UseSwagger();
//app.UseSwaggerUI(
//    c =>
//    {
//        c.SwaggerEndpoint(
//            url: "V1/swagger.json",
//            name: "PayCheck.Api V1");
//        c.RoutePrefix = string.Empty;
//    });

////app.UseSwagger();
////app.UseSwaggerUI(c =>
////{
////    string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";

////    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "PayCheck.Api");

////});

//app.UseRouting();

//app.UseAuthentication();    // Authentication DEVE ser executado antes do Authorization.
//app.UseAuthorization();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

//app.Run();