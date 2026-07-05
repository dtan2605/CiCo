using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cico.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBiometricProfileIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BiometricProfiles_Employees_EmployeeId1",
                table: "BiometricProfiles");

            migrationBuilder.DropIndex(
                name: "IX_BiometricProfiles_EmployeeId",
                table: "BiometricProfiles");

            migrationBuilder.DropIndex(
                name: "IX_BiometricProfiles_EmployeeId1",
                table: "BiometricProfiles");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "BiometricProfiles");

            migrationBuilder.CreateIndex(
                name: "IX_BiometricProfiles_EmployeeId_Type",
                table: "BiometricProfiles",
                columns: new[] { "EmployeeId", "Type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BiometricProfiles_EmployeeId_Type",
                table: "BiometricProfiles");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId1",
                table: "BiometricProfiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BiometricProfiles_EmployeeId",
                table: "BiometricProfiles",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_BiometricProfiles_EmployeeId1",
                table: "BiometricProfiles",
                column: "EmployeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BiometricProfiles_Employees_EmployeeId1",
                table: "BiometricProfiles",
                column: "EmployeeId1",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
