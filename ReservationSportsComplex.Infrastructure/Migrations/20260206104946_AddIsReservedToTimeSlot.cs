using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSportsComplex.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsReservedToTimeSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReserved",
                table: "TimeSlots",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReserved",
                table: "TimeSlots");
        }
    }
}
