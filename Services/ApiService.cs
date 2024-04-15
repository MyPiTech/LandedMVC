// ***********************************************************************
// Assembly         : LandedMVC
// Author           : Shawn Wheeler
// Created          : 02-20-2024
//
// Last Modified By : Shawn Wheeler
// Last Modified On : 02-26-2024
// ***********************************************************************
// <copyright file="ApiService.cs" company="LandedMVC">
//     Copyright (c) MyPiTech. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Reflection;
using System.Text.Json;
using LandedMVC.Attributes;
using LandedMVC.Extensions;
using LandedMVC.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LandedMVC.Services
{

	/// <summary>
	/// Class ApiService. This class cannot be inherited.
	/// </summary>
	/// <typeparam name="DTO">The type of the dto.</typeparam>
	public sealed class ApiService<DTO> where DTO : class
	{
		/// <summary>
		/// The logger
		/// </summary>
		private readonly ILogger<ApiService<DTO>> _logger;
		/// <summary>
		/// The client
		/// </summary>
		private readonly HttpClient _client;
		/// <summary>
		/// The json read options
		/// </summary>
		private readonly JsonSerializerOptions _jsonReadOptions;
		/// <summary>
		/// The json write options
		/// </summary>
		private readonly JsonSerializerOptions _jsonWriteOptions;
		/// <summary>
		/// No records message
		/// </summary>
		private const string NO_RECORDS = "\"No records were found.\"";
		/// <summary>
		/// The hub connection identifier
		/// </summary>
		private string _hubConnectionId = string.Empty;

		private readonly IHubContext<ConsoleHub, IConsoleHub> _consoleHub;

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiService{DTO}" /> class.
		/// </summary>
		/// <param name="httpClient">The HTTP client.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="consoleHub">The console hub.</param>
		public ApiService(
			HttpClient httpClient, 
			ILogger<ApiService<DTO>> logger, 
			IHubContext<ConsoleHub, IConsoleHub> consoleHub
		)
        {
            _client = httpClient;
            _logger = logger;
            _jsonReadOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            _jsonWriteOptions = new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never };
			_consoleHub = consoleHub;
		}

		/// <summary>
		/// Get all as an asynchronous operation.
		/// </summary>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;DTO[]&gt; representing the asynchronous operation.</returns>
		public async Task<DTO[]?> GetAllAsync(CancellationToken token = default)
        {
			try
			{
				var results = await GetAllAsync(null, token);
				await _logger.LogInformationAsync("ApiService\\GetAllAsync", results ?? []);
				return results;
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "ApiService\\GetAllAsync");
				throw;
			}
		}

		/// <summary>
		/// Get all as an asynchronous operation.
		/// </summary>
		/// <param name="fDto">A function that returns the dto.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;DTO[]&gt; representing the asynchronous operation.</returns>
		public async Task<DTO[]?> GetAllAsync(Func<DTO>? fDto = null, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(fDto);

				var response = await _client.GetAsync(apiRoute, token);
				if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					var message = await response.Content.ReadAsStringAsync(token);
					if(message.Equals(NO_RECORDS))
					{
						return default;
					}
				}

				response.EnsureSuccessStatusCode();
				var results = await response.Content.ReadFromJsonAsync<DTO[]>(token);
				await _logger.LogInformationAsync("ApiService\\GetAllAsync - fDto", results ?? []);
				return results;
            }
            catch (Exception ex)
            {
				await _logger.LogErrorAsync(ex, "ApiService\\GetAllAsync - fDto");
				throw;
			}
        }

		/// <summary>
		/// Get one as an asynchronous operation.
		/// </summary>
		/// <param name="fDto">A function that returns the dto.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;DTO&gt; representing the asynchronous operation.</returns>
		public async Task<DTO?> GetOneAsync(Func<DTO> fDto, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(fDto, true);

                DTO? response = await _client.GetFromJsonAsync<DTO>(apiRoute, _jsonReadOptions, token);
				await _logger.LogInformationAsync("ApiService\\GetOneAsync", response  ?? fDto());
				return response;
            }
            catch (Exception ex)
            {
				await _logger.LogErrorAsync(ex, "ApiService\\GetOneAsync");
				throw;
			}
        }

		/// <summary>
		/// Add as an asynchronous operation.
		/// </summary>
		/// <param name="dto">The dto.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;DTO&gt; representing the asynchronous operation.</returns>
		public async Task<DTO?> AddAsync(DTO dto, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(() => dto);
                var response = await _client.PostAsJsonAsync(apiRoute, dto, _jsonWriteOptions, token);
				response.EnsureSuccessStatusCode();
				var result = await response.Content.ReadFromJsonAsync<DTO>(token);
				await _logger.LogInformationAsync("ApiService\\AddAsync", result);
				return result;
			}
            catch (Exception ex)
            {
				await _logger.LogErrorAsync(ex, "ApiService\\AddAsync");
				throw;
			}
        }

		/// <summary>
		/// Edit as an asynchronous operation.
		/// </summary>
		/// <param name="dto">The dto.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public async Task EditAsync(DTO dto, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(() => dto, true);
				var response = await _client.PutAsJsonAsync(apiRoute, dto, _jsonWriteOptions, token);

				response.EnsureSuccessStatusCode();
				await _logger.LogInformationAsync("ApiService\\EditAsync", response);
			}
            catch (Exception ex)
            {
				await _logger.LogErrorAsync(ex, "ApiService\\EditAsync");
				throw;
			}
        }

		/// <summary>
		/// Delete as an asynchronous operation.
		/// </summary>
		/// <param name="fDto">A function that returns the dto.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public async Task DeleteAsync(Func<DTO> fDto, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(fDto, true);
				var response = await _client.DeleteAsync(apiRoute, token);
				response.EnsureSuccessStatusCode();
				await _logger.LogInformationAsync("ApiService\\DeleteAsync", response);
			}
            catch (Exception ex)
            {
				await _logger.LogErrorAsync(ex, "ApiService\\DeleteAsync");
				throw;
			}
        }

		/// <summary>
		/// Post as an asynchronous operation.
		/// </summary>
		/// <param name="dto">The dto.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;HttpResponseMessage&gt; representing the asynchronous operation.</returns>
		public async Task<HttpResponseMessage> PostAsync(DTO dto, CancellationToken token = default)
		{
			try
			{
				var apiRoute = GetRoute(() => dto);
				var response = await _client.PostAsJsonAsync(apiRoute, dto, _jsonWriteOptions, token);

				response.EnsureSuccessStatusCode();
				await _logger.LogInformationAsync("ApiService\\PostAsync", response);
				return response;
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "ApiService\\PostAsync");
				throw;
			}
		}

		/// <summary>
		/// Gets the route.
		/// </summary>
		/// <param name="fDto">A function that returns the dto.</param>
		/// <param name="appendPrimary">if set to <c>true</c> [append primary].</param>
		/// <returns>System.String.</returns>
		private string GetRoute(Func<DTO>? fDto, bool appendPrimary = false)
        {
            if (fDto != null)
            {
                return GetRouteString(fDto(), appendPrimary);
            }

            return GetRouteString(dto: null, appendPrimary);
        }

		/// <summary>
		/// Gets the route string.
		/// </summary>
		/// <param name="dto">The dto.</param>
		/// <param name="appendPrimary">if set to <c>true</c> [append primary].</param>
		/// <returns>System.String.</returns>
		/// <exception cref="Exception">Route not found.</exception>
		/// <exception cref="System.Exception">Route not found.</exception>
		private static string GetRouteString(DTO? dto, bool appendPrimary = false)
        {
            Type rType = typeof(DTO);

            if (Attribute.GetCustomAttribute(rType, typeof(ApiRouteAttribute)) is not ApiRouteAttribute apiRoute)
                throw new Exception("Route not found.");

            var route = apiRoute.Route;

            if (dto != null)
            {
                var returnDto = dto;
                var props = rType.GetProperties().Where(p => Attribute.IsDefined(p, typeof(ApiKeyAttribute)));
                foreach (var prop in props)
                {
                    var keyAttribute = prop.GetCustomAttribute<ApiKeyAttribute>();
                    if (keyAttribute != null)
                    {
                        var value = prop.GetValue(returnDto);
                        if (keyAttribute.IsPrimary && appendPrimary)
                        {
                            route = $"{route}/{value}";
                        }
                        else
                        {
                            var pattern = $"{{{keyAttribute.ApiRouteParam}}}";
                            route = route.Replace(pattern, value?.ToString() ?? string.Empty);
                        }
                    }
                }
            }

            return route;
        }
    }
}
