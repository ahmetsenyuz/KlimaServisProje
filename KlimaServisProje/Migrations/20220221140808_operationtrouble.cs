using Microsoft.EntityFrameworkCore.Migrations;

namespace KlimaServisProje.Migrations
{
    public partial class operationtrouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TroubleOperations",
                columns: table => new
                {
                    TroubleId = table.Column<int>(type: "int", nullable: false),
                    OperationId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TroubleOperations", x => new { x.TroubleId, x.OperationId });
                    table.ForeignKey(
                        name: "FK_TroubleOperations_OperationPrices_OperationId",
                        column: x => x.OperationId,
                        principalTable: "OperationPrices",
                        principalColumn: "operationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TroubleOperations_TroubleRegisters_TroubleId",
                        column: x => x.TroubleId,
                        principalTable: "TroubleRegisters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TroubleOperations_OperationId",
                table: "TroubleOperations",
                column: "OperationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TroubleOperations");
        }
    }
}
