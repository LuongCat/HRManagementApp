namespace HRManagementApp.DAL;

using HRManagementApp.models;
using System.Data;

public class LuongRepository
{
    public List<Luong> GetAllSalary()
    {
        string query = "SELECT * FROM Luong";
        DataTable dt = Database.ExecuteQuery(query);

        List<Luong> list = new List<Luong>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new Luong
            {
                MaLuong = int.Parse(row["MaLuong"].ToString()),
                MaNV = int.Parse(row["MaNV"].ToString()),
                Thang = int.Parse(row["Thang"].ToString()),
                Nam = int.Parse(row["Nam"].ToString()),
                TongNgayCong = int.Parse(row["TongNgayCong"].ToString()),
                TienLuong = decimal.Parse(row["TienLuong"].ToString()),
                LuongThucNhan = decimal.Parse(row["LuongThucNhan"].ToString()),
                TrangThai = row["TrangThai"].ToString(),
                ChotLuong = row["ChotLuong"].ToString(),
            });
        }

        return list;
    }

    // Lấy theo mã nhân viên
    public List<Luong> GetSalaryByMaNV(int maNV)
    {
        string query = "SELECT * FROM luong WHERE MaNV=@MaNV";
        var parameters = new Dictionary<string, object> { { "@MaNV", maNV } };
        DataTable dt = Database.ExecuteQuery(query, parameters);

        List<Luong> list = new List<Luong>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new Luong
            {
                MaLuong = int.Parse(row["MaLuong"].ToString()),
                MaNV = int.Parse(row["MaNV"].ToString()),
                Thang = int.Parse(row["Thang"].ToString()),
                Nam = int.Parse(row["Nam"].ToString()),
                TongNgayCong = int.Parse(row["TongNgayCong"].ToString()),
                TienLuong = decimal.Parse(row["TienLuong"].ToString()),
                LuongThucNhan = decimal.Parse(row["LuongThucNhan"].ToString()),
                TrangThai = row["TrangThai"].ToString(),
                ChotLuong = row["ChotLuong"].ToString(),
            });
        }

        return list;
    }

    public Luong GetSalaryByMonthYear(int maNV, int thang, int nam)
    {
        string query = @"
        SELECT * FROM Luong
        WHERE MaNV = @MaNV AND Thang = @Thang AND Nam = @Nam
    ";

        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "@MaNV", maNV },
            { "@Thang", thang },
            { "@Nam", nam }
        };

        DataTable dt = Database.ExecuteQuery(query, parameters);

        if (dt.Rows.Count == 0)
            return null;

        DataRow row = dt.Rows[0];

        return new Luong
        {
            MaLuong = int.Parse(row["MaLuong"].ToString()),
            MaNV = int.Parse(row["MaNV"].ToString()),
            Thang = int.Parse(row["Thang"].ToString()),
            Nam = int.Parse(row["Nam"].ToString()),
            TongNgayCong = int.Parse(row["TongNgayCong"].ToString()),

            // Nếu một số cột có thể NULL trong DB, dùng TryParse để tránh lỗi
            TienLuong = row["TienLuong"] != DBNull.Value ? decimal.Parse(row["TienLuong"].ToString()) : 0,
            LuongThucNhan = row["LuongThucNhan"] != DBNull.Value ? decimal.Parse(row["LuongThucNhan"].ToString()) : 0,
            ChotLuong = row["ChotLuong"].ToString(),
        };
    }


    // Thêm mới
    public bool AddSalary(Luong luong)
    {
        string query = @"INSERT INTO luong (MaNV, Thang, Nam, TongNgayCong,TienLuong,LuongThucNhan, TrangThai,ChotLuong)
                         VALUES (@MaNV, @Thang, @Nam, @TongNgayCong, @TienLuong,@LuongThucNhan, @TrangThai, @ChotLuong)";
        var parameters = new Dictionary<string, object>
        {
            { "@MaNV", luong.MaNV },
            { "@Thang", luong.Thang },
            { "@Nam", luong.Nam },
            { "@TongNgayCong", luong.TongNgayCong },
            { "@TienLuong", luong.TienLuong },
            { "@LuongThucNhan", luong.LuongThucNhan },
            { "@TrangThai", luong.TrangThai },
            { "@ChotLuong", string.IsNullOrEmpty(luong.ChotLuong) ? "Chưa chốt" : luong.ChotLuong }
        };
        return Database.ExecuteNonQuery(query, parameters) > 0;
    }
    public int AddSalaryReturnID(Luong luong)
    {
        // Câu lệnh SQL bao gồm cả SELECT LAST_INSERT_ID() ở cuối
        string query = @"
        INSERT INTO luong (MaNV, Thang, Nam, TongNgayCong, TienLuong, LuongThucNhan, TrangThai, ChotLuong)
        VALUES (@MaNV, @Thang, @Nam, @TongNgayCong, @TienLuong, @LuongThucNhan, @TrangThai, @ChotLuong);
        SELECT LAST_INSERT_ID();"; 

        var parameters = new Dictionary<string, object>
        {
            { "@MaNV", luong.MaNV },
            { "@Thang", luong.Thang },
            { "@Nam", luong.Nam },
            { "@TongNgayCong", luong.TongNgayCong ?? 0 },
            { "@TienLuong", luong.TienLuong ?? 0 },
            { "@LuongThucNhan", luong.LuongThucNhan ?? 0 },
            { "@TrangThai", string.IsNullOrEmpty(luong.TrangThai) ? "Chưa trả" : luong.TrangThai },
            { "@ChotLuong", string.IsNullOrEmpty(luong.ChotLuong) ? "Chưa chốt" : luong.ChotLuong }
        };

        // Gọi hàm ExecuteScalar mới viết
        object result = Database.ExecuteScalar(query, parameters);

        // Chuyển đổi kết quả về int an toàn
        if (result != null && result != DBNull.Value)
        {
            return Convert.ToInt32(result);
        }
    
        return 0; // Thất bại
    }

    // Sửa
    public bool UpdateSalary(Luong luong)
    {
        string query = @"UPDATE luong
                         SET MaNV=@MaNV, Thang=@Thang, Nam=@Nam, TongNgayCong=@TongNgayCong,TienLuong=@TienLuong ,
                             LuongThucNhan=@LuongThucNhan, TrangThai=@TrangThai ,ChotLuong = @ChotLuong
                         WHERE MaLuong=@MaLuong";
        var parameters = new Dictionary<string, object>
        {
            { "@MaNV", luong.MaNV },
            { "@Thang", luong.Thang },
            { "@Nam", luong.Nam },
            { "@TongNgayCong", luong.TongNgayCong },
            { "@TienLuong", luong.TienLuong },
            { "@LuongThucNhan", luong.LuongThucNhan },
            { "@TrangThai", luong.TrangThai },
            { "@MaLuong", luong.MaLuong },
            { "@ChotLuong", luong.ChotLuong }
        };
        return Database.ExecuteNonQuery(query, parameters) > 0;
    }

    // Xóa
    public bool DeleteSalary(int maLuong)
    {
        string query = "DELETE FROM luong WHERE MaLuong=@MaLuong";
        var parameters = new Dictionary<string, object> { { "@MaLuong", maLuong } };
        return Database.ExecuteNonQuery(query, parameters) > 0;
    }


    // ==========================================
    // CÁC HÀM MỚI THÊM THEO YÊU CẦU
    // ==========================================

    /// <summary>
    /// Thay đổi trạng thái thanh toán lương (VD: "Đã thanh toán", "Chưa thanh toán")
    /// </summary>
    public bool UpdateTrangThai(int maLuong, string trangThaiMoi)
    {
        string query = "UPDATE Luong SET TrangThai = @TrangThai WHERE MaLuong = @MaLuong";

        var parameters = new Dictionary<string, object>
        {
            { "@TrangThai", trangThaiMoi },
            { "@MaLuong", maLuong }
        };

        return Database.ExecuteNonQuery(query, parameters) > 0;
    }

    /// <summary>
    /// Thay đổi trạng thái chốt lương (VD: "Đã chốt", "Chưa chốt")
    /// </summary>
    public bool UpdateChotLuong(int maLuong, string trangThaiChot)
    {
        string query = "UPDATE Luong SET ChotLuong = @ChotLuong WHERE MaLuong = @MaLuong";

        var parameters = new Dictionary<string, object>
        {
            { "@ChotLuong", trangThaiChot },
            { "@MaLuong", maLuong }
        };

        return Database.ExecuteNonQuery(query, parameters) > 0;
    }

    /// <summary>
    /// Lấy trạng thái chốt lương hiện tại của nhân viên trong tháng/năm cụ thể
    /// </summary>
    /// <returns>Chuỗi trạng thái (VD: "Đã chốt") hoặc null nếu không tìm thấy bảng lương</returns>
    public string GetChotLuongStatus(int maNV, int thang, int nam)
    {
        string query = "SELECT ChotLuong FROM Luong WHERE MaNV = @MaNV AND Thang = @Thang AND Nam = @Nam";

        var parameters = new Dictionary<string, object>
        {
            { "@MaNV", maNV },
            { "@Thang", thang },
            { "@Nam", nam }
        };

        DataTable dt = Database.ExecuteQuery(query, parameters);

        if (dt != null && dt.Rows.Count > 0)
        {
            // Kiểm tra DBNull để tránh lỗi
            if (dt.Rows[0]["ChotLuong"] != DBNull.Value)
            {
                return dt.Rows[0]["ChotLuong"].ToString();
            }
        }

        return null; // Trả về null nếu chưa có bảng lương hoặc giá trị null
    }

    /// <summary>
    /// (Tùy chọn) Lấy trạng thái chốt lương theo Mã Lương trực tiếp
    /// </summary>
    public string GetChotLuongStatusByMaLuong(int maLuong)
    {
        string query = "SELECT ChotLuong FROM Luong WHERE MaLuong = @MaLuong";
        var parameters = new Dictionary<string, object> { { "@MaLuong", maLuong } };

        DataTable dt = Database.ExecuteQuery(query, parameters);

        if (dt != null && dt.Rows.Count > 0)
        {
            return dt.Rows[0]["ChotLuong"]?.ToString();
        }

        return null;
    }
}