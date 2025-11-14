using System.Collections.Generic;
using System.Data;
using HRManagementApp.models;

namespace HRManagementApp.DAL
{
    public class NhanVienRepository
    {
        public List<NhanVien> GetListNhanVien()
        {
            string query = @"
                SELECT 
                    nv.MaNV, nv.HoTen, nv.NgaySinh, nv.SoCCCD, nv.DienThoai, nv.NgayVaoLam, nv.Trangthai,nv.GioiTinh
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
                    GioiTinh  = Convert.ToBoolean(row["GioiTinh"]),
                    
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


        public List<PhongBan> AllPhongBanOfNhanVien(int MaNV)
        {
            String query = @" select
               pb.MaPB, pb.TenPB,pb.MoTa
               from NhanVien nv 
               Left Join nhanvien_phongban nvpb on nvpb.MaNV = nv.MaNV
               Left Join phongban pb on pb.MaPB = nvpb.MaPB
               where nv.MaNV=@MaNV
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@MaNV", MaNV }
            };
            DataTable data = Database.ExecuteQuery(query, parameters);

            var list = new List<PhongBan>();
            foreach (DataRow row in data.Rows)
            {
                if (row["MaPB"] == DBNull.Value)
                {
                    continue;
                }

                var Phongban = new PhongBan
                {
                    MaPB = (int)row["MaPB"],
                    TenPB = row["TenPB"]?.ToString() ?? "",
                    MoTa = row["MoTa"]?.ToString() ?? ""
                };
                list.Add(Phongban);
            }

            return list;
        }

        public List<ChucVu> AllChucVuOfNhanVien(int MaNV)
        {
            string query = @" select 
                cv.MaCV,cv.TenCV,cv.LuongCB,cv.PhuCap
                From NhanVien nv 
                LEFT JOIN  nhanvien_chucvu nvcv on nvcv.MaNV=nv.MaNV
                LEFT JOIN chucvu cv  on nvcv.MaCV=cv.MaCV
                WHERE nv.MaNV=@MaNV
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@MaNV", MaNV }
            };

            DataTable data = Database.ExecuteQuery(query, parameters);
            var list = new List<ChucVu>();
            foreach (DataRow row in data.Rows)
            {
                if (row["MaCV"] == DBNull.Value)
                {
                    continue;
                }

                var ChucVu = new ChucVu
                {
                    MaCV = (int)row["MaCV"],
                    TenCV = row["TenCV"]?.ToString() ?? "",
                    LuongCB = row["LuongCB"] != DBNull.Value ? (decimal)row["LuongCB"] : 0,
                    PhuCap = row["PhuCap"] != DBNull.Value ? (decimal)row["PhuCap"] : 0
                };
                list.Add(ChucVu);
            }

            return list;
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

                { "@NgayVaoLam", (object?)nv.NgayVaoLam ?? DBNull.Value },
                { "@TrangThai", nv.TrangThai }
            };

            // Giả sử bạn có một class Database với ExecuteNonQuery
            int rowsAffected = Database.ExecuteNonQuery(query, parameters);

            return rowsAffected > 0;
        }




        public bool DeleteNhanVien(int maNV)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                var param = new Dictionary<string, object> { { "@MaNV", maNV } };

                // Xóa mapping
                Database.ExecuteNonQueryTransaction("DELETE FROM NhanVien_PhongBan WHERE MaNV = @MaNV", param, conn,
                    transaction);
                Database.ExecuteNonQueryTransaction("DELETE FROM NhanVien_ChucVu WHERE MaNV = @MaNV", param, conn,
                    transaction);

                // Xóa nhân viên
                int rows = Database.ExecuteNonQueryTransaction("DELETE FROM NhanVien WHERE MaNV = @MaNV", param, conn,
                    transaction);

                if (rows == 0)
                {
                    transaction.Rollback();
                    return false;
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("Lỗi khi xóa nhân viên: " + ex.Message);
                return false;
            }

        }

    }

}