using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace ConcertEvent
{
    public class EventManager
    {
        public List<Event> Events = new();

        public void AddEvent()
        {
            var response = "";
            while (response != "YES")
            {
                var newEvent = new Event();

                string eventName;

                do
                {
                    Console.Write("Let make an event. Enter event name: ");
                    eventName = Console.ReadLine() ?? "";

                } while (string.IsNullOrWhiteSpace(eventName));

                newEvent.Name = eventName;

                // Keep asking to get valid datetime
                newEvent.DateTime = GetEventDateTime();

                newEvent.TicketLink = GetTicketLink();

                var newArtist = new Artist();

                Console.Write("Enter artist name: ");

                newArtist.Name = Console.ReadLine();

                newEvent.Artist = newArtist;

                var newVenue = new Venue();

                Console.Write("Enter venue name: ");

                newVenue.Name = Console.ReadLine();

                newEvent.Venue = newVenue;

                newEvent.PrintInfo();
            }
        }
        public DateTime GetEventDateTime()
        {
            while (true)
            {
                Console.Write("Enter event datetime (yyyy-MM-dd HH:mm:ss): ");

                string input = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.Write("Enter event datetime (yyyy-MM-dd HH:mm:ss): ");
                }

                if (Helper.TryParseDateTime(input, out DateTime dt))
                {
                    return dt; // valid datetime, exit loop
                }

                Console.WriteLine("Invalid format! Please use yyyy-MM-dd HH:mm:ss");

            }
        }

        public string GetTicketLink()
        {
            while (true)
            {
                Console.Write("Enter ticket link url: ");

                string input = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.Write("Enter ticket link url: ");
                }

                if (Helper.IsValidUrl(input))
                {
                    return input;
                }

                Console.WriteLine("Invalid format! Please entry url");
            }
        }
    }
}
