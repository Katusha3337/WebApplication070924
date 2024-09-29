var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов в контейнер
builder.Services.AddSingleton<IUserService, UserService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapPost("/users", (User user, IUserService userService) =>
{
    userService.AddUser(user);
    return Results.Ok();
});

app.MapDelete("/users/{id}", (int id, IUserService userService) =>
{
    userService.DeleteUser(id);
    return Results.Ok();
});

app.MapGet("/users/{id}", (int id, IUserService userService) =>
{
    var user = userService.GetUser(id);
    return user != null ? Results.Ok(user) : Results.NotFound();
});

app.MapPut("/users", (User user, IUserService userService) =>
{
    userService.UpdateUser(user);
    return Results.Ok();
});

app.MapGet("/users", (IUserService userService) =>
{
    var users = userService.GetAllUsers();
    return Results.Ok(users);
});

app.MapFallbackToFile("index.html");

app.Run();

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
public interface IUserService
{
    void AddUser(User user);
    void DeleteUser(int id);
    User GetUser(int id);
    void UpdateUser(User user);
    IEnumerable<User> GetAllUsers();
}
public class UserService : IUserService
{
    private readonly List<User> _users = new List<User>();
    private int _nextId = 1;

    public void AddUser(User user)
    {
        user.Id = _nextId++;
        _users.Add(user);
    }

    public void DeleteUser(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            _users.Remove(user);
        }
    }

    public User GetUser(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public void UpdateUser(User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
        }
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _users;
    }
}