using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BookmarkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookmarkDbConnection")));
builder.Services.AddGraphQLServer().AddQueryType<Query>().AddProjections().AddFiltering().AddSorting();
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

public class BookmarkDbContext : DbContext
{
    public BookmarkDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<Link> Links { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Link>().HasData(new Link
        {
            Id = 1,
            Url = "https://example.com",
            Title = "Example",
            Description = "This is an example link",
            ImageUrl = "https://example.com/image.png",
            CreatedOn = DateTime.Now
        });

        modelBuilder.Entity<Link>().HasData(new Link
        {
            Id = 2,
            Url = "https://dotnetthoughts.net",
            Title = "DotnetThoughts",
            Description = "DotnetThoughts is a blog about .NET",
            ImageUrl = "https://dotnetthoughts.net/image.png",
            CreatedOn = DateTime.Now
        });

        modelBuilder.Entity<Tag>().HasData(new Tag
        {
            Id = 1,
            Name = "example",
            LinkId = 1
        });

        modelBuilder.Entity<Tag>().HasData(new Tag
        {
            Id = 2,
            Name = "dotnet",
            LinkId = 2
        });
        modelBuilder.Entity<Tag>().HasData(new Tag
        {
            Id = 3,
            Name = "blog",
            LinkId = 2
        });
    }
}


public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Link> Links([Service] BookmarkDbContext bookmarkDbContext)
            => bookmarkDbContext.Links;
}