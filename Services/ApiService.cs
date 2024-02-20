using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Text.Json;
using TestMVC.Attributes;


namespace TestMVC.Services
{
    public sealed class ApiService<R> where R : class
    {
        private readonly ILogger<ApiService<R>> _logger;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _JsonReadOptions;
        private readonly JsonSerializerOptions _JsonWriteOptions;

        public ApiService(HttpClient httpClient, ILogger<ApiService<R>> logger)
        {
            _client = httpClient;
            _logger = logger;
            _JsonReadOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            _JsonWriteOptions = new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never };
        }

        public async Task<R[]?> GetAllAsync(CancellationToken token = default)
        {
            return await GetAllAsync(null, token);
        }

        public async Task<R[]?> GetAllAsync(Func<R>? newR = null, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(newR);

                R[]? response = await _client.GetFromJsonAsync<R[]>(apiRoute, _JsonReadOptions, token);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }

        public async Task<R?> GetOneAsync(Func<R> newR, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(newR, true);

                R? response = await _client.GetFromJsonAsync<R>(apiRoute, _JsonReadOptions, token);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }

        public async Task AddAsync(R dto, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(() => dto);
                await _client.PostAsJsonAsync(apiRoute, dto, _JsonWriteOptions, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task EditAsync(R dto, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(() => dto, true);
                await _client.PutAsJsonAsync(apiRoute, dto, _JsonWriteOptions, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task DeleteAsync(Func<R> newR, CancellationToken token = default)
        {
            try
            {
                var apiRoute = GetRoute(newR, true);
                await _client.DeleteAsync(apiRoute, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private string GetRoute(Func<R>? newR, bool appendPrimary = false)
        {
            if (newR != null)
            {
                return GetRouteString(newR(), appendPrimary);
            }

            return GetRouteString(dto: null, appendPrimary);
        }

        private static string GetRouteString(R? dto, bool appendPrimary = false)
        {
            Type rType = typeof(R);

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
