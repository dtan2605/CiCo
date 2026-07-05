using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cico.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CurrentScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AdminNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduleRequests_Schedules_CurrentScheduleId",
                        column: x => x.CurrentScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduleRequests_Schedules_RequestedScheduleId",
                        column: x => x.RequestedScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleRequests_CurrentScheduleId",
                table: "ScheduleRequests",
                column: "CurrentScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleRequests_EmployeeId",
                table: "ScheduleRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleRequests_RequestedScheduleId",
                table: "ScheduleRequests",
                column: "RequestedScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleRequests");
        }
    }
}
