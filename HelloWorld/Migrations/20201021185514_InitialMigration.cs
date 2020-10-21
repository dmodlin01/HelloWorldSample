using Microsoft.EntityFrameworkCore.Migrations;

namespace HelloWorldWebAPI.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageId", "Message" },
                values: new object[] { 1, "Hello World" });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageId", "Message" },
                values: new object[] { 2, "Greetings" });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageId", "Message" },
                values: new object[] { 3, "Welcome" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
