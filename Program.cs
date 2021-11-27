using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContextFactory<BookmarkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookmarkDbConnection")));
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddMutationType<Mutation>();
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
    [UseDbContext(typeof(BookmarkDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Link> Links([ScopedService] BookmarkDbContext bookmarkDbContext)
            => bookmarkDbContext.Links;
}

public class Mutation
{
    [UseDbContext(typeof(BookmarkDbContext))]
    public async Task<LinkOutput> AddLink(LinkInput linkInput,
        [ScopedService] BookmarkDbContext bookmarkDbContext)
    {
        if (string.IsNullOrEmpty(linkInput.Url))
        {
            throw new ArgumentNullException(nameof(linkInput.Url));
        }

        var link = new Link
        {
            Url = linkInput.Url,
            Title = linkInput.Title,
            Description = linkInput.Description,
            ImageUrl = linkInput.ImageUrl,
            CreatedOn = DateTime.UtcNow
        };
        bookmarkDbContext.Links.Add(link);
        await bookmarkDbContext.SaveChangesAsync();
        return new LinkOutput(true, "Link created successfully", link.Id, link.Url,
            link.Title, link.Description, link.ImageUrl, link.CreatedOn);
    }

    [UseDbContext(typeof(BookmarkDbContext))]
    public async Task<LinkOutput> UpdateLink(int id, LinkInput linkInput, [ScopedService] BookmarkDbContext bookmarkDbContext)
    {
        var link = bookmarkDbContext.Links.Find(id);
        if (link != null)
        {
            if (link.Title != linkInput.Title)
            {
                link.Title = linkInput.Title;
            }
            if (link.Description != linkInput.Description)
            {
                link.Description = linkInput.Description;
            }
            if (link.ImageUrl != linkInput.ImageUrl)
            {
                link.ImageUrl = linkInput.ImageUrl;
            }
            if (link.Url != linkInput.Url)
            {
                link.Url = linkInput.Url;
            }

            await bookmarkDbContext.SaveChangesAsync();
            return new LinkOutput(true, "Link updated successfully", link.Id, link.Url, link.Title, link.Description, link.ImageUrl, link.CreatedOn);
        }

        return new LinkOutput(false, "Unable to update the Link.", null, null, null, null, null, null);
    }

    [UseDbContext(typeof(BookmarkDbContext))]
    public async Task<LinkOutput> DeleteLink(int id, [ScopedService] BookmarkDbContext bookmarkDbContext)
    {
        var link = bookmarkDbContext.Links.Find(id);
        if (link != null)
        {
            bookmarkDbContext.Links.Remove(link);
            await bookmarkDbContext.SaveChangesAsync();
            return new LinkOutput(true, "Link deleted successfully.", null, null, null, null, null, null);
        }

        return new LinkOutput(false, "Link not found.", null, null, null, null, null, null);
    }
}

public record LinkInput(string Url = "", string Title = "", string Description = "", string ImageUrl = "");
public record LinkOutput(bool IsSuccess, string Message, int? Id, string? Url, string? Title, string? Description, string? ImageUrl, DateTime? CreatedOn);