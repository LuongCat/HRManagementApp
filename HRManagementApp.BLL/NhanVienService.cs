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
        private readonly VaiTroNhanVienReponsitory _vaiTroRepo;

        public NhanVienService()
        {
            _repository = new NhanVienRepository();
            
            _vaiTroRepo = new VaiTroNhanVienReponsitory(_repository);
            
            _repository.vaiTroNhanVien = _vaiTroRepo;
        }

        // =====================================================
        // LẤY DANH SÁCH NHÂN VIÊN
        // =====================================================
        public List<NhanVien> GetListNhanVien()
        {
            return _repository.GetListNhanVien();
        }

        public NhanVien GetEmployeeByName(string name)
        {
            return _repository.GetEmployeeByName(name);
        }

        public List<PhongBan> GetListPhongBanOfNhanVien(int maNV)
        {
            return _repository.AllPhongBanOfNhanVien(maNV);
        }

        public List<ChucVu> GetListChucVuOfNhanVien(int maNV)
        {
            return _repository.AllChucVuOfNhanVien(maNV);
        }



        public NhanVien GetEmployeeById(int id)
        {
            return _repository.GetEmployeeById(id);
        }
        // =====================================================
        // THÊM NHÂN VIÊN
        // =====================================================
        public bool AddNhanVien(NhanVien nv)
        {
            return _repository.AddEmployeeHavingDeparmentAndRole(nv);
        }

        // =====================================================
        // CẬP NHẬT NHÂN VIÊN CƠ BẢN
        // =====================================================
        public bool UpdateNhanVien(NhanVien nv)
        {
            try
            {
                var old = _repository.GetEmployeeById(nv.MaNV);
                if (old == null) return false;

                old.HoTen = nv.HoTen;
                old.NgaySinh = nv.NgaySinh;
                old.SoCCCD = nv.SoCCCD;
                old.DienThoai = nv.DienThoai;
                old.GioiTinh = nv.GioiTinh;

                old.NgayVaoLam = nv.NgayVaoLam;
                old.TrangThai = nv.TrangThai;

                return _repository.UpdateBasicNhanVien(old);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi UpdateNhanVien: {ex.Message}");
                return false;
            }
        }

        // =====================================================
        // CẬP NHẬT NHÂN VIÊN CÓ VALIDATION
        // =====================================================
        public (bool success, string message) UpdateNhanVienWithValidation(NhanVien nv)
        {
            try
            {
                // ===== 1. VALIDATE =====
                if (string.IsNullOrWhiteSpace(nv.HoTen))
                    return (false, "Họ tên không được để trống");

                if (!string.IsNullOrEmpty(nv.DienThoai))
                {
                    string phone = nv.DienThoai.Replace(" ", "").Replace("-", "");
                    if (phone.Length < 10 || phone.Length > 11 || !phone.All(char.IsDigit))
                        return (false, "Số điện thoại không hợp lệ (10-11 số)");
                }

                if (!string.IsNullOrEmpty(nv.SoCCCD))
                {
                    if (nv.SoCCCD.Length != 12 || !nv.SoCCCD.All(char.IsDigit))
                        return (false, "CCCD phải có đúng 12 số");
                }

                // ===== 2. LẤY DỮ LIỆU GỐC =====
                var old = _repository.GetEmployeeById(nv.MaNV);
                if (old == null) return (false, "Không tìm thấy nhân viên");

                // ===== 3. CẬP NHẬT =====
                old.HoTen = nv.HoTen;
                old.NgaySinh = nv.NgaySinh;
                old.SoCCCD = nv.SoCCCD;
                old.DienThoai = nv.DienThoai;
                old.GioiTinh = nv.GioiTinh;

                old.NgayVaoLam = nv.NgayVaoLam;
                old.TrangThai = nv.TrangThai;

                bool ok = _repository.UpdateBasicNhanVien(old);
                return ok
                    ? (true, "Cập nhật thành công")
                    : (false, "Không thể cập nhật nhân viên");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // =====================================================
        // XÓA NHÂN VIÊN
        // =====================================================
        public bool DeleteNhanVien(NhanVien nv)
        {
            if (nv == null) return false;
            return _repository.DeleteNhanVien(nv.MaNV);
        }

        // =====================================================
        // Chỉnh trạng thái NHÂN VIÊN
        // =====================================================
        public bool UpdateSate(int MaNV)
        {
            return _repository.ChangeSate(MaNV);
        }
    }
}
