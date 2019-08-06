using Microsoft.EntityFrameworkCore.Migrations;

namespace stage_parser.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    uuid = table.Column<string>(nullable: true),
                    source = table.Column<string>(nullable: true),
                    url = table.Column<string>(nullable: true),
                    floor_level = table.Column<int>(nullable: false),
                    floor_is_last = table.Column<string>(nullable: true),
                    Floor_type = table.Column<string>(nullable: true),
                    raw_floor_level = table.Column<int>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    Multilevel_floor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Offers");
        }
    }
}
