using System;
using System.Collections.Generic;
using System.Linq;
using HRManagementApp.DAL;
using HRManagementApp.models;

namespace HRManagementApp.BLL
{
    public class NhanVienService
    {
        private readonly NhanVienRepository _repository;

        public NhanVienService()
        {
            _repository = new NhanVienRepository();
        }

        // ✅ Lấy tất cả nhân viên
        public List<NhanVien> GetAllNhanVien()
        {
            try
            {
                return _repository.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi lấy danh sách nhân viên: " + ex.Message);
                return new List<NhanVien>();
            }
        }
        
        public List<NhanVien> GetNhanVienDayDu()
        {
            return _repository.GetAllFull();
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        public bool UpdateNhanVien(NhanVien nhanVien)
        {
            try
            {
                
                // Tìm nhân viên trong database
                var existingEmployee = _repository.GetById(nhanVien.MaNV);
                
                if (existingEmployee == null)
                {
                    return false;
                }
                
                // Cập nhật các thuộc tính
                existingEmployee.HoTen = nhanVien.HoTen;
                existingEmployee.NgaySinh = nhanVien.NgaySinh;
                existingEmployee.SoCCCD = nhanVien.SoCCCD;
                existingEmployee.DienThoai = nhanVien.DienThoai;
                existingEmployee.MaPB = nhanVien.MaPB;
                existingEmployee.MaCV = nhanVien.MaCV;
                existingEmployee.NgayVaoLam = nhanVien.NgayVaoLam;
                existingEmployee.TrangThai = nhanVien.TrangThai;

                // Lưu thay đổi
                return true;
                
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                Console.WriteLine($"Lỗi UpdateNhanVien: {ex.Message}");
                return false;
            }
        }

/// <summary>
/// Cập nhật thông tin nhân viên với validation
/// </summary>
public (bool success, string message) UpdateNhanVienWithValidation(NhanVien nhanVien)
{
    try
    {
        // Validate dữ liệu
        if (string.IsNullOrWhiteSpace(nhanVien.HoTen))
        {
            return (false, "Họ tên không được để trống");
        }

        if (!string.IsNullOrEmpty(nhanVien.DienThoai))
        {
            string cleanPhone = nhanVien.DienThoai.Replace(" ", "").Replace("-", "");
            if (cleanPhone.Length < 10 || cleanPhone.Length > 11 || !cleanPhone.All(char.IsDigit))
            {
                return (false, "Số điện thoại không hợp lệ (10-11 số)");
            }
        }

        if (!string.IsNullOrEmpty(nhanVien.SoCCCD))
        {
            if (nhanVien.SoCCCD.Length != 12 || !nhanVien.SoCCCD.All(char.IsDigit))
            {
                return (false, "Số CCCD phải có 12 số");
            }
        }

        
            var existingEmployee = _repository.GetById(nhanVien.MaNV);
            
            if (existingEmployee == null)
            {
                return (false, "Không tìm thấy nhân viên");
            }

            // Kiểm tra trùng CCCD (nếu có thay đổi)
            

            // Kiểm tra trùng số điện thoại (nếu có thay đổi)
           

            // Cập nhật
            existingEmployee.HoTen = nhanVien.HoTen;
            existingEmployee.NgaySinh = nhanVien.NgaySinh;
            existingEmployee.SoCCCD = nhanVien.SoCCCD;
            existingEmployee.DienThoai = nhanVien.DienThoai;
            existingEmployee.MaPB = nhanVien.MaPB;
            existingEmployee.MaCV = nhanVien.MaCV;
            existingEmployee.NgayVaoLam = nhanVien.NgayVaoLam;
            existingEmployee.TrangThai = nhanVien.TrangThai;

            
            return (true, "Cập nhật thành công");
        
    }
    catch (Exception ex)
    {
        return (false, $"Lỗi: {ex.Message}");
    }
}
        
    }
}