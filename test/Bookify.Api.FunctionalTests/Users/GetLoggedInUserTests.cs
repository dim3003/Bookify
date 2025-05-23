﻿using Bookify.Api.FunctionalTests.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using System.Net.Http.Headers;

namespace Bookify.Api.FunctionalTests.Users;

public class GetLoggedInUserTests : BaseFunctionalTests
{
    public GetLoggedInUserTests(FunctionalTestsWebAppFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task Get_ShouldReturnUnauthorized_WhenAccessTokenIsMissing()
    {
        // Act
        var response = await HttpClient.GetAsync("api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_ShouldReturnUser_WhenAccessTokenIsNotMissing()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);

        // Act
        var user = await HttpClient.GetAsync("api/v1/users/me");

        // Assert
        user.Should().NotBeNull();
    } 
}
