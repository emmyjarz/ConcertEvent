namespace ConcertEvent.Models
{
    public class Artist
    {
        public string? Name;

        public override string ToString()
        {
            // warning CS8603: Possible null reference return. for return Name;
            return Name ?? string.Empty;
        }
    }
}