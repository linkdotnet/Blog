using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkDotNet.Blog.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddBlogPostRecordIndex : Migration
{
    private static readonly string[] BlogPostRecordsColumns = ["BlogPostId", "DateClicked"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.CreateIndex(
            name: "IX_BlogPostRecords_BlogPostId_DateClicked",
            table: "BlogPostRecords",
            columns: BlogPostRecordsColumns);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropIndex(
            name: "IX_BlogPostRecords_BlogPostId_DateClicked",
            table: "BlogPostRecords");
    }
}
