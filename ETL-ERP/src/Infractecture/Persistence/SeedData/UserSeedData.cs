using Domain.Entities.Identity;
using Domain.Enums;
using Domain.Utility.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.SeedData;

public static class UserSeedData
{
    public static void Up(MigrationBuilder migrationBuilder)
    {
        var hasher = new PasswordHasher<ApplicationUser>();
        var hashPassword = hasher.HashPassword(null, "12345");

        // Seed Role
        migrationBuilder.Sql("SET IDENTITY_INSERT AspNetRoles ON");

        migrationBuilder.Sql("INSERT INTO AspNetRoles (Id,Name,NormalizedName,Status,IsActive,IsDeleted) VALUES ('" + (int)ApplicationRoleStatusEnum.SuperAdmin + "','" +
            ConstantsValue.UserRoleName.SuperAdmin + "','" + ConstantsValue.UserRoleName.SuperAdmin.ToUpper() + "','" + (int)ApplicationRoleStatusEnum.SuperAdmin + "','true','false')");
        migrationBuilder.Sql("INSERT INTO AspNetRoles (Id,Name,NormalizedName,Status,IsActive,IsDeleted) VALUES ('" + (int)ApplicationRoleStatusEnum.GeneralUser + "','" +
            ConstantsValue.UserRoleName.Admin + "','" + ConstantsValue.UserRoleName.Admin.ToUpper() + "','" + (int)ApplicationRoleStatusEnum.GeneralUser + "','true','false')");

        migrationBuilder.Sql("SET IDENTITY_INSERT AspNetRoles OFF");

        // Seed User
        migrationBuilder.Sql("SET IDENTITY_INSERT AspNetUsers ON");

        migrationBuilder.Sql("INSERT INTO AspNetUsers (Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,PhoneNumberConfirmed,TwoFactorEnabled," +
            "LockoutEnabled,AccessFailedCount,FullName,PasswordChangedCount,Status,IsActive,IsDeleted,ActionDate,SecurityStamp,IsMaskEmail,UserLevelId)" +
            " VALUES (1,'admin','ADMIN','admin@gmail.com','ADMIN@GMAIL.COM','false','" + hashPassword + "','false','false','false','0','Administrator','1','" +
            (int)ApplicationUserStatusEnum.SuperAdmin + "','true','false','" + DateTime.Now + "','" + Guid.NewGuid() + "','false','" + (int)ApplicationUserStatusEnum.SuperAdmin + "')");
        migrationBuilder.Sql("INSERT INTO AspNetUsers (Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,PhoneNumberConfirmed,TwoFactorEnabled," +
            "LockoutEnabled,AccessFailedCount,FullName,PasswordChangedCount,Status,IsActive,IsDeleted,ActionDate,SecurityStamp,IsMaskEmail,UserLevelId)" +
            " VALUES (2,'dev','DEV','dev@gmail.com','DEV@GMAIL.COM','false','" + hashPassword + "','false','false','false','0','Development','1','" +
            (int)ApplicationUserStatusEnum.SuperAdmin + "','true','false','" + DateTime.Now + "','" + Guid.NewGuid() + "','false','" + (int)ApplicationUserStatusEnum.SuperAdmin + "')");

        migrationBuilder.Sql("SET IDENTITY_INSERT AspNetUsers OFF");

        // Seed UserRole
        migrationBuilder.Sql("INSERT INTO AspNetUserRoles (UserId,RoleId) VALUES ((SELECT Id FROM AspNetUsers WHERE UserName = 'admin')," +
            "(SELECT Id FROM AspNetRoles WHERE Name = '" + ConstantsValue.UserRoleName.SuperAdmin + "'))");
        migrationBuilder.Sql("INSERT INTO AspNetUserRoles (UserId,RoleId) VALUES ((SELECT Id FROM AspNetUsers WHERE UserName = 'dev')," +
            "(SELECT Id FROM AspNetRoles WHERE Name = '" + ConstantsValue.UserRoleName.SuperAdmin + "'))");

        // Seed Role Claim
        migrationBuilder.Sql("INSERT INTO AspNetRoleClaims (RoleId,ClaimType,ClaimValue) VALUES (" +
            "(SELECT Id FROM AspNetRoles WHERE Name = '" + ConstantsValue.UserRoleName.SuperAdmin + "')," +
            "'" + ConstantsValue.RolePermission.Type + "','" + ConstantsValue.RolePermission.Value + "')");
    }

    public static void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DELETE FROM AspNetRoleClaims WHERE ClaimType = '" + ConstantsValue.RolePermission.Type + "'" +
            " AND ClaimValue = '" + ConstantsValue.RolePermission.Value + "'");
        migrationBuilder.Sql("DELETE FROM AspNetUsers WHERE UserName IN ('admin','dev')");
        migrationBuilder.Sql("DELETE FROM AspNetRoles WHERE Name IN ('" +
            ConstantsValue.UserRoleName.SuperAdmin + "','" +
            ConstantsValue.UserRoleName.Admin + "')");
    }
}
