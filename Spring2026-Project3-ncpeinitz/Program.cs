using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spring2026_Project3_ncpeinitz.Data;
using Azure;
using Azure.AI.OpenAI;
using Spring2026_Project3_ncpeinitz.Services;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped(pro =>
{
    IConfiguration config = pro.GetRequiredService<IConfiguration>();
    string key = config["AzureOpenAI:Key"] ?? throw new InvalidOperationException("Missing AzureOpenAI: Key");
    string endpoint = config["AzureOpenAI:Endpoint"] ?? throw new InvalidOperationException("Missing AzureOpenAI: Endpoint");
    return new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));
});

builder.Services.AddScoped<AiTextService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
