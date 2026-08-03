using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTraineeMentorSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Trainees",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Mentors",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Trainees_UserId",
                table: "Trainees",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mentors_UserId",
                table: "Mentors",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Mentors_Users_UserId",
                table: "Mentors",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainees_Users_UserId",
                table: "Trainees",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mentors_Users_UserId",
                table: "Mentors");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainees_Users_UserId",
                table: "Trainees");

            migrationBuilder.DropIndex(
                name: "IX_Trainees_UserId",
                table: "Trainees");

            migrationBuilder.DropIndex(
                name: "IX_Mentors_UserId",
                table: "Mentors");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Trainees");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Mentors");
        }
    }
}
