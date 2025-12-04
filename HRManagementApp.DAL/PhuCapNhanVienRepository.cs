namespace HRManagementApp.DAL;
using HRManagementApp.models;
using System.Data;
public class PhuCapNhanVienRepository
{
    public List<PhuCapNhanVien> GetAllBenefit()
    {
        string query = "SELECT * FROM phucap_nhanvien";
        DataTable dt = Database.ExecuteQuery(query);

        List<PhuCapNhanVien> list = new List<PhuCapNhanVien>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new PhuCapNhanVien
            {
                ID = int.Parse(row["ID"].ToString()),
                MaNV = int.Parse(row["MaNV"].ToString()),
                TenPhuCap = row["TenPhuCap"].ToString(),
                SoTien = decimal.Parse(row["SoTien"].ToString()),
                ApDungTuNgay = DateTime.Parse(row["ApDungTuNgay"].ToString()),
                ApDungDenNgay = string.IsNullOrEmpty(row["ApDungDenNgay"].ToString()) ? (DateTime?)null : DateTime.Parse(row["ApDungDenNgay"].ToString())
            });
        }
        return list;
    }
    
    public bool AddPhuCap(PhuCapNhanVien pc)
    {
        string query = "INSERT INTO phucap_nhanvien (MaNV, TenPhuCap, SoTien, ApDungTuNgay, ApDungDenNgay) " +
                       "VALUES (@MaNV, @TenPhuCap, @SoTien, @ApDungTuNgay, @ApDungDenNgay)";

        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            {"@MaNV", pc.MaNV},
            {"@TenPhuCap", pc.TenPhuCap},
            {"@SoTien", pc.SoTien},
            {"@ApDungTuNgay", pc.ApDungTuNgay},
            {"@ApDungDenNgay", (object)pc.ApDungDenNgay ?? DBNull.Value}
        };

        return Database.ExecuteNonQuery(query, parameters) > 0;
    }

    public bool UpdatePhuCap(PhuCapNhanVien pc)
    {
        string query = "UPDATE phucap_nhanvien SET " +
                       "MaNV = @MaNV, TenPhuCap = @TenPhuCap, SoTien = @SoTien, " +
                       "ApDungTuNgay = @ApDungTuNgay, ApDungDenNgay = @ApDungDenNgay " +
                       "WHERE ID = @ID";

        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            {"@ID", pc.ID},
            {"@MaNV", pc.MaNV},
            {"@TenPhuCap", pc.TenPhuCap},
            {"@SoTien", pc.SoTien},
            {"@ApDungTuNgay", pc.ApDungTuNgay},
            {"@ApDungDenNgay", (object)pc.ApDungDenNgay ?? DBNull.Value}
        };

        return Database.ExecuteNonQuery(query, parameters) > 0;
    }
    
    public bool DeletePhuCap(int id)
    {
        string query = "DELETE FROM phucap_nhanvien WHERE ID = @ID";
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            {"@ID", id}
        };
        return Database.ExecuteNonQuery(query, parameters) > 0;
    }

    public List<PhuCapNhanVien> GetPhuCapByMaNV(int maNV)
    {
        string query = "SELECT * FROM phucap_nhanvien WHERE MaNV = @MaNV";
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            {"@MaNV", maNV}
        };

        DataTable dt = Database.ExecuteQuery(query, parameters);
        List<PhuCapNhanVien> list = new List<PhuCapNhanVien>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new PhuCapNhanVien
            {
                ID = int.Parse(row["ID"].ToString()),
                MaNV = int.Parse(row["MaNV"].ToString()),
                TenPhuCap = row["TenPhuCap"].ToString(),
                SoTien = decimal.Parse(row["SoTien"].ToString()),
                ApDungTuNgay = DateTime.Parse(row["ApDungTuNgay"].ToString()),
                ApDungDenNgay = string.IsNullOrEmpty(row["ApDungDenNgay"].ToString()) ? (DateTime?)null : DateTime.Parse(row["ApDungDenNgay"].ToString())
            });
        }

        return list;
    }


}