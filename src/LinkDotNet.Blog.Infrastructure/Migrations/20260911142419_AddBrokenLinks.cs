using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkDotNet.Blog.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddBrokenLinks : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.CreateTable(
            name: "BrokenLinks",
            columns: table => new
            {
                Id = table.Column<string>(type: "varchar(900)", unicode: false, nullable: false),
                BlogPostId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                BlogPostTitle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Reason = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                CheckedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BrokenLinks", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropTable(
            name: "BrokenLinks");
    }
}
