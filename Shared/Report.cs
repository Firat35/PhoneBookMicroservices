using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class Report
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Status { get; set; }
        public List<Location> locations { get; set; }
    }
    public enum ReportStatus
    {
        Creating,
        Completed
    }
}