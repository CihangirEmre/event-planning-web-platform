using EventPlannerProject.Models;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen();//Swagger ekleme

// Add DbContext with connection string from appsettings.json
builder.Services.AddDbContext<EtkinlikPlatformDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EtkinlikPlatformDB")));

// Add Session service
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session süresi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // GDPR ve diðer gereksinimler için gerekli
});


var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Sadece geliþtirme ortamýnda Swagger'ý etkinleþtiriyoruz
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        c.RoutePrefix = "swagger"; // Swagger UI'yi /swagger altýnda çalýþtýrýr
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");




app.Run();
