using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkDotNet.Blog.Web.Migrations;

/// <inheritdoc />
public partial class AddBlogPostTemplate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.AlterColumn<string>(
            name: "AuthorName",
            table: "BlogPosts",
            type: "TEXT",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(256)",
            oldMaxLength: 256,
            oldNullable: true);

        migrationBuilder.CreateTable(
            name: "BlogPostTemplates",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", unicode: false, nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                Title = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                ShortDescription = table.Column<string>(type: "TEXT", nullable: false),
                Content = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BlogPostTemplates", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropTable(
            name: "BlogPostTemplates");

        migrationBuilder.AlterColumn<string>(
            name: "AuthorName",
            table: "BlogPosts",
            type: "TEXT",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(256)",
            oldMaxLength: 256,
            oldNullable: true);
    }
}