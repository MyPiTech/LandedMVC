using LandedMVC.Dtos;
using LandedMVC.Services;
using Microsoft.Net.Http.Headers;
using TestApi.Dtos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var apiBase = builder.Configuration.GetValue<string>("ApiBase") ?? string.Empty;
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
