using Npgsql;

namespace ConcertEvent.DataAccessLayer
{
    public class DALArtist
    {
        private readonly string? _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        public DALArtist()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Database connection string is not set in environment variables.");
            }
        }

        public int? getIdByName(string name)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            conn.Open();

            var sql = "SELECT id FROM artists WHERE name = @name";

            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@name", name);

            var result = cmd.ExecuteScalar();

            return result != null ? Convert.ToInt32(result) : (int?)null;
        }

        public int Insert(string name)
        {
            string sql = $"INSERT INTO artists (name) VALUES (@name) RETURNING id";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using var command = new NpgsqlCommand(sql, connection);

                command.Parameters.AddWithValue("@name", name);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        // Upsert artist: return artist ID
        public int GetOrCreateByName(string name)
        {
            var existingId = getIdByName(name);

            if (existingId.HasValue)
            {
                return existingId.Value;
            }

            return Insert(name);
        }

        public int[] GetOrCreateByNames(string? names)
        {
            if (string.IsNullOrEmpty(names))
            {
                return Array.Empty<int>();
            }

            return names.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(name => new DALArtist().GetOrCreateByName(name.Trim()))
                      .ToArray() ?? Array.Empty<int>();
        }
    }
}
