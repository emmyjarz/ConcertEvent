namespace ConcertEvent.Models
{
    public class Venue
    {
        public string? Name;

        public override string ToString()
        {
            return Name ?? string.Empty;
        }
    }
}