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

        public Event? GetOne(int id)
        {
            string sql = @"SELECT e.id, e.name, e.ticket_link, e.date_time, v.name AS venue_name, STRING_AGG(a.name, ', ') AS artists_name 
            FROM events e 
            LEFT JOIN venues v ON e.venue_id = v.id 
            LEFT JOIN event_artists ea ON e.id = ea.event_id 
            LEFT JOIN artists a ON ea.artist_id = a.id 
            WHERE e.id = @id
            GROUP BY e.id, e.name, e.ticket_link, e.date_time, v.name";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        return new Event(reader);
                    }
                }
            }
        }

        public Event CreateOrUpdate(Event ev)
        {
            int? venueId = null;

            if (!string.IsNullOrEmpty(ev.Venue?.Name))
            {
                venueId = new DALVenue().GetOrCreateByName(ev.Venue.Name.Trim());
            }

            int[] artistIds = new DALArtist().GetOrCreateByNames(ev.Artist?.Name);

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                if (ev.Id > 0)
                {
                    // UPDATE
                    const string updateSql = @"UPDATE events 
                    SET name = @name, 
                    ticket_link = @ticketLink,
                    date_time = @dateTime,
                    venue_id = @venueId
                    WHERE id = @id;";

                    using var cmd = new NpgsqlCommand(updateSql, connection, transaction);
                    cmd.Parameters.AddWithValue("@id", ev.Id);
                    cmd.Parameters.AddWithValue("@name", ev.Name!);
                    cmd.Parameters.AddWithValue("@ticketLink", ev.TicketLink!);
                    cmd.Parameters.AddWithValue("@dateTime", ev.DateTime);
                    cmd.Parameters.AddWithValue("@venueId",
                        venueId.HasValue ? venueId.Value : DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    // INSERT
                    const string insertSql = @"INSERT INTO events (name, ticket_link, date_time, venue_id) VALUES (@name, @ticketLink, @dateTime, @venueId) RETURNING id;";

                    using var cmd = new NpgsqlCommand(insertSql, connection, transaction);
                    cmd.Parameters.AddWithValue("@name", ev.Name!);
                    cmd.Parameters.AddWithValue("@ticketLink", ev.TicketLink!);
                    cmd.Parameters.AddWithValue("@dateTime", ev.DateTime);
                    cmd.Parameters.AddWithValue("@venueId",
                        venueId.HasValue ? venueId.Value : DBNull.Value);

                    ev.Id = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Sync artists
                SyncArtists(ev.Id, artistIds, connection, transaction);

                transaction.Commit();

                return ev;
            }
            catch
            {
                transaction.Rollback();

                throw;
            }
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM events WHERE id = @id";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using var command = new NpgsqlCommand(sql, connection);

                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }
        private void SyncArtists(
            int eventId,
            int[] artistIds,
            NpgsqlConnection connection,
            NpgsqlTransaction transaction)
        {
            // Remove old relationships
            using (var deleteCmd = new NpgsqlCommand(
                "DELETE FROM event_artists WHERE event_id = @eventId",
                connection,
                transaction))
            {
                deleteCmd.Parameters.AddWithValue("@eventId", eventId);
                deleteCmd.ExecuteNonQuery();
            }

            // Insert new relationships
            if (artistIds.Length == 0)
                return;

            const string insertSql = @"INSERT INTO event_artists (event_id, artist_id) VALUES (@eventId, @artistId);";

            foreach (var artistId in artistIds)
            {
                using var insertCmd = new NpgsqlCommand(insertSql, connection, transaction);
                insertCmd.Parameters.AddWithValue("@eventId", eventId);
                insertCmd.Parameters.AddWithValue("@artistId", artistId);
                insertCmd.ExecuteNonQuery();
            }
        }
    }
}