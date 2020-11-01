using Microsoft.EntityFrameworkCore.Migrations;

namespace HelloWorldWeb.Migrations
{
    public partial class seeddata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Users",
                columns: new[] { "UserId", "FullName", "UserName" },
                values: new object[] { 8902550, "Jane Smith", "jane" });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Users",
                columns: new[] { "UserId", "FullName", "UserName" },
                values: new object[] { 8775895, "Bob Smith", "bob" });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Messages",
                columns: new[] { "MessageId", "Message", "MessageBody", "UserId" },
                values: new object[,]
                {
                    { 2, "Greetings", "Greeting Jane", 8902550 },
                    { 3, "Invitation", "Jane, you are cordially invited to the Halloween gala. Costumes are encouraged.", 8902550 },
                    { 1, "Hello World", "Hello World and all of its inhabitants!", 8775895 },
                    { 4, "Invitation", "Bob, we are looking forward to seeing you this halloween. We ask that you please wear a fitting costume (preferably not the homeless look again).", 8775895 },
                    { 5, "William, it is your mother. Call me!", "William Smith, for nine months I carried you in my belly.. Is calling your mother once a week too much to ask for?.", 8775895 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Messages",
                keyColumn: "MessageId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8775895);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8902550);
        }
    }
}
