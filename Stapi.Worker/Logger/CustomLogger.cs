using Npgsql;

namespace Stapi.Worker.Logger;

internal static class CustomLogger
{
    public static void ConsoleLog(string type, string message)
    {
        Console.WriteLine($"[{DateTime.UtcNow:u} {type}]: {message}");
    }

    public static void FileLog(string type, string message)
    {
        File.AppendAllText("data.log", $"[{DateTime.UtcNow:u} {type}]: {message}\n");
    }

    public static void DatabaseLog(string type, string message)
    {
        try
        {
            using var connection = new NpgsqlConnection("Host=postgres;Database=postgres;Username=postgres;Password=postgres");
            connection.Open();

            using var command = new NpgsqlCommand("INSERT INTO import.logs (app_name, created_at, type, message) VALUES ('" + nameof(Stapi)+"."+nameof(Worker) + "', now(), '" + type + "', '" + message + "')", connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
        catch
        {
        }
    }

    public static void Log(string type, string message)
    {
        ConsoleLog(type, message);
        FileLog(type, message);
        DatabaseLog(type, message);
    }
}