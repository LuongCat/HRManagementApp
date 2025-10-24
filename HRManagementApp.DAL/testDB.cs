namespace HRManagementApp.DAL;
using MySql.Data.MySqlClient;

public class testDB
{
    public void CheckConnection()
    {
        using (MySqlConnection conn = Database.GetConnection())
        {
            try
            {
                conn.Open();
                Console.WriteLine("✅ Kết nối thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi kết nối: " + ex.Message);
            }
        }
    }
}