namespace HRManagementApp.DAL;
using HRManagementApp.models;
using System.Data;
public class PhongBanReponsitory
{
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
                MoTa  = row["MoTa"]  != DBNull.Value ? row["MoTa"].ToString() : null,
                MaTruongPhong = row["MaTruongPhong"] != DBNull.Value ? Convert.ToInt32(row["MaTruongPhong"]) : 0
            });
        }

        return list; 
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
            MoTa  = row["MoTa"]  != DBNull.Value ? row["MoTa"].ToString() : null,
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
            MoTa  = row["MoTa"]  != DBNull.Value ? row["MoTa"].ToString() : null,
            MaTruongPhong = row["MaTruongPhong"] != DBNull.Value ? Convert.ToInt32(row["MaTruongPhong"]) : 0
        };
    }


}