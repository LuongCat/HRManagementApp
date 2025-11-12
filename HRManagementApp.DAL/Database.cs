namespace HRManagementApp.DAL;
using MySql.Data.MySqlClient;
using System.Data;


public static class Database
{
    private static readonly string connectionString = 
        "Server=localhost;Port=3306;Database=hrmanagement;Uid=root;Pwd=123456789;Charset=utf8;";

    public static MySqlConnection GetConnection()
    {
        var connection = new MySqlConnection(connectionString);
        return connection;
    }
    
    
    public static DataTable ExecuteQuery(string query, Dictionary<string, object>? parameters = null)
    {
        DataTable data = new DataTable();

        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = new MySqlCommand(query, conn))
            {
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(data);
                }
            }
        }

        return data;
    }
    
    
    public static int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = new MySqlCommand(query, conn))
            {
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                return cmd.ExecuteNonQuery();
            }
        }
    }

    public static object ExecuteScalar(string query)
    {
        using var conn = GetConnection();
        using var cmd = new MySqlCommand(query, conn);
        
        conn.Open();
        return cmd.ExecuteScalar();
    }
}