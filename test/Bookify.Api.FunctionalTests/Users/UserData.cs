using Bookify.Api.Controllers.Users;

namespace Bookify.Api.FunctionalTests.Users;

public static class UserData
{
    public static RegisterUserRequest RegisterTestUserRequest = new("test", "test", "test@test.com", "12345");
}
