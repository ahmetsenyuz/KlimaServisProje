using Microsoft.EntityFrameworkCore.Migrations;

namespace KlimaServisProje.Migrations
{
    public partial class UpdateTroubleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Finished",
                table: "TroubleRegisters",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Finished",
                table: "TroubleRegisters");
        }
    }
}
