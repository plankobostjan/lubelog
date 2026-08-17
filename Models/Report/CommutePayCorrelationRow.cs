namespace CarCareTracker.Models
{
    public class CommutePayCorrelationRow
    {
        public int MonthId { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal CommutePayTotal { get; set; }
        public decimal GasCostTotal { get; set; }
        public decimal TotalVehicleCost { get; set; }
        public decimal NetDifference { get { return CommutePayTotal - TotalVehicleCost; } }
    }
}
