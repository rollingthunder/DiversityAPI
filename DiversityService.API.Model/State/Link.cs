namespace DiversityService.API.Model
{
    using System;

    public class Link
    {
        public string Action { get; set; }
        public string Rel { get; set; }
        public Uri Href { get; set; }
    }
}
