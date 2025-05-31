using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;

namespace FinanceTracker.ApiService.Tests.Infrastructure
{
    /// <summary>
    /// Extension methods for HttpClient to simplify integration testing
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Sends a GET request and asserts a successful response
        /// </summary>
        public static async Task<HttpResponseMessage> GetAndAssertSuccessAsync(this HttpClient client, string requestUri)
        {
            var response = await client.GetAsync(requestUri);
            response.IsSuccessStatusCode.Should().BeTrue($"GET {requestUri} should return success status code. Actual: {response.StatusCode}");
            return response;
        }

        /// <summary>
        /// Sends a GET request and returns the deserialized response
        /// </summary>
        public static async Task<T> GetAndDeserializeAsync<T>(this HttpClient client, string requestUri)
        {
            var response = await client.GetAndAssertSuccessAsync(requestUri);
            var content = await response.Content.ReadFromJsonAsync<T>();
            content.Should().NotBeNull($"Response from GET {requestUri} should deserialize to {typeof(T).Name}");
            return content!;
        }

        /// <summary>
        /// Sends a POST request with JSON content and asserts a successful response
        /// </summary>
        public static async Task<HttpResponseMessage> PostJsonAndAssertSuccessAsync<T>(this HttpClient client, string requestUri, T content)
        {
            var response = await client.PostAsJsonAsync(requestUri, content);
            response.IsSuccessStatusCode.Should().BeTrue($"POST {requestUri} should return success status code. Actual: {response.StatusCode}");
            return response;
        }

        /// <summary>
        /// Sends a POST request with JSON content and returns the deserialized response
        /// </summary>
        public static async Task<TResponse> PostJsonAndDeserializeAsync<TRequest, TResponse>(this HttpClient client, string requestUri, TRequest content)
        {
            var response = await client.PostJsonAndAssertSuccessAsync(requestUri, content);
            var responseContent = await response.Content.ReadFromJsonAsync<TResponse>();
            responseContent.Should().NotBeNull($"Response from POST {requestUri} should deserialize to {typeof(TResponse).Name}");
            return responseContent!;
        }

        /// <summary>
        /// Sends a PUT request with JSON content and asserts a successful response
        /// </summary>
        public static async Task<HttpResponseMessage> PutJsonAndAssertSuccessAsync<T>(this HttpClient client, string requestUri, T content)
        {
            var response = await client.PutAsJsonAsync(requestUri, content);
            response.IsSuccessStatusCode.Should().BeTrue($"PUT {requestUri} should return success status code. Actual: {response.StatusCode}");
            return response;
        }

        /// <summary>
        /// Sends a DELETE request and asserts a successful response
        /// </summary>
        public static async Task<HttpResponseMessage> DeleteAndAssertSuccessAsync(this HttpClient client, string requestUri)
        {
            var response = await client.DeleteAsync(requestUri);
            response.IsSuccessStatusCode.Should().BeTrue($"DELETE {requestUri} should return success status code. Actual: {response.StatusCode}");
            return response;
        }

        /// <summary>
        /// Asserts that a response has the expected status code
        /// </summary>
        public static void AssertStatusCode(this HttpResponseMessage response, System.Net.HttpStatusCode expectedStatusCode)
        {
            response.StatusCode.Should().Be(expectedStatusCode, $"Response should have status code {expectedStatusCode}");
        }

        /// <summary>
        /// Asserts that a response is a client error (4xx)
        /// </summary>
        public static void AssertClientError(this HttpResponseMessage response)
        {
            ((int)response.StatusCode).Should().BeInRange(400, 499, "Response should be a client error (4xx)");
        }

        /// <summary>
        /// Asserts that a response is a server error (5xx)
        /// </summary>
        public static void AssertServerError(this HttpResponseMessage response)
        {
            ((int)response.StatusCode).Should().BeInRange(500, 599, "Response should be a server error (5xx)");
        }
    }
}
