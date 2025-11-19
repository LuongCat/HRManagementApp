namespace HRManagementApp.DAL;
using HRManagementApp.models;
using System.Data;
public class ChucVuRepository
{
    public List<ChucVu> GetAll()
    {
        string query = "SELECT * FROM ChucVu";
        DataTable dt = Database.ExecuteQuery(query);

        List<ChucVu> list = new List<ChucVu>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new ChucVu
            {
                MaCV = int.Parse(row["MaCV"].ToString()),
                TenCV = row["TenCV"].ToString(),
                PhuCap = decimal.Parse(row["PhuCap"].ToString()),
                LuongCB = decimal.Parse(row["LuongCB"].ToString()),
                TienPhuCapKiemNhiem = decimal.Parse(row["TienPhuCapKiemNhiem"].ToString()),
                IsActive = row["HoatDong"].ToString()
            });
        }

        return list;
    }

    public ChucVu? GetChucVuByName(string tenCV)
    {
        string query = @"
        SELECT *
        FROM chucvu
        WHERE TenCV = @TenCV
        LIMIT 1
    ";

        var parameters = new Dictionary<string, object>
        {
            { "@TenCV", tenCV }
        };

        DataTable dt = Database.ExecuteQuery(query, parameters);

        if (dt.Rows.Count == 0)
            return null;

        DataRow row = dt.Rows[0];

        return new ChucVu
        {
            MaCV = Convert.ToInt32(row["MaCV"]),
            TenCV = row["TenCV"]?.ToString() ?? "",
            PhuCap = row["PhuCap"] == DBNull.Value ? null : Convert.ToDecimal(row["PhuCap"]),
            LuongCB = row["LuongCB"] == DBNull.Value ? null : Convert.ToDecimal(row["LuongCB"]),
            TienPhuCapKiemNhiem = decimal.Parse(row["TienPhuCapKiemNhiem"].ToString()),
            IsActive = row["HoatDong"].ToString()
        };
    }

    
    public ChucVu? GetChucVuById(int id)
    {
        string query = @"
        SELECT *
        FROM chucvu
        WHERE MaCV = @MaCV
        LIMIT 1
    ";

        var parameters = new Dictionary<string, object>
        {
            { "@MaCV", id }
        };

        DataTable dt = Database.ExecuteQuery(query, parameters);

        if (dt.Rows.Count == 0)
            return null;

        DataRow row = dt.Rows[0];

        return new ChucVu
        {
            MaCV = Convert.ToInt32(row["MaCV"]),
            TenCV = row["TenCV"]?.ToString() ?? "",
            PhuCap = row["PhuCap"] == DBNull.Value ? null : Convert.ToDecimal(row["PhuCap"]),
            LuongCB = row["LuongCB"] == DBNull.Value ? null : Convert.ToDecimal(row["LuongCB"]),
            TienPhuCapKiemNhiem = decimal.Parse(row["TienPhuCapKiemNhiem"].ToString()),
            IsActive = row["HoatDong"].ToString()
        };
    }
    
    public bool InsertChucVu(ChucVu chucVu)
    {
        string query = @"
        INSERT INTO chucvu (TenCV, PhuCap, LuongCB, TienPhuCapKiemNhiem)
        VALUES (@TenCV, @PhuCap, @LuongCB, @TienPhuCapKiemNhiem);
    ";

        var parameters = new Dictionary<string, object>
        {
            { "@TenCV", chucVu.TenCV },
            { "@PhuCap", chucVu.PhuCap ?? 0 },
            { "@LuongCB", chucVu.LuongCB ?? 0 },
            { "@TienPhuCapKiemNhiem", chucVu.TienPhuCapKiemNhiem }
        };

        int affected = Database.ExecuteNonQuery(query, parameters);
        return affected > 0;
    }

    public bool UpdateChucVu(ChucVu chucVu)
    {
        string query = @"
        UPDATE chucvu
        SET TenCV = @TenCV,
            PhuCap = @PhuCap,
            LuongCB = @LuongCB,
            TienPhuCapKiemNhiem = @TienPhuCapKiemNhiem
        WHERE MaCV = @MaCV;
    ";

        var parameters = new Dictionary<string, object>
        {
            { "@MaCV", chucVu.MaCV },
            { "@TenCV", chucVu.TenCV },
            { "@PhuCap", chucVu.PhuCap ?? 0 },
            { "@LuongCB", chucVu.LuongCB ?? 0 },
            { "@TienPhuCapKiemNhiem", chucVu.TienPhuCapKiemNhiem }
        };

        int affected = Database.ExecuteNonQuery(query, parameters);
        return affected > 0;
    }

    public bool ChangeStatus(int maCV, string currentStatus)
    {
        string query = @"
        UPDATE chucvu
        SET HoatDong = @HoatDong
        WHERE MaCV = @MaCV;
    ";

        // Toggle lại trạng thái
        string newStatus = currentStatus == "Active" ? "inactive" : "Active";

        var parameters = new Dictionary<string, object>
        {
            { "@MaCV", maCV },
            { "@HoatDong" , newStatus }
        };

        int affected = Database.ExecuteNonQuery(query, parameters);
        return affected > 0;
    }
    
    
}