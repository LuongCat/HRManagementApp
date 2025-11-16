DROP DATABASE IF EXISTS `hrmanagement`;
CREATE DATABASE `hrmanagement`;
USE `hrmanagement`;

--
-- Table structure for table `calam`
--

CREATE TABLE `calam` (
  `MaCa` int NOT NULL AUTO_INCREMENT,
  `TenCa` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GioVao` time DEFAULT NULL,
  `GioRa` time DEFAULT NULL,
  `HeSoLuongCaLam` decimal(4,2) DEFAULT NULL,
  PRIMARY KEY (`MaCa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `calam`
--



--
-- Table structure for table `calamviec`
--

CREATE TABLE `calamviec` (
  `MaLich` int NOT NULL AUTO_INCREMENT,
  `MaNV` int NOT NULL,
  `MaDD` int DEFAULT NULL,
  `NgayBatDau` date DEFAULT NULL,
  `NgayKetThuc` date DEFAULT NULL,
  `CaLam` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`MaLich`),
  KEY `clv_MaNV` (`MaNV`),
  KEY `clv_MaDD` (`MaDD`),
  CONSTRAINT `calamviec_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `calamviec_ibfk_2` FOREIGN KEY (`MaDD`) REFERENCES `diadiemlamviec` (`MaDD`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `calamviec`
--



--
-- Table structure for table `chamcong`
--

CREATE TABLE `chamcong` (
  `MaCC` int NOT NULL AUTO_INCREMENT,
  `MaNV` int NOT NULL,
  `Ngay` date NOT NULL,
  `GioCC` datetime DEFAULT NULL,
  `MaDD` int DEFAULT NULL,
  PRIMARY KEY (`MaCC`),
  KEY `MaNV` (`MaNV`),
  KEY `MaDD` (`MaDD`),
  CONSTRAINT `chamcong_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `chamcong_ibfk_2` FOREIGN KEY (`MaDD`) REFERENCES `diadiemlamviec` (`MaDD`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=81 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `chamcong`
--

INSERT INTO `chamcong` VALUES 
(61,1,'2024-09-01','2024-09-01 08:00:00',1),
(62,1,'2024-09-02','2024-09-02 08:05:00',1),
(63,1,'2024-09-03','2024-09-03 08:10:00',2),
(64,1,'2024-09-04','2024-09-04 08:00:00',1),
(65,2,'2024-09-01','2024-09-01 08:15:00',1),
(66,2,'2024-09-02','2024-09-02 08:00:00',2),
(67,2,'2024-09-03','2024-09-03 08:05:00',1),
(68,2,'2024-09-04','2024-09-04 08:00:00',1),
(69,3,'2024-09-01','2024-09-01 07:55:00',2),
(70,3,'2024-09-02','2024-09-02 08:00:00',2),
(71,3,'2024-09-03','2024-09-03 08:10:00',3),
(72,3,'2024-09-04','2024-09-04 08:00:00',2),
(73,4,'2024-09-01','2024-09-01 08:20:00',3),
(74,4,'2024-09-02','2024-09-02 08:05:00',1),
(75,4,'2024-09-03','2024-09-03 08:00:00',1),
(76,4,'2024-09-04','2024-09-04 08:00:00',3);

--
-- Table structure for table `chamcong_lienketdon`
--

CREATE TABLE `chamcong_lienketdon` (
  `MaCC` int NOT NULL,
  `MaDon` int NOT NULL,
  PRIMARY KEY (`MaCC`,`MaDon`),
  KEY `cclkd_ibfk_2` (`MaDon`),
  CONSTRAINT `cclkd_ibfk_1` FOREIGN KEY (`MaCC`) REFERENCES `chamcong` (`MaCC`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `cclkd_ibfk_2` FOREIGN KEY (`MaDon`) REFERENCES `dontu` (`MaDon`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `chamcong_lienketdon`
--



--
-- Table structure for table `chucvu`
--

CREATE TABLE `chucvu` (
  `MaCV` int NOT NULL AUTO_INCREMENT,
  `TenCV` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `PhuCap` decimal(18,2) DEFAULT NULL,
  `LuongCB` decimal(18,2) DEFAULT NULL,
  PRIMARY KEY (`MaCV`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `chucvu`
--

INSERT INTO `chucvu` VALUES 
(1,'Nhân viên',1000000.00,8000000.00),
(2,'Tổ trưởng',1500000.00,10000000.00),
(3,'Trưởng phòng',3000000.00,15000000.00),
(4,'Giám đốc',5000000.00,25000000.00);

--
-- Table structure for table `diadiemlamviec`
--

CREATE TABLE `diadiemlamviec` (
  `MaDD` int NOT NULL AUTO_INCREMENT,
  `TenDiaDiem` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DiaChi` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BanKinh` int DEFAULT NULL,
  PRIMARY KEY (`MaDD`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO `diadiemlamviec` VALUES 
(1,'Văn phòng chính','123 Nguyễn Huệ, Quận 1, TP.HCM',50),
(2,'Chi nhánh Hà Nội','45 Lý Thường Kiệt, Hoàn Kiếm, Hà Nội',60),
(3,'Nhà máy Bình Dương','KCN VSIP 1, Thuận An, Bình Dương',80),
(4,'Kho miền Tây','Số 12, Trần Hưng Đạo, Cần Thơ',70),
(5,'Trung tâm R&D','200 Hoàng Quốc Việt, Cầu Giấy, Hà Nội',55);

--
-- Table structure for table `dontu`
--

CREATE TABLE `dontu` (
  `MaDon` int NOT NULL AUTO_INCREMENT,
  `MaNV` int NOT NULL,
  `MaLoaiDon` int DEFAULT NULL,
  `NgayBatDau` datetime DEFAULT NULL,
  `NgayKetThuc` datetime DEFAULT NULL,
  `LyDo` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThai` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NgayGui` date DEFAULT NULL,
  `NguoiDuyet` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`MaDon`),
  KEY `MaNV` (`MaNV`),
  KEY `MaLoaiDon` (`MaLoaiDon`),
  CONSTRAINT `dontu_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `dontu_ibfk_2` FOREIGN KEY (`MaLoaiDon`) REFERENCES `loaidon` (`MaLoaiDon`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `dontu`
--



--
-- Table structure for table `loaidon`
--

CREATE TABLE `loaidon` (
  `MaLoaiDon` int NOT NULL AUTO_INCREMENT,
  `TenLoaiDon` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MoTa` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`MaLoaiDon`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `loaidon`
--



--
-- Table structure for table `luong`
--

CREATE TABLE `luong` (
  `MaLuong` int NOT NULL AUTO_INCREMENT,
  `MaNV` int NOT NULL,
  `Thang` int NOT NULL,
  `Nam` int NOT NULL,
  `TongNgayCong` int DEFAULT NULL,
  `LuongThucNhan` decimal(18,2) DEFAULT NULL,
  `TrangThai` char(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`MaLuong`),
  KEY `MaNV` (`MaNV`),
  CONSTRAINT `luong_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `luong`
--

INSERT INTO `luong` VALUES 
(1,1,9,2024,26,18000000.00,NULL),
(2,2,9,2024,24,11000000.00,NULL),
(3,3,9,2024,27,14500000.00,NULL),
(4,4,9,2024,25,10500000.00,NULL);

--
-- Table structure for table `nhanvien`
--

CREATE TABLE `nhanvien` (
  `MaNV` int NOT NULL AUTO_INCREMENT,
  `HoTen` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NgaySinh` date DEFAULT NULL,
  `SoCCCD` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DienThoai` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `MaPB` int DEFAULT NULL,
  `MaCV` int DEFAULT NULL,
  `TrangThai` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NgayVaoLam` date DEFAULT NULL,
  `GioiTinh` int DEFAULT NULL,
  PRIMARY KEY (`MaNV`),
  UNIQUE KEY `SoCCCD` (`SoCCCD`),
  KEY `MaPB` (`MaPB`),
  KEY `MaCV` (`MaCV`),
  CONSTRAINT `nhanvien_ibfk_1` FOREIGN KEY (`MaPB`) REFERENCES `phongban` (`MaPB`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `nhanvien_ibfk_2` FOREIGN KEY (`MaCV`) REFERENCES `chucvu` (`MaCV`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `nhanvien`
--

INSERT INTO `nhanvien` VALUES 
(1,'Nguyễn Văn Ann','1990-05-15','012345678901','0901123456',1,2,'Nghỉ việc','2020-01-10',1),
(2,'Trần Thị Bánh','1993-08-21','023456789012','0902345678',2,1,'Còn làm việc','2022-03-18',0),
(3,'Lê Văn C','1988-12-05','034567890123','0903456789',2,2,'Còn làm việc','2019-06-01',1),
(4,'Phạm Thị D','1995-11-30','045678901234','0904567890',3,1,'Còn làm việc','2022-02-25',0);

--
-- Table structure for table `nhanvien_chucvu`
--

CREATE TABLE `nhanvien_chucvu` (
  `MaNV` int NOT NULL,
  `MaCV` int NOT NULL,
  `NgayBatDau` date NOT NULL,
  `NgayKetThuc` date DEFAULT NULL,
  `LaChucVuChinh` tinyint(1) DEFAULT '1',
  `HeSoPhuCapKiemNhiem` decimal(6,4) DEFAULT NULL,
  `TienPhuCapKiemNhiem` decimal(18,2) DEFAULT NULL,
  `LoaiChucVu` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GhiChu` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`MaNV`,`MaCV`,`NgayBatDau`),
  KEY `nv_cv_fk_cv` (`MaCV`),
  CONSTRAINT `nv_cv_fk_cv` FOREIGN KEY (`MaCV`) REFERENCES `chucvu` (`MaCV`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `nv_cv_fk_nv` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `nhanvien_chucvu`
--

INSERT INTO `nhanvien_chucvu` VALUES 
(1,2,'2024-02-01',NULL,1,NULL,NULL,'Chính thức',''),
(1,3,'2023-01-01',NULL,1,NULL,NULL,'Chính thức',''),
(2,3,'2024-07-01','2025-01-01',0,0.1500,1500000.00,'Kiêm nhiệm','');

--
-- Table structure for table `nhanvien_phongban`
--

CREATE TABLE `nhanvien_phongban` (
  `MaNV` int NOT NULL,
  `MaPB` int NOT NULL,
  `TyLePhanBo` decimal(5,2) DEFAULT NULL,
  `NgayBatDau` date NOT NULL,
  `NgayKetThuc` date DEFAULT NULL,
  PRIMARY KEY (`MaNV`,`MaPB`,`NgayBatDau`),
  KEY `nv_pb_fk_pb` (`MaPB`),
  CONSTRAINT `nv_pb_fk_nv` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `nv_pb_fk_pb` FOREIGN KEY (`MaPB`) REFERENCES `phongban` (`MaPB`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `nhanvien_phongban`
--

INSERT INTO `nhanvien_phongban` VALUES 
(1,1,50.00,'2023-01-15','2024-06-30'),
(1,2,100.00,'2024-03-01',NULL);

--
-- Table structure for table `phamviquanly`
--

CREATE TABLE `phamviquanly` (
  `MaPhamVi` int NOT NULL AUTO_INCREMENT,
  `MaNVQuanLy` int NOT NULL,
  `MaPB` int NOT NULL,
  `TuNgay` date DEFAULT NULL,
  `DenNgay` date DEFAULT NULL,
  PRIMARY KEY (`MaPhamVi`),
  KEY `pvq_MaNVQuanLy` (`MaNVQuanLy`),
  KEY `pvq_MaPB` (`MaPB`),
  CONSTRAINT `phamvi_ibfk_1` FOREIGN KEY (`MaNVQuanLy`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `phamvi_ibfk_2` FOREIGN KEY (`MaPB`) REFERENCES `phongban` (`MaPB`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `phamviquanly`
--



--
-- Table structure for table `phongban`
--

CREATE TABLE `phongban` (
  `MaPB` int NOT NULL AUTO_INCREMENT,
  `TenPB` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MoTa` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `MaTRuongPhong` int DEFAULT NULL,
  PRIMARY KEY (`MaPB`),
  KEY `FK_Nhanvien_matruongphong` (`MaTRuongPhong`),
  CONSTRAINT `FK_Nhanvien_matruongphong` FOREIGN KEY (`MaTRuongPhong`) REFERENCES `nhanvien` (`MaNV`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `phongban`
--

INSERT INTO `phongban` VALUES 
(1,'Phòng Nhân Sự','Quản lý nhân sự và tuyển dụng',1),
(2,'Phòng Kỹ Thuật','Phát triển và bảo trì hệ thống',NULL),
(3,'Phòng Kinh Doanh','Tư vấn và bán hàng',NULL),
(4,'Phòng Kế Toán','Quản lý tài chính và thu chi',NULL);

--
-- Table structure for table `quyenhan`
--

CREATE TABLE `quyenhan` (
  `MaQuyen` int NOT NULL AUTO_INCREMENT,
  `TenQuyen` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `MoTa` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`MaQuyen`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `quyenhan`
--

INSERT INTO `quyenhan` VALUES 
(1,'DuyetDon','Quyền duyệt đơn'),
(2,'XemBaoCao','Quyền xem báo cáo'),
(3,'QuanLyNhanVien','Quyền quản lý nhân viên');

--
-- Table structure for table `taikhoan`
--

CREATE TABLE `taikhoan` (
  `MaTK` int NOT NULL AUTO_INCREMENT,
  `MaNV` int DEFAULT NULL,
  `TenDangNhap` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MatKhau` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `VaiTro` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThai` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`MaTK`),
  UNIQUE KEY `TenDangNhap` (`TenDangNhap`),
  KEY `MaNV` (`MaNV`),
  CONSTRAINT `taikhoan_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `taikhoan`
--



--
-- Table structure for table `taikhoan_vaitro`
--

CREATE TABLE `taikhoan_vaitro` (
  `MaTK` int NOT NULL,
  `MaVaiTro` int NOT NULL,
  PRIMARY KEY (`MaTK`,`MaVaiTro`),
  KEY `tk_vt_ibfk_2` (`MaVaiTro`),
  CONSTRAINT `tk_vt_ibfk_1` FOREIGN KEY (`MaTK`) REFERENCES `taikhoan` (`MaTK`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `tk_vt_ibfk_2` FOREIGN KEY (`MaVaiTro`) REFERENCES `vaitro` (`MaVaiTro`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `taikhoan_vaitro`
--



--
-- Table structure for table `uyquyen`
--

CREATE TABLE `uyquyen` (
  `MaUyQuyen` int NOT NULL AUTO_INCREMENT,
  `NguoiUyQuyen` int NOT NULL,
  `NguoiDuocUyQuyen` int NOT NULL,
  `TuNgay` date DEFAULT NULL,
  `DenNgay` date DEFAULT NULL,
  `LyDo` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`MaUyQuyen`),
  KEY `uyq_nguo` (`NguoiUyQuyen`),
  KEY `uyq_duoc` (`NguoiDuocUyQuyen`),
  CONSTRAINT `uyquyen_ibfk_1` FOREIGN KEY (`NguoiUyQuyen`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `uyquyen_ibfk_2` FOREIGN KEY (`NguoiDuocUyQuyen`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `uyquyen`
--



--
-- Table structure for table `vaitro`
--


CREATE TABLE `vaitro` (
  `MaVaiTro` int NOT NULL AUTO_INCREMENT,
  `TenVaiTro` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `MoTa` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`MaVaiTro`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `vaitro`
--

INSERT INTO `vaitro` VALUES 
(1,'Admin','Quản trị hệ thống'),
(2,'QuanLy','Quản lý bộ phận'),
(3,'NhanVien','Nhân viên');

--
-- Table structure for table `vaitro_quyenhan`
--

CREATE TABLE `vaitro_quyenhan` (
  `MaVaiTro` int NOT NULL,
  `MaQuyenHan` int NOT NULL,
  PRIMARY KEY (`MaVaiTro`,`MaQuyenHan`),
  KEY `vtr_qh_ibfk_2` (`MaQuyenHan`),
  CONSTRAINT `vtr_qh_ibfk_1` FOREIGN KEY (`MaVaiTro`) REFERENCES `vaitro` (`MaVaiTro`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `vtr_qh_ibfk_2` FOREIGN KEY (`MaQuyenHan`) REFERENCES `quyenhan` (`MaQuyen`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `vaitro_quyenhan`
--


