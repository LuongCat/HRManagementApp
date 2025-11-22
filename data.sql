DROP DATABASE IF EXISTS `hrmanagement`;
	CREATE DATABASE `hrmanagement`;
	USE `hrmanagement`;

	-- ============================================
	-- 1. BẢNG KHÔNG PHỤ THUỘC				
	-- ============================================

CREATE TABLE `nhanvien` (
                            `MaNV` int NOT NULL AUTO_INCREMENT,
                            `HoTen` varchar(100) NOT NULL,
                            `NgaySinh` date DEFAULT NULL,
                            `SoCCCD` varchar(20) DEFAULT NULL,
                            `DienThoai` varchar(15) DEFAULT NULL,
                            `TrangThai` ENUM('Còn làm việc', 'Nghỉ việc') DEFAULT 'Còn làm việc',
                            `NgayVaoLam` date DEFAULT (CURRENT_DATE),
                            `GioiTinh` ENUM('Nam', 'Nữ') DEFAULT 'Nam',
                            PRIMARY KEY (`MaNV`),
                            UNIQUE KEY `SoCCCD` (`SoCCCD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `chucvu` (
                          `MaCV` int NOT NULL AUTO_INCREMENT,
                          `TenCV` varchar(100) NOT NULL,
                          `PhuCap` decimal(18,2) DEFAULT 0,
                          `LuongCB` decimal(18,2) DEFAULT 0,
                          `TienPhuCapKiemNhiem` DECIMAL(18,2) DEFAULT 0,
                          `HoatDong` ENUM('Active', 'inActive') DEFAULT "Active",
                          PRIMARY KEY (`MaCV`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;



CREATE TABLE `loaidon` (
                           `MaLoaiDon` int NOT NULL AUTO_INCREMENT,
                           `TenLoaiDon` varchar(100) NOT NULL,
                           `MoTa` varchar(50) DEFAULT NULL,
                           PRIMARY KEY (`MaLoaiDon`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;



CREATE TABLE `quyenhan` (
                            `MaQuyen` int NOT NULL AUTO_INCREMENT,
                            `TenQuyen` varchar(100) DEFAULT NULL,
                            `MoTa` varchar(200) DEFAULT NULL,
                            PRIMARY KEY (`MaQuyen`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `vaitro` (
                          `MaVaiTro` int NOT NULL AUTO_INCREMENT,
                          `TenVaiTro` varchar(50) DEFAULT NULL,
                          `MoTa` varchar(100) DEFAULT NULL,
                          PRIMARY KEY (`MaVaiTro`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================
-- 2. BẢNG PHỤ THUỘC 1 LỚP
-- ============================================

CREATE TABLE `phongban` (
                            `MaPB` int NOT NULL AUTO_INCREMENT,
                            `TenPB` varchar(100) NOT NULL,
                            `MoTa` varchar(200) DEFAULT NULL,
                            `MaTruongPhong` int DEFAULT NULL,
                            PRIMARY KEY (`MaPB`),
                            FOREIGN KEY (`MaTruongPhong`) REFERENCES `nhanvien` (`MaNV`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `taikhoan` (
                            `MaTK` int NOT NULL AUTO_INCREMENT,
                            `MaNV` int DEFAULT NULL,
                            `TenDangNhap` varchar(50) NOT NULL,
                            `MatKhau` varchar(256) NOT NULL,
                            `VaiTro` varchar(50) DEFAULT NULL,
                            `TrangThai` ENUM('Hoạt động', 'Đã khóa', 'Đã xóa') DEFAULT 'Hoạt động',
                            PRIMARY KEY (`MaTK`),
                            UNIQUE KEY `TenDangNhap` (`TenDangNhap`),
                            KEY `MaNV` (`MaNV`),
                            CONSTRAINT `taikhoan_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;



CREATE TABLE `chamcong` (
                            `MaCC` int NOT NULL AUTO_INCREMENT,
                            `MaNV` int NOT NULL,
                            `Ngay` date NOT NULL DEFAULT (CURRENT_DATE),
                            `GioCC` time DEFAULT (CURRENT_TIME),

                            PRIMARY KEY (`MaCC`),
                            KEY `MaNV` (`MaNV`),
                            CONSTRAINT `chamcong_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `dontu` (
                         `MaDon` int NOT NULL AUTO_INCREMENT,
                         `MaNV` int NOT NULL,
                         `MaLoaiDon` int DEFAULT NULL,
                         `NgayBatDau` datetime DEFAULT (CURRENT_TIMESTAMP),
                         `NgayKetThuc` datetime DEFAULT (CURRENT_TIMESTAMP),
                         `LyDo` varchar(200) DEFAULT NULL,
                         `TrangThai` ENUM('Chưa duyệt', 'Đã duyệt', 'Từ chối') DEFAULT 'Chưa duyệt',
                         `NgayGui` datetime DEFAULT (CURRENT_TIMESTAMP),
                         `NguoiDuyet` varchar(100) DEFAULT NULL,
                         PRIMARY KEY (`MaDon`),
                         KEY `MaNV` (`MaNV`),
                         KEY `MaLoaiDon` (`MaLoaiDon`),
                         CONSTRAINT `dontu_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE,
                         CONSTRAINT `dontu_ibfk_2` FOREIGN KEY (`MaLoaiDon`) REFERENCES `loaidon` (`MaLoaiDon`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `luong` (
                         `MaLuong` int NOT NULL AUTO_INCREMENT,
                         `MaNV` int NOT NULL,
                         `Thang` int NOT NULL,
                         `Nam` int NOT NULL,
                         `TongNgayCong` int DEFAULT 0,
                         `TienLuong` decimal(18,2) DEFAULT 0,
                         `LuongThucNhan` decimal(18,2) DEFAULT 0,
                         `TrangThai` ENUM('Chưa trả', 'Đã trả') DEFAULT 'Chưa trả',
                         PRIMARY KEY (`MaLuong`),
                         KEY `MaNV` (`MaNV`),
                         CONSTRAINT `luong_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================
-- 3. BẢNG LIÊN KẾT N-N
-- ============================================

CREATE TABLE `vaitro_quyenhan` (
                                   `MaVaiTro` int NOT NULL,
                                   `MaQuyenHan` int NOT NULL,
                                   PRIMARY KEY (`MaVaiTro`,`MaQuyenHan`),
                                   KEY `vtr_qh_ibfk_2` (`MaQuyenHan`),
                                   CONSTRAINT `vtr_qh_ibfk_1` FOREIGN KEY (`MaVaiTro`) REFERENCES `vaitro` (`MaVaiTro`) ON DELETE CASCADE ON UPDATE CASCADE,
                                   CONSTRAINT `vtr_qh_ibfk_2` FOREIGN KEY (`MaQuyenHan`) REFERENCES `quyenhan` (`MaQuyen`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `taikhoan_vaitro` (
                                   `MaTK` int NOT NULL,
                                   `MaVaiTro` int NOT NULL,
                                   PRIMARY KEY (`MaTK`,`MaVaiTro`),
                                   KEY `tk_vt_ibfk_2` (`MaVaiTro`),
                                   CONSTRAINT `tk_vt_ibfk_1` FOREIGN KEY (`MaTK`) REFERENCES `taikhoan` (`MaTK`) ON DELETE CASCADE ON UPDATE CASCADE,
                                   CONSTRAINT `tk_vt_ibfk_2` FOREIGN KEY (`MaVaiTro`) REFERENCES `vaitro` (`MaVaiTro`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `nhanvien_chucvu` (
                                   `MaNV` INT NOT NULL,
                                   `MaCV` INT NOT NULL,
                                   `MaPB` INT NOT NULL,
                                   `HeSoLuongCoban` DECIMAL(6,4) DEFAULT 1.0,
                                   `HeSoPhuCapKiemNhiem` DECIMAL(6,4) DEFAULT 0,
                                   `LoaiChucVu` ENUM('Chính thức', 'Kiêm nhiệm') DEFAULT 'Chính thức',
                                   `GhiChu` VARCHAR(200) DEFAULT NULL,

                                   PRIMARY KEY (`MaNV`, `MaCV`, `MaPB`),
                                   KEY `nv_cv_fk_cv` (`MaCV`),
                                   KEY `nv_cv_fk_pb` (`MaPB`),

                                   CONSTRAINT `nv_cv_fk_nv` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE,
                                   CONSTRAINT `nv_cv_fk_cv` FOREIGN KEY (`MaCV`) REFERENCES `chucvu` (`MaCV`) ON DELETE CASCADE ON UPDATE CASCADE,
                                   CONSTRAINT `nv_cv_fk_pb` FOREIGN KEY (`MaPB`) REFERENCES `phongban` (`MaPB`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;





CREATE TABLE `chamcong_lienketdon` (
                                       `MaCC` int NOT NULL,
                                       `MaDon` int NOT NULL,
                                       PRIMARY KEY (`MaCC`,`MaDon`),
                                       KEY `cclkd_ibfk_2` (`MaDon`),
                                       CONSTRAINT `cclkd_ibfk_1` FOREIGN KEY (`MaCC`) REFERENCES `chamcong` (`MaCC`) ON DELETE CASCADE ON UPDATE CASCADE,
                                       CONSTRAINT `cclkd_ibfk_2` FOREIGN KEY (`MaDon`) REFERENCES `dontu` (`MaDon`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================
-- them
-- ============================================

-- Bảng phụ cấp cho nhân viên
CREATE TABLE `phucap_nhanvien` (
                                   `ID` int NOT NULL AUTO_INCREMENT,
                                   `MaNV` int NOT NULL,
                                   `TenPhuCap` varchar(100) NOT NULL,
                                   `SoTien` decimal(18,2) NOT NULL DEFAULT 0,
                                   `ApDungTuNgay` date NOT NULL,
                                   `ApDungDenNgay` date DEFAULT NULL,
                                   PRIMARY KEY (`ID`),
                                   KEY `fk_phucap_nv` (`MaNV`),
                                   CONSTRAINT `fk_phucap_nv` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien`(`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Bảng khấu trừ cho nhân viên
CREATE TABLE `khautru` (
                           `MaKT` int NOT NULL AUTO_INCREMENT,
                           `MaNV` int NOT NULL,
                           `TenKhoanTru` varchar(100) NOT NULL,
                           `SoTien` decimal(18,2) NOT NULL DEFAULT 0,
                           `Ngay` date NOT NULL DEFAULT (CURRENT_DATE),
                           `GhiChu` varchar(200) DEFAULT NULL,
                           PRIMARY KEY (`MaKT`),
                           KEY `fk_khautru_nv` (`MaNV`),
                           CONSTRAINT `fk_khautru_nv` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien`(`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Bảng thuế thu nhập cá nhân
CREATE TABLE `thue` (
                        `MaThue` int NOT NULL AUTO_INCREMENT,
                        `MaNV` int NOT NULL,
                        `TenThue` varchar(100) NOT NULL,
                        `SoTien` decimal(18,2) NOT NULL DEFAULT 0,
                        `ApDungTuNgay` date NOT NULL,
                        `ApDungDenNgay` date DEFAULT NULL,
                        PRIMARY KEY (`MaThue`),
                        KEY `fk_thue_nv` (`MaNV`),
                        CONSTRAINT `fk_thue_nv` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien`(`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;






-- ============================================
-- 4. DỮ LIỆU MẪU
-- ============================================

INSERT INTO chucvu (MaCV, TenCV, PhuCap, LuongCB, TienPhuCapKiemNhiem) VALUES
                                                                           (1,'Nhân viên',1000000.00,8000000.00,0),
                                                                           (2,'Tổ trưởng',1500000.00,10000000.00,10000),
                                                                           (3,'Trưởng phòng',3000000.00,15000000.00,10000),
                                                                           (4,'Giám đốc',5000000.00,25000000.00,20000);

INSERT INTO `nhanvien` VALUES
                           (1,'Nguyễn Văn Ann','1990-05-15','012345678901','0901123456','Nghỉ việc','2020-01-10','Nam'),
                           (2,'Trần Thị Bánh','1993-08-21','023456789012','0902345678','Còn làm việc','2022-03-18','Nữ'),
                           (3,'Lê Văn C','1988-12-05','034567890123','0903456789','Còn làm việc','2019-06-01','Nam'),
                           (4,'Phạm Thị D','1995-11-30','045678901234','0904567890','Còn làm việc','2022-02-25','Nữ');

INSERT INTO `phongban` VALUES
                           (1,'Phòng Nhân Sự','Quản lý nhân sự và tuyển dụng',1),
                           (2,'Phòng Kỹ Thuật','Phát triển và bảo trì hệ thống',2),
                           (3,'Phòng Kinh Doanh','Tư vấn và bán hàng',3),
                           (4,'Phòng Kế Toán','Quản lý tài chính và thu chi',4);


INSERT INTO chamcong (MaNV, Ngay, GioCC) VALUES
                                             (1,'2024-09-01','08:00:00'),
                                             (1,'2024-09-02','08:05:00'),
                                             (1,'2024-09-03','08:10:00'),
                                             (1,'2024-09-04','08:00:00'),
                                             (2,'2024-09-01','08:15:00'),
                                             (2,'2024-09-02','08:00:00'),
                                             (2,'2024-09-03','08:05:00'),
                                             (2,'2024-09-04','08:00:00'),
                                             (3,'2024-09-01','07:55:00'),
                                             (3,'2024-09-02','08:00:00'),
                                             (3,'2024-09-03','08:10:00'),
                                             (3,'2024-09-04','08:00:00'),
                                             (4,'2024-09-01','08:20:00'),
                                             (4,'2024-09-02','08:05:00'),
                                             (4,'2024-09-03','08:00:00'),
                                             (4,'2024-09-04','08:00:00');


INSERT INTO `luong` VALUES
                        (1,1,9,2024,26,100,90,'Đã trả'),
                        (2,2,9,2024,24,100,90,'Đã trả'),
                        (3,3,9,2024,27,100,90,'Đã trả'),
                        (4,4,9,2024,25,100,90,'Đã trả');

INSERT INTO `nhanvien_chucvu`
(MaNV, MaCV, MaPB, HeSoPhuCapKiemNhiem, LoaiChucVu, GhiChu)
VALUES
    (1, 2, 1, 0,  'Chính thức', ''),
    (1, 3, 1, 0,  'Chính thức', ''),
    (2, 3, 2, 0.1500, 'Kiêm nhiệm', '');


INSERT INTO `vaitro` VALUES
                         (1,'Admin','Quản trị hệ thống'),
                         (2,'QuanLy','Quản lý bộ phận'),
                         (3,'NhanVien','Nhân viên');

INSERT INTO `quyenhan` VALUES
                           (1,'DuyetDon','Quyền duyệt đơn'),
                           (2,'XemBaoCao','Quyền xem báo cáo'),
                           (3,'QuanLyNhanVien','Quyền quản lý nhân viên');

INSERT INTO `vaitro_quyenhan` VALUES
                                  (1,1),(1,2),(1,3),
                                  (2,1),(2,2),
                                  (3,2);

-- ============================================
-- Dữ liệu mẫu cho phucap_nhanvien
-- ============================================

INSERT INTO `phucap_nhanvien` (`MaNV`, `TenPhuCap`, `SoTien`, `ApDungTuNgay`, `ApDungDenNgay`) VALUES
                                                                                                   (1, 'Phụ cấp thâm niên', 500000, '2024-01-01', NULL),
                                                                                                   (1, 'Phụ cấp trách nhiệm', 300000, '2024-06-01', '2024-12-31'),
                                                                                                   (2, 'Phụ cấp trách nhiệm', 200000, '2024-01-01', NULL),
                                                                                                   (3, 'Phụ cấp ca đêm', 150000, '2024-03-01', NULL),
                                                                                                   (4, 'Phụ cấp trách nhiệm', 250000, '2024-05-01', NULL);

-- ============================================
-- Dữ liệu mẫu cho khautru
-- ============================================

INSERT INTO `khautru` (`MaNV`, `TenKhoanTru`, `SoTien`, `Ngay`, `GhiChu`) VALUES
                                                                              (1, 'Bảo hiểm xã hội', 800000, '2024-09-30', 'Khấu trừ tháng 9'),
                                                                              (1, 'Bảo hiểm y tế', 200000, '2024-09-30', NULL),
                                                                              (2, 'Bảo hiểm xã hội', 750000, '2024-09-30', NULL),
                                                                              (3, 'Bảo hiểm xã hội', 700000, '2024-09-30', NULL),
                                                                              (4, 'Bảo hiểm xã hội', 650000, '2024-09-30', NULL);

-- ============================================
-- Dữ liệu mẫu cho thue
-- ============================================

INSERT INTO `thue` (`MaNV`, `TenThue`, `SoTien`, `ApDungTuNgay`, `ApDungDenNgay`) VALUES
                                                                                      (1, 'Thuế thu nhập cá nhân', 1200000, '2024-09-01', '2024-09-30'),
                                                                                      (2, 'Thuế thu nhập cá nhân', 800000, '2024-09-01', '2024-09-30'),
                                                                                      (3, 'Thuế thu nhập cá nhân', 1000000, '2024-09-01', '2024-09-30'),
                                                                                      (4, 'Thuế thu nhập cá nhân', 750000, '2024-09-01', '2024-09-30');


-- ============================================
-- 1. DỮ LIỆU MẪU CHO BẢNG CA LÀM VIỆC (calam)
-- ============================================

-- ============================================
-- 2. DỮ LIỆU MẪU CHO BẢNG LOẠI ĐƠN (loaidon)
-- ============================================
INSERT INTO `loaidon` (`TenLoaiDon`, `MoTa`) VALUES
                                                 ('Nghỉ phép năm', 'Nghỉ hưởng lương theo quy định năm'),
                                                 ('Nghỉ ốm', 'Nghỉ có giấy xác nhận của bác sĩ'),
                                                 ('Nghỉ thai sản', 'Nghỉ sinh con theo chế độ BHXH'),
                                                 ('Nghỉ không lương', 'Nghỉ việc riêng tự nguyện không hưởng lương'),
                                                 ('Công tác', 'Đi công tác bên ngoài công ty');

-- ============================================
-- 3. DỮ LIỆU MẪU CHO BẢNG TÀI KHOẢN (taikhoan)
-- ============================================
-- Lưu ý: Mật khẩu ở đây là demo dạng text, thực tế nên mã hóa
INSERT INTO `taikhoan` (`MaNV`, `TenDangNhap`, `MatKhau`, `VaiTro`, `TrangThai`) VALUES
                                                                                     (1, 'nguyenann', '123456', 'NhanVien', 'Hoạt động'),
                                                                                     (2, 'tranthibanh', 'password123', 'QuanLy', 'Hoạt động'),
                                                                                     (3, 'levanc', 'admin123', 'Admin', 'Hoạt động'),
                                                                                     (4, 'phamthid', 'pass456', 'NhanVien', 'Đã khóa'),
                                                                                     (NULL, 'superadmin', 'rootpass', 'Admin', 'Hoạt động');

-- ============================================
-- 4. DỮ LIỆU MẪU CHO BẢNG TÀI KHOẢN - VAI TRÒ (taikhoan_vaitro)
-- ============================================
-- Giả định MaTK 1-5 vừa tạo ở trên, MaVaiTro 1(Admin), 2(QuanLy), 3(NhanVien) có sẵn
INSERT INTO `taikhoan_vaitro` (`MaTK`, `MaVaiTro`) VALUES
                                                       (1, 3), -- TK 1 là Nhân viên
                                                       (2, 2), -- TK 2 là Quản lý
                                                       (3, 1), -- TK 3 là Admin
                                                       (4, 3), -- TK 4 là Nhân viên
                                                       (5, 1); -- TK 5 là Admin

-- ============================================
-- 5. DỮ LIỆU MẪU CHO BẢNG LỊCH LÀM VIỆC (calamviec)
-- ============================================

-- ============================================
-- 6. DỮ LIỆU MẪU CHO BẢNG ĐƠN TỪ (dontu)
-- ============================================
INSERT INTO `dontu` (`MaNV`, `MaLoaiDon`, `NgayBatDau`, `NgayKetThuc`, `LyDo`, `TrangThai`, `NguoiDuyet`) VALUES
                                                                                                              (1, 1, '2024-10-01 08:00:00', '2024-10-02 17:00:00', 'Về quê thăm gia đình', 'Đã duyệt', 'Trần Thị Bánh'),
                                                                                                              (2, 2, '2024-10-05 08:00:00', '2024-10-05 17:00:00', 'Sốt cao', 'Chưa duyệt', NULL),
                                                                                                              (3, 5, '2024-10-10 08:00:00', '2024-10-12 17:00:00', 'Gặp khách hàng miền Bắc', 'Đã duyệt', 'Giám đốc'),
                                                                                                              (4, 4, '2024-10-15 08:00:00', '2024-10-15 12:00:00', 'Đi họp phụ huynh', 'Từ chối', 'Trần Thị Bánh'),
                                                                                                              (1, 2, '2024-10-20 08:00:00', '2024-10-20 17:00:00', 'Đau bụng', 'Chưa duyệt', NULL);

-- ============================================
-- 7. DỮ LIỆU MẪU CHO BẢNG CHẤM CÔNG - LIÊN KẾT ĐƠN (chamcong_lienketdon)
-- ============================================





