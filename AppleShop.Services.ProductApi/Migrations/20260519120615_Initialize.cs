using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppleShop.Service.ProductApi.Migrations
{
    public partial class Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CreatedAt", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Sweet and crisp organic Gala apples. Grown without synthetic pesticides.", "https://picsum.photos/seed/gala-apple/400/300", "Organic Gala Apple", 0.99m },
                    { 2L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Ripe, yellow bananas – perfect for smoothies or a quick snack.", "https://picsum.photos/seed/banana/400/300", "Banana", 0.59m },
                    { 3L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Juicy, sweet organic strawberries. Washed and ready to eat.", "https://picsum.photos/seed/strawberries/400/300", "Organic Strawberries (1 lb)", 4.99m },
                    { 4L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Seedless navel oranges – high in Vitamin C.", "https://picsum.photos/seed/orange/400/300", "Navel Orange", 0.79m },
                    { 5L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Sweet, crunchy red grapes. No seeds, great for snacking.", "https://picsum.photos/seed/red-grapes/400/300", "Red Seedless Grapes (0.5 KG bag)", 3.49m },
                    { 6L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Sweet, aromatic honey mangoes with smooth, fiberless flesh.", "https://picsum.photos/seed/honey-mango/400/300", "Honey Mango", 2.49m },
                    { 7L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Sweet golden pineapple – ready to core and slice.", "https://picsum.photos/seed/pineapple/400/300", "Pineapple (whole)", 3.99m },
                    { 8L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Zesty green kiwis, rich in vitamin C and fiber.", "https://picsum.photos/seed/kiwi/400/300", "Kiwi Fruit (6-pack)", 3.29m },
                    { 9L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Plump, antioxidant-rich blueberries – perfect for baking or snacking.", "https://picsum.photos/seed/blueberries/400/300", "Fresh Blueberries (250 Grams)", 4.49m },
                    { 10L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Juicy, soft Bartlett pears with a classic sweet flavor.", "https://picsum.photos/seed/bartlett-pear/400/300", "Bartlett Pear", 1.29m },
                    { 11L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Premium yellow‑red Rainier cherries – very sweet and delicate.", "https://picsum.photos/seed/rainier-cherry/400/300", "Rainier Cherry (0.5 KG)", 6.99m },
                    { 12L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Dark red‑skinned plums with sweet, juicy interior.", "https://picsum.photos/seed/black-plum/400/300", "Black Plum", 1.49m },
                    { 13L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Strawberries, blueberries, raspberries, blackberries – frozen fresh.", "https://picsum.photos/seed/mixed-berries/400/300", "Frozen Mixed Berry Blend (0.5 KG)", 5.49m },
                    { 14L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Fresh, crisp organic cucumber – perfect for salads or snacking.", "https://picsum.photos/seed/cucumber/400/300", "Organic Cucumber", 1.29m },
                    { 15L, new DateTime(2026, 2, 15, 16, 30, 0, 0, DateTimeKind.Utc), "Soft, honey‑sweet fresh figs – eat as is or pair with cheese.", "https://picsum.photos/seed/figs/400/300", "Fresh Figs (6-pack)", 4.79m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
