using ConcertEvent.DataAccessLayer;
using ConcertEvent.Models;

namespace ConcertEvent
{
    public class EventManager
    {
        private readonly DALEvent _eventDal;

        public EventManager()
        {
            _eventDal = new DALEvent();
        }

        public void ListEvents()
        {
            var events = _eventDal.GetAll();
            if (!events.Any())
            {
                Console.WriteLine("No events found.");
                return;
            }

            foreach (var eachEvent in events)
            {
                eachEvent.PrintInfo();
            }
        }

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

                _eventDal.Insert(newEvent);

                Console.WriteLine("Would you like to add another event? Type Yes/No");

                response = (Console.ReadLine())?.ToUpper();
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
