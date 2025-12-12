using System;
using System.Collections.Generic;
using System.Data;
using HRManagementApp.models;

namespace HRManagementApp.DAL
{
    public class CaLamRepository
    {
        // 1. Lấy tất cả ca làm (không lấy ca đã xóa)
        public List<CaLam> GetAll()
        {
            string query = "SELECT * FROM calam WHERE TrangThai != 'Đã xóa'";
            DataTable dt = Database.ExecuteQuery(query);
            
            List<CaLam> list = new List<CaLam>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapDataRow(row));
            }
            return list;
        }

        // 2. Lấy ca làm theo ID
        public CaLam GetById(int maCa)
        {
            string query = "SELECT * FROM calam WHERE MaCa = @MaCa";
            var parameters = new Dictionary<string, object> { { "@MaCa", maCa } };
            
            DataTable dt = Database.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return MapDataRow(dt.Rows[0]);
            }
            return null;
        }

        // 3. Lấy danh sách ca làm của một nhân viên cụ thể (JOIN bảng trung gian)
        public List<CaLam> GetByNhanVien(int maNV)
        {
            string query = @"
                SELECT c.* FROM calam c
                JOIN nhanvien_calam nc ON c.MaCa = nc.MaCa
                WHERE nc.MaNV = @MaNV AND c.TrangThai = 'Hoạt động'";

            var parameters = new Dictionary<string, object> { { "@MaNV", maNV } };
            DataTable dt = Database.ExecuteQuery(query, parameters);

            List<CaLam> list = new List<CaLam>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapDataRow(row));
            }
            return list;
        }

        // 4. Tạo ca làm mới
        public bool CreateCaLam(CaLam ca)
        {
            string query = @"INSERT INTO calam (TenCa, GioBatDau, GioKetThuc, TrangThai) 
                             VALUES (@TenCa, @GioBatDau, @GioKetThuc, @TrangThai)";
            
            var parameters = new Dictionary<string, object>
            {
                { "@TenCa", ca.TenCa },
                { "@GioBatDau", ca.GioBatDau },
                { "@GioKetThuc", ca.GioKetThuc },
                { "@TrangThai", ca.TrangThai }
            };
            return Database.ExecuteNonQuery(query, parameters) > 0;
        }

        // 5. Cập nhật ca làm
        public bool UpdateCaLam(CaLam ca)
        {
            string query = @"UPDATE calam 
                             SET TenCa = @TenCa, GioBatDau = @GioBatDau, GioKetThuc = @GioKetThuc, TrangThai = @TrangThai 
                             WHERE MaCa = @MaCa";
            
            var parameters = new Dictionary<string, object>
            {
                { "@MaCa", ca.MaCa },
                { "@TenCa", ca.TenCa },
                { "@GioBatDau", ca.GioBatDau },
                { "@GioKetThuc", ca.GioKetThuc },
                { "@TrangThai", ca.TrangThai }
            };
            return Database.ExecuteNonQuery(query, parameters) > 0;
        }

        // 6. Xóa ca làm (Soft Delete - Chuyển trạng thái sang Đã xóa)
        public bool DeleteCaLam(int maCa)
        {
            string query = "UPDATE calam SET TrangThai = 'Đã xóa' WHERE MaCa = @MaCa";
            var parameters = new Dictionary<string, object>
            {
                { "@MaCa", maCa }
            };
            return Database.ExecuteNonQuery(query, parameters) > 0;
        }

        // 7. Gán ca làm cho nhân viên
        public bool AssignCaLamToNhanVien(int maNV, int maCa)
        {
            // Sử dụng INSERT IGNORE (hoặc check exists) để tránh lỗi trùng lặp nếu đã gán rồi
            string query = "INSERT IGNORE INTO nhanvien_calam (MaNV, MaCa) VALUES (@MaNV, @MaCa)";
            var parameters = new Dictionary<string, object>
            {
                { "@MaNV", maNV },
                { "@MaCa", maCa }
            };
            return Database.ExecuteNonQuery(query, parameters) > 0;
        }

        // 8. Hủy gán ca làm của nhân viên
        public bool RemoveCaLamFromNhanVien(int maNV, int maCa)
        {
            string query = "DELETE FROM nhanvien_calam WHERE MaNV = @MaNV AND MaCa = @MaCa";
            var parameters = new Dictionary<string, object>
            {
                { "@MaNV", maNV },
                { "@MaCa", maCa }
            };
            return Database.ExecuteNonQuery(query, parameters) > 0;
        }

        // Hàm helper để map dữ liệu từ DataRow sang Object CaLam
        private CaLam MapDataRow(DataRow row)
        {
            return new CaLam
            {
                MaCa = Convert.ToInt32(row["MaCa"]),
                TenCa = row["TenCa"].ToString(),
                // Lưu ý: MySQL Time map sang C# TimeSpan
                GioBatDau = row["GioBatDau"] != DBNull.Value ? (TimeSpan)row["GioBatDau"] : TimeSpan.Zero,
                GioKetThuc = row["GioKetThuc"] != DBNull.Value ? (TimeSpan)row["GioKetThuc"] : TimeSpan.Zero,
                TrangThai = row["TrangThai"].ToString()
            };
        }
    }
}