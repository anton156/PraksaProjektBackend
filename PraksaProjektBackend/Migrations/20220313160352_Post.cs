using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PraksaProjektBackend.Migrations
{
    public partial class Post : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.PostId);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9d8f8ee9-49d1-4f84-8afa-aed670f9fa45", "AQAAAAEAACcQAAAAEBkod3b+/Ntc4wphtfdndWqcd+s7BpxI8ryw1NynjRVRBYL0jIzHc+nHY5IH8S2ggA==", "2ec354d0-cd53-47ad-b3b1-99074be36917" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f0ac4392-3aa1-44b3-b78c-243f6b4e7b6f", "AQAAAAEAACcQAAAAEGrriZ6ELJQgO7DBeJ8qy8QCtvo3h8aGeQqNlMdWav9Fu/pEEh31ENKPNXDRCJWsVA==", "455dd84b-5ec3-413e-a33a-f3be5a52132f" });
        }
    }
}
