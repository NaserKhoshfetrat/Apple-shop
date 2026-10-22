using AppleShop.Web.Models.ProductService.Dtos;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AppleShop.Web.Commons
{
    public class BaseApiConsumer
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public BaseApiConsumer(HttpClient httpClient)
        {
            _client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<T> GetAsync<T>(string apiUrl,
                             string authorizationToken,
                             Dictionary<string, string>? queryString = null,
                             Dictionary<string, string>? headers = null,
                             string? path = null)
        {
            //The second generic parameter is object because there is no request body (we pass null)
            return await SendInternalAsync<T, object>(HttpMethod.Get, null, apiUrl, authorizationToken, headers, path, queryString);
        }

        public async Task<T> PostAsync<T, U>(U requestData,
                                        string apiUrl,
                                        string authorizationToken,
                                        Dictionary<string, string>? headers = null,
                                        string? path = null,
                                        Dictionary<string, string>? queryString = null)
        {
            return await SendInternalAsync<T, U>(HttpMethod.Post, requestData, apiUrl, authorizationToken, headers, path, queryString);
        }

        public async Task<T> PatchAsync<T, U>(U requestData,
                                         string apiUrl,
                                         string authorizationToken,
                                         Dictionary<string, string>? headers = null,
                                         string? path = null,
                                         Dictionary<string, string>? queryString = null)
        {
            return await SendInternalAsync<T, U>(HttpMethod.Patch, requestData, apiUrl, authorizationToken, headers, path, queryString);
        }

        public async Task<T> PutAsync<T, U>(U requestData,
                                     string apiUrl,
                                     string authorizationToken,
                                     Dictionary<string, string>? headers = null,
                                     string? path = null,
                                     Dictionary<string, string>? queryString = null)
        {
            return await SendInternalAsync<T, U>(HttpMethod.Put, requestData, apiUrl, authorizationToken, headers, path, queryString);
        }

        public async Task<T> DeleteAsync<T, U>(U? requestData,
                                          string apiUrl,
                                          string authorizationToken,
                                          Dictionary<string, string>? headers = null,
                                          string? path = null,
                                          Dictionary<string, string>? queryString = null)
        {
            return await SendInternalAsync<T, U>(HttpMethod.Delete, requestData, apiUrl, authorizationToken, headers, path, queryString);
        }

        public async Task<T> SendAsync<T, U>(string httpMethod,
                                        U? requestData,
                                        string apiUrl,
                                        string authorizationToken,
                                        Dictionary<string, string>? headers = null,
                                        string? path = null,
                                        Dictionary<string, string>? queryString = null)
        {
            var method = new HttpMethod(httpMethod.ToUpperInvariant());
            return await SendInternalAsync<T, U>(method, requestData, apiUrl, authorizationToken, headers, path, queryString);
        }

        public async Task<T> SendAsync<T, U>(HttpMethod httpMethod,
                                        U? requestData,
                                        string apiUrl,
                                        string authorizationToken,
                                        Dictionary<string, string>? headers = null,
                                        string? path = null,
                                        Dictionary<string, string>? queryString = null)
        {
            return await SendInternalAsync<T, U>(httpMethod, requestData, apiUrl, authorizationToken, headers, path, queryString);
        }

        private async Task<T> SendInternalAsync<T, U>(HttpMethod httpMethod,
                                           U? requestData,
                                           string apiUrl,
                                           string authorizationToken,
                                           Dictionary<string, string>? headers = null,
                                           string? path = null,
                                           Dictionary<string, string>? queryString = null)
        {
            try
            {
                var fullUrl = BuildUrl(apiUrl, path, queryString);
                var request = new HttpRequestMessage(httpMethod, fullUrl);

                if (!string.IsNullOrWhiteSpace(authorizationToken)) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authorizationToken);

                if (headers != null)
                {
                    foreach (var h in headers)
                        request.Headers.TryAddWithoutValidation(h.Key, h.Value);
                }

                if (requestData != null) request.Content = SerializeContent(requestData);

                var response = await _client.SendAsync(request);
                return await HandleResponse<T>(response);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                // handle 401
                var dto = new BaseResponseDto<string>()
                {
                    Result = null,
                    Error = new List<string> { "Unauthorized access – please check your credentials.", Convert.ToString(ex.Message) },
                    IsSuccessful = false
                };
                var res = JsonSerializer.Serialize(dto);
                var apiResponseDto = JsonSerializer.Deserialize<T>(res);
                return apiResponseDto;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                // Handle 403 
                var dto = new BaseResponseDto<string>()
                {
                    Result = null,
                    Error = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccessful = false
                };
                var res = JsonSerializer.Serialize(dto);
                var apiResponseDto = JsonSerializer.Deserialize<T>(res);
                return apiResponseDto;
            }
            catch (HttpRequestException ex)
            {
                // Handle other HTTP errors (e.g., 404, 500)
                var dto = new BaseResponseDto<string>()
                {
                    Result = null,
                    Error = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccessful = false
                };
                var res = JsonSerializer.Serialize(dto);
                var apiResponseDto = JsonSerializer.Deserialize<T>(res);
                return apiResponseDto;
            }
            catch (Exception ex)
            {
                var dto = new BaseResponseDto<string>()
                {
                    Result = null,
                    Error = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccessful = false
                };
                var res = JsonSerializer.Serialize(dto);
                var apiResponseDto = JsonSerializer.Deserialize<T>(res);
                return apiResponseDto;
            }
        }

        private string BuildUrl(string baseUrl, string? path, Dictionary<string, string>? queryString)
        {
            var url = baseUrl;
            if (!string.IsNullOrEmpty(path))
            {
                url = baseUrl.TrimEnd('/') + '/' + path.TrimStart('/');
            }
            if (queryString != null && queryString.Any())
            {
                url = QueryHelpers.AddQueryString(url, queryString);
            }
            return url;
        }

        private StringContent? SerializeContent<T>(T data)
        {
            if (data == null) return null;
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[{response.StatusCode}] CT={response.Content.Headers.ContentType} Len={jsonResponse.Length}");
            Console.WriteLine(jsonResponse);

            response.EnsureSuccessStatusCode();
            if (string.IsNullOrEmpty(jsonResponse))
                return default!;

            return JsonSerializer.Deserialize<T>(jsonResponse, _jsonOptions)
                   ?? throw new InvalidOperationException("Deserialization returned null.");
        }
    }
}