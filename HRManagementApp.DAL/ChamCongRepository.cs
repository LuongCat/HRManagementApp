namespace HRManagementApp.DAL;

using HRManagementApp.models;
using System.Data;

public class ChamCongRepository
{
    public List<ChamCong> GetAllChamCongByNhanVienId(int maNV)
    {
        var listChamCong = new List<ChamCong>();

        string query = @"
        SELECT MaCC, MaNV, Ngay, GioVao, GioRa, ThoiGianLam
        FROM chamcong
        WHERE MaNV = @MaNV
        ORDER BY Ngay DESC, GioVao DESC";

        var parameters = new Dictionary<string, object>
        {
            { "@MaNV", maNV }
        };

        // 2. Thực thi query lấy về DataTable
        DataTable data = Database.ExecuteQuery(query, parameters);

        // 3. Duyệt từng dòng và map vào List
        if (data != null && data.Rows.Count > 0)
        {
            foreach (DataRow row in data.Rows)
            {
                var item = new ChamCong
                {
                    MaCC = Convert.ToInt32(row["MaCC"]),
                    MaNV = Convert.ToInt32(row["MaNV"]),

                    // Xử lý Ngày (DateTime)
                    Ngay = row["Ngay"] != DBNull.Value
                        ? Convert.ToDateTime(row["Ngay"])
                        : (DateTime?)null,

                    // Xử lý Giờ Vào (TimeSpan)
                    // Lưu ý: MySQL TIME map sang C# là TimeSpan
                    GioVao = row["GioVao"] != DBNull.Value
                        ? (TimeSpan)row["GioVao"]
                        : (TimeSpan?)null,

                    // Xử lý Giờ Ra
                    GioRa = row["GioRa"] != DBNull.Value
                        ? (TimeSpan)row["GioRa"]
                        : (TimeSpan?)null,

                    // Xử lý Thời gian làm (Cột tự tính)
                    ThoiGianLam = row["ThoiGianLam"] != DBNull.Value
                        ? (TimeSpan)row["ThoiGianLam"]
                        : (TimeSpan?)null
                };

                listChamCong.Add(item);
            }
        }

        return listChamCong;
    }

    public List<ChamCong> GetChamCongByMonth(int maNV, int thang, int nam)
    {
        var listChamCong = new List<ChamCong>();
        string query = @"
        SELECT MaCC, MaNV, Ngay, GioVao, GioRa, ThoiGianLam
        FROM chamcong
        WHERE MaNV = @MaNV 
          AND MONTH(Ngay) = @Thang 
          AND YEAR(Ngay) = @Nam
        ORDER BY Ngay ASC"; // Xem lịch tháng thì xếp từ ngày 1 -> 30

        var parameters = new Dictionary<string, object>
        {
            { "@MaNV", maNV },
            { "@Thang", thang },
            { "@Nam", nam }
        };

        DataTable data = Database.ExecuteQuery(query, parameters);

        // 3. Duyệt từng dòng và map vào List
        if (data != null && data.Rows.Count > 0)
        {
            foreach (DataRow row in data.Rows)
            {
                var item = new ChamCong
                {
                    MaCC = Convert.ToInt32(row["MaCC"]),
                    MaNV = Convert.ToInt32(row["MaNV"]),

                    // Xử lý Ngày (DateTime)
                    Ngay = row["Ngay"] != DBNull.Value
                        ? Convert.ToDateTime(row["Ngay"])
                        : (DateTime?)null,

                    // Xử lý Giờ Vào (TimeSpan)
                    // Lưu ý: MySQL TIME map sang C# là TimeSpan
                    GioVao = row["GioVao"] != DBNull.Value
                        ? (TimeSpan)row["GioVao"]
                        : (TimeSpan?)null,

                    // Xử lý Giờ Ra
                    GioRa = row["GioRa"] != DBNull.Value
                        ? (TimeSpan)row["GioRa"]
                        : (TimeSpan?)null,

                    // Xử lý Thời gian làm (Cột tự tính)
                    ThoiGianLam = row["ThoiGianLam"] != DBNull.Value
                        ? (TimeSpan)row["ThoiGianLam"]
                        : (TimeSpan?)null
                };

                listChamCong.Add(item);
            }
        }

        return listChamCong;
    }

    public List<ChamCong> GetAllAttendancByMonthYear(int Day ,int Month, int Year)
    {
        var listChamCong = new List<ChamCong>();
        string query = @"
        SELECT MaCC, MaNV, Ngay, GioVao, GioRa, ThoiGianLam
        FROM chamcong
        WHERE  
          MONTH(Ngay) = @Thang 
          AND YEAR(Ngay) = @Nam
            AND DAY(Ngay)=@Ngay
        ORDER BY Ngay ASC";

        var parameters = new Dictionary<string, object>
        {
            { "@Thang", Month },
            { "@Nam", Year },
            { "@Ngay", Day },
        }; 
        
        DataTable data = Database.ExecuteQuery(query, parameters);
        
        if (data != null && data.Rows.Count > 0)
        {
            foreach (DataRow row in data.Rows)
            {
                var item = new ChamCong
                {
                    MaCC = Convert.ToInt32(row["MaCC"]),
                    MaNV = Convert.ToInt32(row["MaNV"]),

                    // Xử lý Ngày (DateTime)
                    Ngay = row["Ngay"] != DBNull.Value
                        ? Convert.ToDateTime(row["Ngay"])
                        : (DateTime?)null,

                    // Xử lý Giờ Vào (TimeSpan)
                    // Lưu ý: MySQL TIME map sang C# là TimeSpan
                    GioVao = row["GioVao"] != DBNull.Value
                        ? (TimeSpan)row["GioVao"]
                        : (TimeSpan?)null,

                    // Xử lý Giờ Ra
                    GioRa = row["GioRa"] != DBNull.Value
                        ? (TimeSpan)row["GioRa"]
                        : (TimeSpan?)null,
                    
                    // Xử lý Thời gian làm (Cột tự tính)
                    ThoiGianLam = row["ThoiGianLam"] != DBNull.Value
                        ? (TimeSpan)row["ThoiGianLam"]
                        : (TimeSpan?)null
                };

                listChamCong.Add(item);
            }
        }
        
        return listChamCong;
    }
    
    
    
    
    public KetQuaChamCong GetChamCongStatistics(int maNV, int thang, int nam)
    {
        var ketQua = new KetQuaChamCong();
        
        string query = @"
        SELECT ThoiGianLam
        FROM chamcong
        WHERE MaNV = @MaNV 
          AND MONTH(Ngay) = @Thang 
          AND YEAR(Ngay) = @Nam
          AND ThoiGianLam IS NOT NULL;";

        var parameters = new Dictionary<string, object>
        {
            { "@MaNV", maNV },
            { "@Thang", thang },
            { "@Nam", nam } 
        };

      
        DataTable data = Database.ExecuteQuery(query, parameters);

        // 3. Xử lý tính toán
        if (data != null && data.Rows.Count > 0)
        {
            foreach (DataRow row in data.Rows)
            {
                if (row["ThoiGianLam"] == DBNull.Value) continue;

                // MySQL TIME -> C# TimeSpan
                TimeSpan thoiGianLam = (TimeSpan)row["ThoiGianLam"];
                double soGio = thoiGianLam.TotalHours;

                // --- LOGIC TÍNH CÔNG & ĐI TRỄ ---

                // Trường hợp 1: Dưới 4 tiếng -> KHÔNG TÍNH CÔNG
                if (soGio < 4)
                {
                    continue; // Bỏ qua ngày này
                }

                // Trường hợp 2: Từ 4 đến dưới 5 tiếng -> TÍNH CÔNG, PHẠT +2
                else if (soGio >= 4 && soGio < 5)
                {
                    ketQua.SoNgayDiLam++;
                    ketQua.DiemDiTre += 2;
                }

                // Trường hợp 3: Từ 5 đến dưới 6 tiếng -> TÍNH CÔNG, PHẠT +1
                else if (soGio >= 5 && soGio < 6)
                {
                    ketQua.SoNgayDiLam++;
                    ketQua.DiemDiTre += 1;
                }

                // Trường hợp 4: Trên 6 tiếng -> TÍNH CÔNG, KHÔNG PHẠT
                else
                {
                    ketQua.SoNgayDiLam++;
                }
            }
        }

        return ketQua;
    }
    
    
    // =========================================================
    // PHẦN 2: BỔ SUNG THÊM - SỬA - XÓA (CRUD)
    // =========================================================

    // 1. Lấy chi tiết 1 bản ghi chấm công (Dùng để hiển thị lên form sửa)
    public ChamCong GetChamCongById(int maCC)
    {
        string query = "SELECT * FROM chamcong WHERE MaCC = @MaCC";
        var parameters = new Dictionary<string, object> { { "@MaCC", maCC } };

        DataTable data = Database.ExecuteQuery(query, parameters);
        if (data != null && data.Rows.Count > 0)
        {
            return MapDataRowToChamCong(data.Rows[0]);
        }
        return null;
    }

    // 2. Thêm mới chấm công (Thường dùng cho Admin thêm tay nếu nhân viên quên chấm)
    public bool AddChamCong(ChamCong cc)
    {
        // Không insert MaCC (Auto Increment) và ThoiGianLam (Generated Column)
        string query = @"
            INSERT INTO chamcong (MaNV, Ngay, GioVao, GioRa) 
            VALUES (@MaNV, @Ngay, @GioVao, @GioRa)";

        var parameters = new Dictionary<string, object>
        {
            { "@MaNV", cc.MaNV },
            { "@Ngay", cc.Ngay ?? (object)DBNull.Value },
            { "@GioVao", cc.GioVao ?? (object)DBNull.Value },
            { "@GioRa", cc.GioRa ?? (object)DBNull.Value }
        };

        // Giả sử Database.ExecuteNonQuery trả về số dòng bị ảnh hưởng (int)
        return Database.ExecuteNonQuery(query, parameters) > 0;
    }

    // 3. Cập nhật chấm công (Sửa giờ vào/ra)
    public bool UpdateChamCong(ChamCong cc)
    {
        string query = @"
            UPDATE chamcong 
            SET Ngay = @Ngay, 
                GioVao = @GioVao, 
                GioRa = @GioRa 
            WHERE MaCC = @MaCC";

        var parameters = new Dictionary<string, object>
        {
            { "@MaCC", cc.MaCC },
            { "@Ngay", cc.Ngay ?? (object)DBNull.Value },
            { "@GioVao", cc.GioVao ?? (object)DBNull.Value },
            { "@GioRa", cc.GioRa ?? (object)DBNull.Value }
        };

        return Database.ExecuteNonQuery(query, parameters) > 0;
    }

    // 4. Xóa chấm công
    public bool DeleteChamCong(int maCC)
    {
        string query = "DELETE FROM chamcong WHERE MaCC = @MaCC";
        var parameters = new Dictionary<string, object> { { "@MaCC", maCC } };

        return Database.ExecuteNonQuery(query, parameters) > 0;
    }

    // =========================================================
    // HÀM PHỤ TRỢ (HELPER) ĐỂ MAP DỮ LIỆU
    // =========================================================
    private ChamCong MapDataRowToChamCong(DataRow row)
    {
        return new ChamCong
        {
            MaCC = Convert.ToInt32(row["MaCC"]),
            MaNV = Convert.ToInt32(row["MaNV"]),
            Ngay = row["Ngay"] != DBNull.Value ? Convert.ToDateTime(row["Ngay"]) : (DateTime?)null,
            GioVao = row["GioVao"] != DBNull.Value ? (TimeSpan)row["GioVao"] : (TimeSpan?)null,
            GioRa = row["GioRa"] != DBNull.Value ? (TimeSpan)row["GioRa"] : (TimeSpan?)null,
            ThoiGianLam = row["ThoiGianLam"] != DBNull.Value ? (TimeSpan)row["ThoiGianLam"] : (TimeSpan?)null
        };
    }
    
    

   public ChamCong GetTodayRecord(int maNV)
        {
            string query = "SELECT * FROM chamcong WHERE MaNV = @MaNV AND Ngay = CURRENT_DATE() LIMIT 1";
            var param = new Dictionary<string, object> { { "@MaNV", maNV } };
            
            DataTable dt = Database.ExecuteQuery(query, param);
            if (dt.Rows.Count > 0)
            {
                return MapDataRow(dt.Rows[0]);
            }
            return null;
        }

        public bool CheckIn(int maNV)
        {
            // Insert bản ghi mới với Giờ vào là hiện tại, Giờ ra là NULL
            string query = @"INSERT INTO chamcong (MaNV, Ngay, GioVao, GioRa) 
                             VALUES (@MaNV, CURRENT_DATE(), CURRENT_TIME(), NULL)";
            
            var param = new Dictionary<string, object> { { "@MaNV", maNV } };
            return Database.ExecuteNonQuery(query, param) > 0;
        }

        public bool CheckOut(int maCC)
        {
            string query = "UPDATE chamcong SET GioRa = CURRENT_TIME() WHERE MaCC = @MaCC";
            var param = new Dictionary<string, object> { { "@MaCC", maCC } };
            return Database.ExecuteNonQuery(query, param) > 0;
        }

        public List<ChamCong> GetByMonth(int maNV, int month, int year)
        {
            List<ChamCong> list = new List<ChamCong>();
            string query = @"SELECT *, 
                             TIMEDIFF(GioRa, GioVao) as ThoiGianLamTinhToan 
                             FROM chamcong 
                             WHERE MaNV = @MaNV AND MONTH(Ngay) = @Month AND YEAR(Ngay) = @Year
                             ORDER BY Ngay ASC";

            var param = new Dictionary<string, object> 
            { 
                { "@MaNV", maNV },
                { "@Month", month },
                { "@Year", year }
            };

            DataTable dt = Database.ExecuteQuery(query, param);
            foreach (DataRow row in dt.Rows)
            {
                var cc = MapDataRow(row);
                // Nếu DB chưa có cột ảo ThoiGianLam thì lấy từ tính toán
                if (row["ThoiGianLamTinhToan"] != DBNull.Value)
                {
                    cc.ThoiGianLam = (TimeSpan)row["ThoiGianLamTinhToan"];
                }
                list.Add(cc);
            }
            return list;
        }

        public List<ChamCong> GetByDate(int maNV, DateTime date)
        {
             List<ChamCong> list = new List<ChamCong>();
             string query = "SELECT * FROM chamcong WHERE MaNV = @MaNV AND Ngay = @Ngay";
             var param = new Dictionary<string, object> 
             { 
                 { "@MaNV", maNV },
                 { "@Ngay", date.ToString("yyyy-MM-dd") }
             };
             DataTable dt = Database.ExecuteQuery(query, param);
             foreach(DataRow row in dt.Rows) list.Add(MapDataRow(row));
             return list;
        }

        private ChamCong MapDataRow(DataRow row)
        {
            return new ChamCong
            {
                MaCC = Convert.ToInt32(row["MaCC"]),
                MaNV = Convert.ToInt32(row["MaNV"]),
                Ngay = row["Ngay"] != DBNull.Value ? Convert.ToDateTime(row["Ngay"]) : null,
                GioVao = row["GioVao"] != DBNull.Value ? (TimeSpan)row["GioVao"] : null,
                GioRa = row["GioRa"] != DBNull.Value ? (TimeSpan)row["GioRa"] : null
            };
        }
}