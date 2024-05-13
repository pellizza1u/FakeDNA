using Microsoft.AspNetCore.Identity;

namespace FakeDNA.Models
{
    public class FakeDNAUser : IdentityUser
    {
        public ICollection<TimeOff> TimeOffs { get; } = [];
        public double PaidLeaveTotal { get; set; }
        public double CompensationLeaveTotal { get; set; }

        public double PaidLeaveAmount => TimeOffs
            .Where(leave => leave.Status != TimeOffStatus.Cancelled && 
                            leave.Status != TimeOffStatus.Rejected && 
                            leave.Reason == TimeOffReason.PaidLeave)
            .Count();

        public double CompensationLeaveAmount => TimeOffs
            .Where(leave => leave.Status != TimeOffStatus.Cancelled && 
                            leave.Status != TimeOffStatus.Rejected && 
                            leave.Reason == TimeOffReason.HolidayCompensation)
            .Count();

        public double SickLeaveAmount => TimeOffs
            .Where(leave => leave.Status != TimeOffStatus.Cancelled && 
                            leave.Status != TimeOffStatus.Rejected && 
                            leave.Reason == TimeOffReason.SickLeave)
            .Count();

        public double PaidLeaveRemaining => PaidLeaveTotal - PaidLeaveAmount;
        public double CompensationLeaveRemaining => CompensationLeaveTotal - CompensationLeaveAmount;

    }
}
