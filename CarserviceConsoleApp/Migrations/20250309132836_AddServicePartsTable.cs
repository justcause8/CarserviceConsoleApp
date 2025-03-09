using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarserviceConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class AddServicePartsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Clients__3213E83FFE56F497", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__3213E83FBEDFDAE0", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Parts__3213E83F93B0D7B3", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Services__3213E83FE45EA95B", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    client_id = table.Column<int>(type: "int", nullable: false),
                    brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    year = table.Column<DateOnly>(type: "date", nullable: false),
                    vin = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cars__3213E83FDD63F329", x => x.id);
                    table.ForeignKey(
                        name: "Cars_fk1",
                        column: x => x.client_id,
                        principalTable: "Clients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    part_id = table.Column<int>(type: "int", nullable: false),
                    stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Inventor__3213E83FAD1B48CF", x => x.id);
                    table.ForeignKey(
                        name: "Inventory_fk1",
                        column: x => x.part_id,
                        principalTable: "Parts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceParts",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    PartId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceParts", x => new { x.ServiceId, x.PartId });
                    table.ForeignKey(
                        name: "FK_ServiceParts_Parts_PartId",
                        column: x => x.PartId,
                        principalTable: "Parts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceParts_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    car_id = table.Column<int>(type: "int", nullable: false),
                    client_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    completed_at = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__3213E83F6F81E7F8", x => x.id);
                    table.ForeignKey(
                        name: "Orders_fk1",
                        column: x => x.car_id,
                        principalTable: "Cars",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Orders_fk2",
                        column: x => x.client_id,
                        principalTable: "Clients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Order_assignments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    employee_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Order_as__3213E83F49F0FD4F", x => x.id);
                    table.ForeignKey(
                        name: "Order_assignments_fk1",
                        column: x => x.order_id,
                        principalTable: "Orders",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Order_assignments_fk2",
                        column: x => x.employee_id,
                        principalTable: "Employees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Order_parts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    part_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Order_pa__3213E83F78E3B22F", x => x.id);
                    table.ForeignKey(
                        name: "Order_parts_fk1",
                        column: x => x.order_id,
                        principalTable: "Orders",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Order_parts_fk2",
                        column: x => x.part_id,
                        principalTable: "Parts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Order_services",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    service_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Order_se__3213E83F6BACFC8E", x => x.id);
                    table.ForeignKey(
                        name: "Order_services_fk1",
                        column: x => x.order_id,
                        principalTable: "Orders",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "Order_services_fk2",
                        column: x => x.service_id,
                        principalTable: "Services",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_client_id",
                table: "Cars",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Cars__3213E83E9B763F19",
                table: "Cars",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Clients__3213E83EF96C9753",
                table: "Clients",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Employee__3213E83E3CB6D2C8",
                table: "Employees",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_part_id",
                table: "Inventory",
                column: "part_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Inventor__3213E83EB1F51875",
                table: "Inventory",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_assignments_employee_id",
                table: "Order_assignments",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_assignments_order_id",
                table: "Order_assignments",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Order_as__3213E83E85CB8FA2",
                table: "Order_assignments",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_parts_order_id",
                table: "Order_parts",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_parts_part_id",
                table: "Order_parts",
                column: "part_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Order_pa__3213E83E5FF29685",
                table: "Order_parts",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_services_order_id",
                table: "Order_services",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_services_service_id",
                table: "Order_services",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Order_se__3213E83E3286ED1D",
                table: "Order_services",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_car_id",
                table: "Orders",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_client_id",
                table: "Orders",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Orders__3213E83ECFC3255A",
                table: "Orders",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Parts__3213E83E554EC61E",
                table: "Parts",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceParts_PartId",
                table: "ServiceParts",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "UQ__Services__3213E83E04F93EE2",
                table: "Services",
                column: "id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "Order_assignments");

            migrationBuilder.DropTable(
                name: "Order_parts");

            migrationBuilder.DropTable(
                name: "Order_services");

            migrationBuilder.DropTable(
                name: "ServiceParts");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
