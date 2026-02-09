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
                Event newEvent = PromptForEventInfo(new Event());

                var createdEvent = _eventDal.CreateOrUpdate(newEvent);

                createdEvent.PrintInfo();

                Console.WriteLine("Would you like to add another event? Type Yes/No");

                response = (Console.ReadLine())?.ToUpper();
            }
        }

        public void UpdateEvent()
        {
            var existingEvent = PromptForGetEventById("update");

            if (existingEvent is null)
                return;

            Console.Write("Are you sure you want to update this event? (Yes/No): ");

            var confirm = (Console.ReadLine())?.ToUpper();

            if (confirm != "YES")
            {
                Console.WriteLine("Update cancelled.");
                return;
            }

            var updatedEvent = _eventDal.CreateOrUpdate(PromptForEventInfo(existingEvent));

            updatedEvent.PrintInfo();

            Helper.WriteColored("Event updated successfully.", ConsoleColor.Green);
        }


        public void DeleteEvent()
        {
            var existingEvent = PromptForGetEventById("delete");

            if (existingEvent is null)
                return;

            Console.Write("Are you sure you want to delete this event? (Yes/No): ");

            var confirm = (Console.ReadLine())?.ToUpper();

            if (confirm != "YES")
            {
                Console.WriteLine("Deletion cancelled.");
                return;
            }

            _eventDal.Delete(existingEvent.Id);

            Helper.WriteColored($"Event with ID {existingEvent.Id} has been deleted.", ConsoleColor.Green);
            return;
        }

        public Event PromptForEventInfo(Event newEvent)
        {
            string eventName;

            do
            {
                Console.Write("Enter event name: ");
                eventName = Console.ReadLine() ?? "";

            } while (string.IsNullOrWhiteSpace(eventName));

            newEvent.Name = eventName;

            newEvent.DateTime = PromptForEventDateTime();
            newEvent.TicketLink = PromptForTicketLink();

            var newArtist = new Artist();

            Console.Write("Enter artist name: ");

            newArtist.Name = Console.ReadLine();

            newEvent.Artist = newArtist;

            var newVenue = new Venue();

            Console.Write("Enter venue name: ");

            newVenue.Name = Console.ReadLine();

            newEvent.Venue = newVenue;

            return newEvent;
        }

        public DateTime PromptForEventDateTime()
        {
            while (true)
            {
                Console.Write($"Enter event datetime ({Event.DateTimeFormat}): ");

                string input = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(input))
                {
                    Helper.WriteColored("Input cannot be empty!", ConsoleColor.Red);
                    continue; // go back to the top of the loop
                }

                if (Helper.TryParseDateTime(Event.DateTimeFormat, input, out DateTime dt))
                {
                    return dt; // valid datetime, exit loop
                }

                Helper.WriteColored($"Invalid format! Please use {Event.DateTimeFormat}", ConsoleColor.Red);
            }
        }

        public string PromptForTicketLink()
        {
            while (true)
            {
                Console.Write("Enter ticket link url: ");

                string input = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(input))
                {
                    Helper.WriteColored("Input cannot be empty!", ConsoleColor.Red);
                    continue; // go back to the top of the loop
                }

                if (Helper.IsValidUrl(input))
                {
                    return input;
                }

                Helper.WriteColored("Invalid format! Please entry url", ConsoleColor.Red);
            }
        }

        public Event? PromptForGetEventById(string action)
        {
            Console.Write($"Please enter event ID to {action}: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Helper.WriteColored("Invalid input. Please enter a valid integer ID.", ConsoleColor.Red);

                return null;
            }

            var existingEvent = _eventDal.GetOne(id);

            if (existingEvent is null)
            {
                Helper.WriteColored($"Event with ID {id} not found.", ConsoleColor.Red);
                return null;
            }

            existingEvent.PrintInfo();

            return existingEvent;
        }
    }
}
