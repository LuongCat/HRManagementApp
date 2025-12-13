namespace HRManagementApp.DAL;

using HRManagementApp.models;
using System.Collections.Generic;
using System.Data;
using System;

public class SystemLogRepository
{
    // =======================================================
    // 1. GHI LOG
    // =======================================================
    public void AddLog(SystemLog log)
    {
        try
        {
            string query = @"
                INSERT INTO SystemLog (ThoiGian, NguoiThucHien, HanhDong, BangLienQuan, MaBanGhi, MoTa)
                VALUES (@ThoiGian, @User, @Action, @Table, @RefID, @Desc)";

            var parameters = new Dictionary<string, object>
            {
                { "@ThoiGian", log.ThoiGian },
                { "@User", log.NguoiThucHien ?? "System" },
                { "@Action", log.HanhDong },
                { "@Table", log.BangLienQuan },
                { "@RefID", log.MaBanGhi },
                { "@Desc", log.MoTa }
            };

            Database.ExecuteNonQuery(query, parameters);
        }
        catch
        {
            // Bỏ qua lỗi log
        }
    }

    // =======================================================
    // 2. LẤY TOÀN BỘ LOG (Limit 100)
    // =======================================================
    public List<SystemLog> GetAllLogs()
    {
        var list = new List<SystemLog>();
        string query = "SELECT * FROM SystemLog ORDER BY ThoiGian DESC LIMIT 100";
        
        DataTable data = Database.ExecuteQuery(query, null);
        
        if (data != null && data.Rows.Count > 0)
        {
            foreach (DataRow row in data.Rows)
            {
                list.Add(MapDataRowToSystemLog(row));
            }
        }
        return list;
    }

    // =======================================================
    // 3. LẤY LOG THEO TÊN BẢNG (Bổ sung phần thiếu)
    // =======================================================
    public List<SystemLog> GetLogsByTable(string tableName)
    {
        var list = new List<SystemLog>();
        // Lấy tất cả log liên quan đến bảng (VD: 'NhanVien')
        string query = "SELECT * FROM SystemLog WHERE BangLienQuan = @Table ORDER BY ThoiGian DESC";
        
        var parameters = new Dictionary<string, object>
        {
            { "@Table", tableName }
        };

        DataTable data = Database.ExecuteQuery(query, parameters);

        if (data != null && data.Rows.Count > 0)
        {
            foreach (DataRow row in data.Rows)
            {
                list.Add(MapDataRowToSystemLog(row));
            }
        }
        return list;
    }

    // =======================================================
    // 4. (MỞ RỘNG) LẤY LOG CỦA MỘT ĐỐI TƯỢNG CỤ THỂ
    // VD: Xem lịch sử thay đổi của nhân viên có mã là 10
    // =======================================================
    public List<SystemLog> GetLogsByRecord(string tableName, string recordId)
    {
        var list = new List<SystemLog>();
        string query = @"SELECT * FROM SystemLog 
                         WHERE BangLienQuan = @Table AND MaBanGhi = @RefID 
                         ORDER BY ThoiGian DESC";

        var parameters = new Dictionary<string, object>
        {
            { "@Table", tableName },
            { "@RefID", recordId }
        };

        DataTable data = Database.ExecuteQuery(query, parameters);

        if (data != null && data.Rows.Count > 0)
        {
            foreach (DataRow row in data.Rows)
            {
                list.Add(MapDataRowToSystemLog(row));
            }
        }
        return list;
    }

    // =======================================================
    // HELPER: Map từ DataTable sang Object
    // =======================================================
    private SystemLog MapDataRowToSystemLog(DataRow row)
    {
        return new SystemLog
        {
            LogID = Convert.ToInt32(row["LogID"]),
            ThoiGian = Convert.ToDateTime(row["ThoiGian"]),
            NguoiThucHien = row["NguoiThucHien"] != DBNull.Value ? row["NguoiThucHien"].ToString() : "Unknown",
            HanhDong = row["HanhDong"] != DBNull.Value ? row["HanhDong"].ToString() : "",
            BangLienQuan = row["BangLienQuan"] != DBNull.Value ? row["BangLienQuan"].ToString() : "",
            MaBanGhi = row["MaBanGhi"] != DBNull.Value ? row["MaBanGhi"].ToString() : "",
            MoTa = row["MoTa"] != DBNull.Value ? row["MoTa"].ToString() : ""
        };
    }
}