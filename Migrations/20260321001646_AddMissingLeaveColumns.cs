using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsumbiCampusSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingLeaveColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "MealRecords",
                type: "datetime",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<bool>(
                name: "AttemptedWithoutCard",
                table: "MealRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "MealRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "MealRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "MealRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "AttemptedWithoutApproval",
                table: "LeaveRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "LeaveRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "LeaveRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "LeaveRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MealRecords_StaffId",
                table: "MealRecords",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_MealRecords_StudentId",
                table: "MealRecords",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRecords_StaffId",
                table: "LeaveRecords",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRecords_StudentId",
                table: "LeaveRecords",
                column: "StudentId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_MealRecords_Students_StudentId",
                table: "MealRecords",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MealRecords_Users_StaffId",
                table: "MealRecords",
                column: "StaffId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRecords_Students_StudentId",
                table: "LeaveRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRecords_Users_StaffId",
                table: "LeaveRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MealRecords_Students_StudentId",
                table: "MealRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MealRecords_Users_StaffId",
                table: "MealRecords");

            migrationBuilder.DropIndex(
                name: "IX_MealRecords_StaffId",
                table: "MealRecords");

            migrationBuilder.DropIndex(
                name: "IX_MealRecords_StudentId",
                table: "MealRecords");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRecords_StaffId",
                table: "LeaveRecords");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRecords_StudentId",
                table: "LeaveRecords");

            migrationBuilder.DropColumn(
                name: "AttemptedWithoutCard",
                table: "MealRecords");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "MealRecords");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "MealRecords");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "MealRecords");

            migrationBuilder.DropColumn(
                name: "AttemptedWithoutApproval",
                table: "LeaveRecords");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "LeaveRecords");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "LeaveRecords");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "LeaveRecords");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "MealRecords",
                type: "datetime",
                nullable: true,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "GETDATE()");
        }
    }
}
