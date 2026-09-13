using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkDotNet.Blog.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ChangeBlogPostRecordIndex : Migration
{
    private static readonly string[] NewColumns = ["DateClicked", "BlogPostId", "Clicks"];

    private static readonly string[] OldColumns = ["BlogPostId", "DateClicked"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropIndex(
            name: "IX_BlogPostRecords_BlogPostId_DateClicked",
            table: "BlogPostRecords");

        migrationBuilder.CreateIndex(
            name: "IX_BlogPostRecords_DateClicked_BlogPostId_Clicks",
            table: "BlogPostRecords",
            columns: NewColumns);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropIndex(
            name: "IX_BlogPostRecords_DateClicked_BlogPostId_Clicks",
            table: "BlogPostRecords");

        migrationBuilder.CreateIndex(
            name: "IX_BlogPostRecords_BlogPostId_DateClicked",
            table: "BlogPostRecords",
            columns: OldColumns);
    }
}
