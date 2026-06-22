using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class SubmissionFileModel_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionFile_Submissions_SubmissionId",
                table: "SubmissionFile");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionFile_Users_UploadedByUserId",
                table: "SubmissionFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubmissionFile",
                table: "SubmissionFile");

            migrationBuilder.RenameTable(
                name: "SubmissionFile",
                newName: "SubmissionFiles");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionFile_UploadedByUserId",
                table: "SubmissionFiles",
                newName: "IX_SubmissionFiles_UploadedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionFile_SubmissionId",
                table: "SubmissionFiles",
                newName: "IX_SubmissionFiles_SubmissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubmissionFiles",
                table: "SubmissionFiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionFiles_Submissions_SubmissionId",
                table: "SubmissionFiles",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionFiles_Users_UploadedByUserId",
                table: "SubmissionFiles",
                column: "UploadedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionFiles_Submissions_SubmissionId",
                table: "SubmissionFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionFiles_Users_UploadedByUserId",
                table: "SubmissionFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubmissionFiles",
                table: "SubmissionFiles");

            migrationBuilder.RenameTable(
                name: "SubmissionFiles",
                newName: "SubmissionFile");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionFiles_UploadedByUserId",
                table: "SubmissionFile",
                newName: "IX_SubmissionFile_UploadedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionFiles_SubmissionId",
                table: "SubmissionFile",
                newName: "IX_SubmissionFile_SubmissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubmissionFile",
                table: "SubmissionFile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionFile_Submissions_SubmissionId",
                table: "SubmissionFile",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionFile_Users_UploadedByUserId",
                table: "SubmissionFile",
                column: "UploadedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
