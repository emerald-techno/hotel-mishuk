using Domain.Enums.AppEnums;
using Domain.Utility;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.SeedData
{
    public static class LeaveTypeSeedData
    {
        public static void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"

            SET IDENTITY_INSERT [dbo].[LeaveTypes] ON

            INSERT INTO [dbo].[LeaveTypes] ([Id],[TypeName],[Balance],[Desc],[Gender],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({LeaveTypeEnum.MaternityLeave.ToInt64()}, 'Maternity Leave', 180, 'Maternity Leave', 'F', GETDATE(), 1, 0);

            INSERT INTO [dbo].[LeaveTypes] ([Id],[TypeName],[Balance],[Desc],[Gender],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({LeaveTypeEnum.CasualLeave.ToInt64()}, 'Casual Leave (C / L)', 15, 'Casual Leave (C / L)', 'A', GETDATE(), 1, 0);

            INSERT INTO [dbo].[LeaveTypes] ([Id],[TypeName],[Balance],[Desc],[Gender],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({LeaveTypeEnum.MedicalLeave.ToInt64()}, 'Medical Leave (M/ L)', 30, 'Medical Leave (M/ L)', 'A', GETDATE(), 1, 0);

            INSERT INTO [dbo].[LeaveTypes] ([Id],[TypeName],[Balance],[Desc],[Gender],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({LeaveTypeEnum.EarnedLeave.ToInt64()}, 'Earned Leave (E / L)', 30, 'Earned Leave (E / L)', 'A', GETDATE(), 1, 0);

            INSERT INTO [dbo].[LeaveTypes] ([Id],[TypeName],[Balance],[Desc],[Gender],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({LeaveTypeEnum.LeaveWithoutPay.ToInt64()}, 'Leave Without Pay', 0, 'Leave Without Pay', 'A', GETDATE(), 1, 0);

            INSERT INTO [dbo].[LeaveTypes] ([Id],[TypeName],[Balance],[Desc],[Gender],[ActionDate],[ActionById],[IsDeleted]) 
            VALUES ({LeaveTypeEnum.StudyLeave.ToInt64()}, 'Study Leave', 0, 'Study Leave', 'A', GETDATE(), 1, 0);

            SET IDENTITY_INSERT [dbo].[LeaveTypes] OFF

            ");
        }

        public static void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"

                        DELETE LeaveTypes
            ");
        }

    }
}
