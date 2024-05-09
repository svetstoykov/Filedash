using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Filedash.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContentLengthColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "UploadedFiles",
                newName: "CreatedDateUtc");

            migrationBuilder.AddColumn<long>(
                name: "ContentLength",
                table: "UploadedFiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentLength",
                table: "UploadedFiles");

            migrationBuilder.RenameColumn(
                name: "CreatedDateUtc",
                table: "UploadedFiles",
                newName: "CreatedDate");
        }
    }
}
