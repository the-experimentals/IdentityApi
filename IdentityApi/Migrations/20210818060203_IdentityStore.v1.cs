using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityApi.Migrations
{
    public partial class IdentityStorev1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PROFILE",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NAME = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CREATED_ON = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATED_BY = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MODIFIED_ON = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MODIFIED_BY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LOCKED = table.Column<bool>(type: "bit", nullable: false),
                    LOGIN_ATTEMPTS = table.Column<int>(type: "int", nullable: false),
                    LANGUAGE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EMAIL_VERIFIED = table.Column<bool>(type: "bit", nullable: false),
                    STATUS = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROFILE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "REFRESH_TOKEN",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TOKEN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GENERATED_ON = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PROFILE_ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LIFE_SPAN = table.Column<int>(type: "int", nullable: false),
                    DEVICE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BROWSER = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IPv4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SHA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ACTIVE = table.Column<bool>(type: "bit", nullable: false),
                    REFRESHED_ON = table.Column<DateTime>(type: "datetime2", nullable: false),
                    STATUS = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REFRESH_TOKEN", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CREDENTIALS",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PROFILE_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    USERNAME = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    SECRET_HASH = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SALT = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CREDENTIALS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CREDENTIALS_PROFILE_PROFILE_ID",
                        column: x => x.PROFILE_ID,
                        principalTable: "PROFILE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CREDENTIALS_PROFILE_ID",
                table: "CREDENTIALS",
                column: "PROFILE_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CREDENTIALS");

            migrationBuilder.DropTable(
                name: "REFRESH_TOKEN");

            migrationBuilder.DropTable(
                name: "PROFILE");
        }
    }
}
