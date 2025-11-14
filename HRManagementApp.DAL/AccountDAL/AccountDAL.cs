using System;
using System.Collections.Generic;
using System.Data;
using HRManagementApp.models;

namespace HRManagementApp.DAL
{
    public class AccountDAL
    {
        public List<AccountModel> getAllAccountModel()
        {
            List<AccountModel> accounts = new List<AccountModel>();
                                    
            try
            {
                string query = @"SELECT 
                                    tk.TenDangNhap,
                                    nv.HoTen AS Name,
                                    nv.DienThoai AS SDT,
                                    vt.TenVaiTro AS VaiTro,
                                    pb.TenPB AS PhongBan,
                                    tk.TrangThai
                                FROM taikhoan tk
                                LEFT JOIN nhanvien nv ON tk.MaNV = nv.MaNV

                                -- Lấy vai trò chính (MaVaiTro nhỏ nhất)
                                LEFT JOIN (
                                    SELECT MaTK, MIN(MaVaiTro) AS MaVaiTro
                                    FROM taikhoan_vaitro
                                    GROUP BY MaTK
                                ) AS tk_vt_min ON tk.MaTK = tk_vt_min.MaTK

                                LEFT JOIN vaitro vt ON vt.MaVaiTro = tk_vt_min.MaVaiTro

                                -- Lấy phòng ban chính (nếu có LoạiChucVu='Chính thức')
                                LEFT JOIN (
                                    SELECT 
                                        npb.MaNV, 
                                        MIN(npb.MaPB) AS MaPB -- nếu 1 NV có nhiều chính thức, lấy nhỏ nhất
                                    FROM nhanvien_phongban npb
                                    GROUP BY npb.MaNV
                                ) AS pb_min ON pb_min.MaNV = nv.MaNV

                                LEFT JOIN phongban pb ON pb.MaPB = pb_min.MaPB;
                                ";
                
                DataTable data = Database.ExecuteQuery(query);

                if (data.Rows.Count == 0)
                {
                    Console.WriteLine("Không có dữ liệu từ database");
                    return accounts;
                }

                foreach (DataRow row in data.Rows)
                {
                    AccountModel account = new AccountModel(
                        row["TenDangNhap"].ToString(),
                        row["Name"].ToString(),
                        row["SDT"].ToString(),  // ← Khớp với alias AS SDT
                        row["VaiTro"].ToString(),
                        row["PhongBan"].ToString(),
                        row["TrangThai"].ToString()
                    );

                    accounts.Add(account);
                    Console.WriteLine($"Loaded: {account.TenDangNhap}"); // Debug
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return accounts;
        }
    }
}