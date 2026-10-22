# BaseApiConsumer Documentation

## Overview

`BaseApiConsumer` is a foundational HTTP client wrapper that simplifies making API calls to external services. It handles:

- All common HTTP methods (GET, POST, PUT, PATCH, DELETE)
- JWT Bearer token authentication
- Query string parameters, custom headers, and path segments
- JSON serialization/deserialization (camelCase, case‑insensitive)
- Uniform error handling that returns a standardized `BaseResponseDto<T>` even when HTTP errors occur

The class is designed to be **extended** – you create a dedicated client per microservice (e.g., `ProductApiClient`, `ShoppingCartApiClient`) and then build service layers on top of them.

---

## How It Works

### 1. Core Components

**Constructor**
```csharp
public BaseApiConsumer(HttpClient httpClient)
```
- Accepts an injected `HttpClient` instance (managed by the DI container).
- Initializes JSON options with:
  - `PropertyNameCaseInsensitive = true` – allows `userId` to match `UserId`.
  - `PropertyNamingPolicy = JsonNamingPolicy.CamelCase` – all outgoing JSON uses camelCase.

### 2. Request Building

The `BuildUrl` method constructs the final URL:

```
baseUrl + path + ?queryString
```

**Example:**
- Base: `https://api.example.com/products`
- `path`: `/123`
- `queryString`: `{ "includeDetails": "true" }`
- **Result:** `https://api.example.com/products/123?includeDetails=true`

### 3. Execution Flow

```
Call API Method → Build URL → Create HttpRequestMessage
→ Add Authorization Header (Bearer) → Add Custom Headers
→ Serialize Request Body (if any) → Send Request
→ Handle Response → Deserialize to T → Return Result
```

### 4. Error Handling

Any exception (HTTP or general) is caught and transformed into a `BaseResponseDto<T>` with:
- `IsSuccessful = false`
- `Error` list containing the exception message
- `Result = null` (default)

This ensures your service layer always receives a consistent object, even on failures.

---

## Usage Guide

### 1. Create an API Client

For each microservice, create a class that inherits from `BaseApiConsumer`.  
*Example – Product client:*

```csharp
using AppleShop.Web.Commons;

namespace AppleShop.Web.Services.Integration.Product
{
    public class ProductApiClient : BaseApiConsumer
    {
        public ProductApiClient(HttpClient httpClient) : base(httpClient) { }
    }
}
```

### 2. Create a Service Class

Build a service that uses the client and implements your business logic.  
*Example – Product service:*

```csharp
using AppleShop.Web.Models;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.Product;
using Microsoft.Extensions.Options;

namespace AppleShop.Web.Services.Integration.Product
{
    public class ProductService : IProductService
    {
        private readonly ProductApiClient _apiClient;
        private readonly ServiceConfiguration _config;

        public ProductService(ProductApiClient apiClient, IOptions<ServiceConfiguration> options)
        {
            _apiClient = apiClient;
            _config = options.Value;
        }

        // Service methods go here (see examples below)
    }
}
```

### 3. Service Method Examples

#### GET – Retrieve a List (with path and query parameters)

```csharp
public async Task<BaseResponseDto<List<ProductDto>>> GetProductsAsync(
    int pageSize, int pageIndex, string accessToken)
{
    string apiUrl = new Uri(
        new Uri(_config.ProductApi.BaseUrl),
        _config.ProductApi.GetProductsRelativeUrl
    ).ToString();

    // Path segments
    string? path = $"/{pageSize}/{pageIndex}";

    // Optional query string
    var queryString = new Dictionary<string, string>
    {
        { "includeInactive", "false" }
    };

    return await _apiClient.GetAsync<BaseResponseDto<List<ProductDto>>>(
        apiUrl,
        accessToken,
        queryString,
        headers: null,
        path
    );
}
```

#### POST – Create a Resource (with request body)

```csharp
public async Task<BaseResponseDto<ProductDto>> CreateProductAsync(
    ProductDto product, string accessToken)
{
    string apiUrl = new Uri(
        new Uri(_config.ProductApi.BaseUrl),
        _config.ProductApi.CreateRelativeUrl
    ).ToString();

    return await _apiClient.PostAsync<BaseResponseDto<ProductDto>, ProductDto>(
        product,
        apiUrl,
        accessToken
    );
}
```

#### PATCH – Partial Update

```csharp
public async Task<BaseResponseDto<ProductDto>> UpdateProductAsync(
    ProductDto product, string accessToken)
{
    string apiUrl = new Uri(
        new Uri(_config.ProductApi.BaseUrl),
        _config.ProductApi.UpdateRelativeUrl
    ).ToString();

    return await _apiClient.PatchAsync<BaseResponseDto<ProductDto>, ProductDto>(
        product,
        apiUrl,
        accessToken
    );
}
```

#### DELETE – Remove a Resource (with query parameters)

```csharp
public async Task<BaseResponseDto<bool>> DeleteProductAsync(
    long productId, string accessToken)
{
    string apiUrl = new Uri(
        new Uri(_config.ProductApi.BaseUrl),
        _config.ProductApi.DeleteRelativeUrl
    ).ToString();

    var queryString = new Dictionary<string, string>
    {
        { "pid", productId.ToString() }
    };

    // No request body, so we pass null for U and use object as placeholder
    return await _apiClient.DeleteAsync<BaseResponseDto<bool>, object>(
        null,
        apiUrl,
        accessToken,
        queryString: queryString
    );
}
```

#### POST with a Simple Body (e.g., anonymous type)

```csharp
public async Task<BaseResponseDto<bool>> RemoveFromCartAsync(
    long shoppingCartDetailId, string accessToken)
{
    string apiUrl = ...;

    var body = new { shoppingCartDetailId = shoppingCartDetailId };

    return await _apiClient.PostAsync<BaseResponseDto<bool>, object>(
        body,
        apiUrl,
        accessToken
    );
}
```

### 4. Method Parameter Reference

| Parameter          | Type                | Required | Description                                 |
|--------------------|---------------------|----------|---------------------------------------------|
| `requestData`      | `U` (generic)       | No       | Request body (ignored for GET)              |
| `apiUrl`           | `string`            | Yes      | Full base URL of the endpoint               |
| `authorizationToken`| `string`           | Yes      | JWT Bearer token (can be null/empty)        |
| `headers`          | `Dictionary`        | No       | Custom HTTP headers                         |
| `path`             | `string`            | No       | Additional path segments after base URL     |
| `queryString`      | `Dictionary`        | No       | Query string parameters                     |

> **Note:** `authorizationToken` is automatically prefixed with `"Bearer "`. If you don't need authentication, pass `null` or an empty string.

### 5. Dependency Injection Setup (Program.cs)

```csharp
// Register HttpClients
builder.Services.AddHttpClient<ProductApiClient>();
builder.Services.AddHttpClient<ShoppingCartApiClient>();

// Register services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

// Bind API endpoint configuration
builder.Services.Configure<ServiceConfiguration>(
    builder.Configuration.GetSection("ServiceConfiguration")
);
```

### 6. Configuration (appsettings.json)

```json
{
  "ServiceConfiguration": {
    "ProductApi": {
      "BaseUrl": "https://api.product.example.com",
      "GetProductsRelativeUrl": "/api/v1/products",
      "CreateRelativeUrl": "/api/v1/products",
      "UpdateRelativeUrl": "/api/v1/products",
      "DeleteRelativeUrl": "/api/v1/products",
      "GetProductDetailRelativeUrl": "/api/v1/products/details"
    },
    "ShoppingCartApi": {
      "BaseUrl": "https://api.cart.example.com",
      "AddCartRelativeUrl": "/api/v1/cart",
      "GetCartRelativeUrl": "/api/v1/cart",
      "UpdateCartRelativeUrl": "/api/v1/cart",
      "RemoveCartRelativeUrl": "/api/v1/cart/remove"
    }
  }
}
```

---

## Best Practices

1. **Access Token Handling**  
   Retrieve the token from the current HTTP context (e.g., `IHttpContextAccessor`) or from a secure storage; do not hard‑code it.

2. **Service Lifetime**  
   Register your service classes as **Scoped** to maintain consistency per request.

3. **Error Handling**  
   Always check `response.IsSuccessful` before using `response.Result`. Log the `response.Error` list for diagnostics.

   ```csharp
   if (!response.IsSuccessful)
   {
       _logger.LogError("API call failed: {Errors}", string.Join(", ", response.Error));
       // Return an appropriate error view or throw a custom exception
   }
   ```

4. **Resource Management**  
   `HttpClient` is managed by the DI container – **do not dispose it manually**.

5. **Response Types**  
   Always use `BaseResponseDto<T>` (or a similar wrapper) to ensure consistent error handling across all microservices.

6. **URL Building**  
   Prefer to store relative URLs in configuration and combine them with the base URL using `Uri` to avoid trailing slash issues.

---

## Example Controller Usage

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductController(IProductService productService, IHttpContextAccessor httpContextAccessor)
    {
        _productService = productService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 1)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", "");

        var result = await _productService.GetProductsAsync(pageSize, pageIndex, token);

        if (result.IsSuccessful)
            return Ok(result.Result);
        else
            return BadRequest(result.Error);
    }
}
```

---

## Full Class Code (for reference)

```csharp
using AppleShop.Web.Models.ProductService.Dtos;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices;
using Microsoft.AspNetCore.WebUtilities;
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

                if (!string.IsNullOrWhiteSpace(authorizationToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authorizationToken);

                if (headers != null)
                {
                    foreach (var h in headers)
                        request.Headers.TryAddWithoutValidation(h.Key, h.Value);
                }

                if (requestData != null)
                    request.Content = SerializeContent(requestData);

                var response = await _client.SendAsync(request);
                return await HandleResponse<T>(response);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                var dto = new BaseResponseDto<string>()
                {
                    Result = null,
                    Error = new List<string> { "Unauthorized access – please check your credentials.", Convert.ToString(ex.Message) },
                    IsSuccessful = false
                };
                var res = JsonSerializer.Serialize(dto);
                return JsonSerializer.Deserialize<T>(res)!;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                var dto = new BaseResponseDto<string>()
                {
                    Result = null,
                    Error = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccessful = false
                };
                var res = JsonSerializer.Serialize(dto);
                return JsonSerializer.Deserialize<T>(res)!;
            }
            catch (HttpRequestException ex)
            {
                var dto = new BaseResponseDto<string>()
                {
                    Result = null,
                    Error = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccessful = false
                };
                var res = JsonSerializer.Serialize(dto);
                return JsonSerializer.Deserialize<T>(res)!;
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
                return JsonSerializer.Deserialize<T>(res)!;
            }
        }

        private string BuildUrl(string baseUrl, string? path, Dictionary<string, string>? queryString)
        {
            var url = baseUrl;
            if (!string.IsNullOrEmpty(path))
                url = baseUrl.TrimEnd('/') + '/' + path.TrimStart('/');

            if (queryString != null && queryString.Any())
                url = QueryHelpers.AddQueryString(url, queryString);

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
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(jsonResponse))
                return default!;

            return JsonSerializer.Deserialize<T>(jsonResponse, _jsonOptions)
                   ?? throw new InvalidOperationException("Deserialization returned null.");
        }
    }
}
```

---

## Conclusion

`BaseApiConsumer` provides a robust, reusable foundation for all HTTP calls to backend microservices. By inheriting from it, you keep your code DRY, centralize error handling, and maintain a consistent pattern across your entire application. Use the examples above to quickly integrate new API clients and services.