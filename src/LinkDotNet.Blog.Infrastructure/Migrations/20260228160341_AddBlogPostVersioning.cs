using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkDotNet.Blog.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddBlogPostVersioning : Migration
{
    private static readonly string[] BlogPostVersionColumns = ["BlogPostId", "Version"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.CreateTable(
            name: "BlogPostVersions",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", unicode: false, nullable: false),
                BlogPostId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                Version = table.Column<int>(type: "INTEGER", nullable: false),
                Title = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                ShortDescription = table.Column<string>(type: "TEXT", nullable: false),
                Content = table.Column<string>(type: "TEXT", nullable: false),
                PreviewImageUrl = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                PreviewImageUrlFallback = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                Tags = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                ReadingTimeInMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                AuthorName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BlogPostVersions", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_BlogPostVersions_BlogPostId_Version",
            table: "BlogPostVersions",
            columns: BlogPostVersionColumns,
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
