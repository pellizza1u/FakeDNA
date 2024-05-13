namespace FakeDNA.Models
{
    public class TimeOffRequest
    {
        public TimeOffReason Reason { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeOffDayPeriod StartPeriod { get; set; }
        public TimeOffDayPeriod EndPeriod { get; set; }
    }
}
