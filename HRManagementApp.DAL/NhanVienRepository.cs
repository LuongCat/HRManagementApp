using System.Collections.Generic;
using System.Data;
using HRManagementApp.models;

namespace HRManagementApp.DAL
{
    public class NhanVienRepository
    {
        public List<NhanVien> GetAll()
        {
            string query = "SELECT * FROM NhanVien";
            DataTable data = Database.ExecuteQuery(query);
            
            var list = new List<NhanVien>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(new NhanVien
                {
                    MaNV = (int)row["MaNV"],
                    HoTen = row["HoTen"].ToString(),
                    NgaySinh = row["NgaySinh"] as DateTime?,
                    SoCCCD = row["SoCCCD"].ToString(),
                    DienThoai = row["DienThoai"].ToString(),
                    MaPB = row["MaPB"] as int?,
                    MaCV = row["MaCV"] as int?,
                    TrangThai = row["TrangThai"] as string, 
                    NgayVaoLam = row["NgayVaoLam"] as DateTime?
                });
            }
            return list;
        }
        
       
       public List<NhanVien> GetAllFull()
        {
            string query = @"
                SELECT 
                    nv.MaNV, nv.HoTen, nv.NgaySinh, nv.SoCCCD, nv.DienThoai, nv.NgayVaoLam, nv.Trangthai,
                    pb.MaPB, pb.TenPB, pb.MoTa,
                    cv.MaCV, cv.TenCV, cv.LuongCB, cv.PhuCap
                FROM NhanVien nv
                LEFT JOIN PhongBan pb ON nv.MaPB = pb.MaPB
                LEFT JOIN ChucVu cv ON nv.MaCV = cv.MaCV
                
            ";

            DataTable data = Database.ExecuteQuery(query);
            var list = new List<NhanVien>();

            foreach (DataRow row in data.Rows)
            {
                var nv = new NhanVien
                {
                    MaNV = (int)row["MaNV"],
                    HoTen = row["HoTen"].ToString(),
                    NgaySinh = row["NgaySinh"] as DateTime?,
                    SoCCCD = row["SoCCCD"].ToString(),
                    DienThoai = row["DienThoai"].ToString(),
                    NgayVaoLam = row["NgayVaoLam"] as DateTime?,
                    TrangThai = row["Trangthai"].ToString(),
                    PhongBan = new PhongBan
                    {
                        MaPB = row["MaPB"] != DBNull.Value ? (int)row["MaPB"] : 0,
                        TenPB = row["TenPB"]?.ToString(),
                        MoTa = row["MoTa"]?.ToString()
                    },

                    ChucVu = new ChucVu
                    {
                        MaCV = row["MaCV"] != DBNull.Value ? (int)row["MaCV"] : 0,
                        TenCV = row["TenCV"]?.ToString(),
                        LuongCB = row["LuongCB"] != DBNull.Value ? (decimal)row["LuongCB"] : 0,
                        PhuCap = row["PhuCap"] != DBNull.Value ? (decimal)row["PhuCap"] : 0
                    }
                };
                
                
                

                list.Add(nv);
            }

            return list;
        }

        public int CountEmployees()
        {
            string query = "SELECT COUNT(*) as TotalEmployee FROM NhanVien";
            object result = Database.ExecuteScalar(query);
            
            return  Convert.ToInt32(result);
        }

        public int CountJoinedThisMonth()
        {
            string query = "SELECT COUNT(*) as TotalEmployee FROM NhanVien " +
                           "WHERE NgayVaoLam >= DATE_FORMAT(CURDATE(), '%Y-%m-01') " +
                           "AND NgayVaoLam < DATE_ADD(DATE_FORMAT(CURDATE(), '%Y-%m-01'), INTERVAL 1 MONTH)";
            
            object result = Database.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        public NhanVien GetById(int id)
        {
            String query = @" Select * From  NhanVien Where MaNV = @MaNV";
            
            var parameters = new Dictionary<string, object>
            {
                { "@MaNV", id }
            };
            
            DataTable data = Database.ExecuteQuery(query, parameters);
            if (data.Rows.Count == 0)
                return null;
            DataRow row = data.Rows[0];
            var nhanvien = new NhanVien
            {
                MaNV = (int)row["MaNV"],
                HoTen = row["HoTen"].ToString(),
                NgaySinh = row["NgaySinh"] as DateTime?,
                SoCCCD = row["SoCCCD"].ToString(),
                DienThoai = row["DienThoai"].ToString(),
                MaPB = row["MaPB"] as int?,
                MaCV = row["MaCV"] as int?,
                TrangThai = row["TrangThai"] as string,
                NgayVaoLam = row["NgayVaoLam"] as DateTime?
            };
            
            return nhanvien;
        }
        
        
        public bool UpdateNhanVien(NhanVien nv)
        {
            string query = @"
        UPDATE NhanVien
        SET 
            HoTen = @HoTen,
            NgaySinh = @NgaySinh,
            SoCCCD = @SoCCCD,
            DienThoai = @DienThoai,
            MaPB = @MaPB,
            MaCV = @MaCV,
            NgayVaoLam = @NgayVaoLam,
            TrangThai = @TrangThai
        WHERE MaNV = @MaNV
    ";

            var parameters = new Dictionary<string, object>
            {
                { "@MaNV", nv.MaNV },
                { "@HoTen", nv.HoTen },
                { "@NgaySinh", (object?)nv.NgaySinh ?? DBNull.Value },
                { "@SoCCCD", (object?)nv.SoCCCD ?? DBNull.Value },
                { "@DienThoai", (object?)nv.DienThoai ?? DBNull.Value },
                { "@MaPB", (object?)nv.MaPB ?? DBNull.Value },
                { "@MaCV", (object?)nv.MaCV ?? DBNull.Value },
                { "@NgayVaoLam", (object?)nv.NgayVaoLam ?? DBNull.Value },
                { "@TrangThai", nv.TrangThai }
            };

            // Giả sử bạn có một class Database với ExecuteNonQuery
            int rowsAffected = Database.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }

       
    }
}