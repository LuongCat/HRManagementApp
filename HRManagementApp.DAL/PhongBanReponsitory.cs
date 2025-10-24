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
                MoTa  = row["MoTa"]  != DBNull.Value ? row["MoTa"].ToString() : null
            });
        }

        return list; 
    }

}