var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGraphQLServer().AddQueryType<Query>();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
});
app.Run();

public class Link
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreatedOn { get; set; }
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LinkId { get; set; }
    public Link Link { get; set; }
}

public class Query
{
    public IQueryable<Link> Links => new List<Link>
    {
        new Link
        {
            Id = 1,
            Url = "https://example.com",
            Title = "Example",
            Description = "This is an example link",
            ImageUrl = "https://example.com/image.png",
            Tags = new List<Tag> { new Tag(){ Name = "Example" } },
            CreatedOn = DateTime.Now
        },
        new Link
        {
            Id = 2,
            Url = "https://dotnetthoughts.net",
            Title = "DotnetThoughts",
            Description = "DotnetThoughts is a blog about .NET",
            ImageUrl = "https://dotnetthoughts.net/image.png",
            Tags = new List<Tag>
            {
                new Tag(){ Name = "Programming" },
                new Tag(){ Name = "Blog" },
                new Tag(){ Name = "dotnet" }
            },
            CreatedOn = DateTime.Now
        },
    }.AsQueryable();
}