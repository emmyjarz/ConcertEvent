using Npgsql;

namespace ConcertEvent.Models
{
    public class Event
    {
        public static string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public int Id { get; internal set; }

        public string? Name;

        public string? TicketLink;

        public DateTime DateTime { get; set; }

        public Artist? Artist;

        public Venue? Venue;

        public Event() { }

        public Event(NpgsqlDataReader reader)
        {
            Id = reader.GetInt32(0);
            Name = reader.GetString(1);
            TicketLink = reader.GetString(2);
            DateTime = reader.GetDateTime(reader.GetOrdinal("date_time"));
            Venue = reader.IsDBNull(reader.GetOrdinal("venue_name")) ? null : new Venue { Name = reader.GetString(reader.GetOrdinal("venue_name")) };
            Artist = reader.IsDBNull(reader.GetOrdinal("artists_name")) ? null : new Artist { Name = reader.GetString(reader.GetOrdinal("artists_name")) };
        }

        public void PrintInfo()
        {
            Console.WriteLine("=== Event Info ===");
            Console.WriteLine($"ID: {Id} | Name: {Name} | DateTime: {DateTime.ToString(Event.DateTimeFormat)} | Ticket Link: {TicketLink} | Artist: {Artist?.Name} | Venue: {Venue?.Name}");
            Console.WriteLine("==================");
        }
    }
}