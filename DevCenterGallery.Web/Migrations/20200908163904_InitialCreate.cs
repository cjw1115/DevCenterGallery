using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevCenterGallery.Web.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(nullable: true),
                    SasUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BigId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    LogoUri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubmissionId = table.Column<string>(nullable: true),
                    FriendlyName = table.Column<string>(nullable: true),
                    PublishedDateTime = table.Column<DateTime>(nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(nullable: false),
                    ReleaseRank = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submissions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Asset",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssetType = table.Column<string>(nullable: true),
                    FileInfoId = table.Column<int>(nullable: true),
                    PackageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asset_FileInfo_FileInfoId",
                        column: x => x.FileInfoId,
                        principalTable: "FileInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PackageId = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    PackageVersion = table.Column<string>(nullable: true),
                    Architecture = table.Column<string>(nullable: true),
                    PcakgeFileInfoId = table.Column<int>(nullable: true),
                    TargetPlatformId = table.Column<int>(nullable: true),
                    PreinstallKitStatus = table.Column<int>(nullable: false),
                    SubmissionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_FileInfo_PcakgeFileInfoId",
                        column: x => x.PcakgeFileInfoId,
                        principalTable: "FileInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Packages_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TargetPlatform",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MinVersion = table.Column<string>(nullable: true),
                    PlatformName = table.Column<string>(nullable: true),
                    PackageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetPlatform", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetPlatform_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asset_FileInfoId",
                table: "Asset",
                column: "FileInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_PackageId",
                table: "Asset",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_PcakgeFileInfoId",
                table: "Packages",
                column: "PcakgeFileInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_SubmissionId",
                table: "Packages",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_TargetPlatformId",
                table: "Packages",
                column: "TargetPlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ProductId",
                table: "Submissions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetPlatform_PackageId",
                table: "TargetPlatform",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Packages_PackageId",
                table: "Asset",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_TargetPlatform_TargetPlatformId",
                table: "Packages",
                column: "TargetPlatformId",
                principalTable: "TargetPlatform",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_FileInfo_PcakgeFileInfoId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_TargetPlatform_Packages_PackageId",
                table: "TargetPlatform");

            migrationBuilder.DropTable(
                name: "Asset");

            migrationBuilder.DropTable(
                name: "FileInfo");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "TargetPlatform");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
