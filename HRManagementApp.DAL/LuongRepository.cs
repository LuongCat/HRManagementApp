namespace HRManagementApp.DAL;
using HRManagementApp.models;
using System.Data;
public class LuongRepository
{
    public List<Luong> GetAll()
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
                LuongThucNhan = decimal.Parse(row["LuongThucNhan"].ToString())
            });
        }

        return list;
    }
}