using System;
using System.Collections.Generic;
using System.Data;
using HRManagementApp.models;

namespace HRManagementApp.DAL
{
    public class DonTuRepository
    {
        public List<DonTu> GetAll()
        {
            string query = @"
                SELECT d.*, n.HoTen, l.TenLoaiDon 
                FROM dontu d
                JOIN nhanvien n ON d.MaNV = n.MaNV
                JOIN loaidon l ON d.MaLoaiDon = l.MaLoaiDon
                ORDER BY d.NgayGui DESC";

            DataTable dt = Database.ExecuteQuery(query);
            List<DonTu> list = new List<DonTu>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DonTu
                {
                    MaDon = Convert.ToInt32(row["MaDon"]),
                    MaNV = Convert.ToInt32(row["MaNV"]),
                    MaLoaiDon = Convert.ToInt32(row["MaLoaiDon"]),
                    NgayBatDau = Convert.ToDateTime(row["NgayBatDau"]),
                    NgayKetThuc = Convert.ToDateTime(row["NgayKetThuc"]),
                    LyDo = row["LyDo"].ToString(),
                    TrangThai = row["TrangThai"].ToString(),
                    NgayGui = Convert.ToDateTime(row["NgayGui"]),
                    NguoiDuyet = row["NguoiDuyet"] != DBNull.Value ? row["NguoiDuyet"].ToString() : "",
                    HoTenNhanVien = row["HoTen"].ToString(),
                    TenLoaiDon = row["TenLoaiDon"].ToString()
                });
            }
            return list;
        }

        public List<LoaiDon> GetLoaiDon()
        {
            string query = "SELECT * FROM loaidon";
            DataTable dt = Database.ExecuteQuery(query);
            List<LoaiDon> list = new List<LoaiDon>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new LoaiDon
                {
                    MaLoaiDon = Convert.ToInt32(row["MaLoaiDon"]),
                    TenLoaiDon = row["TenLoaiDon"].ToString(),
                    MoTa = row["MoTa"] != DBNull.Value ? row["MoTa"].ToString() : "",
                    CoLuong = row["MoTa"] != DBNull.Value ? row["CoLuong"].ToString() : ""
                });
            }
            return list;
        }

        public List<DonTu> GetByNhanVien(int maNV)
        {
            string query = @"
                SELECT d.*, n.HoTen, l.TenLoaiDon 
                FROM dontu d
                JOIN nhanvien n ON d.MaNV = n.MaNV
                JOIN loaidon l ON d.MaLoaiDon = l.MaLoaiDon
                WHERE d.MaNV = @MaNV
                ORDER BY d.NgayGui DESC";

            var parameters = new Dictionary<string, object> { { "@MaNV", maNV } };
            DataTable dt = Database.ExecuteQuery(query, parameters);
            
            List<DonTu> list = new List<DonTu>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DonTu
                {
                    MaDon = Convert.ToInt32(row["MaDon"]),
                    MaNV = Convert.ToInt32(row["MaNV"]),
                    MaLoaiDon = Convert.ToInt32(row["MaLoaiDon"]),
                    NgayBatDau = Convert.ToDateTime(row["NgayBatDau"]),
                    NgayKetThuc = Convert.ToDateTime(row["NgayKetThuc"]),
                    LyDo = row["LyDo"].ToString(),
                    TrangThai = row["TrangThai"].ToString(),
                    NgayGui = Convert.ToDateTime(row["NgayGui"]),
                    NguoiDuyet = row["NguoiDuyet"] != DBNull.Value ? row["NguoiDuyet"].ToString() : "",
                    HoTenNhanVien = row["HoTen"].ToString(),
                    TenLoaiDon = row["TenLoaiDon"].ToString()
                });
            }
            return list;
        }

        public bool CreateDonTu(DonTu don)
        {
            string query = @"INSERT INTO dontu (MaNV, MaLoaiDon, NgayBatDau, NgayKetThuc, LyDo, TrangThai, NgayGui)
                             VALUES (@MaNV, @MaLoaiDon, @NgayBatDau, @NgayKetThuc, @LyDo, 'Chưa duyệt', NOW())";
            var parameters = new Dictionary<string, object>
            {
                { "@MaNV", don.MaNV },
                { "@MaLoaiDon", don.MaLoaiDon },
                { "@NgayBatDau", don.NgayBatDau },
                { "@NgayKetThuc", don.NgayKetThuc },
                { "@LyDo", don.LyDo }
            };
            return Database.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool CreateLoaiDon(LoaiDon loai)
        {
            string query = "INSERT INTO loaidon (TenLoaiDon, MoTa) VALUES (@Ten, @MoTa)";
            var parameters = new Dictionary<string, object>
            {
                { "@Ten", loai.TenLoaiDon },
                { "@MoTa", string.IsNullOrEmpty(loai.MoTa) ? DBNull.Value : loai.MoTa }
            };
            return Database.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateTrangThai(int maDon, string trangThai, string nguoiDuyet)
        {
            string query = @"UPDATE dontu SET TrangThai = @TrangThai, NguoiDuyet = @NguoiDuyet WHERE MaDon = @MaDon";
            var parameters = new Dictionary<string, object>
            {
                { "@MaDon", maDon },
                { "@TrangThai", trangThai },
                { "@NguoiDuyet", nguoiDuyet }
            };
            return Database.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateLoaiDon(LoaiDon loai)
        {
            string query = "UPDATE loaidon SET TenLoaiDon = @Ten, MoTa = @MoTa , CoLuong = @CoLuong WHERE MaLoaiDon = @Ma";
            var parameters = new Dictionary<string, object>
            {
                { "@Ma", loai.MaLoaiDon },
                { "@Ten", loai.TenLoaiDon },
                { "@MoTa", string.IsNullOrEmpty(loai.MoTa) ? DBNull.Value : loai.MoTa },
                { "@CoLuong", string.IsNullOrEmpty(loai.CoLuong) ? DBNull.Value : loai.CoLuong }
            };
            return Database.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteLoaiDon(int maLoai)
        {
            string query = "DELETE FROM loaidon WHERE MaLoaiDon = @Ma";
            var parameters = new Dictionary<string, object>
            {
                { "@Ma", maLoai }
            };
            return Database.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteDonTu(int maDon)
        {
            string query = "DELETE FROM dontu WHERE MaDon = @MaDon";
            var parameters = new Dictionary<string, object> { { "@MaDon", maDon } };
            return Database.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}