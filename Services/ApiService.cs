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
// <summary>Generic API service</summary>
// ***********************************************************************
using System.Reflection;
using System.Text.Json;
using LandedMVC.Attributes;


namespace LandedMVC.Services
{

	public sealed class ApiService<DTO> where DTO : class
    {
        private readonly ILogger<ApiService<DTO>> _logger;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _JsonReadOptions;
        private readonly JsonSerializerOptions _JsonWriteOptions;

        public ApiService(HttpClient httpClient, ILogger<ApiService<DTO>> logger)
        {
            _client = httpClient;
            _logger = logger;
            _JsonReadOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            _JsonWriteOptions = new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never };
        }

        public async Task<DTO[]?> GetAllAsync(CancellationToken token = default)
        {
            return await GetAllAsync(null, token);
        }

        public async Task<DTO[]?> GetAllAsync(Func<DTO>? newR = null, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(newR);

                DTO[]? response = await _client.GetFromJsonAsync<DTO[]>(apiRoute, _JsonReadOptions, token);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
				throw;
			}
        }

        public async Task<DTO?> GetOneAsync(Func<DTO> newR, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(newR, true);

                DTO? response = await _client.GetFromJsonAsync<DTO>(apiRoute, _JsonReadOptions, token);

                return response;
            }
            catch (Exception ex)
            {
				_logger.LogError(ex.Message);
				throw;
			}
        }

        public async Task<DTO?> AddAsync(DTO dto, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(() => dto);
                var response = await _client.PostAsJsonAsync(apiRoute, dto, _JsonWriteOptions, token);
                
                response.EnsureSuccessStatusCode();
				return await response.Content.ReadFromJsonAsync<DTO>(token);
			}
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
				throw;
			}
        }

		public async Task<HttpResponseMessage> PostAsync(DTO dto, CancellationToken token = default)
		{
			try
			{
				var apiRoute = GetRoute(() => dto);
				HttpResponseMessage response = await _client.PostAsJsonAsync(apiRoute, dto, _JsonWriteOptions, token);

				response.EnsureSuccessStatusCode();
				return response;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw;
			}
		}

		public async Task EditAsync(DTO dto, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(() => dto, true);
				var response = await _client.PutAsJsonAsync(apiRoute, dto, _JsonWriteOptions, token);

				response.EnsureSuccessStatusCode();
			}
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
				throw;
			}
        }

        public async Task DeleteAsync(Func<DTO> newR, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(newR, true);
				var response = await _client.DeleteAsync(apiRoute, token);
				response.EnsureSuccessStatusCode();
			}
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
				throw;
			}
        }

		/*public async Task PostAsync(Func<DTO> newR, CancellationToken token = default)
		{
			try
			{
				var apiRoute = GetRoute(newR);
				var response = await _client.PostAsJsonAsync(apiRoute, newR(), _JsonWriteOptions, token);

				response.EnsureSuccessStatusCode();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw;
			}
		}*/

		private string GetRoute(Func<DTO>? newR, bool appendPrimary = false)
        {
            if (newR != null)
            {
                return GetRouteString(newR(), appendPrimary);
            }

            return GetRouteString(dto: null, appendPrimary);
        }

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
