namespace HRManagementApp.DAL;
using HRManagementApp.models;
using System.Data;
public class ThueRepository
{
    public List<Thue> GetAllTax()
    {
        string query = "SELECT * FROM thue";
        DataTable dt = Database.ExecuteQuery(query);

        List<Thue> list = new List<Thue>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new Thue
            {
                MaThue = int.Parse(row["MaThue"].ToString()),
                MaNV = int.Parse(row["MaNV"].ToString()),
                TenThue = row["TenThue"].ToString(),
                SoTien = decimal.Parse(row["SoTien"].ToString()),
                ApDungTuNgay = DateTime.Parse(row["ApDungTuNgay"].ToString()),
                ApDungDenNgay = string.IsNullOrEmpty(row["ApDungDenNgay"].ToString()) ? (DateTime?)null : DateTime.Parse(row["ApDungDenNgay"].ToString())
            });
        }
        return list;
    }

    // Lấy theo mã nhân viên
    public List<Thue> GetTaxByMaNV(int maNV)
    {
        string query = "SELECT * FROM thue WHERE MaNV=@MaNV";
        var parameters = new Dictionary<string, object> { { "@MaNV", maNV } };
        DataTable dt = Database.ExecuteQuery(query, parameters);

        List<Thue> list = new List<Thue>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new Thue
            {
                MaThue = int.Parse(row["MaThue"].ToString()),
                MaNV = int.Parse(row["MaNV"].ToString()),
                TenThue = row["TenThue"].ToString(),
                SoTien = decimal.Parse(row["SoTien"].ToString()),
                ApDungTuNgay = DateTime.Parse(row["ApDungTuNgay"].ToString()),
                ApDungDenNgay = string.IsNullOrEmpty(row["ApDungDenNgay"].ToString()) ? (DateTime?)null : DateTime.Parse(row["ApDungDenNgay"].ToString())
            });
        }
        return list;
    }

    // Thêm mới
    public bool AddTax(Thue thue)
    {
        string query = @"INSERT INTO thue (MaNV, TenThue, SoTien, ApDungTuNgay, ApDungDenNgay)
                         VALUES (@MaNV, @TenThue, @SoTien, @ApDungTuNgay, @ApDungDenNgay)";
        var parameters = new Dictionary<string, object>
        {
            {"@MaNV", thue.MaNV },
            {"@TenThue", thue.TenThue },
            {"@SoTien", thue.SoTien },
            {"@ApDungTuNgay", thue.ApDungTuNgay },
            {"@ApDungDenNgay", thue.ApDungDenNgay ?? (object)DBNull.Value }
        };
        return Database.ExecuteNonQuery(query, parameters) > 0;
    }

    // Sửa
    public bool UpdateTax(Thue thue)
    {
        string query = @"UPDATE thue
                         SET MaNV=@MaNV, TenThue=@TenThue, SoTien=@SoTien, ApDungTuNgay=@ApDungTuNgay, ApDungDenNgay=@ApDungDenNgay
                         WHERE MaThue=@MaThue";
        var parameters = new Dictionary<string, object>
        {
            {"@MaNV", thue.MaNV },
            {"@TenThue", thue.TenThue },
            {"@SoTien", thue.SoTien },
            {"@ApDungTuNgay", thue.ApDungTuNgay },
            {"@ApDungDenNgay", thue.ApDungDenNgay ?? (object)DBNull.Value },
            {"@MaThue", thue.MaThue }
        };
        return Database.ExecuteNonQuery(query, parameters) > 0;
    }

    // Xóa
    public bool DeleteTax(int maThue, int maNV)
    {
        string query = "DELETE FROM thue WHERE MaThue=@MaThue  and MaNV=@MaNV";
        var parameters = new Dictionary<string, object>
        {
            { "@MaThue", maThue },
            {"@MaNV", maNV },
        };
        return Database.ExecuteNonQuery(query, parameters) > 0;
    }
}