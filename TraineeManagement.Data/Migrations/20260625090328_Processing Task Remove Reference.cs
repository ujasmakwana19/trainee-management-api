using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class ProcessingTaskRemoveReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessingJobs_SubmissionFiles_SubmissionFileId",
                table: "ProcessingJobs");

            migrationBuilder.DropIndex(
                name: "IX_ProcessingJobs_SubmissionFileId",
                table: "ProcessingJobs");

            migrationBuilder.DropColumn(
                name: "SubmissionFileId",
                table: "ProcessingJobs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SubmissionFileId",
                table: "ProcessingJobs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_SubmissionFileId",
                table: "ProcessingJobs",
                column: "SubmissionFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessingJobs_SubmissionFiles_SubmissionFileId",
                table: "ProcessingJobs",
                column: "SubmissionFileId",
                principalTable: "SubmissionFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
