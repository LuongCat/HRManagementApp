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
                TrangThai = row["TrangThai"].ToString()
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
                TrangThai = row["TrangThai"].ToString()
            });
        }

        return list;
    }

    // Thêm mới
    public bool AddSalary(Luong luong)
    {
        string query = @"INSERT INTO luong (MaNV, Thang, Nam, TongNgayCong,TienLuong,LuongThucNhan, TrangThai)
                         VALUES (@MaNV, @Thang, @Nam, @TongNgayCong, @TienLuong,@LuongThucNhan, @TrangThai)";
        var parameters = new Dictionary<string, object>
        {
            {"@MaNV", luong.MaNV },
            {"@Thang", luong.Thang },
            {"@Nam", luong.Nam },
            {"@TongNgayCong", luong.TongNgayCong },
            {"@TienLuong", luong.TienLuong },
            {"@LuongThucNhan", luong.LuongThucNhan },
            {"@TrangThai", luong.TrangThai }
        };
        return Database.ExecuteNonQuery(query, parameters) > 0;
    }

    // Sửa
    public bool UpdateSalary(Luong luong)
    {
        string query = @"UPDATE luong
                         SET MaNV=@MaNV, Thang=@Thang, Nam=@Nam, TongNgayCong=@TongNgayCong,TienLuong=@TienLuong ,
                             LuongThucNhan=@LuongThucNhan, TrangThai=@TrangThai
                         WHERE MaLuong=@MaLuong";
        var parameters = new Dictionary<string, object>
        {
            {"@MaNV", luong.MaNV },
            {"@Thang", luong.Thang },
            {"@Nam", luong.Nam },
            {"@TongNgayCong", luong.TongNgayCong },
            {"@TienLuong", luong.TienLuong },
            {"@LuongThucNhan", luong.LuongThucNhan },
            {"@TrangThai", luong.TrangThai },
            {"@MaLuong", luong.MaLuong }
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
    
    
}