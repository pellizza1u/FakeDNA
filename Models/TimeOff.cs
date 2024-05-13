using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FakeDNA.Models
{
    public class TimeOff
    {
        public Guid Id { get; set; }

        public TimeOffReason Reason { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public double Duration => (EndDate - StartDate).TotalDays;

        public TimeOffDayPeriod StartDayPeriod { get; set; }

        public TimeOffDayPeriod EndDayPeriod { get; set; }

        public TimeOffStatus Status { get; set; }

        [JsonIgnore]
        public FakeDNAUser Requester { get; set; } = null!;
    }
}
