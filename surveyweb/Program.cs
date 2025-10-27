using SurveyWeb.Data.Models;
using SurveyWeb.Repositories.Interfaces;
using SurveyWeb.Repositories.Implementations;
using SurveyWeb.Services.Interfaces;
using SurveyWeb.Services.Implementations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register DbContext
builder.Services.AddDbContext<SurveyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SurveyDb")));

// Register Repositories
builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

// Register Services
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Configure default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Survey}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
