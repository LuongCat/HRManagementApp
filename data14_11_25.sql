CREATE DATABASE  IF NOT EXISTS `hrmanagement` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `hrmanagement`;
-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: localhost    Database: hrmanagement
-- ------------------------------------------------------
-- Server version	8.0.39

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `calam`
--

DROP TABLE IF EXISTS `calam`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `calam` (
  `MaCa` int NOT NULL AUTO_INCREMENT,
  `TenCa` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GioVao` time DEFAULT NULL,
  `GioRa` time DEFAULT NULL,
  `HeSoLuongCaLam` decimal(4,2) DEFAULT NULL,
  PRIMARY KEY (`MaCa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `calam`
--

LOCK TABLES `calam` WRITE;
/*!40000 ALTER TABLE `calam` DISABLE KEYS */;
/*!40000 ALTER TABLE `calam` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `calamviec`
--

DROP TABLE IF EXISTS `calamviec`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `calamviec`
--

LOCK TABLES `calamviec` WRITE;
/*!40000 ALTER TABLE `calamviec` DISABLE KEYS */;
/*!40000 ALTER TABLE `calamviec` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chamcong`
--

DROP TABLE IF EXISTS `chamcong`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chamcong`
--

LOCK TABLES `chamcong` WRITE;
/*!40000 ALTER TABLE `chamcong` DISABLE KEYS */;
INSERT INTO `chamcong` VALUES (61,1,'2024-09-01','2024-09-01 08:00:00',1),(62,1,'2024-09-02','2024-09-02 08:05:00',1),(63,1,'2024-09-03','2024-09-03 08:10:00',2),(64,1,'2024-09-04','2024-09-04 08:00:00',1),(65,2,'2024-09-01','2024-09-01 08:15:00',1),(66,2,'2024-09-02','2024-09-02 08:00:00',2),(67,2,'2024-09-03','2024-09-03 08:05:00',1),(68,2,'2024-09-04','2024-09-04 08:00:00',1),(69,3,'2024-09-01','2024-09-01 07:55:00',2),(70,3,'2024-09-02','2024-09-02 08:00:00',2),(71,3,'2024-09-03','2024-09-03 08:10:00',3),(72,3,'2024-09-04','2024-09-04 08:00:00',2),(73,4,'2024-09-01','2024-09-01 08:20:00',3),(74,4,'2024-09-02','2024-09-02 08:05:00',1),(75,4,'2024-09-03','2024-09-03 08:00:00',1),(76,4,'2024-09-04','2024-09-04 08:00:00',3);
/*!40000 ALTER TABLE `chamcong` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chamcong_lienketdon`
--

DROP TABLE IF EXISTS `chamcong_lienketdon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chamcong_lienketdon` (
  `MaCC` int NOT NULL,
  `MaDon` int NOT NULL,
  PRIMARY KEY (`MaCC`,`MaDon`),
  KEY `cclkd_ibfk_2` (`MaDon`),
  CONSTRAINT `cclkd_ibfk_1` FOREIGN KEY (`MaCC`) REFERENCES `chamcong` (`MaCC`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `cclkd_ibfk_2` FOREIGN KEY (`MaDon`) REFERENCES `dontu` (`MaDon`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chamcong_lienketdon`
--

LOCK TABLES `chamcong_lienketdon` WRITE;
/*!40000 ALTER TABLE `chamcong_lienketdon` DISABLE KEYS */;
/*!40000 ALTER TABLE `chamcong_lienketdon` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chucvu`
--

DROP TABLE IF EXISTS `chucvu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chucvu` (
  `MaCV` int NOT NULL AUTO_INCREMENT,
  `TenCV` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `PhuCap` decimal(18,2) DEFAULT NULL,
  `LuongCB` decimal(18,2) DEFAULT NULL,
  PRIMARY KEY (`MaCV`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chucvu`
--

LOCK TABLES `chucvu` WRITE;
/*!40000 ALTER TABLE `chucvu` DISABLE KEYS */;
INSERT INTO `chucvu` VALUES (1,'Nhân viên',1000000.00,8000000.00),(2,'Tổ trưởng',1500000.00,10000000.00),(3,'Trưởng phòng',3000000.00,15000000.00),(4,'Giám đốc',5000000.00,25000000.00);
/*!40000 ALTER TABLE `chucvu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `diadiemlamviec`
--

DROP TABLE IF EXISTS `diadiemlamviec`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `diadiemlamviec` (
  `MaDD` int NOT NULL AUTO_INCREMENT,
  `TenDiaDiem` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DiaChi` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `BanKinh` int DEFAULT NULL,
  PRIMARY KEY (`MaDD`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `diadiemlamviec`
--

LOCK TABLES `diadiemlamviec` WRITE;
/*!40000 ALTER TABLE `diadiemlamviec` DISABLE KEYS */;
INSERT INTO `diadiemlamviec` VALUES (1,'Văn phòng chính','123 Nguyễn Huệ, Quận 1, TP.HCM',50),(2,'Chi nhánh Hà Nội','45 Lý Thường Kiệt, Hoàn Kiếm, Hà Nội',60),(3,'Nhà máy Bình Dương','KCN VSIP 1, Thuận An, Bình Dương',80),(4,'Kho miền Tây','Số 12, Trần Hưng Đạo, Cần Thơ',70),(5,'Trung tâm R&D','200 Hoàng Quốc Việt, Cầu Giấy, Hà Nội',55);
/*!40000 ALTER TABLE `diadiemlamviec` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dontu`
--

DROP TABLE IF EXISTS `dontu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dontu`
--

LOCK TABLES `dontu` WRITE;
/*!40000 ALTER TABLE `dontu` DISABLE KEYS */;
/*!40000 ALTER TABLE `dontu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `loaidon`
--

DROP TABLE IF EXISTS `loaidon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `loaidon` (
  `MaLoaiDon` int NOT NULL AUTO_INCREMENT,
  `TenLoaiDon` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MoTa` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`MaLoaiDon`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `loaidon`
--

LOCK TABLES `loaidon` WRITE;
/*!40000 ALTER TABLE `loaidon` DISABLE KEYS */;
/*!40000 ALTER TABLE `loaidon` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `luong`
--

DROP TABLE IF EXISTS `luong`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `luong`
--

LOCK TABLES `luong` WRITE;
/*!40000 ALTER TABLE `luong` DISABLE KEYS */;
INSERT INTO `luong` VALUES (1,1,9,2024,26,18000000.00,NULL),(2,2,9,2024,24,11000000.00,NULL),(3,3,9,2024,27,14500000.00,NULL),(4,4,9,2024,25,10500000.00,NULL);
/*!40000 ALTER TABLE `luong` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nhanvien`
--

DROP TABLE IF EXISTS `nhanvien`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nhanvien`
--

LOCK TABLES `nhanvien` WRITE;
/*!40000 ALTER TABLE `nhanvien` DISABLE KEYS */;
INSERT INTO `nhanvien` VALUES (1,'Nguyễn Văn Ann','1990-05-15','012345678901','0901123456',1,2,'Nghỉ việc','2020-01-10',1),(2,'Trần Thị Bánh','1993-08-21','023456789012','0902345678',2,1,'Còn làm việc','2022-03-18',0),(3,'Lê Văn C','1988-12-05','034567890123','0903456789',2,2,'Còn làm việc','2019-06-01',1),(4,'Phạm Thị D','1995-11-30','045678901234','0904567890',3,1,'Còn làm việc','2022-02-25',0);
/*!40000 ALTER TABLE `nhanvien` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nhanvien_chucvu`
--

DROP TABLE IF EXISTS `nhanvien_chucvu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nhanvien_chucvu`
--

LOCK TABLES `nhanvien_chucvu` WRITE;
/*!40000 ALTER TABLE `nhanvien_chucvu` DISABLE KEYS */;
INSERT INTO `nhanvien_chucvu` VALUES (1,2,'2024-02-01',NULL,1,NULL,NULL,'Chính thức',''),(1,3,'2023-01-01',NULL,1,NULL,NULL,'Chính thức',''),(2,3,'2024-07-01','2025-01-01',0,0.1500,1500000.00,'Kiêm nhiệm','');
/*!40000 ALTER TABLE `nhanvien_chucvu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nhanvien_phongban`
--

DROP TABLE IF EXISTS `nhanvien_phongban`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nhanvien_phongban`
--

LOCK TABLES `nhanvien_phongban` WRITE;
/*!40000 ALTER TABLE `nhanvien_phongban` DISABLE KEYS */;
INSERT INTO `nhanvien_phongban` VALUES (1,1,50.00,'2023-01-15','2024-06-30'),(1,2,100.00,'2024-03-01',NULL);
/*!40000 ALTER TABLE `nhanvien_phongban` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `phamviquanly`
--

DROP TABLE IF EXISTS `phamviquanly`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `phamviquanly`
--

LOCK TABLES `phamviquanly` WRITE;
/*!40000 ALTER TABLE `phamviquanly` DISABLE KEYS */;
/*!40000 ALTER TABLE `phamviquanly` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `phongban`
--

DROP TABLE IF EXISTS `phongban`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `phongban` (
  `MaPB` int NOT NULL AUTO_INCREMENT,
  `TenPB` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MoTa` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `MaTRuongPhong` int DEFAULT NULL,
  PRIMARY KEY (`MaPB`),
  KEY `FK_Nhanvien_matruongphong` (`MaTRuongPhong`),
  CONSTRAINT `FK_Nhanvien_matruongphong` FOREIGN KEY (`MaTRuongPhong`) REFERENCES `nhanvien` (`MaNV`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `phongban`
--

LOCK TABLES `phongban` WRITE;
/*!40000 ALTER TABLE `phongban` DISABLE KEYS */;
INSERT INTO `phongban` VALUES (1,'Phòng Nhân Sự','Quản lý nhân sự và tuyển dụng',1),(2,'Phòng Kỹ Thuật','Phát triển và bảo trì hệ thống',NULL),(3,'Phòng Kinh Doanh','Tư vấn và bán hàng',NULL),(4,'Phòng Kế Toán','Quản lý tài chính và thu chi',NULL);
/*!40000 ALTER TABLE `phongban` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `quyenhan`
--

DROP TABLE IF EXISTS `quyenhan`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `quyenhan` (
  `MaQuyen` int NOT NULL AUTO_INCREMENT,
  `TenQuyen` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `MoTa` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`MaQuyen`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `quyenhan`
--

LOCK TABLES `quyenhan` WRITE;
/*!40000 ALTER TABLE `quyenhan` DISABLE KEYS */;
INSERT INTO `quyenhan` VALUES (1,'DuyetDon','Quyền duyệt đơn'),(2,'XemBaoCao','Quyền xem báo cáo'),(3,'QuanLyNhanVien','Quyền quản lý nhân viên');
/*!40000 ALTER TABLE `quyenhan` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `taikhoan`
--

DROP TABLE IF EXISTS `taikhoan`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `taikhoan`
--

LOCK TABLES `taikhoan` WRITE;
/*!40000 ALTER TABLE `taikhoan` DISABLE KEYS */;
/*!40000 ALTER TABLE `taikhoan` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `taikhoan_vaitro`
--

DROP TABLE IF EXISTS `taikhoan_vaitro`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `taikhoan_vaitro` (
  `MaTK` int NOT NULL,
  `MaVaiTro` int NOT NULL,
  PRIMARY KEY (`MaTK`,`MaVaiTro`),
  KEY `tk_vt_ibfk_2` (`MaVaiTro`),
  CONSTRAINT `tk_vt_ibfk_1` FOREIGN KEY (`MaTK`) REFERENCES `taikhoan` (`MaTK`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `tk_vt_ibfk_2` FOREIGN KEY (`MaVaiTro`) REFERENCES `vaitro` (`MaVaiTro`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `taikhoan_vaitro`
--

LOCK TABLES `taikhoan_vaitro` WRITE;
/*!40000 ALTER TABLE `taikhoan_vaitro` DISABLE KEYS */;
/*!40000 ALTER TABLE `taikhoan_vaitro` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `uyquyen`
--

DROP TABLE IF EXISTS `uyquyen`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `uyquyen`
--

LOCK TABLES `uyquyen` WRITE;
/*!40000 ALTER TABLE `uyquyen` DISABLE KEYS */;
/*!40000 ALTER TABLE `uyquyen` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vaitro`
--

DROP TABLE IF EXISTS `vaitro`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vaitro` (
  `MaVaiTro` int NOT NULL AUTO_INCREMENT,
  `TenVaiTro` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `MoTa` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`MaVaiTro`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vaitro`
--

LOCK TABLES `vaitro` WRITE;
/*!40000 ALTER TABLE `vaitro` DISABLE KEYS */;
INSERT INTO `vaitro` VALUES (1,'Admin','Quản trị hệ thống'),(2,'QuanLy','Quản lý bộ phận'),(3,'NhanVien','Nhân viên');
/*!40000 ALTER TABLE `vaitro` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vaitro_quyenhan`
--

DROP TABLE IF EXISTS `vaitro_quyenhan`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vaitro_quyenhan` (
  `MaVaiTro` int NOT NULL,
  `MaQuyenHan` int NOT NULL,
  PRIMARY KEY (`MaVaiTro`,`MaQuyenHan`),
  KEY `vtr_qh_ibfk_2` (`MaQuyenHan`),
  CONSTRAINT `vtr_qh_ibfk_1` FOREIGN KEY (`MaVaiTro`) REFERENCES `vaitro` (`MaVaiTro`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `vtr_qh_ibfk_2` FOREIGN KEY (`MaQuyenHan`) REFERENCES `quyenhan` (`MaQuyen`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vaitro_quyenhan`
--

LOCK TABLES `vaitro_quyenhan` WRITE;
/*!40000 ALTER TABLE `vaitro_quyenhan` DISABLE KEYS */;
/*!40000 ALTER TABLE `vaitro_quyenhan` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-11-14 13:19:18
