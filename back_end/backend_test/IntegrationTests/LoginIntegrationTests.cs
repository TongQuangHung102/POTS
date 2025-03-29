/*using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using backend.Dtos;
using backend.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace backend_test.IntegrationTests
{
    public class LoginIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public LoginIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
        {
            var request = new LoginRequestDto
            {
                Email = "notfound@example.com",
                Password = "password123"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/Login", request);

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
        {
            var request = new LoginRequestDto
            {
                Email = "test@example.com",
                Password = "wrongPassword"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/Login", request);

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_ShouldReturnForbidden_WhenUserIsInactive()
        {
            var request = new LoginRequestDto
            {
                Email = "inactive@example.com",
                Password = "password123"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/Login", request);

            Assert.Contains(response.StatusCode, new[] { System.Net.HttpStatusCode.Forbidden, System.Net.HttpStatusCode.Unauthorized });
        }
        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenPasswordIsNull()
        {
            var request = new LoginRequestDto
            {
                Email = "userwithoutpassword@example.com",
                Password = null
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/Login", request);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

    }
}
*/