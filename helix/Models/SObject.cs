namespace helix.Models
{
    public class SObject
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? RA { get; set; }
        public string? DEC { get; set; }

        public double? RA0 { get; set; }
        public double? RA1 { get; set; }
        public double? RA2 { get; set; }

        public double? DEC0 { get; set; }
        public double? DEC1 { get; set; }
        public double? DEC2 { get; set; }
    }
}
