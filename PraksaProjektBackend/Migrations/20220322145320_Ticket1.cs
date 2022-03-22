using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PraksaProjektBackend.Migrations
{
    public partial class Ticket1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8a59ffb9-6d5d-49d9-9c1f-fd7dcf85061a", "AQAAAAEAACcQAAAAEK69P05OiZqcthDSF5vBvbsBv5dbH6emEfDy9BnWwrzk47HA86WkG09NtVyxiE3hzA==", "84453299-4a5a-4ef7-9a0d-3896ac1f7416" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b903c833-8d24-4036-853d-71b00bc04d18", "AQAAAAEAACcQAAAAEOU9rBvjClX/lJAQbE3RGbM5j8YqmQsUzWqiurPFHPmnjNkzhGgvSLvZx2NTDZqIzQ==", "bb3419b7-40e6-46f5-9f29-18a103f2461c" });
        }
    }
}
