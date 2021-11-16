using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraphQLDemo.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Links",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Links", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Links_LinkId",
                        column: x => x.LinkId,
                        principalTable: "Links",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Links",
                columns: new[] { "Id", "CreatedOn", "Description", "ImageUrl", "Title", "Url" },
                values: new object[] { 1, new DateTime(2021, 11, 16, 9, 35, 9, 839, DateTimeKind.Local).AddTicks(2011), "This is an example link", "https://example.com/image.png", "Example", "https://example.com" });

            migrationBuilder.InsertData(
                table: "Links",
                columns: new[] { "Id", "CreatedOn", "Description", "ImageUrl", "Title", "Url" },
                values: new object[] { 2, new DateTime(2021, 11, 16, 9, 35, 9, 839, DateTimeKind.Local).AddTicks(2049), "DotnetThoughts is a blog about .NET", "https://dotnetthoughts.net/image.png", "DotnetThoughts", "https://dotnetthoughts.net" });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "LinkId", "Name" },
                values: new object[] { 1, 1, "example" });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "LinkId", "Name" },
                values: new object[] { 2, 2, "dotnet" });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "LinkId", "Name" },
                values: new object[] { 3, 2, "blog" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_LinkId",
                table: "Tags",
                column: "LinkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Links");
        }
    }
}
