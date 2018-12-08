using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HostingUserMgmt.Repository.Migrations
{
    public partial class AuditDateTime_RoleEntitlement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "Users",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "ApiCredentials",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "ApiCredentials",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAtUtc",
                table: "ApiCredentials",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Entitlements",
                columns: table => new
                {
                    CreatedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entitlements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    CreatedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleEntitlement",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    EntitlementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleEntitlement", x => new { x.RoleId, x.EntitlementId });
                    table.ForeignKey(
                        name: "FK_RoleEntitlement_Entitlements_EntitlementId",
                        column: x => x.EntitlementId,
                        principalTable: "Entitlements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleEntitlement_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiCredentials_Username",
                table: "ApiCredentials",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleEntitlement_EntitlementId",
                table: "RoleEntitlement",
                column: "EntitlementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleEntitlement");

            migrationBuilder.DropTable(
                name: "Entitlements");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_ApiCredentials_Username",
                table: "ApiCredentials");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "ApiCredentials");

            migrationBuilder.DropColumn(
                name: "ModifiedAtUtc",
                table: "ApiCredentials");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "ApiCredentials",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
