

using LandedMVC.Dtos;
using LandedMVC.Hubs;
using LandedMVC.Services;
using Microsoft.Net.Http.Headers;

namespace LandedMVC
{
	public static class ServiceConfig
	{
		public static WebApplicationBuilder ConfigureSevices(this WebApplicationBuilder builder) {
			var apiBase = builder.Configuration.GetValue<string>("ApiBase") ?? string.Empty;

			builder.Services.AddSignalR();
			builder.Services.AddHttpContextAccessor();
			builder.Services.AddControllersWithViews();
			builder.Services.AddDistributedMemoryCache();

			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(1);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});
			builder.Services.AddHttpClient<ApiService<UserDto>>(
				client =>
				{
					client.BaseAddress = new Uri(apiBase);
					client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
				});
			builder.Services.AddHttpClient<ApiService<UserEventDto>>(
				client =>
				{
					client.BaseAddress = new Uri(apiBase);
					client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
				});
			builder.Services.AddHttpClient<ApiService<EmailDto>>(
				client =>
				{
					client.BaseAddress = new Uri(apiBase);
					client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
				});
			builder.Services.AddHttpClient<ApiService<EventDto>>(
				client =>
				{
					client.BaseAddress = new Uri(apiBase);
					client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
				});
			return builder;
		}
		public static void ConfigureAppAndRun(this WebApplicationBuilder builder)
		{
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
			app.UseSession();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.MapHub<ConsoleHub>("/console");
			app.Run();
		}
	}
}
