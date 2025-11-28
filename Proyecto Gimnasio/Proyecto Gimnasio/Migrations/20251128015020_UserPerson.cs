using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_Gimnasio.Migrations
{
    /// <inheritdoc />
    public partial class UserPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_saleDetailsPlans_Planss_PlansIdPlan",
                table: "saleDetailsPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_saleDetailsPlans_Sales_SaleIdSale",
                table: "saleDetailsPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_saleDetailsProducts_Products_ProductIdProduct",
                table: "saleDetailsProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_saleDetailsProducts_Sales_SaleIdSale",
                table: "saleDetailsProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Persons_PersonIdPerson",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_PersonIdPerson",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_saleDetailsProducts_ProductIdProduct",
                table: "saleDetailsProducts");

            migrationBuilder.DropIndex(
                name: "IX_saleDetailsProducts_SaleIdSale",
                table: "saleDetailsProducts");

            migrationBuilder.DropIndex(
                name: "IX_saleDetailsPlans_PlansIdPlan",
                table: "saleDetailsPlans");

            migrationBuilder.DropIndex(
                name: "IX_saleDetailsPlans_SaleIdSale",
                table: "saleDetailsPlans");

            migrationBuilder.DropColumn(
                name: "IdPerson",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PersonIdPerson",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "ProductIdProduct",
                table: "saleDetailsProducts");

            migrationBuilder.DropColumn(
                name: "SaleIdSale",
                table: "saleDetailsProducts");

            migrationBuilder.DropColumn(
                name: "PlansIdPlan",
                table: "saleDetailsPlans");

            migrationBuilder.DropColumn(
                name: "SaleIdSale",
                table: "saleDetailsPlans");

            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "primarySession",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "NameProduct",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NamePlan",
                table: "Planss",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Planss",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AddColumn<int>(
                name: "IdPerson",
                table: "Planss",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "SecondLastName",
                table: "Persons",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Persons",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "LasName",
                table: "Persons",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NameCategory",
                table: "Categories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_saleDetailsProducts_IdProduct",
                table: "saleDetailsProducts",
                column: "IdProduct");

            migrationBuilder.CreateIndex(
                name: "IX_saleDetailsProducts_IdSale",
                table: "saleDetailsProducts",
                column: "IdSale");

            migrationBuilder.CreateIndex(
                name: "IX_saleDetailsPlans_IdPlan",
                table: "saleDetailsPlans",
                column: "IdPlan");

            migrationBuilder.CreateIndex(
                name: "IX_saleDetailsPlans_IdSale",
                table: "saleDetailsPlans",
                column: "IdSale");

            migrationBuilder.CreateIndex(
                name: "IX_Planss_IdPerson",
                table: "Planss",
                column: "IdPerson");

            migrationBuilder.AddForeignKey(
                name: "FK_Planss_Persons_IdPerson",
                table: "Planss",
                column: "IdPerson",
                principalTable: "Persons",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_saleDetailsPlans_Planss_IdPlan",
                table: "saleDetailsPlans",
                column: "IdPlan",
                principalTable: "Planss",
                principalColumn: "IdPlan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_saleDetailsPlans_Sales_IdSale",
                table: "saleDetailsPlans",
                column: "IdSale",
                principalTable: "Sales",
                principalColumn: "IdSale",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_saleDetailsProducts_Products_IdProduct",
                table: "saleDetailsProducts",
                column: "IdProduct",
                principalTable: "Products",
                principalColumn: "IdProduct",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_saleDetailsProducts_Sales_IdSale",
                table: "saleDetailsProducts",
                column: "IdSale",
                principalTable: "Sales",
                principalColumn: "IdSale",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Planss_Persons_IdPerson",
                table: "Planss");

            migrationBuilder.DropForeignKey(
                name: "FK_saleDetailsPlans_Planss_IdPlan",
                table: "saleDetailsPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_saleDetailsPlans_Sales_IdSale",
                table: "saleDetailsPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_saleDetailsProducts_Products_IdProduct",
                table: "saleDetailsProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_saleDetailsProducts_Sales_IdSale",
                table: "saleDetailsProducts");

            migrationBuilder.DropIndex(
                name: "IX_saleDetailsProducts_IdProduct",
                table: "saleDetailsProducts");

            migrationBuilder.DropIndex(
                name: "IX_saleDetailsProducts_IdSale",
                table: "saleDetailsProducts");

            migrationBuilder.DropIndex(
                name: "IX_saleDetailsPlans_IdPlan",
                table: "saleDetailsPlans");

            migrationBuilder.DropIndex(
                name: "IX_saleDetailsPlans_IdSale",
                table: "saleDetailsPlans");

            migrationBuilder.DropIndex(
                name: "IX_Planss_IdPerson",
                table: "Planss");

            migrationBuilder.DropColumn(
                name: "primarySession",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IdPerson",
                table: "Planss");

            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "IdPerson",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PersonIdPerson",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductIdProduct",
                table: "saleDetailsProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleIdSale",
                table: "saleDetailsProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlansIdPlan",
                table: "saleDetailsPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleIdSale",
                table: "saleDetailsPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "NameProduct",
                table: "Products",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "NamePlan",
                table: "Planss",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Planss",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "SecondLastName",
                table: "Persons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Persons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "LasName",
                table: "Persons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "NameCategory",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PersonIdPerson",
                table: "Sales",
                column: "PersonIdPerson");

            migrationBuilder.CreateIndex(
                name: "IX_saleDetailsProducts_ProductIdProduct",
                table: "saleDetailsProducts",
                column: "ProductIdProduct");

            migrationBuilder.CreateIndex(
                name: "IX_saleDetailsProducts_SaleIdSale",
                table: "saleDetailsProducts",
                column: "SaleIdSale");

            migrationBuilder.CreateIndex(
                name: "IX_saleDetailsPlans_PlansIdPlan",
                table: "saleDetailsPlans",
                column: "PlansIdPlan");

            migrationBuilder.CreateIndex(
                name: "IX_saleDetailsPlans_SaleIdSale",
                table: "saleDetailsPlans",
                column: "SaleIdSale");

            migrationBuilder.AddForeignKey(
                name: "FK_saleDetailsPlans_Planss_PlansIdPlan",
                table: "saleDetailsPlans",
                column: "PlansIdPlan",
                principalTable: "Planss",
                principalColumn: "IdPlan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_saleDetailsPlans_Sales_SaleIdSale",
                table: "saleDetailsPlans",
                column: "SaleIdSale",
                principalTable: "Sales",
                principalColumn: "IdSale",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_saleDetailsProducts_Products_ProductIdProduct",
                table: "saleDetailsProducts",
                column: "ProductIdProduct",
                principalTable: "Products",
                principalColumn: "IdProduct",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_saleDetailsProducts_Sales_SaleIdSale",
                table: "saleDetailsProducts",
                column: "SaleIdSale",
                principalTable: "Sales",
                principalColumn: "IdSale",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Persons_PersonIdPerson",
                table: "Sales",
                column: "PersonIdPerson",
                principalTable: "Persons",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
