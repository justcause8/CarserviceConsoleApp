using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarserviceConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class ThirdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Cars_fk1",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "Order_parts_fk1",
                table: "Order_parts");

            migrationBuilder.DropForeignKey(
                name: "Order_parts_fk2",
                table: "Order_parts");

            migrationBuilder.DropForeignKey(
                name: "Order_services_fk1",
                table: "Order_services");

            migrationBuilder.DropForeignKey(
                name: "Order_services_fk2",
                table: "Order_services");

            migrationBuilder.DropForeignKey(
                name: "Orders_fk1",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "Cars_fk1",
                table: "Cars",
                column: "client_id",
                principalTable: "Clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Order_parts_fk1",
                table: "Order_parts",
                column: "order_id",
                principalTable: "Orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Order_parts_fk2",
                table: "Order_parts",
                column: "part_id",
                principalTable: "Parts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Order_services_fk1",
                table: "Order_services",
                column: "order_id",
                principalTable: "Orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Order_services_fk2",
                table: "Order_services",
                column: "service_id",
                principalTable: "Services",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "Orders_fk1",
                table: "Orders",
                column: "car_id",
                principalTable: "Cars",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Cars_fk1",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "Order_parts_fk1",
                table: "Order_parts");

            migrationBuilder.DropForeignKey(
                name: "Order_parts_fk2",
                table: "Order_parts");

            migrationBuilder.DropForeignKey(
                name: "Order_services_fk1",
                table: "Order_services");

            migrationBuilder.DropForeignKey(
                name: "Order_services_fk2",
                table: "Order_services");

            migrationBuilder.DropForeignKey(
                name: "Orders_fk1",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "Cars_fk1",
                table: "Cars",
                column: "client_id",
                principalTable: "Clients",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "Order_parts_fk1",
                table: "Order_parts",
                column: "order_id",
                principalTable: "Orders",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "Order_parts_fk2",
                table: "Order_parts",
                column: "part_id",
                principalTable: "Parts",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "Order_services_fk1",
                table: "Order_services",
                column: "order_id",
                principalTable: "Orders",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "Order_services_fk2",
                table: "Order_services",
                column: "service_id",
                principalTable: "Services",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "Orders_fk1",
                table: "Orders",
                column: "car_id",
                principalTable: "Cars",
                principalColumn: "id");
        }
    }
}
