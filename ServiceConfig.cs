// ***********************************************************************
// Assembly         : LandedMVC
// Author           : Shawn Wheeler
// Created          : 04-13-2024
//
// Last Modified By : Shawn Wheeler
// Last Modified On : 04-30-2024
// ***********************************************************************
// <copyright file="ServiceConfig.cs" company="LandedMVC">
//     Copyright (c) MyPiTech. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using LandedMVC.Dtos;
using LandedMVC.Hubs;
using LandedMVC.Services;
using Microsoft.Net.Http.Headers;

namespace LandedMVC
{
	/// <summary>
	/// Class ServiceConfig.
	/// </summary>
	public static class ServiceConfig
	{
		/// <summary>
		/// Configures the sevices.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <returns>WebApplicationBuilder.</returns>
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
		/// <summary>
		/// Configures the application and runs the app.
		/// </summary>
		/// <param name="builder">The builder.</param>
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
			app.MapHub<DataTableHub>("/dataTable");
			app.Run();
		}
	}
}
