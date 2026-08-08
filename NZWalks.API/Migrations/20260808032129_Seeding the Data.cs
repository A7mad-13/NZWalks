using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NZWalks.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedingtheData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Difficulties",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("54466f17-02af-48e7-8ed3-5a4a8bfacf6f"), "Easy" },
                    { new Guid("ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c"), "Medium" },
                    { new Guid("f808ddcd-b5e5-4d80-b732-1ca523e48434"), "Hard" }
                });

            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "Id", "Code", "Name", "RegionImageUrl" },
                values: new object[,]
                {
                    { new Guid("14ceba71-4b51-4777-9b17-46602cf66153"), "BOP", "Bay Of Plenty", null },
                    { new Guid("6884f7d7-ad1f-4101-8df3-7a6fa7387d81"), "NTL", "Northland", null },
                    { new Guid("f7248fc3-2585-4efb-8d1d-1c555f4087f6"), "AKL", "Auckland", "https://images.pexels.com/photos/5169056/pexels-photo-5169056.jpeg" }
                });

            migrationBuilder.InsertData(
                table: "Walks",
                columns: new[] { "Id", "DifficultyId", "LengthInKM", "Name", "RegionId", "WalkImageUrl" },
                values: new object[,]
                {
                    { new Guid("2d5a9c81-6b34-4e7f-a1c8-9d02f4b5e3a6"), new Guid("f808ddcd-b5e5-4d80-b732-1ca523e48434"), 12.0, "Cape Reinga Coastal Walk", new Guid("6884f7d7-ad1f-4101-8df3-7a6fa7387d81"), null },
                    { new Guid("7e0f4b3a-1c9d-4a2e-9f52-3b8c1d0e6a74"), new Guid("ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c"), 5.5, "Rangitoto Island Summit Track", new Guid("f7248fc3-2585-4efb-8d1d-1c555f4087f6"), "https://images.pexels.com/photos/1687845/pexels-photo-1687845.jpeg" },
                    { new Guid("b3c17e59-8a4d-42f6-b0e1-5c7d9a2f8b40"), new Guid("54466f17-02af-48e7-8ed3-5a4a8bfacf6f"), 3.3999999999999999, "Mount Maunganui Base Track", new Guid("14ceba71-4b51-4777-9b17-46602cf66153"), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Walks",
                keyColumn: "Id",
                keyValue: new Guid("2d5a9c81-6b34-4e7f-a1c8-9d02f4b5e3a6"));

            migrationBuilder.DeleteData(
                table: "Walks",
                keyColumn: "Id",
                keyValue: new Guid("7e0f4b3a-1c9d-4a2e-9f52-3b8c1d0e6a74"));

            migrationBuilder.DeleteData(
                table: "Walks",
                keyColumn: "Id",
                keyValue: new Guid("b3c17e59-8a4d-42f6-b0e1-5c7d9a2f8b40"));

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: new Guid("54466f17-02af-48e7-8ed3-5a4a8bfacf6f"));

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: new Guid("ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c"));

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: new Guid("f808ddcd-b5e5-4d80-b732-1ca523e48434"));

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("14ceba71-4b51-4777-9b17-46602cf66153"));

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("6884f7d7-ad1f-4101-8df3-7a6fa7387d81"));

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("f7248fc3-2585-4efb-8d1d-1c555f4087f6"));
        }
    }
}
