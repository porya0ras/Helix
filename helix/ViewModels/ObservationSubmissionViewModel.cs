namespace helix.ViewModels
{
    public class ObservationSubmissionViewModel
    {
        public Guid? Id { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime SDateTime { get; set; }
        public long FilterId { get; set; }
        public long SObjectId { get; set; }
        public long TelescopeId { get; set; }
        public long DetectorId { get; set; }
        public bool Status { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

   
}
