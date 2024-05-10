using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Filedash.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEncodingType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EncodingType",
                table: "UploadedFiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncodingType",
                table: "UploadedFiles");
        }
    }
}
