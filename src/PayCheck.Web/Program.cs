using ARVTech.Shared.Email;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using PayCheck.Web;
using PayCheck.Web.Infrastructure.Http;
using PayCheck.Web.Infrastructure.Http.Interfaces;
using System.Net;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    "appsettings.json",
    optional: false,
    reloadOnChange: true).AddJsonFile(
    $"appsettings.{builder.Environment.EnvironmentName}.json",
    optional: true,
    reloadOnChange: true).AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddRazorPages();

//  ExternalApis.
var appSettingsExternalApisSection = builder.Configuration.GetSection("ExternalApis");      //  Section AppSettings/ExternalApis.
builder.Services.Configure<ExternalApis>(appSettingsExternalApisSection);

// Registrar HttpClientFactory
builder.Services.AddHttpClient<IHttpClientService, HttpClientService>(
    (sp, client) =>
    {
        var externalApis = sp.GetRequiredService<IOptions<ExternalApis>>();

        client.BaseAddress = new Uri(
            externalApis.Value.PayCheck);

        client.Timeout = TimeSpan.FromSeconds(1200);

        // Accept
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        // Encoding (gzip/deflate)
        client.DefaultRequestHeaders.AcceptEncoding.Add(
            new StringWithQualityHeaderValue("gzip"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(
            new StringWithQualityHeaderValue("deflate"));

        // Keep-alive
        client.DefaultRequestHeaders.Connection.Add("keep-alive");

    }).ConfigurePrimaryHttpMessageHandler(
        () => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        });

//  Mail Settings.
var mailSettingsSection = builder.Configuration.GetSection("MailSettings");                 //  Section AppSettings/MailSettings.
builder.Services.Configure<MailSettings>(mailSettingsSection);

var mailSettings = mailSettingsSection.Get<MailSettings>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Access/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

builder.Services.AddTransient<IEmailService>(
    provider => new EmailService(
        mailSettings.Server,
        mailSettings.Port,
        mailSettings.SenderName,
        mailSettings.SenderEmail,
        mailSettings.Username,
        mailSettings.Password));

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Access}/{action=Login}/{id?}");

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();