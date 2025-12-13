using System;
using System.Collections.Generic;
using HRManagementApp.DAL;
using HRManagementApp.models;

namespace HRManagementApp.BLL
{
    public class CaLamService
    {
        private readonly CaLamRepository _repo;

        public CaLamService()
        {
            _repo = new CaLamRepository();
        }

        // 1. Lấy danh sách tất cả ca làm
        public List<CaLam> GetAllCaLam()
        {
            try
            {
                return _repo.GetAll();
            }
            catch (Exception ex)
            {
                // Bạn có thể log lỗi vào file ở đây
                Console.WriteLine("Service Error - GetAllCaLam: " + ex.Message);
                return new List<CaLam>();
            }
        }

        // 2. Lấy ca làm theo ID
        public CaLam GetCaLamById(int maCa)
        {
            try
            {
                return _repo.GetById(maCa);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Error - GetCaLamById: " + ex.Message);
                return null;
            }
        }

        // 3. Lấy danh sách ca làm của nhân viên
        public List<CaLam> GetCaLamByNhanVien(int maNV)
        {
            try
            {
                return _repo.GetByNhanVien(maNV);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Error - GetCaLamByNhanVien: " + ex.Message);
                return new List<CaLam>();
            }
        }

        // 4. Thêm mới ca làm
        public bool InsertCaLam(CaLam ca)
        {
            // --- VALIDATION LOGIC ---
            if (string.IsNullOrWhiteSpace(ca.TenCa))
                throw new ArgumentException("Tên ca làm không được để trống.");

            try
            {
                return _repo.CreateCaLam(ca);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Error - InsertCaLam: " + ex.Message);
                return false;
            }
        }

        // 5. Cập nhật ca làm
        public bool UpdateCaLam(CaLam ca)
        {
            if (ca.MaCa <= 0) return false;
            if (string.IsNullOrWhiteSpace(ca.TenCa)) return false;

            try
            {
                return _repo.UpdateCaLam(ca);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Error - UpdateCaLam: " + ex.Message);
                return false;
            }
        }

        // 6. Xóa ca làm (Chuyển trạng thái sang Đã xóa)
        public bool DeleteCaLam(int maCa)
        {
            if (maCa <= 0) return false;

            try
            {
                return _repo.DeleteCaLam(maCa);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Error - DeleteCaLam: " + ex.Message);
                return false;
            }
        }

        // 7. Phân công ca làm cho nhân viên
        public string PhanCongCaLam(int maNV, int maCa)
        {
            try
            {
                bool result = _repo.AssignCaLamToNhanVien(maNV, maCa);
                
                return result ? "Phân công thành công" : "Phân công thất bại (Có thể đã tồn tại)";
            }
            catch (Exception ex)
            {
                return "Lỗi hệ thống: " + ex.Message;
            }
        }

        // 8. Hủy phân công (Xóa nhân viên khỏi ca)
        public bool HuyPhanCongCaLam(int maNV, int maCa)
        {
            try
            {
                return _repo.RemoveCaLamFromNhanVien(maNV, maCa);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Error - HuyPhanCongCaLam: " + ex.Message);
                return false;
            }
        }
    }
}