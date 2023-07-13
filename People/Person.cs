namespace People
{
    //public class WeatherForecast
    //{
    //    public DateTime Date { get; set; }

    //    public int TemperatureC { get; set; }

    //    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    //    public string? Summary { get; set; }
    //}
    public class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public List<ContactInfo> ContactInfos { get; set; } = new List<ContactInfo>();
    }

    public class ContactInfo
    {
        public Guid Id { get; set; }
        public string InfoType { get; set; }
        public string InfoContent { get; set; }
    }
}