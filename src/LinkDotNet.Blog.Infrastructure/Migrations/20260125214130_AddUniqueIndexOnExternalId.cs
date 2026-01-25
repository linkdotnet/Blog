using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkDotNet.Blog.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddUniqueIndexOnExternalId : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.CreateIndex(
            name: "IX_BlogPosts_ExternalId",
            table: "BlogPosts",
            column: "ExternalId",
            unique: true,
            filter: "ExternalId IS NOT NULL");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropIndex(
            name: "IX_BlogPosts_ExternalId",
            table: "BlogPosts");
    }
}
