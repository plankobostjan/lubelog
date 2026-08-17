namespace CarCareTracker.Models
{
    public class CommutePayRecordInput
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string Date { get; set; } = DateTime.Now.ToShortDateString();
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<UploadedFiles> Files { get; set; } = new List<UploadedFiles>();
        public List<string> Tags { get; set; } = new List<string>();
        public List<ExtraField> ExtraFields { get; set; } = new List<ExtraField>();
        public CommutePayRecord ToCommutePayRecord() { return new CommutePayRecord {
            Id = Id,
            VehicleId = VehicleId,
            Date = DateTime.Parse(Date),
            Description = Description,
            Amount = Amount,
            Notes = Notes,
            Files = Files,
            Tags = Tags,
            ExtraFields = ExtraFields
        }; }
    }
}
