public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new List<User>
    {
        new User(1, "Alice", "alice@domain.com"),
        new User(2, "Bob", "bob@domain.com"),
        new User(3, "Jeff", "jeff@domain.com")
    };

    public List<User> GetAll()
    {
        return _users;
    }

    public User GetById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public void Post(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }
        user.Id = _users.Count + 1; // Assign a new ID
        _users.Add(user);
    }

    public void Put(int id, User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }
        var existingUser = GetById(id);
        if (existingUser == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }
        existingUser.Name = user.Name;
        existingUser.Email = user.Email;
    }

    public void Delete(int id)
    {
        var user = GetById(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }
        _users.Remove(user);
    }
}