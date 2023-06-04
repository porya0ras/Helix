namespace helix.Models
{
    public class SObject
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? RA { get; set; }
        public string? DEC { get; set; }

        public int? RA0 { get; set; }
        public int? RA1 { get; set; }
        public int? RA2 { get; set; }

        public int? DEC0 { get; set; }
        public int? DEC1 { get; set; }
        public int? DEC2 { get; set; }
    }
}
