using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsumbiCampusSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddGateStatusToLeaveRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRecords_StaffMembers_StaffId",
                table: "LeaveRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRecords_Students_StudentId",
                table: "LeaveRecords");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRecords_StaffId",
                table: "LeaveRecords");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRecords_StudentId",
                table: "LeaveRecords");

            migrationBuilder.DropColumn(
                name: "AttemptedWithoutApproval",
                table: "LeaveRecords");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "LeaveRecords");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "LeaveRecords");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "LeaveRecords",
                newName: "GateStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GateStatus",
                table: "LeaveRecords",
                newName: "Remarks");

            migrationBuilder.AddColumn<bool>(
                name: "AttemptedWithoutApproval",
                table: "LeaveRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "LeaveRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "LeaveRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRecords_StaffId",
                table: "LeaveRecords",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRecords_StudentId",
                table: "LeaveRecords",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRecords_StaffMembers_StaffId",
                table: "LeaveRecords",
                column: "StaffId",
                principalTable: "StaffMembers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRecords_Students_StudentId",
                table: "LeaveRecords",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}
