using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkDotNet.Blog.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddBlogPostVersioning : Migration
{
    private static readonly string[] BlogPostVersionsColumns = ["BlogPostId", "VersionNumber"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.CreateTable(
            name: "BlogPostVersions",
            columns: table => new
            {
                Id = table.Column<string>(type: "varchar(900)", unicode: false, nullable: false),
                BlogPostId = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                VersionNumber = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PreviewImageUrl = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                PreviewImageUrlFallback = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Tags = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                IsPublished = table.Column<bool>(type: "bit", nullable: false),
                ReadingTimeInMinutes = table.Column<int>(type: "int", nullable: false),
                AuthorName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BlogPostVersions", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_BlogPostVersions_BlogPostId_VersionNumber",
            table: "BlogPostVersions",
            columns: BlogPostVersionsColumns,
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropTable(
            name: "BlogPostVersions");
    }
}
