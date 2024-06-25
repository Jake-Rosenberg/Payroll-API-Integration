using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AD_Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Payroll = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserPrincipalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: true),
                    LastPasswordChange = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AD_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UKG_Employees",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Payroll = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Facility = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shift = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmploymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayRuleOrg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UKG_Employees", x => x.PersonId);
                });

            migrationBuilder.CreateTable(
                name: "UKG_D_LatePunches",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    HoursLate = table.Column<TimeSpan>(type: "time", nullable: false),
                    ShiftStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PunchIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Facility = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shift = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UKG_D_LatePunches", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_UKG_D_LatePunches_UKG_Employees_PersonId",
                        column: x => x.PersonId,
                        principalTable: "UKG_Employees",
                        principalColumn: "PersonId");
                });

            migrationBuilder.CreateTable(
                name: "UKG_HoursWorked",
                columns: table => new
                {
                    HoursWorkedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    TotalHoursWorked = table.Column<double>(type: "float", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UKG_HoursWorked", x => x.HoursWorkedId);
                    table.ForeignKey(
                        name: "FK_UKG_HoursWorked_UKG_Employees_PersonId",
                        column: x => x.PersonId,
                        principalTable: "UKG_Employees",
                        principalColumn: "PersonId");
                });

            migrationBuilder.CreateTable(
                name: "UKG_PaycodeEdits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    ApplyDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Paycode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UKG_PaycodeEdits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UKG_PaycodeEdits_UKG_Employees_PersonId",
                        column: x => x.PersonId,
                        principalTable: "UKG_Employees",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UKG_ScheduleShifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    ShiftStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShiftEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UKG_ScheduleShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UKG_ScheduleShifts_UKG_Employees_PersonId",
                        column: x => x.PersonId,
                        principalTable: "UKG_Employees",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UKG_WorkedShifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    ScheduledShiftId = table.Column<int>(type: "int", nullable: true),
                    PunchIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PunchOut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoursLate = table.Column<TimeSpan>(type: "time", nullable: true),
                    Facility = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shift = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UKG_WorkedShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UKG_WorkedShifts_UKG_Employees_PersonId",
                        column: x => x.PersonId,
                        principalTable: "UKG_Employees",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UKG_WorkedShifts_UKG_ScheduleShifts_ScheduledShiftId",
                        column: x => x.ScheduledShiftId,
                        principalTable: "UKG_ScheduleShifts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UKG_D_LatePunches_PersonId",
                table: "UKG_D_LatePunches",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_UKG_HoursWorked_PersonId",
                table: "UKG_HoursWorked",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UKG_PaycodeEdits_PersonId",
                table: "UKG_PaycodeEdits",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_UKG_ScheduleShifts_PersonId",
                table: "UKG_ScheduleShifts",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_UKG_WorkedShifts_PersonId",
                table: "UKG_WorkedShifts",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_UKG_WorkedShifts_ScheduledShiftId",
                table: "UKG_WorkedShifts",
                column: "ScheduledShiftId",
                unique: true,
                filter: "[ScheduledShiftId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AD_Employees");

            migrationBuilder.DropTable(
                name: "UKG_D_LatePunches");

            migrationBuilder.DropTable(
                name: "UKG_HoursWorked");

            migrationBuilder.DropTable(
                name: "UKG_PaycodeEdits");

            migrationBuilder.DropTable(
                name: "UKG_WorkedShifts");

            migrationBuilder.DropTable(
                name: "UKG_ScheduleShifts");

            migrationBuilder.DropTable(
                name: "UKG_Employees");
        }
    }
}
