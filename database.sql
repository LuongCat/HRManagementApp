DROP DATABASE IF EXISTS `hrmanagement`;
CREATE DATABASE `hrmanagement`;
USE `hrmanagement`;

CREATE TABLE `nhanvien` (
  `MaNV` int NOT NULL AUTO_INCREMENT,
  `HoTen` varchar(100) NOT NULL,
  `NgaySinh` date DEFAULT NULL,
  `SoCCCD` varchar(20) DEFAULT NULL,
  `DienThoai` varchar(15) DEFAULT NULL,
  `TrangThai` ENUM('Còn làm việc', 'Nghỉ việc') NOT NULL DEFAULT 'Còn làm việc',
  `NgayVaoLam` date DEFAULT (CURRENT_DATE),
  `GioiTinh` ENUM('Nam', 'Nữ') DEFAULT 'Nam',
  `MaPB` INT,
  PRIMARY KEY (`MaNV`),
  UNIQUE KEY `SoCCCD` (`SoCCCD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `chucvu` (
  `MaCV` int NOT NULL AUTO_INCREMENT,
  `TenCV` varchar(100) NOT NULL,
  `PhuCap` decimal(18,2) DEFAULT 0,
  `LuongCB` decimal(18,2) DEFAULT 0,
  `TienPhuCapKiemNhiem` DECIMAL(18,2) DEFAULT 0,
  `TrangThai` enum('Hoạt động', 'Tạm ngưng', 'Đã xóa') NOT NULL DEFAULT 'Hoạt động',
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
  `TrangThai` ENUM('Hoạt động', 'Đã khóa', 'Đã xóa') NOT NULL DEFAULT 'Hoạt động',
  PRIMARY KEY (`MaTK`),
  UNIQUE KEY `TenDangNhap` (`TenDangNhap`),
  KEY `MaNV` (`MaNV`),
  CONSTRAINT `taikhoan_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `calam` (
  `MaCa` int NOT NULL AUTO_INCREMENT PRIMARY KEY,
  `TenCa` varchar(20) NOT NULL,
  `GioBatDau` time DEFAULT '00:00:00',
  `GioKetThuc` time DEFAULT '00:00:00',
  `TrangThai` enum('Hoạt động', 'Tạm ngừng', 'Đã xóa') NOT NULL DEFAULT 'Hoạt động'
)ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `chamcong` (
  `MaCC` int NOT NULL AUTO_INCREMENT,
  `MaNV` int NOT NULL,
  `Ngay` date NOT NULL DEFAULT (CURRENT_DATE),
  `GioVao` time DEFAULT (CURRENT_TIME),
  `GioRa` time DEFAULT NULL,
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
  `TrangThai` ENUM('Chưa duyệt', 'Đã duyệt', 'Từ chối') NOT NULL DEFAULT 'Chưa duyệt',
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
  `TrangThai` ENUM('Chưa trả', 'Đã trả') NOT NULL DEFAULT 'Chưa trả',
  PRIMARY KEY (`MaLuong`),
  KEY `MaNV` (`MaNV`),
  CONSTRAINT `luong_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

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
  `HeSoLuongCoBan` DECIMAL(6,4) DEFAULT 0,
  `HeSoPhuCapKiemNhiem` DECIMAL(6,4) DEFAULT 0,
  `LoaiChucVu` ENUM('Chính thức', 'Kiêm nhiệm') NOT NULL DEFAULT 'Chính thức',
  `GhiChu` VARCHAR(200) DEFAULT NULL,

  PRIMARY KEY (`MaNV`, `MaCV`),
  KEY `nv_cv_fk_cv` (`MaCV`),
  
  CONSTRAINT `nv_cv_fk_nv` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `nv_cv_fk_cv` FOREIGN KEY (`MaCV`) REFERENCES `chucvu` (`MaCV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

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
--  DỮ LIỆU MẪU
-- ============================================

INSERT INTO `chucvu` VALUES 
(1,'Nhân viên',1000000.00,8000000.00,0,'Hoạt động'),
(2,'Tổ trưởng',1500000.00,10000000.00,10000,'Hoạt động'),
(3,'Trưởng phòng',3000000.00,15000000.00,10000,'Hoạt động'),
(4,'Giám đốc',5000000.00,25000000.00,20000,'Hoạt động');

INSERT INTO taikhoan (MaNV, TenDangNhap, MatKhau, TrangThai)
VALUES 
-- lưu ý pass:123456
(1, 'admin', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Hoạt động'),
(2, 'user2', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Hoạt động'),
(3, 'user3', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Hoạt động'),
(4, 'user4', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'Hoạt động'),



INSERT INTO `nhanvien` VALUES 
(1,'Nguyễn Văn Ann','1990-05-15','012345678901','0901123456','Nghỉ việc','2020-01-10','Nam',1),
(2,'Trần Thị Bánh','1993-08-21','023456789012','0902345678','Còn làm việc','2022-03-18','Nữ',2),
(3,'Lê Văn C','1988-12-05','034567890123','0903456789','Còn làm việc','2019-06-01','Nam',3),
(4,'Phạm Thị D','1995-11-30','045678901234','0904567890','Còn làm việc','2022-02-25','Nữ',4);

INSERT INTO `phongban` VALUES 
(1,'Phòng Nhân Sự','Quản lý nhân sự và tuyển dụng',1),
(2,'Phòng Kỹ Thuật','Phát triển và bảo trì hệ thống',2),
(3,'Phòng Kinh Doanh','Tư vấn và bán hàng',3),
(4,'Phòng Kế Toán','Quản lý tài chính và thu chi',4);

INSERT INTO `chamcong` VALUES 
(61,1,'2024-09-01','08:00:00','16:00:00'),
(62,1,'2024-09-02','08:05:00','16:00:00'),
(63,1,'2024-09-03','08:10:00','16:00:00'),
(64,1,'2024-09-04','08:00:00','16:00:00'),
(65,2,'2024-09-01','08:15:00','16:00:00'),
(66,2,'2024-09-02','08:00:00','16:00:00'),
(67,2,'2024-09-03','08:05:00','16:00:00'),
(68,2,'2024-09-04','08:00:00','16:00:00'),
(69,3,'2024-09-01','07:55:00','16:00:00'),
(70,3,'2024-09-02','08:00:00','16:00:00'),
(71,3,'2024-09-03','08:10:00','16:00:00'),
(72,3,'2024-09-04','08:00:00','16:00:00'),
(73,4,'2024-09-01','08:20:00','16:00:00'),
(74,4,'2024-09-02','08:05:00','16:00:00'),
(75,4,'2024-09-03','08:00:00','16:00:00'),
(76,4,'2024-09-04','08:00:00','16:00:00');

INSERT INTO `luong` VALUES 
(1,1,9,2024,26,18000000.00,17000000.00,'Đã trả'),
(2,2,9,2024,24,11000000.00,11000000.00,'Đã trả'),
(3,3,9,2024,27,14500000.00,14500000.00,'Đã trả'),
(4,4,9,2024,25,10500000.00,10500000.00,'Đã trả');

INSERT INTO `nhanvien_chucvu` 
(MaNV, MaCV, HeSoLuongCoBan, HeSoPhuCapKiemNhiem, LoaiChucVu, GhiChu)
VALUES
(1, 2, 1, 0,  'Chính thức', ''),
(1, 3, 1, 0,  'Chính thức', ''),
(2, 3, 1, 0.1500, 'Kiêm nhiệm', '');

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

INSERT INTO `phucap_nhanvien` (`MaNV`, `TenPhuCap`, `SoTien`, `ApDungTuNgay`, `ApDungDenNgay`) VALUES
(1, 'Phụ cấp thâm niên', 500000, '2024-01-01', NULL),
(1, 'Phụ cấp trách nhiệm', 300000, '2024-06-01', '2024-12-31'),
(2, 'Phụ cấp trách nhiệm', 200000, '2024-01-01', NULL),
(3, 'Phụ cấp ca đêm', 150000, '2024-03-01', NULL),
(4, 'Phụ cấp trách nhiệm', 250000, '2024-05-01', NULL);

INSERT INTO `khautru` (`MaNV`, `TenKhoanTru`, `SoTien`, `Ngay`, `GhiChu`) VALUES
(1, 'Bảo hiểm xã hội', 800000, '2024-09-30', 'Khấu trừ tháng 9'),                                                                              
(1, 'Bảo hiểm y tế', 200000, '2024-09-30', NULL),
(2, 'Bảo hiểm xã hội', 750000, '2024-09-30', NULL),                                                                              
(3, 'Bảo hiểm xã hội', 700000, '2024-09-30', NULL),
(4, 'Bảo hiểm xã hội', 650000, '2024-09-30', NULL);

INSERT INTO `thue` (`MaNV`, `TenThue`, `SoTien`, `ApDungTuNgay`, `ApDungDenNgay`) VALUES
(1, 'Thuế thu nhập cá nhân', 1200000, '2024-09-01', '2024-09-30'),
(2, 'Thuế thu nhập cá nhân', 800000, '2024-09-01', '2024-09-30'),
(3, 'Thuế thu nhập cá nhân', 1000000, '2024-09-01', '2024-09-30'),
(4, 'Thuế thu nhập cá nhân', 750000, '2024-09-01', '2024-09-30');

ALTER TABLE nhanvien
ADD CONSTRAINT fk_nv_pb FOREIGN KEY (MaPB)  REFERENCES phongban(MaPB) ON DELETE SET NULL ON UPDATE CASCADE;
