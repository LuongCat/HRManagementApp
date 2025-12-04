    using System.Data;
    using HRManagementApp.models;

    namespace HRManagementApp.DAL
    {
        public class VaiTroNhanVienReponsitory
        {

            private readonly NhanVienRepository _nhanVienRepository;

            public VaiTroNhanVienReponsitory(NhanVienRepository nhanVienRepository)
            {
                _nhanVienRepository = nhanVienRepository;
            }


            public List<VaiTroNhanVien> GetVaiTroNhanVien(int MaNV)
            {
                string query = @"
                    SELECT 
                        nvcv.MaNV,
                        nvcv.MaCV,
                        nvcv.LoaiChucVu,
                        nvcv.HeSoPhuCapKiemNhiem,
                        nvcv.GhiChu,
                        nvcv.HeSoLuongCoBan,

                        cv.MaCV AS CV_MaCV,
                        cv.TenCV AS CV_TenCV,
                        cv.PhuCap AS CV_PhuCap,
                        cv.LuongCB ,
                        cv.TienPhuCapKiemNhiem

                      
                    FROM nhanvien_chucvu nvcv
                    JOIN chucvu cv ON nvcv.MaCV = cv.MaCV
           
                    WHERE nvcv.MaNV = @MaNV;
                ";

                var paramNV = new Dictionary<string, object>
                {
                    { "@MaNV", MaNV }
                };

                DataTable dt = Database.ExecuteQuery(query, paramNV);

                List<VaiTroNhanVien> list = new List<VaiTroNhanVien>();

                foreach (DataRow row in dt.Rows)
                {
                    var item = new VaiTroNhanVien
                    {
                        MaNV = Convert.ToInt32(row["MaNV"]),
                        MaCV = Convert.ToInt32(row["MaCV"]),
                        LoaiChucVu = row["LoaiChucVu"].ToString(),
                        HeSoPhuCapKiemNhiem = Convert.ToDecimal(row["HeSoPhuCapKiemNhiem"]),
                        GhiChu = row["GhiChu"] == DBNull.Value ? "" : row["GhiChu"].ToString(),
                        HeSoLuongCoBan = Convert.ToDecimal(row["HeSoLuongCoBan"]),

                        ChucVu = new ChucVu
                        {
                            MaCV = Convert.ToInt32(row["CV_MaCV"]),
                            TenCV = row["CV_TenCV"].ToString(),
                            PhuCap = Convert.ToDecimal(row["CV_PhuCap"]),
                            LuongCB = Convert.ToDecimal(row["LuongCB"]),
                            TienPhuCapKiemNhiem = Convert.ToDecimal(row["TienPhuCapKiemNhiem"])
                        },

                        
                    };

                    list.Add(item);
                }

                return list;
            }


            public bool UpdateVaiTroNhanVien(VaiTroNhanVien vtnv)
            {
                string query = @"
            UPDATE nhanvien_chucvu
            SET 
                LoaiChucVu = @LoaiChucVu,
                TienPhuCapKiemNhiem = @TienPhuCapKiemNhiem,
                HeSoLuongCoBan = @HeSoLuongCoBan,
                GhiChu = @GhiChu
            WHERE MaNV = @MaNV AND MaCV = @MaCV;
        ";

                var parameters = new Dictionary<string, object>
                {
                    { "@MaNV", vtnv.MaNV },
                    { "@MaCV", vtnv.MaCV },
                  

                    { "@LoaiChucVu", vtnv.LoaiChucVu },
                    { "@TienPhuCapKiemNhiem", vtnv.HeSoPhuCapKiemNhiem ?? 0 },
                    {"@HeSoLuongCoBan", vtnv.HeSoLuongCoBan ?? 1 },
                    { "@GhiChu", string.IsNullOrEmpty(vtnv.GhiChu) ? DBNull.Value : vtnv.GhiChu }
                };

                int affected = Database.ExecuteNonQuery(query, parameters);

                return affected > 0;
            }

            public bool InsertVaiTroNhanVien(VaiTroNhanVien vtnv)
            {
                string query = @"
            INSERT INTO nhanvien_chucvu 
            (MaNV, MaCV, LoaiChucVu, HeSoPhuCapKiemNhiem, GhiChu)
            VALUES
            (@MaNV, @MaCV, @LoaiChucVu, @HeSoPhuCapKiemNhiem, @GhiChu);
        ";

                var parameters = new Dictionary<string, object>
                {
                    { "@MaNV", vtnv.MaNV },
                    { "@MaCV", vtnv.MaCV },
                    { "@LoaiChucVu", vtnv.LoaiChucVu },
                    { "@HeSoPhuCapKiemNhiem", vtnv.HeSoPhuCapKiemNhiem ?? 0 },
                    { "@GhiChu", string.IsNullOrEmpty(vtnv.GhiChu) ? DBNull.Value : vtnv.GhiChu }
                };

                int affected = Database.ExecuteNonQuery(query, parameters);
                return affected > 0;
            }

            public bool DeleteVaiTroNhanVien(int MaNV, int MaCV)
            {
                string query = @"
            DELETE FROM nhanvien_chucvu
            WHERE MaNV = @MaNV AND MaCV = @MaCV AND MaPB = @MaPB;
        ";

                var parameters = new Dictionary<string, object>
                {
                    { "@MaNV", MaNV },
                    { "@MaCV", MaCV },
                };

                int affected = Database.ExecuteNonQuery(query, parameters);
                return affected > 0;
            }

            
            
            public bool RemoveEmployeeRole(int MaNV)
            {
                // Xóa tất cả các vai trò của nhân viên trong phòng ban này
                string query = @"
            DELETE FROM nhanvien_chucvu
            WHERE MaNV = @MaNV ;
        ";

                var parameters = new Dictionary<string, object>
                {
                    { "@MaNV", MaNV },
                };

                int affected = Database.ExecuteNonQuery(query, parameters);

                // Trả về true nếu có ít nhất 1 bản ghi bị xóa
                return affected > 0;
            }



            public List<NhanVien> GetNhanVienOfPhongBan(int MaPB)
            {
                List<NhanVien> nhanViens = new List<NhanVien>();

                string query = @"
            SELECT MaNV 
            FROM nhanvien 
            WHERE MaPB = @MaPB;
        ";

                var parameter = new Dictionary<string, object>
                {
                    { "@MaPB", MaPB }
                };

                DataTable dt = Database.ExecuteQuery(query, parameter);

                foreach (DataRow row in dt.Rows) 
                {
                    int maNV = Convert.ToInt32(row["MaNV"]);
                    
                    var nv = _nhanVienRepository.GetEmployeeById(maNV);

                    if (nv != null)
                        nhanViens.Add(nv);
                }

                return nhanViens;
            }
            
            
            public List<NhanVien> GetEmployeesNotInDepartment()
            {
                List<NhanVien> nhanViens = new List<NhanVien>();

                string query = @"
        SELECT MaNV
        FROM nhanvien
        WHERE MaPB IS NULL;
    ";

                DataTable dt = Database.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    int maNV = Convert.ToInt32(row["MaNV"]);
                    var nv = _nhanVienRepository.GetEmployeeById(maNV);
                    if (nv != null)
                        nhanViens.Add(nv);
                }

                return nhanViens;
            }

            
            
            


        }
    }