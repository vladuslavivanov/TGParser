using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGParser.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityDaysPropToInvoiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantityDays",
                table: "Invoices",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityDays",
                table: "Invoices");
        }
    }
}
