namespace FakeDNA.Models
{
    public class LeaveCounter
    {
        public TimeOffReason Reason { get; set; }
        public double Total { get; set; }
        public double Remaining { get; set; }
        public double Amount { get; set; }
    }
}
