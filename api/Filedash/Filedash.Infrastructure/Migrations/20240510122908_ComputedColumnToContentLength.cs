using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Filedash.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ComputedColumnToContentLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ContentLength",
                table: "UploadedFiles",
                type: "bigint",
                nullable: false,
                computedColumnSql: "DATALENGTH([Content])",
                stored: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ContentLength",
                table: "UploadedFiles",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComputedColumnSql: "DATALENGTH([Content])");
        }
    }
}
