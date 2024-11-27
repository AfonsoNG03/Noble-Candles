using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Noble_Candles.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candles_Categories_CategoryId",
                table: "Candles");

            migrationBuilder.DropForeignKey(
                name: "FK_Candles_Colors_ColorId",
                table: "Candles");

            migrationBuilder.DropForeignKey(
                name: "FK_Candles_Fragrances_FragranceId",
                table: "Candles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fragrances",
                table: "Fragrances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Colors",
                table: "Colors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Fragrances",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Colors",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "FragranceId",
                table: "Candles",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(150)");

            migrationBuilder.AlterColumn<int>(
                name: "ColorId",
                table: "Candles",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(150)");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Candles",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fragrances",
                table: "Fragrances",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Colors",
                table: "Colors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Candles_Categories_CategoryId",
                table: "Candles",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Candles_Colors_ColorId",
                table: "Candles",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Candles_Fragrances_FragranceId",
                table: "Candles",
                column: "FragranceId",
                principalTable: "Fragrances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candles_Categories_CategoryId",
                table: "Candles");

            migrationBuilder.DropForeignKey(
                name: "FK_Candles_Colors_ColorId",
                table: "Candles");

            migrationBuilder.DropForeignKey(
                name: "FK_Candles_Fragrances_FragranceId",
                table: "Candles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fragrances",
                table: "Fragrances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Colors",
                table: "Colors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Fragrances");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "FragranceId",
                table: "Candles",
                type: "VARCHAR(150)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ColorId",
                table: "Candles",
                type: "VARCHAR(150)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryId",
                table: "Candles",
                type: "VARCHAR(100)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fragrances",
                table: "Fragrances",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Colors",
                table: "Colors",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Candles_Categories_CategoryId",
                table: "Candles",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Candles_Colors_ColorId",
                table: "Candles",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Candles_Fragrances_FragranceId",
                table: "Candles",
                column: "FragranceId",
                principalTable: "Fragrances",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
