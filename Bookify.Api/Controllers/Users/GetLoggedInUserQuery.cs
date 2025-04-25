using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Users.GetLoggedInUser;

namespace Bookify.Api.Controllers.Users;

public sealed record GetLoggedInUserQuery : IQuery<UserResponse>;