using RealEstateApp.Core.Application;
using RealEstateApp.Infrastructure.Identity;
using RealEstateApp.Infrastructure.Persistence;
using RealEstateApp.Infrastructure.Shared;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddSession(opt =>
        {
            opt.IdleTimeout = TimeSpan.FromMinutes(60);
            opt.Cookie.HttpOnly = true;
        });

        builder.Services.AddPersistenceLayerIoc(builder.Configuration);
        builder.Services.AddApplicationLayerIoc();
        builder.Services.AddIdentityLayerIocForWebApp(builder.Configuration);
        builder.Services.AddSharedLayerIoc(builder.Configuration);
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        var app = builder.Build();

        await app.Services.RunIdentitySeedAsync();
        await app.Services.RunPersistenceSeedAsync();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=GeneralHome}/{action=Index}/{id?}")
            .WithStaticAssets();

        await app.RunAsync();
    }
}