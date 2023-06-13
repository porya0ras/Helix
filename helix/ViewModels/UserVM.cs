namespace helix.ViewModels
{
    public class UserVM: RegisterVM
    {
        public string Id { get; set; }
        public string Type { get; set; }

        public string CurrentPassword { get; set; }
    }
}
