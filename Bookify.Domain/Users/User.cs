using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users;
public sealed class User : Entity
{
    public User(Guid id, Firstname firstname, LastName lastName, Email email) : 
        base(id) 
    { 
        Firstname = firstname;
        Lastname = lastName;
        Email = email;
    } 

    public Firstname Firstname { get; set; }

    public LastName Lastname { get; set; }

    public Email Email { get; set; }

    public static User Create(Firstname firstName, LastName lastName, Email email)
    {
        var user = new User(Guid.NewGuid(), firstName, lastName, email);

        return user;
    }
}
