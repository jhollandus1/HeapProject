using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeapProject.Migrations.ApplicationDB
{
    /// <inheritdoc />
    public partial class PreferredTimeZone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreferredTimeZone",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredTimeZone",
                table: "AspNetUsers");
        }
    }
}
