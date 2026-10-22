using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppleShop.Services.CouponApi.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    CouponId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CouponCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DiscountAmount = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.CouponId);
                });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "CouponId", "CouponCode", "CreatedAt", "DiscountAmount" },
                values: new object[,]
                {
                    { 1, "10off", new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), 10.0 },
                    { 2, "20off", new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), 20.0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_CouponCode",
                table: "Coupons",
                column: "CouponCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");
        }
    }
}
