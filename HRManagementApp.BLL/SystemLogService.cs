namespace HRManagementApp.BLL; // Hoặc HRManagementApp.Services

using HRManagementApp.DAL;
using HRManagementApp.models;
using System;
using System.Collections.Generic;
using System.Linq; // Dùng LINQ để lọc dữ liệu nhanh trên RAM

public class SystemLogService
{
    private readonly SystemLogRepository _repo = new SystemLogRepository();

    // =============================================================
    // 1. CÁC HÀM LẤY DỮ LIỆU HIỂN THỊ (READ)
    // =============================================================

    /// <summary>
    /// Lấy toàn bộ lịch sử (mặc định lấy 100 dòng mới nhất từ Repo)
    /// </summary>
    public List<SystemLog> GetAllLogs()
    {
        return _repo.GetAllLogs();
    }

    /// <summary>
    /// Lấy lịch sử theo tên bảng (VD: Chỉ xem ai đã sửa bảng NhanVien)
    /// </summary>
    public List<SystemLog> GetLogsByTable(string tableName)
    {
        return _repo.GetLogsByTable(tableName);
    }

    /// <summary>
    /// Hàm tìm kiếm thông minh (Search)
    /// Logic này xử lý tại Service để giảm tải cho Database
    /// </summary>
    public List<SystemLog> SearchLogs(string keyword)
    {
        var allLogs = _repo.GetAllLogs();

        if (string.IsNullOrEmpty(keyword))
        {
            return allLogs;
        }

        // Chuyển keyword về chữ thường để tìm kiếm không phân biệt hoa thường
        string lowerKey = keyword.ToLower();

        // Lọc dữ liệu: Tìm trong Tên người, Hành động, hoặc Mô tả
        return allLogs.Where(log => 
            (log.NguoiThucHien != null && log.NguoiThucHien.ToLower().Contains(lowerKey)) ||
            (log.HanhDong != null && log.HanhDong.ToLower().Contains(lowerKey)) ||
            (log.MoTa != null && log.MoTa.ToLower().Contains(lowerKey))
        ).ToList();
    }

    // =============================================================
    // 2. HÀM GHI LOG (WRITE)
    // =============================================================
    
    /// <summary>
    /// Hàm này để UI hoặc các Service khác gọi khi muốn ghi log thủ công
    /// </summary>
    public void WriteLog(string nguoiThucHien, string hanhDong, string bang, string maBanGhi, string moTa)
    {
        var log = new SystemLog
        {
            NguoiThucHien = nguoiThucHien,
            HanhDong = hanhDong,
            BangLienQuan = bang,
            MaBanGhi = maBanGhi,
            MoTa = moTa,
            ThoiGian = DateTime.Now
        };

        _repo.AddLog(log);
    }
}