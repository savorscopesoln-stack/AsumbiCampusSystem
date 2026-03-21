using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsumbiCampusSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingLeaveRecordColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRecords_Students_StudentId",
                table: "LeaveRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRecords_Users_StaffId",
                table: "LeaveRecords");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "LeaveRecords",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRecords_StaffMembers_StaffId",
                table: "LeaveRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRecords_Students_StudentId",
                table: "LeaveRecords");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "LeaveRecords",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRecords_Students_StudentId",
                table: "LeaveRecords",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRecords_Users_StaffId",
                table: "LeaveRecords",
                column: "StaffId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
