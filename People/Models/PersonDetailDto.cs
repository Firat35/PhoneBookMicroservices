﻿namespace People.Models
{
    public class PersonDetailDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public List<ContactInfoDto> ContactInfos { get; set; } = new List<ContactInfoDto>();
    }
}
