using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductCatalogApi.Data.Migrations
{
    public partial class updatecatalogtablename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItem_CatalogBrand_CatalogBrandId",
                table: "CatalogItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItem_CatalogType_CatalogTypeId",
                table: "CatalogItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CatalogItem",
                table: "CatalogItem");

            migrationBuilder.RenameTable(
                name: "CatalogItem",
                newName: "Catalog");

            migrationBuilder.RenameIndex(
                name: "IX_CatalogItem_CatalogTypeId",
                table: "Catalog",
                newName: "IX_Catalog_CatalogTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_CatalogItem_CatalogBrandId",
                table: "Catalog",
                newName: "IX_Catalog_CatalogBrandId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Catalog",
                table: "Catalog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_CatalogBrand_CatalogBrandId",
                table: "Catalog",
                column: "CatalogBrandId",
                principalTable: "CatalogBrand",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_CatalogType_CatalogTypeId",
                table: "Catalog",
                column: "CatalogTypeId",
                principalTable: "CatalogType",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catalog_CatalogBrand_CatalogBrandId",
                table: "Catalog");

            migrationBuilder.DropForeignKey(
                name: "FK_Catalog_CatalogType_CatalogTypeId",
                table: "Catalog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Catalog",
                table: "Catalog");

            migrationBuilder.RenameTable(
                name: "Catalog",
                newName: "CatalogItem");

            migrationBuilder.RenameIndex(
                name: "IX_Catalog_CatalogTypeId",
                table: "CatalogItem",
                newName: "IX_CatalogItem_CatalogTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Catalog_CatalogBrandId",
                table: "CatalogItem",
                newName: "IX_CatalogItem_CatalogBrandId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatalogItem",
                table: "CatalogItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItem_CatalogBrand_CatalogBrandId",
                table: "CatalogItem",
                column: "CatalogBrandId",
                principalTable: "CatalogBrand",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItem_CatalogType_CatalogTypeId",
                table: "CatalogItem",
                column: "CatalogTypeId",
                principalTable: "CatalogType",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
