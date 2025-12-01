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
            string query =
                "UPDATE loaidon SET TenLoaiDon = @Ten, MoTa = @MoTa , CoLuong = @CoLuong WHERE MaLoaiDon = @Ma";
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


        public KetQuaNghi GetSoNgayNghi(int maNV, int thang, int nam)
        {
            var ketQua = new KetQuaNghi();
            
            DateTime dauThang = new DateTime(nam, thang, 1);
            DateTime cuoiThang = dauThang.AddMonths(1).AddDays(-1);

            string query = @"
        SELECT d.NgayBatDau, d.NgayKetThuc, l.CoLuong
        FROM dontu d
        JOIN loaidon l ON d.MaLoaiDon = l.MaLoaiDon
        WHERE d.MaNV = @MaNV
          AND d.TrangThai = 'Đã duyệt'
          AND d.NgayBatDau <= @CuoiThang
          AND d.NgayKetThuc >= @DauThang;";

            var parameters = new Dictionary<string, object>
            {
                { "@MaNV", maNV },
                { "@DauThang", dauThang },
                { "@CuoiThang", cuoiThang }
            };

            DataTable data = Database.ExecuteQuery(query, parameters);

            // 2. Duyệt từng đơn để tính toán
            if (data != null && data.Rows.Count > 0)
            {
                foreach (DataRow row in data.Rows)
                {
                    DateTime ngayBatDauDon = Convert.ToDateTime(row["NgayBatDau"]);
                    DateTime ngayKetThucDon = Convert.ToDateTime(row["NgayKetThuc"]);
                    string coLuong = row["CoLuong"].ToString(); // Giá trị là "Yes" hoặc "No"

                    // 3. TÍNH GIAO THOA (Intersection)
                    // Chỉ tính những ngày nằm trong tháng hiện tại
                    DateTime startCount = ngayBatDauDon < dauThang ? dauThang : ngayBatDauDon;

                    // Min(Cuối đơn, Cuối tháng)
                    DateTime endCount = ngayKetThucDon > cuoiThang ? cuoiThang : ngayKetThucDon;

                    // 4. Đếm số ngày (Trừ chủ nhật)
                    int soNgay = CountWorkingDays(startCount, endCount);

                    // 5. Cộng vào biến tương ứng
                    if (coLuong == "Yes")
                    {
                        ketQua.NghiCoLuong += soNgay;
                    }
                    else // CoLuong == "No"
                    {
                        ketQua.NghiKhongLuong += soNgay;
                    }
                }
            }

            return ketQua;
        }
        private int CountWorkingDays(DateTime start, DateTime end)
        {
            int count = 0;
            for (DateTime date = start.Date; date <= end.Date; date = date.AddDays(1))
            {
                // Nếu công ty nghỉ cả thứ 7 thì thêm điều kiện: && date.DayOfWeek != DayOfWeek.Saturday
                if (date.DayOfWeek != DayOfWeek.Sunday)
                {
                    count++;
                }
            }
            return count;
        }
    }
}