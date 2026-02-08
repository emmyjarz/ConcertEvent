using ConcertEvent.Models;
using Npgsql;

namespace ConcertEvent.DataAccessLayer
{
    public class DALEvent
    {
        private readonly string? _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        public DALEvent()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Database connection string is not set in environment variables.");
            }
        }

        public List<Event> GetAll()
        {
            List<Event> result = new List<Event>();

            string sql = @"SELECT e.id, e.name, e.ticket_link, e.date_time, v.name AS venue_name, STRING_AGG(a.name, ', ') AS artists_name 
            FROM events e 
            LEFT JOIN venues v ON e.venue_id = v.id 
            LEFT JOIN event_artists ea ON e.id = ea.event_id 
            LEFT JOIN artists a ON ea.artist_id = a.id 
            GROUP BY e.id, e.name, e.ticket_link, e.date_time, v.name 
            ORDER BY e.date_time DESC";

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

        public void Insert(Event ev)
        {
            int? venueId = null;

            if (!string.IsNullOrEmpty(ev.Venue?.Name))
            {
                venueId = new DALVenue().GetOrCreateByName(ev.Venue.Name.Trim());
            }

            int[] artistIds = Array.Empty<int>();

            if (!string.IsNullOrEmpty(ev.Artist?.Name))
            {
                artistIds = ev.Artist?.Name.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(name => new DALArtist().GetOrCreateByName(name.Trim()))
                    .ToArray() ?? Array.Empty<int>();
            }

            string sql = @"
                INSERT INTO events (name, ticket_link, date_time, venue_id)
                VALUES (@name, @ticketLink, @dateTime, @venueId)
                RETURNING id;
            ";

            int eventId;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    using var cmd = new NpgsqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@name", ev.Name!);
                    cmd.Parameters.AddWithValue("@ticketLink", ev.TicketLink!);
                    cmd.Parameters.AddWithValue("@dateTime", ev.DateTime);
                    cmd.Parameters.AddWithValue("@venueId", venueId.HasValue ? venueId.Value : DBNull.Value);
                    eventId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            if (artistIds.Length > 0)
            {
                string artistSql = $"INSERT INTO event_artists (event_id, artist_id) VALUES (@eventId, @artistId)";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    foreach (var artistId in artistIds)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = artistSql;
                            command.Parameters.AddWithValue("@eventId", eventId);
                            command.Parameters.AddWithValue("@artistId", artistId);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
