namespace HRManagementApp.DAL;
using HRManagementApp.models;
using System.Data;
public class ChamCongRepository
{
    public ChamCong GetLastChamCongByNhanVienId(int maNV)
    {
        string query = @"
        SELECT MaCC, MaNV, Ngay, GioVao, GioRa, TrangThai
        FROM ChamCong
        WHERE MaNV = @MaNV
        ORDER BY Ngay DESC, GioVao DESC
        LIMIT 1;";

        var parameters = new Dictionary<string, object>
        {
            { "@MaNV", maNV }
        };

        DataTable data = Database.ExecuteQuery(query, parameters);

        if (data.Rows.Count == 0)
            return null;

        DataRow row = data.Rows[0];

        var chamCong = new ChamCong
        {
            MaCC = row["MaCC"] != DBNull.Value ? Convert.ToInt32(row["MaCC"]) : 0,
            MaNV = row["MaNV"] != DBNull.Value ? Convert.ToInt32(row["MaNV"]) : 0,
            Ngay = row["Ngay"] != DBNull.Value ? Convert.ToDateTime(row["Ngay"]) : (DateTime?)null,
            GioVao = row["GioVao"] != DBNull.Value ? Convert.ToDateTime(row["GioCC"]) : (DateTime?)null,
            GioRa = row["GioRa"] != DBNull.Value ? Convert.ToDateTime(row["GioCC"]) : (DateTime?)null,
            TrangThai = row["TrangThai"]?.ToString()
        };

        return chamCong;
    }




}       