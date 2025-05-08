using Bookify.Api.Controllers.Users;
using Bookify.Api.FunctionalTests.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Bookify.Api.FunctionalTests.Users;

public class LoginUserTests : BaseFunctionalTests
{
    private const string Email = "login@test.com";
    private const string Password = "12345";

    public LoginUserTests(FunctionalTestsWebAppFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new LogInUserRequest(Email, Password);

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/users/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest("first", "last", Email, Password);
        await HttpClient.PostAsJsonAsync("api/v1/users/register", registerRequest);

        var request = new LogInUserRequest(Email, Password);

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/users/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
