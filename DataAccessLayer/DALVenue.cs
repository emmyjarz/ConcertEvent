using Npgsql;

namespace ConcertEvent.DataAccessLayer
{
    public class DALVenue
    {
        private readonly string? _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        public DALVenue()
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

            var sql = "SELECT id FROM venues WHERE name = @name";

            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@name", name);

            var result = cmd.ExecuteScalar();

            return result != null ? Convert.ToInt32(result) : (int?)null;
        }

        public int Insert(string name)
        {
            string sql = $"INSERT INTO venues (name) VALUES (@name) RETURNING id";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using var command = new NpgsqlCommand(sql, connection);

                command.Parameters.AddWithValue("@name", name);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        // Upsert venue: return venue ID
        public int GetOrCreateByName(string name)
        {
            var existingId = getIdByName(name);

            if (existingId.HasValue)
            {
                return existingId.Value;
            }

            return Insert(name);
        }
    }
}
