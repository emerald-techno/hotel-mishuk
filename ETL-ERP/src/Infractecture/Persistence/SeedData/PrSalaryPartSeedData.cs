using Domain.Enums.AppEnums;
using Domain.Utility;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.SeedData
{
    public static class PrSalaryPartSeedData
    {
        public static void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"

            SET IDENTITY_INSERT [dbo].[PrSalaryParts] ON

            INSERT INTO [dbo].[PrSalaryParts] ([Id],[PartName],[PartCode],[PartType],[ValueType],[Value],[IsEmpWise],[IsEnable],[Remarks],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({PrSalaryPartEnum.BasicSalary.ToInt64()}, 'Basic Salary', 'BS', 'A', 'A', 20000, 0, 1,'Basic Salary Remarks', GETDATE(), 1, 0);

            INSERT INTO [dbo].[PrSalaryParts] ([Id],[PartName],[PartCode],[PartType],[ValueType],[Value],[IsEmpWise],[IsEnable],[Remarks],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({PrSalaryPartEnum.HouseRent.ToInt64()}, 'House Rent', 'HR', 'A', 'A', 4000, 0, 1,'House Rent Remarks', GETDATE(), 1, 0);

            INSERT INTO [dbo].[PrSalaryParts] ([Id],[PartName],[PartCode],[PartType],[ValueType],[Value],[IsEmpWise],[IsEnable],[Remarks],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({PrSalaryPartEnum.MedicalAllowance.ToInt64()}, 'Medical Allowance', 'MA', 'A', 'A', 2000, 0, 1,'Medical Allowance Remarks', GETDATE(), 1, 0);

            INSERT INTO [dbo].[PrSalaryParts] ([Id],[PartName],[PartCode],[PartType],[ValueType],[Value],[IsEmpWise],[IsEnable],[Remarks],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({PrSalaryPartEnum.Conveyance.ToInt64()}, 'Conveyance', 'CV', 'A', 'A', 0, 0, 1,'Conveyance Remarks', GETDATE(), 1, 0);

            INSERT INTO [dbo].[PrSalaryParts] ([Id],[PartName],[PartCode],[PartType],[ValueType],[Value],[IsEmpWise],[IsEnable],[Remarks],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({PrSalaryPartEnum.Transport.ToInt64()}, 'Transport', 'TR', 'A', 'A', 0, 0, 1,'Transport Remarks', GETDATE(), 1, 0);

            INSERT INTO [dbo].[PrSalaryParts] ([Id],[PartName],[PartCode],[PartType],[ValueType],[Value],[IsEmpWise],[IsEnable],[Remarks],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({PrSalaryPartEnum.MobileAllowance.ToInt64()}, 'Mobile Allowance', 'MOB', 'A', 'A', 0, 0, 1,'Mobile Allowance Remarks', GETDATE(), 1, 0);

            INSERT INTO [dbo].[PrSalaryParts] ([Id],[PartName],[PartCode],[PartType],[ValueType],[Value],[IsEmpWise],[IsEnable],[Remarks],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({PrSalaryPartEnum.PF.ToInt64()}, 'PF', 'PF', 'D', 'P', 0, 0, 1,'PF Remarks', GETDATE(), 1, 0);

            INSERT INTO [dbo].[PrSalaryParts] ([Id],[PartName],[PartCode],[PartType],[ValueType],[Value],[IsEmpWise],[IsEnable],[Remarks],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({PrSalaryPartEnum.Tax.ToInt64()}, 'TAX', 'TAX', 'D', 'P', 0, 0, 1,'TAX Remarks', GETDATE(), 1, 0);

            INSERT INTO [dbo].[PrSalaryParts] ([Id],[PartName],[PartCode],[PartType],[ValueType],[Value],[IsEmpWise],[IsEnable],[Remarks],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({PrSalaryPartEnum.Bonus.ToInt64()}, 'Bonus', 'CV', 'A', 'A', 0, 0, 1,'Bonus Remarks', GETDATE(), 1, 0);

            SET IDENTITY_INSERT [dbo].[PrSalaryParts] OFF

            ");
        }

        public static void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"

                        DELETE PrSalaryParts
            ");
        }

    }
}
