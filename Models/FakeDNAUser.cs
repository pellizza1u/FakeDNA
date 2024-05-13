using Microsoft.AspNetCore.Identity;

namespace FakeDNA.Models
{
    public class FakeDNAUser : IdentityUser
    {
        public ICollection<TimeOff> TimeOffs { get; } = [];
        public double PaidLeaveTotal { get; set; }
        public double CompensationLeaveTotal { get; set; }
        public double PaidLeaveAmount => (from leave in TimeOffs
                                          where !new[] { TimeOffStatus.Cancelled, TimeOffStatus.Rejected }.Contains(leave.Status) && leave.Reason == TimeOffReason.PaidLeave
                                          select leave).Count();
        public double CompensationLeaveAmount => (from leave in TimeOffs
                                                  where !new[] { TimeOffStatus.Cancelled, TimeOffStatus.Rejected }.Contains(leave.Status) && leave.Reason == TimeOffReason.HolidayCompensation
                                                  select leave).Count();
        public double SickLeaveAmount => (from leave in TimeOffs
                                          where !new[] { TimeOffStatus.Cancelled, TimeOffStatus.Rejected }.Contains(leave.Status) && leave.Reason == TimeOffReason.SickLeave
                                          select leave).Count();
        public double PaidLeaveRemaining => PaidLeaveTotal - PaidLeaveAmount;
        public double CompensationLeaveRemaining => CompensationLeaveTotal - CompensationLeaveAmount;
    }
}
