using System;
using System.Collections.Generic;
using HRManagementApp.DAL;
using HRManagementApp.models;

namespace HRManagementApp.BLL
{
    public class DonTuService
    {
        private readonly DonTuRepository _repo = new DonTuRepository();

        public List<DonTu> GetAllDonTu()
        {
            return _repo.GetAll();
        }

        public List<LoaiDon> GetLoaiDonList()
        {
            return _repo.GetLoaiDon();
        }

        public List<DonTu> GetMyLeaveRequests(int maNV)
        {
            return _repo.GetByNhanVien(maNV);
        }

        public bool AddLeaveRequest(DonTu don)
        {
            if (don.NgayKetThuc < don.NgayBatDau)
                throw new ArgumentException("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");

            return _repo.CreateDonTu(don);
        }

        public bool AddLeaveType(LoaiDon loai)
        {
            if (string.IsNullOrWhiteSpace(loai.TenLoaiDon))
                throw new ArgumentException("Tên loại đơn không được để trống");

            return _repo.CreateLoaiDon(loai);
        }

        public bool ApproveRequest(int maDon, string adminName)
        {
            return _repo.UpdateTrangThai(maDon, "Đã duyệt", adminName);
        }

        public bool RejectRequest(int maDon, string adminName)
        {
            return _repo.UpdateTrangThai(maDon, "Từ chối", adminName);
        }

        public bool UpdateLeaveType(LoaiDon loai)
        {
            if (string.IsNullOrWhiteSpace(loai.TenLoaiDon))
                throw new ArgumentException("Tên loại đơn không được để trống");

            return _repo.UpdateLoaiDon(loai);
        }

        public bool DeleteLeaveType(int maLoai)
        {
            // Có thể thêm kiểm tra logic ở đây nếu cần (ví dụ: không cho xóa nếu đang có đơn sử dụng loại này)
            // Tuy nhiên, trong database đã set ON DELETE SET NULL nên có thể xóa thoải mái.
            return _repo.DeleteLoaiDon(maLoai);
        }
        
        public bool DeleteRequest(int maDon)
        {
            return _repo.DeleteDonTu(maDon);
        }

        public KetQuaNghi GetSoNgayNghi(int maNV, int thang, int nam)
        {
            return _repo.GetSoNgayNghi(maNV, thang, nam);
        }
    }
}