using HRManagementApp.Models;
using MySql.Data.MySqlClient;

namespace HRManagementApp.DAL
{
    public class TaiKhoanRepository
    {
        private readonly string _connectionString = "server=localhost;user=root;password=123456;database=hrmanagement;";

        public TaiKhoan? GetTaiKhoanByUsername(string username)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            string query = "SELECT * FROM taikhoan WHERE TenDangNhap=@username";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new TaiKhoan
                {
                    MaTK = reader.GetInt32("MaTK"),
                    MaNV = reader.IsDBNull(reader.GetOrdinal("MaNV")) ? null : reader.GetInt32("MaNV"),
                    TenDangNhap = reader.GetString("TenDangNhap"),
                    MatKhau = reader.GetString("MatKhau"),
                    TrangThai = reader.GetString("TrangThai")
                };
            }
            return null;
        }
    }
}
