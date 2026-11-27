using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCare.Data.Migrations
{
    /// <inheritdoc />
    public partial class PreventDuplicateAppointmentSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_VeterinarianId",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_VeterinarianId_StartsAt",
                table: "Appointments",
                columns: new[] { "VeterinarianId", "StartsAt" },
                unique: true,
                filter: "[Status] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_VeterinarianId_StartsAt",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_VeterinarianId",
                table: "Appointments",
                column: "VeterinarianId");
        }
    }
}
