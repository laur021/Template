using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuPermissionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVisibleToAll = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuSections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SectionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Route = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVisibleToAll = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_MenuSections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "MenuSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuSubItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MenuItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Route = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVisibleToAll = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuSubItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuSubItems_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MenuItemId = table.Column<Guid>(type: "TEXT", nullable: true),
                    MenuSubItemId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageActions_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageActions_MenuSubItems_MenuSubItemId",
                        column: x => x.MenuSubItemId,
                        principalTable: "MenuSubItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleMenuAccess",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SectionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    MenuItemId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SubItemId = table.Column<Guid>(type: "TEXT", nullable: true),
                    HasAccess = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMenuAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleMenuAccess_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleMenuAccess_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoleMenuAccess_MenuSections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "MenuSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleMenuAccess_MenuSubItems_SubItemId",
                        column: x => x.SubItemId,
                        principalTable: "MenuSubItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleActionAccess",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ActionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleActionAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleActionAccess_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleActionAccess_PageActions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "PageActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_DisplayOrder",
                table: "MenuItems",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_Route",
                table: "MenuItems",
                column: "Route");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_SectionId",
                table: "MenuItems",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuSections_DisplayOrder",
                table: "MenuSections",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_MenuSubItems_DisplayOrder",
                table: "MenuSubItems",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_MenuSubItems_MenuItemId",
                table: "MenuSubItems",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuSubItems_Route",
                table: "MenuSubItems",
                column: "Route");

            migrationBuilder.CreateIndex(
                name: "IX_PageActions_Code",
                table: "PageActions",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_PageActions_MenuItemId",
                table: "PageActions",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PageActions_MenuSubItemId",
                table: "PageActions",
                column: "MenuSubItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleActionAccess_ActionId",
                table: "RoleActionAccess",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleActionAccess_RoleId_ActionId",
                table: "RoleActionAccess",
                columns: new[] { "RoleId", "ActionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenuAccess_MenuItemId",
                table: "RoleMenuAccess",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenuAccess_RoleId_SectionId_MenuItemId_SubItemId",
                table: "RoleMenuAccess",
                columns: new[] { "RoleId", "SectionId", "MenuItemId", "SubItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenuAccess_SectionId",
                table: "RoleMenuAccess",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenuAccess_SubItemId",
                table: "RoleMenuAccess",
                column: "SubItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleActionAccess");

            migrationBuilder.DropTable(
                name: "RoleMenuAccess");

            migrationBuilder.DropTable(
                name: "PageActions");

            migrationBuilder.DropTable(
                name: "MenuSubItems");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "MenuSections");
        }
    }
}
