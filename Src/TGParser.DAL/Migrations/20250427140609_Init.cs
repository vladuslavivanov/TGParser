using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TGParser.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Presets",
                columns: table => new
                {
                    PresetId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PresetName = table.Column<string>(type: "text", nullable: false),
                    MinPrice = table.Column<int>(type: "integer", nullable: false),
                    MaxPrice = table.Column<int>(type: "integer", nullable: false),
                    MaxViewsByOthersWorkers = table.Column<int>(type: "integer", nullable: false),
                    MaxViewsOnSite = table.Column<int>(type: "integer", nullable: false),
                    MinDateRegisterSeller = table.Column<DateTime>(type: "date", nullable: false),
                    MaxDateRegisterSeller = table.Column<DateTime>(type: "date", nullable: false),
                    MaxNumberOfPublishBySeller = table.Column<int>(type: "integer", nullable: false),
                    MaxNumbersOfItemsSoldBySeller = table.Column<int>(type: "integer", nullable: false),
                    MaxNumberOfItemsBuysBySeller = table.Column<int>(type: "integer", nullable: false),
                    PeriodSearch = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presets", x => x.PresetId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CountViewed = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "Proxies",
                columns: table => new
                {
                    ProxyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IP = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    ProxyType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proxies", x => x.ProxyId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubscriptionEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SelectedPresetId = table.Column<int>(type: "integer", nullable: false),
                    UserRole = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UserId1 = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_Invoices_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPresets",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PresetId = table.Column<int>(type: "integer", nullable: false),
                    IsSelected = table.Column<bool>(type: "boolean", nullable: false),
                    ShowedId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPresets", x => new { x.PresetId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserPresets_Presets_PresetId",
                        column: x => x.PresetId,
                        principalTable: "Presets",
                        principalColumn: "PresetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPresets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProxies",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ProxyId = table.Column<int>(type: "integer", nullable: false),
                    ShowedId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProxies", x => new { x.ProxyId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserProxies_Proxies_ProxyId",
                        column: x => x.ProxyId,
                        principalTable: "Proxies",
                        principalColumn: "ProxyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProxies_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserViewedItems",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TimeView = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserViewedItems", x => new { x.UserId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_UserViewedItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserViewedItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_UserId1",
                table: "Invoices",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserPresets_PresetId",
                table: "UserPresets",
                column: "PresetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPresets_UserId",
                table: "UserPresets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProxies_ProxyId",
                table: "UserProxies",
                column: "ProxyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProxies_UserId",
                table: "UserProxies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserViewedItems_ProductId",
                table: "UserViewedItems",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "UserPresets");

            migrationBuilder.DropTable(
                name: "UserProxies");

            migrationBuilder.DropTable(
                name: "UserViewedItems");

            migrationBuilder.DropTable(
                name: "Presets");

            migrationBuilder.DropTable(
                name: "Proxies");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
