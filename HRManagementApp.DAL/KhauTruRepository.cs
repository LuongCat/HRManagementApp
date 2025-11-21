namespace HRManagementApp.DAL;
using HRManagementApp.models;
using System.Data;
public class KhauTruRepository
{
     public List<KhauTru> GetAllDeduction()
    {
        string query = "SELECT * FROM khautru";
        DataTable dt = Database.ExecuteQuery(query);

        List<KhauTru> list = new List<KhauTru>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new KhauTru
            {
                MaKT = int.Parse(row["MaKT"].ToString()),
                MaNV = int.Parse(row["MaNV"].ToString()),
                TenKhoanTru = row["TenKhoanTru"].ToString(),
                SoTien = decimal.Parse(row["SoTien"].ToString()),
                Ngay = DateTime.Parse(row["Ngay"].ToString()),
                GhiChu = row["GhiChu"].ToString()
            });
        }
        return list;
    }

    // Lấy theo mã nhân viên
    public List<KhauTru> GetDeductionByMaNV(int maNV)
    {
        string query = "SELECT * FROM khautru WHERE MaNV = @MaNV";
        var parameters = new Dictionary<string, object> { { "@MaNV", maNV } };
        DataTable dt = Database.ExecuteQuery(query, parameters);

        List<KhauTru> list = new List<KhauTru>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new KhauTru
            {
                MaKT = int.Parse(row["MaKT"].ToString()),
                MaNV = int.Parse(row["MaNV"].ToString()),
                TenKhoanTru = row["TenKhoanTru"].ToString(),
                SoTien = decimal.Parse(row["SoTien"].ToString()),
                Ngay = DateTime.Parse(row["Ngay"].ToString()),
                GhiChu = row["GhiChu"].ToString()
            });
        }
        return list;
    }

    // Thêm mới
    public bool AddDeduction(KhauTru kt)
    {
        string query = @"INSERT INTO khautru (MaNV, TenKhoanTru, SoTien, Ngay, GhiChu)
                         VALUES (@MaNV, @TenKhoanTru, @SoTien, @Ngay, @GhiChu)";
        var parameters = new Dictionary<string, object>
        {
            {"@MaNV", kt.MaNV },
            {"@TenKhoanTru", kt.TenKhoanTru },
            {"@SoTien", kt.SoTien },
            {"@Ngay", kt.Ngay },
            {"@GhiChu", kt.GhiChu ?? "" }
        };
        return Database.ExecuteNonQuery(query, parameters) > 0;
    }

    // Sửa
    public bool UpdateDeduction(KhauTru kt)
    {
        string query = @"UPDATE khautru
                         SET MaNV=@MaNV, TenKhoanTru=@TenKhoanTru, SoTien=@SoTien, Ngay=@Ngay, GhiChu=@GhiChu
                         WHERE MaKT=@MaKT";
        var parameters = new Dictionary<string, object>
        {
            {"@MaNV", kt.MaNV },
            {"@TenKhoanTru", kt.TenKhoanTru },
            {"@SoTien", kt.SoTien },
            {"@Ngay", kt.Ngay },
            {"@GhiChu", kt.GhiChu ?? "" },
            {"@MaKT", kt.MaKT }
        };
        return Database.ExecuteNonQuery(query, parameters) > 0;
    }

    // Xóa
    public bool DeleteDeduction(int maKT, int maNV)
    {
        string query = "DELETE FROM khautru WHERE MaKT=@MaKT and  MaNV=@MaNV";
        var parameters = new Dictionary<string, object>
        {
            { "@MaKT", maKT },
            { "@MaNV", maNV },
        };
        return Database.ExecuteNonQuery(query, parameters) > 0;
    }
}