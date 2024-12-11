using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkDotNet.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BlogPost_MembersOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMembersOnly",
                table: "BlogPosts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMembersOnly",
                table: "BlogPosts");
        }
    }
}
