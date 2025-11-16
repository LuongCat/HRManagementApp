using System;
using System.Collections.Generic;
using System.Data;
using HRManagementApp.models;
using MySql.Data.MySqlClient;

namespace HRManagementApp.DAL
{
    public class NhanVienRepository
    {
        // =====================================================
        // LẤY DANH SÁCH NHÂN VIÊN
        // =====================================================
        public List<NhanVien> GetListNhanVien()
        {
            string query = @"
                SELECT MaNV, HoTen, NgaySinh, SoCCCD, DienThoai,
                       NgayVaoLam, TrangThai, GioiTinh
                FROM NhanVien
            ";

            DataTable dt = Database.ExecuteQuery(query);
            var list = new List<NhanVien>();

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["MaNV"]);

                list.Add(new NhanVien
                {
                    MaNV = id,
                    HoTen = row["HoTen"].ToString(),
                    NgaySinh = row["NgaySinh"] as DateTime?,
                    SoCCCD = row["SoCCCD"]?.ToString(),
                    DienThoai = row["DienThoai"]?.ToString(),
                    GioiTinh = row["GioiTinh"]?.ToString(),
                    TrangThai = row["TrangThai"]?.ToString(),
                    NgayVaoLam = row["NgayVaoLam"] as DateTime?,

                    PhongBan = AllPhongBanOfNhanVien(id),
                    ChucVu = AllChucVuOfNhanVien(id)
                });
            }

            return list;
        }

        // =====================================================
        // LẤY PHÒNG BAN CỦA NHÂN VIÊN
        // =====================================================
        public List<PhongBan> AllPhongBanOfNhanVien(int maNV)
        {
            string query = @"
                SELECT pb.MaPB, pb.TenPB, pb.MoTa
                FROM NhanVien nv
                LEFT JOIN NhanVien_ChucVu nvcv ON nvcv.MaNV = nv.MaNV
                LEFT JOIN PhongBan pb ON pb.MaPB = nvcv.MaPB
                WHERE nv.MaNV = @MaNV
            ";

            var param = new Dictionary<string, object> { { "@MaNV", maNV } };
            DataTable dt = Database.ExecuteQuery(query, param);

            var list = new List<PhongBan>();

            foreach (DataRow row in dt.Rows)
            {
                if (row["MaPB"] == DBNull.Value) continue;

                list.Add(new PhongBan
                {
                    MaPB = (int)row["MaPB"],
                    TenPB = row["TenPB"]?.ToString(),
                    MoTa = row["MoTa"]?.ToString()
                });
            }

            return list;
        }

        // =====================================================
        // LẤY CHỨC VỤ CỦA NHÂN VIÊN
        // =====================================================
        public List<ChucVu> AllChucVuOfNhanVien(int maNV)
        {
            string query = @"
                SELECT cv.MaCV, cv.TenCV, cv.LuongCB, cv.PhuCap
                FROM NhanVien nv
                LEFT JOIN NhanVien_ChucVu nvcv ON nvcv.MaNV = nv.MaNV
                LEFT JOIN ChucVu cv ON cv.MaCV = nvcv.MaCV
                WHERE nv.MaNV = @MaNV
            ";

            var param = new Dictionary<string, object> { { "@MaNV", maNV } };
            DataTable dt = Database.ExecuteQuery(query, param);

            var list = new List<ChucVu>();

            foreach (DataRow row in dt.Rows)
            {
                if (row["MaCV"] == DBNull.Value) continue;

                list.Add(new ChucVu
                {
                    MaCV = (int)row["MaCV"],
                    TenCV = row["TenCV"]?.ToString(),
                    LuongCB = row["LuongCB"] != DBNull.Value ? (decimal)row["LuongCB"] : 0,
                    PhuCap = row["PhuCap"] != DBNull.Value ? (decimal)row["PhuCap"] : 0
                });
            }

            return list;
        }

        // =====================================================
        // LẤY NHÂN VIÊN THEO ID
        // =====================================================
        public NhanVien GetById(int id)
        {
            string query = "SELECT * FROM NhanVien WHERE MaNV = @MaNV";
            var param = new Dictionary<string, object> { { "@MaNV", id } };

            DataTable dt = Database.ExecuteQuery(query, param);
            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];

            return new NhanVien
            {
                MaNV = id,
                HoTen = row["HoTen"]?.ToString(),
                NgaySinh = row["NgaySinh"] as DateTime?,
                SoCCCD = row["SoCCCD"]?.ToString(),
                DienThoai = row["DienThoai"]?.ToString(),

                TrangThai = row["TrangThai"]?.ToString(),
                GioiTinh = row["GioiTinh"]?.ToString(),
                NgayVaoLam = row["NgayVaoLam"] as DateTime?,

                PhongBan = AllPhongBanOfNhanVien(id),
                ChucVu = AllChucVuOfNhanVien(id)
            };
        }

        // =====================================================
        // UPDATE NHÂN VIÊN
        // =====================================================
        public bool UpdateNhanVien(NhanVien nv)
        {
            string query = @"
                UPDATE NhanVien SET
                    HoTen = @HoTen,
                    NgaySinh = @NgaySinh,
                    SoCCCD = @SoCCCD,
                    DienThoai = @DienThoai,
                    GioiTinh = @GioiTinh,
                    NgayVaoLam = @NgayVaoLam,
                    TrangThai = @TrangThai
                WHERE MaNV = @MaNV
            ";

            var param = new Dictionary<string, object>
            {
                { "@MaNV", nv.MaNV },
                { "@HoTen", nv.HoTen },
                { "@NgaySinh", (object?)nv.NgaySinh ?? DBNull.Value },
                { "@SoCCCD", (object?)nv.SoCCCD ?? DBNull.Value },
                { "@DienThoai", (object?)nv.DienThoai ?? DBNull.Value },
                { "@GioiTinh", (object?)nv.GioiTinh ?? DBNull.Value },
                { "@NgayVaoLam", (object?)nv.NgayVaoLam ?? DBNull.Value },
                { "@TrangThai", nv.TrangThai }
            };

            return Database.ExecuteNonQuery(query, param) > 0;
        }

        // =====================================================
        // XÓA NHÂN VIÊN
        // =====================================================
        public bool DeleteNhanVien(int maNV)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                var param = new Dictionary<string, object> { { "@MaNV", maNV } };

                // Xóa bảng mapping
                Database.ExecuteNonQueryTransaction(
                    "DELETE FROM NhanVien_ChucVu WHERE MaNV = @MaNV",
                    param, conn, tran
                );

                // Xóa nhân viên
                int rows = Database.ExecuteNonQueryTransaction(
                    "DELETE FROM NhanVien WHERE MaNV = @MaNV",
                    param, conn, tran
                );

                if (rows == 0)
                {
                    tran.Rollback();
                    return false;
                }

                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Console.WriteLine("Lỗi khi xóa nhân viên: " + ex.Message);
                return false;
            }
        }

        // =====================================================
        // THÊM NHÂN VIÊN
        // =====================================================
        public bool AddNhanVien(NhanVien nv)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                // 1. Insert nhân viên
                string insertNV = @"
                    INSERT INTO NhanVien
                    (HoTen, NgaySinh, SoCCCD, DienThoai, GioiTinh, NgayVaoLam, TrangThai)
                    VALUES
                    (@HoTen, @NgaySinh, @SoCCCD, @DienThoai, @GioiTinh, @NgayVaoLam, @TrangThai);
                    SELECT LAST_INSERT_ID();
                ";

                var paramNV = new Dictionary<string, object>
                {
                    { "@HoTen", nv.HoTen },
                    { "@NgaySinh", (object?)nv.NgaySinh ?? DBNull.Value },
                    { "@SoCCCD", (object?)nv.SoCCCD ?? DBNull.Value },
                    { "@DienThoai", (object?)nv.DienThoai ?? DBNull.Value },
                    { "@GioiTinh", nv.GioiTinh },
                    { "@NgayVaoLam", (object?)nv.NgayVaoLam ?? DBNull.Value },
                    { "@TrangThai", nv.TrangThai }
                };

                int newMaNV;
                using (var cmd = new MySqlCommand(insertNV, conn, tran))
                {
                    foreach (var p in paramNV) cmd.Parameters.AddWithValue(p.Key, p.Value);
                    newMaNV = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 2. Insert mapping Chức Vụ + Phòng Ban
                foreach (var cv in nv.ChucVu)
                {
                    foreach (var pb in nv.PhongBan)
                    {
                        string insertMap = @"
                            INSERT INTO NhanVien_ChucVu
                            (MaNV, MaCV, MaPB, HeSoPhuCapKiemNhiem, TienPhuCapKiemNhiem, LoaiChucVu, GhiChu)
                            VALUES
                            (@MaNV, @MaCV, @MaPB, @HeSo, @TienPC, @Loai, @GhiChu)
                        ";

                        var paramMap = new Dictionary<string, object>
                        {
                            { "@MaNV", newMaNV },
                            { "@MaCV", cv.MaCV },
                            { "@MaPB", pb.MaPB },
                            { "@HeSo", 0 },
                            { "@TienPC", 0 },
                            { "@Loai", "Chính thức" },
                            { "@GhiChu", "" }
                        };

                        Database.ExecuteNonQueryTransaction(insertMap, paramMap, conn, tran);
                    }
                }

                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Console.WriteLine("Lỗi khi thêm nhân viên: " + ex.Message);
                return false;
            }
        }

        
        
        // =====================================================
        // CHỈNH TRẠNG THÁI CỦA NHÂN VIÊN
        // =====================================================
        public string GetSate(int MaNV)
        {
            string query = "SELECT TrangThai FROM NhanVien WHERE MaNV = @MaNV";
            var param = new Dictionary<string, object> { { "@MaNV", MaNV } };

            DataTable dt = Database.ExecuteQuery(query, param);

            if (dt.Rows.Count == 0)
                return null; // hoặc "" tùy logic

            return dt.Rows[0]["TrangThai"].ToString();
        }

        public bool ChangeSate(int MaNV)
        {
            string sate = GetSate(MaNV);
            if (string.Equals(sate?.Trim(), "Nghỉ Việc", StringComparison.OrdinalIgnoreCase))
            {
                sate = "Còn làm việc";
            }
            else
            {
                sate = "Nghỉ Việc";
            }


            string query = "UPDATE NhanVien SET TrangThai = @TrangThai WHERE MaNV = @MaNV";
            var param = new Dictionary<string, object>
            {
                { "@MaNV", MaNV },
                {"@TrangThai", sate }
            };
            
            return  Database.ExecuteNonQuery(query, param) > 0;
            
        }
    }
}
