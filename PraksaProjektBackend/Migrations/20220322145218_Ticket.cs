using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PraksaProjektBackend.Migrations
{
    public partial class Ticket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Profit",
                table: "Event",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "CurrentEvent",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    ticketId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    validUses = table.Column<int>(type: "int", nullable: false),
                    chargeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    userId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    userEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    qrPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    valid = table.Column<bool>(type: "bit", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    eventId = table.Column<int>(type: "int", nullable: false),
                    CurrentEventId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.ticketId);
                    table.ForeignKey(
                        name: "FK_Ticket_CurrentEvent_CurrentEventId",
                        column: x => x.CurrentEventId,
                        principalTable: "CurrentEvent",
                        principalColumn: "CurrentEventId");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b903c833-8d24-4036-853d-71b00bc04d18", "AQAAAAEAACcQAAAAEOU9rBvjClX/lJAQbE3RGbM5j8YqmQsUzWqiurPFHPmnjNkzhGgvSLvZx2NTDZqIzQ==", "bb3419b7-40e6-46f5-9f29-18a103f2461c" });

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_CurrentEventId",
                table: "Ticket",
                column: "CurrentEventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.AlterColumn<float>(
                name: "Profit",
                table: "Event",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "CurrentEvent",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a3a39efb-499f-45fa-ac76-403864b5a09c", "AQAAAAEAACcQAAAAEECaU7v139dUJJyEu5WxFgcArRs3HylEpnkutPKtwj+8iXP3gb6rC1sv2zjhFcj5JA==", "885549ec-3c3e-470b-b3db-46951fd7dc7d" });
        }
    }
}
