using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Threading.Tasks;

namespace ConcertEvent.Models
{
    public class Event
    {
        public static string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public string? Name;

        public string? TicketLink;

        // ? question: Do I have to worry about dt.toString in order to save in database?
        public DateTime DateTime;

        public Artist? Artist;

        public Venue? Venue;

        public void PrintInfo()
        {
            Console.WriteLine("=== Event Info ===");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"DateTime: {DateTime.ToString(Event.DateTimeFormat)}");
            Console.WriteLine($"Ticket Link: {TicketLink}");
            Console.WriteLine($"Artist Name: {Artist?.Name}");
            Console.WriteLine($"Venue: {Venue?.Name}");
            Console.WriteLine("==================");
        }
    }
}