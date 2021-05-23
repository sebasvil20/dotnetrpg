using Microsoft.EntityFrameworkCore.Migrations;

namespace dotnet_rpg.Migrations
{
    public partial class RpgClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Class",
                table: "Characters",
                newName: "RpgClassId");

            migrationBuilder.AddColumn<int>(
                name: "RpgClassId",
                table: "Skills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RpgClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RpgClasses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skills_RpgClassId",
                table: "Skills",
                column: "RpgClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_RpgClassId",
                table: "Characters",
                column: "RpgClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_RpgClasses_RpgClassId",
                table: "Characters",
                column: "RpgClassId",
                principalTable: "RpgClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_RpgClasses_RpgClassId",
                table: "Skills",
                column: "RpgClassId",
                principalTable: "RpgClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_RpgClasses_RpgClassId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_RpgClasses_RpgClassId",
                table: "Skills");

            migrationBuilder.DropTable(
                name: "RpgClasses");

            migrationBuilder.DropIndex(
                name: "IX_Skills_RpgClassId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Characters_RpgClassId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "RpgClassId",
                table: "Skills");

            migrationBuilder.RenameColumn(
                name: "RpgClassId",
                table: "Characters",
                newName: "Class");
        }
    }
}
