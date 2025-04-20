using Bookify.Domain.Abstractions;
using Bookify.Domain.Users.Events;

namespace Bookify.Domain.Users;
public sealed class User : Entity
{
    public User(Guid id, FirstName firstname, LastName lastname, Email email) :
        base(id)
    {
        FirstName = firstname;
        LastName = lastname;
        Email = email;
    }

    public User()
    {
    }

    public FirstName FirstName { get; set; }

    public LastName LastName { get; set; }

    public Email Email { get; set; }

    public static User Create(FirstName firstName, LastName lastname, Email email)
    {
        var user = new User(Guid.NewGuid(), firstName, lastname, email);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        return user;
    }
}
