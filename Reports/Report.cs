namespace Reports
{
    public class Report
    {
        public Guid Id { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public int PersonCount { get; set; }
        public int PhoneNumberCount { get; set; }
    }
}