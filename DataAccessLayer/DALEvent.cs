using ConcertEvent.Models;
using Npgsql;

namespace ConcertEvent.DataAccessLayer
{
    public class DALEvent
    {
        private readonly string? _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        public DALEvent()
        {
            Console.WriteLine(_connectionString);
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Database connection string is not set in environment variables.");
            }
        }

        public List<Event> GetAll()
        {
            List<Event> result = new List<Event>();

            string sql = "SELECT e.id, e.name, e.ticket_link, e.date_time, v.name AS venue_name, STRING_AGG(a.name, ', ') AS artists_name FROM events e LEFT JOIN venues v ON e.venue_id = v.id LEFT JOIN event_artists ea ON e.id = ea.event_id LEFT JOIN artists a ON ea.artist_id = a.id GROUP BY e.id, e.name, e.ticket_link, e.date_time, v.name ORDER BY e.date_time DESC";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Event(reader);

                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }
    }
}