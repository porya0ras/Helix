using System.ComponentModel.DataAnnotations.Schema;

namespace helix.Models
{
    public class ObservationSubmission
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public  Filter _Filter { get; set; }
        public  SObject _SObject { get; set; }
        public  Telescope _Telescope { get; set; }
        public  Detector _Detector { get; set; }
        public  User _User { get; set; }


    }
}
