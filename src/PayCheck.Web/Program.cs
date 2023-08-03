using ARVTech.Shared.Email;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using PayCheck.Web;

var builder = WebApplication.CreateBuilder(args);

//  Mail Settings.
var mailSettingsSection = builder.Configuration.GetSection("MailSettings");    //  Section AppSettings/MailSettings.
builder.Services.Configure<MailSettings>(mailSettingsSection);

var mailSettings = mailSettingsSection.Get<MailSettings>();
//var key = Encoding.ASCII.GetBytes(appSettings.Secret);

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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Access}/{action=Login}/{id?}");

app.Run();
