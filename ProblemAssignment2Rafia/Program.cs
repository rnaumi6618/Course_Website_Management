using Microsoft.EntityFrameworkCore;
using ProblemAssignment2Rafia.Entitties;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// add our context as a service:
string? connStr = builder.Configuration.GetConnectionString("CourseStudentDb");
builder.Services.AddDbContext<CourseStudentDbContext>(options => options.UseSqlServer(connStr));
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
