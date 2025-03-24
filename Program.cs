using ProjectPRNLamnthe180410.Repositories.Interface;
using ProjectPRNLamnthe180410.Repositories;
using ProjectPRNLamnthe180410.Services.Interface;
using ProjectPRNLamnthe180410.Services;
using Microsoft.EntityFrameworkCore;

using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AnimeLightNovelContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

// Register repository layers

builder.Services.AddScoped<ILightNovelRepository, LightNovelRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();



// Register service layers

builder.Services.AddScoped<ILightNovelService, LightNovelService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<ICommentService, CommentService>();




builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(365); // No timeout (1 year)
    options.Cookie.HttpOnly = true; // Security
                                    // best practice
    options.Cookie.IsEssential = true; // Required for session to work
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.MapHub<LightNovelHub>("/lightNovelHub");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
