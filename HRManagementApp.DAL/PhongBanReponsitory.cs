namespace HRManagementApp.DAL;

using HRManagementApp.models;
using System.Data;

public class PhongBanReponsitory
{

    private readonly NhanVienRepository _nhanVienRepositor;

    public PhongBanReponsitory()
    {
        _nhanVienRepositor = new NhanVienRepository();
    }
    
    public List<PhongBan> GetAllPhongBan()
    {
        string query = "SELECT * FROM PhongBan";
        DataTable data = Database.ExecuteQuery(query);

        var list = new List<PhongBan>();

        foreach (DataRow row in data.Rows)
        {
            list.Add(new PhongBan
            {
                MaPB = row["MaPB"] != DBNull.Value ? Convert.ToInt32(row["MaPB"]) : 0,
                TenPB = row["TenPB"] != DBNull.Value ? row["TenPB"].ToString()! : string.Empty,
                MoTa = row["MoTa"] != DBNull.Value ? row["MoTa"].ToString() : null,
                MaTruongPhong = row["MaTruongPhong"] != DBNull.Value ? Convert.ToInt32(row["MaTruongPhong"]) : 0
            });
        }

        return list;
    }

    public NhanVien GetDeparmentHead(int maPB)
    {
        string query = @"SELECT MaTruongPhong 
                     FROM PhongBan 
                     WHERE MaPB = @MaPB";

        var parameters = new Dictionary<string, object>
        {
            { "@MaPB", maPB }
        };

        DataTable data = Database.ExecuteQuery(query, parameters);

        // Không có dòng nào → trả về null
        if (data.Rows.Count == 0)
            return null;

        object rawValue = data.Rows[0]["MaTruongPhong"];

        // Giá trị NULL trong DB → trả về null
        if (rawValue == DBNull.Value)
            return null;

        int maTruongPhong = Convert.ToInt32(rawValue);

        // Lấy thông tin nhân viên
        return _nhanVienRepositor.GetEmployeeById(maTruongPhong);
    }


    public PhongBan? GetPhongBanByName(string tenPB)
    {
        string query = @"
        SELECT *
        FROM phongban
        WHERE TenPB = @TenPB
        LIMIT 1
    ";

        var parameters = new Dictionary<string, object>
        {
            { "@TenPB", tenPB }
        };

        DataTable dt = Database.ExecuteQuery(query, parameters);

        if (dt.Rows.Count == 0)
            return null;

        DataRow row = dt.Rows[0];

        return new PhongBan
        {
            MaPB = Convert.ToInt32(row["MaPB"]),
            TenPB = row["TenPB"].ToString()!,
            MoTa = row["MoTa"] != DBNull.Value ? row["MoTa"].ToString() : null,
            MaTruongPhong = row["MaTruongPhong"] != DBNull.Value ? Convert.ToInt32(row["MaTruongPhong"]) : 0
        };
    }

    public PhongBan? GetPhongBanById(int id)
    {
        string query = @"
        SELECT *
        FROM phongban
        WHERE MaPB = @MaPB
        LIMIT 1
    ";

        var parameters = new Dictionary<string, object>
        {
            { "@MaPB", id }
        };

        DataTable dt = Database.ExecuteQuery(query, parameters);

        if (dt.Rows.Count == 0)
            return null;

        DataRow row = dt.Rows[0];

        return new PhongBan
        {
            MaPB = Convert.ToInt32(row["MaPB"]),
            TenPB = row["TenPB"]?.ToString() ?? "",
            MoTa = row["MoTa"] != DBNull.Value ? row["MoTa"].ToString() : null,
            MaTruongPhong = row["MaTruongPhong"] != DBNull.Value ? Convert.ToInt32(row["MaTruongPhong"]) : 0
        };
    }

    public bool UpdateDeparment(PhongBan phongBan)
    {
        if (phongBan == null) return false;

        string query = @"
        UPDATE PhongBan
        SET TenPB = @TenPB,
            MoTa = @MoTa,
            MaTruongPhong = @MaTruongPhong
        WHERE MaPB = @MaPB
    ";

        var parameters = new Dictionary<string, object>
        {
            { "@TenPB", phongBan.TenPB },
            { "@MoTa", (object?)phongBan.MoTa ?? DBNull.Value },
            { "@MaTruongPhong", phongBan.MaTruongPhong },
            { "@MaPB", phongBan.MaPB }
        };

        try
        {
            int rowsAffected = Database.ExecuteNonQuery(query, parameters);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            // Có thể log lỗi tại đây nếu cần
            Console.WriteLine("Lỗi khi cập nhật phòng ban: " + ex.Message);
            return false;
        }
    }


    /// <summary>
    /// Thêm phòng ban mới
    /// </summary>
    public bool InsertDepartment(PhongBan phongBan)
    {
        if (phongBan == null) return false;

        string query = @"
            INSERT INTO PhongBan (TenPB, MoTa, MaTruongPhong)
            VALUES (@TenPB, @MoTa, @MaTruongPhong)
        ";

        var parameters = new Dictionary<string, object>
        {
            { "@TenPB", phongBan.TenPB },
            { "@MoTa", (object?)phongBan.MoTa ?? DBNull.Value },
            { "@MaTruongPhong", phongBan.MaTruongPhong }
        };

        try
        {
            int rowsAffected = Database.ExecuteNonQuery(query, parameters);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi khi thêm phòng ban: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Xóa phòng ban theo ID
    /// </summary>
    public bool DeleteDepartment(int maPB)
    {
        string query = @"
            DELETE FROM PhongBan
            WHERE MaPB = @MaPB
        ";

        var parameters = new Dictionary<string, object>
        {
            { "@MaPB", maPB }
        };

        try
        {
            int rowsAffected = Database.ExecuteNonQuery(query, parameters);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi khi xóa phòng ban: " + ex.Message);
            return false;
        }
    }
}