using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.web.Data.Migrations
{
    /// <inheritdoc />
    public partial class editImageUrlToimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Books",
                newName: "Image");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Books",
                newName: "ImageUrl");
        }
    }
}
