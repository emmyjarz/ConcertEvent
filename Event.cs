using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Threading.Tasks;

namespace ConcertEvent
{
    public class Event
    {
        public string? Name;

        public string? TicketLink;

        // ? question: Do I have to worry about dt.toString in order to save in database?
        public DateTime DateTime;

        public Artist? Artist;

        public Venue? Venue;

        public static bool IsValidUrl(string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            return Uri.TryCreate(str, UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        public void PrintInfo()
        {
            Console.WriteLine("=== Event Info ===");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"DateTime: {DateTime.ToString()}");
            Console.WriteLine($"Ticket Link: {TicketLink}");
            Console.WriteLine($"Artist: {Artist}");
            Console.WriteLine($"Venue: {Venue}");
            Console.WriteLine("==================\n");
        }
    }
}