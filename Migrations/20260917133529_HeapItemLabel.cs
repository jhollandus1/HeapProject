using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeapProject.Migrations
{
    /// <inheritdoc />
    public partial class HeapItemLabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeapItemLabel",
                table: "HeapHistoryItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeapItemLabel",
                table: "HeapHistoryItem");
        }
    }
}
