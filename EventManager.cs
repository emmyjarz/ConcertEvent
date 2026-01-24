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

            while (response != "NO")
            {
                var newEvent = new Event();

                string eventName;

                do
                {
                    Console.Write("Let make an event. Enter event name: ");
                    eventName = Console.ReadLine() ?? "";

                } while (string.IsNullOrWhiteSpace(eventName));

                newEvent.Name = eventName;

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

                Events.Add(newEvent);

                newEvent.PrintInfo();

                Console.WriteLine("Would you like to add another event? Type Yes/No");

                response = (Console.ReadLine())?.ToUpper();
            }

            Console.WriteLine($"You input {Events.Count} event(s).");

            foreach (var eachEvent in Events)
            {
                eachEvent.PrintInfo(eventNameOnly: true);
            }
        }
        public DateTime GetEventDateTime()
        {
            while (true)
            {
                var question = $"Enter event datetime ({Event.DateTimeFormat}): ";

                Console.Write(question);

                string input = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.Write(question);
                }

                if (Helper.TryParseDateTime(Event.DateTimeFormat, input, out DateTime dt))
                {
                    return dt; // valid datetime, exit loop
                }

                Console.WriteLine($"Invalid format! Please use {Event.DateTimeFormat}");
            }
        }

        public string GetTicketLink()
        {
            while (true)
            {
                var question = "Enter ticket link url: ";

                Console.Write(question);

                string input = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.Write(question);
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
