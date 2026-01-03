using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class ggg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banner_Stores_StoreId",
                table: "Banner");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_OwnerId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Users_UserId",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_SubCategories_SubcategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Varients_VariantId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Users_UserId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategories_Categories_CategoryId",
                table: "SubCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategories_Stores_StoreId",
                table: "SubCategories");

            migrationBuilder.AlterColumn<int>(
                name: "Percentage",
                table: "ProductVariants",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddForeignKey(
                name: "FK_Banner_Stores_StoreId",
                table: "Banner",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_OwnerId",
                table: "Categories",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Users_UserId",
                table: "Deliveries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SubCategories_SubcategoryId",
                table: "Products",
                column: "SubcategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Varients_VariantId",
                table: "ProductVariants",
                column: "VariantId",
                principalTable: "Varients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Users_UserId",
                table: "Stores",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategories_Categories_CategoryId",
                table: "SubCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategories_Stores_StoreId",
                table: "SubCategories",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banner_Stores_StoreId",
                table: "Banner");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_OwnerId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Users_UserId",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_SubCategories_SubcategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Varients_VariantId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Users_UserId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategories_Categories_CategoryId",
                table: "SubCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategories_Stores_StoreId",
                table: "SubCategories");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percentage",
                table: "ProductVariants",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Banner_Stores_StoreId",
                table: "Banner",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_OwnerId",
                table: "Categories",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Users_UserId",
                table: "Deliveries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SubCategories_SubcategoryId",
                table: "Products",
                column: "SubcategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Varients_VariantId",
                table: "ProductVariants",
                column: "VariantId",
                principalTable: "Varients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Users_UserId",
                table: "Stores",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategories_Categories_CategoryId",
                table: "SubCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategories_Stores_StoreId",
                table: "SubCategories",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
