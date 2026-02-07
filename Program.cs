using ConcertEvent;
using DotNetEnv;


// var eventManager = new EventManager();

// eventManager.AddEvent();
class Program
{
    static void Main()
    {
        Env.Load();

        var eventManager = new EventManager();

        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("=== Concert Event Manager ===");
            Console.WriteLine("1. List all events");
            Console.WriteLine("2. Add new event");
            Console.WriteLine("3. Update an event");
            Console.WriteLine("4. Delete an event");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option (1-5): ");

            var choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    eventManager.ListEvents();
                    break;
                // case "2":
                //     eventManager.AddEvent();
                //     break;
                // case "3":
                //     eventManager.UpdateEvent();
                //     break;
                // case "4":
                //     eventManager.DeleteEvent();
                //     break;
                case "5":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid selection. Please try again.");
                    break;
            }
        }
    }
}
