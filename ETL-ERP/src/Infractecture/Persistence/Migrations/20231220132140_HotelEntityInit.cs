using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class HotelEntityInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HtBedTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtBedTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtBedTypes_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBedTypes_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtBookingTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtBookingTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtBookingTypes_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBookingTypes_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtComplementaries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "VARCHAR(80)", maxLength: 80, nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtComplementaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtComplementaries_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtComplementaries_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtFloorInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FloorName = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: false),
                    TotalRoom = table.Column<int>(type: "int", nullable: false),
                    StartRoomNo = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    EndRoomNo = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtFloorInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtFloorInfos_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtFloorInfos_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtGuestInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuestCode = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    Salutation = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "VARCHAR(40)", maxLength: 40, nullable: true),
                    Occupation = table.Column<string>(type: "VARCHAR(40)", maxLength: 40, nullable: true),
                    Gender = table.Column<string>(type: "CHAR(1)", maxLength: 1, nullable: true),
                    Address = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: true),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdentityType = table.Column<int>(type: "int", nullable: true),
                    IdentityNo = table.Column<string>(type: "VARCHAR(120)", maxLength: 120, nullable: true),
                    IdentityPhotoUrl = table.Column<string>(type: "VARCHAR(120)", maxLength: 120, nullable: true),
                    PhotoUrl = table.Column<string>(type: "VARCHAR(120)", maxLength: 120, nullable: true),
                    IsVip = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CountryId = table.Column<long>(type: "bigint", nullable: false),
                    DistrictId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtGuestInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtGuestInfos_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtGuestInfos_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtGuestInfos_SetCountries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "SetCountries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtGuestInfos_SetDistricts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "SetDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtRoomFacilityCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtRoomFacilityCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtRoomFacilityCategories_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomFacilityCategories_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtRoomCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    IsAc = table.Column<bool>(type: "bit", nullable: false),
                    IsBalcony = table.Column<bool>(type: "bit", nullable: false),
                    BedNumber = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    OtherInfo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Rent = table.Column<double>(type: "float", nullable: false),
                    ServiceCharge = table.Column<double>(type: "float", nullable: false),
                    Vat = table.Column<double>(type: "float", nullable: false),
                    TotalRent = table.Column<double>(type: "float", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    BedTypeId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtRoomCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtRoomCategories_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomCategories_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomCategories_HtBedTypes_BedTypeId",
                        column: x => x.BedTypeId,
                        principalTable: "HtBedTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtRoomFacilities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    FacilityCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtRoomFacilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtRoomFacilities_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomFacilities_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomFacilities_HtRoomFacilityCategories_FacilityCategoryId",
                        column: x => x.FacilityCategoryId,
                        principalTable: "HtRoomFacilityCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtBookingServices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VisitPurpose = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: true),
                    BookingType = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: true),
                    Remarks = table.Column<string>(type: "VARCHAR(150)", maxLength: 150, nullable: true),
                    BookingStatus = table.Column<int>(type: "int", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    Rent = table.Column<double>(type: "float", nullable: false),
                    ServiceCharge = table.Column<double>(type: "float", nullable: false),
                    Vat = table.Column<double>(type: "float", nullable: false),
                    Tax = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    NetRent = table.Column<double>(type: "float", nullable: false),
                    TotalGuest = table.Column<double>(type: "float", nullable: false),
                    Adult = table.Column<double>(type: "float", nullable: false),
                    Child = table.Column<double>(type: "float", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RoomCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtBookingServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtBookingServices_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBookingServices_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBookingServices_HtRoomCategories_RoomCategoryId",
                        column: x => x.RoomCategoryId,
                        principalTable: "HtRoomCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtRoomInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomNo = table.Column<string>(type: "VARCHAR(25)", maxLength: 25, nullable: false),
                    RoomInformation = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    IsAc = table.Column<bool>(type: "bit", nullable: false),
                    IsBelcony = table.Column<bool>(type: "bit", nullable: false),
                    BedInfo = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: true),
                    NumberOfBed = table.Column<short>(type: "smallint", nullable: false),
                    Person = table.Column<short>(type: "smallint", nullable: false),
                    OtherInfo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RoomSize = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Rent = table.Column<double>(type: "float", nullable: false),
                    Vat = table.Column<double>(type: "float", nullable: false),
                    ServiceCharge = table.Column<double>(type: "float", nullable: false),
                    TotalRent = table.Column<double>(type: "float", nullable: false),
                    BookingStatus = table.Column<int>(type: "int", nullable: false),
                    CleaningStatus = table.Column<int>(type: "int", nullable: false),
                    Remakrs = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    PhotoUrl = table.Column<string>(type: "VARCHAR(120)", maxLength: 120, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RoomCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    FloorId = table.Column<long>(type: "bigint", nullable: false),
                    BedTypeId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtRoomInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtRoomInfos_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomInfos_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomInfos_HtBedTypes_BedTypeId",
                        column: x => x.BedTypeId,
                        principalTable: "HtBedTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomInfos_HtFloorInfos_FloorId",
                        column: x => x.FloorId,
                        principalTable: "HtFloorInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomInfos_HtRoomCategories_RoomCategoryId",
                        column: x => x.RoomCategoryId,
                        principalTable: "HtRoomCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtBookingGuests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookingId = table.Column<long>(type: "bigint", nullable: false),
                    GuestId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtBookingGuests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtBookingGuests_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBookingGuests_HtBookingServices_BookingId",
                        column: x => x.BookingId,
                        principalTable: "HtBookingServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBookingGuests_HtGuestInfos_GuestId",
                        column: x => x.GuestId,
                        principalTable: "HtGuestInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtBookingPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAmount = table.Column<double>(type: "float", nullable: false),
                    PayMode = table.Column<int>(type: "int", nullable: false),
                    TransactionNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ChequeNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    AccountNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    BookingId = table.Column<long>(type: "bigint", nullable: false),
                    BankId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtBookingPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtBookingPayments_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBookingPayments_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBookingPayments_HtBookingServices_BookingId",
                        column: x => x.BookingId,
                        principalTable: "HtBookingServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtBookingRooms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckInTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualCheckInTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualCheckOutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rent = table.Column<double>(type: "float", nullable: false),
                    ServiceCharge = table.Column<double>(type: "float", nullable: false),
                    Vat = table.Column<double>(type: "float", nullable: false),
                    Tax = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    NetRent = table.Column<double>(type: "float", nullable: false),
                    TotalGuest = table.Column<double>(type: "float", nullable: false),
                    Adult = table.Column<double>(type: "float", nullable: false),
                    Child = table.Column<double>(type: "float", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    BookingId = table.Column<long>(type: "bigint", nullable: false),
                    RoomId = table.Column<long>(type: "bigint", nullable: true),
                    ComplementaryId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtBookingRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtBookingRooms_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBookingRooms_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBookingRooms_HtBookingServices_BookingId",
                        column: x => x.BookingId,
                        principalTable: "HtBookingServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBookingRooms_HtComplementaries_ComplementaryId",
                        column: x => x.ComplementaryId,
                        principalTable: "HtComplementaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBookingRooms_HtRoomInfos_RoomId",
                        column: x => x.RoomId,
                        principalTable: "HtRoomInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtRoomFacilityMaps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RoomId = table.Column<long>(type: "bigint", nullable: false),
                    FacilityId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtRoomFacilityMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtRoomFacilityMaps_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomFacilityMaps_HtRoomFacilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "HtRoomFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomFacilityMaps_HtRoomInfos_RoomId",
                        column: x => x.RoomId,
                        principalTable: "HtRoomInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HtBedTypes_ActionById",
                table: "HtBedTypes",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBedTypes_UpdatedById",
                table: "HtBedTypes",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingGuests_ActionById",
                table: "HtBookingGuests",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingGuests_BookingId",
                table: "HtBookingGuests",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingGuests_GuestId",
                table: "HtBookingGuests",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingPayments_ActionById",
                table: "HtBookingPayments",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingPayments_BookingId",
                table: "HtBookingPayments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingPayments_UpdatedById",
                table: "HtBookingPayments",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingRooms_ActionById",
                table: "HtBookingRooms",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingRooms_BookingId",
                table: "HtBookingRooms",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingRooms_ComplementaryId",
                table: "HtBookingRooms",
                column: "ComplementaryId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingRooms_RoomId",
                table: "HtBookingRooms",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingRooms_UpdatedById",
                table: "HtBookingRooms",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingServices_ActionById",
                table: "HtBookingServices",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingServices_RoomCategoryId",
                table: "HtBookingServices",
                column: "RoomCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingServices_UpdatedById",
                table: "HtBookingServices",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingTypes_ActionById",
                table: "HtBookingTypes",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingTypes_UpdatedById",
                table: "HtBookingTypes",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtComplementaries_ActionById",
                table: "HtComplementaries",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtComplementaries_UpdatedById",
                table: "HtComplementaries",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtFloorInfos_ActionById",
                table: "HtFloorInfos",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtFloorInfos_UpdatedById",
                table: "HtFloorInfos",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtGuestInfos_ActionById",
                table: "HtGuestInfos",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtGuestInfos_CountryId",
                table: "HtGuestInfos",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_HtGuestInfos_DistrictId",
                table: "HtGuestInfos",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_HtGuestInfos_UpdatedById",
                table: "HtGuestInfos",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomCategories_ActionById",
                table: "HtRoomCategories",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomCategories_BedTypeId",
                table: "HtRoomCategories",
                column: "BedTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomCategories_UpdatedById",
                table: "HtRoomCategories",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomFacilities_ActionById",
                table: "HtRoomFacilities",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomFacilities_FacilityCategoryId",
                table: "HtRoomFacilities",
                column: "FacilityCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomFacilities_UpdatedById",
                table: "HtRoomFacilities",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomFacilityCategories_ActionById",
                table: "HtRoomFacilityCategories",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomFacilityCategories_UpdatedById",
                table: "HtRoomFacilityCategories",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomFacilityMaps_ActionById",
                table: "HtRoomFacilityMaps",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomFacilityMaps_FacilityId",
                table: "HtRoomFacilityMaps",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomFacilityMaps_RoomId",
                table: "HtRoomFacilityMaps",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomInfos_ActionById",
                table: "HtRoomInfos",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomInfos_BedTypeId",
                table: "HtRoomInfos",
                column: "BedTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomInfos_FloorId",
                table: "HtRoomInfos",
                column: "FloorId");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomInfos_RoomCategoryId",
                table: "HtRoomInfos",
                column: "RoomCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomInfos_RoomNo",
                table: "HtRoomInfos",
                column: "RoomNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomInfos_UpdatedById",
                table: "HtRoomInfos",
                column: "UpdatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HtBookingGuests");

            migrationBuilder.DropTable(
                name: "HtBookingPayments");

            migrationBuilder.DropTable(
                name: "HtBookingRooms");

            migrationBuilder.DropTable(
                name: "HtBookingTypes");

            migrationBuilder.DropTable(
                name: "HtRoomFacilityMaps");

            migrationBuilder.DropTable(
                name: "HtGuestInfos");

            migrationBuilder.DropTable(
                name: "HtBookingServices");

            migrationBuilder.DropTable(
                name: "HtComplementaries");

            migrationBuilder.DropTable(
                name: "HtRoomFacilities");

            migrationBuilder.DropTable(
                name: "HtRoomInfos");

            migrationBuilder.DropTable(
                name: "HtRoomFacilityCategories");

            migrationBuilder.DropTable(
                name: "HtFloorInfos");

            migrationBuilder.DropTable(
                name: "HtRoomCategories");

            migrationBuilder.DropTable(
                name: "HtBedTypes");
        }
    }
}
