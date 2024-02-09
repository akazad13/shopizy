using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Users.ValueObject;

namespace Shopizy.Domain.Users;

public sealed class User : AggregateRoot<UserId, Guid>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Phone { get; private set; }
    public string Password { get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }

    public static User Create(string firstName, string lastName, string phone, string password)
    {
        return new(
            UserId.CreateUnique(),
            firstName,
            lastName,
            phone,
            password,
            DateTime.UtcNow,
            DateTime.UtcNow
        );
    }

    private User(
        UserId id,
        string firstName,
        string lastName,
        string phone,
        string password,
        DateTime createdDateTime,
        DateTime updatedDateTime
    ) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Password = password;
        CreatedDateTime = createdDateTime;
        UpdatedDateTime = updatedDateTime;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
