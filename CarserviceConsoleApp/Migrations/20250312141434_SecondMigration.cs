using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarserviceConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Order_assignments_fk1",
                table: "Order_assignments");

            migrationBuilder.DropForeignKey(
                name: "Order_assignments_fk2",
                table: "Order_assignments");

            migrationBuilder.AddForeignKey(
                name: "Order_assignments_fk1",
                table: "Order_assignments",
                column: "order_id",
                principalTable: "Orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Order_assignments_fk2",
                table: "Order_assignments",
                column: "employee_id",
                principalTable: "Employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Order_assignments_fk1",
                table: "Order_assignments");

            migrationBuilder.DropForeignKey(
                name: "Order_assignments_fk2",
                table: "Order_assignments");

            migrationBuilder.AddForeignKey(
                name: "Order_assignments_fk1",
                table: "Order_assignments",
                column: "order_id",
                principalTable: "Orders",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "Order_assignments_fk2",
                table: "Order_assignments",
                column: "employee_id",
                principalTable: "Employees",
                principalColumn: "id");
        }
    }
}
