using Microsoft.EntityFrameworkCore.Migrations;

namespace Repositories.Migrations
{
    public partial class populatemessagebody : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(maxLength: 100, nullable: false),
                    MessageBody = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageId", "Message", "MessageBody" },
                values: new object[] { 1, "Hello World", "Hello World and all of its inhabitants!" });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageId", "Message", "MessageBody" },
                values: new object[] { 2, "Greetings", "Greeting Friends" });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageId", "Message", "MessageBody" },
                values: new object[] { 3, "Welcome", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
