var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
var configuration = builder.Configuration;

builder.Services.Configure<List<Book>>(configuration.GetSection("Books"));
builder.Services.Configure<UserProfile>(configuration.GetSection("UserProfile"));
builder.Services.Configure<List<UserProfile>>(configuration.GetSection("UserProfiles"));

var app = builder.Build();

app.MapGet("/Library", () =>
{
    return "Welcome to the Library!";
});

app.MapGet("/Library/Books", () =>
{
    var books = configuration.Get<List<Book>>();
    var response = string.Join("\n", books.Select(book => $"{book.Title} by {book.Author}"));
    return response;
});

app.MapGet("/Library/Profile/{id?}", (HttpContext context, int? id) =>
{
    if (!id.HasValue)
    {
        var userProfile = configuration.Get<UserProfile>();
        return context.Response.WriteAsJsonAsync(userProfile);
    }
    else if (id >= 0 && id <= 5)
    {
        var userProfiles = configuration.Get<List<UserProfile>>();
        var userProfile = userProfiles.FirstOrDefault(u => u.Id == id);
        if (userProfile != null)
        {
            return context.Response.WriteAsJsonAsync(userProfile);
        }
    }

    return context.Response.WriteAsync("Invalid user id.");
});

app.Run();
public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
}

public class UserProfile
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}