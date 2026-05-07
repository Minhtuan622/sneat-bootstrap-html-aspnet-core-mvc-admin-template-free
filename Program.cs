using System.Text;
using AspnetCoreMvcFull.Repositories;
using AspnetCoreMvcFull.Services;
using AspnetCoreMvcFull.Workers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
  options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build()));
});
builder.Services.AddSingleton<MetricsRepository>();
builder.Services.AddScoped<LiveMetricsRepository>();
builder.Services.AddScoped<LiveConfigRepository>();
builder.Services.AddScoped<LiveAdRepository>();
builder.Services.AddScoped<ReportLogRepository>();
builder.Services.AddScoped<LiveMetricSnapshotRepository>();
builder.Services.AddScoped<OrdersRepository>();
builder.Services.AddScoped<ImportJobRepository>();
builder.Services.AddScoped<AuditRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<FacebookAdsService>();
builder.Services.AddScoped<ReportBuilderService>();
builder.Services.AddScoped<ExcelImportService>();
builder.Services.AddScoped<SystemSettingsRepository>();
builder.Services.AddScoped<ReportDispatchService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<ErrorLogRepository>();
builder.Services.AddScoped<ErrorLogService>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<LarkService>();
builder.Services.AddHostedService<ReportWorker>();
builder.Services.AddHostedService<AspnetCoreMvcFull.Workers.ImportJobWorker>();

Console.OutputEncoding = Encoding.UTF8;
builder.Services
  .AddAuthentication(
    CookieAuthenticationDefaults
      .AuthenticationScheme
  )
  .AddCookie(options => { options.LoginPath = "/Auth/LoginBasic"; });

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
  pattern: "{controller=Dashboards}/{action=Index}/{id?}");

app.Run();
