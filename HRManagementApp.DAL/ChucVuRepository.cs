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

    
}