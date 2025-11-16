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
                LuongCB = decimal.Parse(row["LuongCB"].ToString())
            });
        }

        return list;
    }

    public ChucVu? GetChucVuByName(string tenCV)
    {
        string query = @"
        SELECT MaCV, TenCV, PhuCap, LuongCB
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
            LuongCB = row["LuongCB"] == DBNull.Value ? null : Convert.ToDecimal(row["LuongCB"])
        };
    }

    
    public ChucVu? GetChucVuById(int id)
    {
        string query = @"
        SELECT MaCV, TenCV, PhuCap, LuongCB
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
            LuongCB = row["LuongCB"] == DBNull.Value ? null : Convert.ToDecimal(row["LuongCB"])
        };
    }

    
}