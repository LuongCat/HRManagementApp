CREATE DATABASE  IF NOT EXISTS `hrmanagement` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `hrmanagement`;
-- MySQL dump 10.13  Distrib 8.0.40, for Win64 (x86_64)
--
-- Host: localhost    Database: hrmanagement
-- ------------------------------------------------------
-- Server version	8.0.35

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
  `TenCa` varchar(20) NOT NULL,
  `GioBatDau` time DEFAULT '00:00:00',
  `GioKetThuc` time DEFAULT '00:00:00',
  `TrangThai` enum('Hoạt động','Tạm ngừng','Đã xóa') NOT NULL DEFAULT 'Hoạt động',
  PRIMARY KEY (`MaCa`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `calam`
--

LOCK TABLES `calam` WRITE;
/*!40000 ALTER TABLE `calam` DISABLE KEYS */;
INSERT INTO `calam` VALUES (1,'Ca 1','08:00:00','11:30:00','Hoạt động');
/*!40000 ALTER TABLE `calam` ENABLE KEYS */;
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
  `Ngay` date NOT NULL DEFAULT (curdate()),
  `GioVao` time DEFAULT (curtime()),
  `GioRa` time DEFAULT NULL,
  `ThoiGianLam` time GENERATED ALWAYS AS (timediff(`GioRa`,`GioVao`)) VIRTUAL,
  PRIMARY KEY (`MaCC`),
  KEY `MaNV` (`MaNV`),
  CONSTRAINT `chamcong_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chamcong`
--

LOCK TABLES `chamcong` WRITE;
/*!40000 ALTER TABLE `chamcong` DISABLE KEYS */;
INSERT INTO `chamcong` (`MaCC`, `MaNV`, `Ngay`, `GioVao`, `GioRa`) VALUES (1,2,'2025-12-06','07:55:00',NULL),(2,1,'2025-12-06','08:00:00','17:30:00'),(3,1,'2025-12-06','07:50:00','17:35:00'),(4,1,'2025-12-10','08:05:00','17:30:00'),(5,1,'2025-12-06','08:00:00','12:00:00'),(6,1,'2025-09-08','07:45:00','17:30:00'),(7,1,'2025-09-09','08:00:00','17:45:00'),(8,1,'2025-09-10','07:58:00','17:30:00'),(9,1,'2025-09-11','08:02:00','17:30:00'),(10,1,'2025-09-13','08:00:00','12:15:00'),(11,1,'2025-09-15','07:50:00','17:30:00'),(12,1,'2025-09-16','08:00:00','17:30:00'),(13,1,'2025-09-17','07:55:00','17:40:00'),(14,6,'2025-12-05','08:10:00','17:30:00'),(15,1,'2025-09-19','07:55:00','17:30:00'),(16,1,'2025-09-20','08:00:00','12:00:00'),(17,1,'2025-09-22','07:45:00','17:30:00'),(18,1,'2025-09-23','08:00:00','17:30:00'),(19,1,'2025-09-24','07:59:00','17:30:00'),(20,1,'2025-09-26','08:00:00','17:30:00'),(21,1,'2025-09-27','08:00:00','12:00:00'),(22,1,'2025-09-29','07:50:00','17:30:00'),(23,1,'2025-09-30','08:00:00','17:30:00');
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
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
  `TenCV` varchar(100) NOT NULL,
  `PhuCap` decimal(18,2) DEFAULT '0.00',
  `LuongCB` decimal(18,2) DEFAULT '0.00',
  `TienPhuCapKiemNhiem` decimal(18,2) DEFAULT '0.00',
  `HoatDong` enum('Active','inActive') DEFAULT 'Active',
  PRIMARY KEY (`MaCV`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chucvu`
--

LOCK TABLES `chucvu` WRITE;
/*!40000 ALTER TABLE `chucvu` DISABLE KEYS */;
INSERT INTO `chucvu` VALUES (1,'Nhân viên',1000000.00,8000000.00,0.00,'Active'),(2,'Tổ trưởng',1500000.00,10000000.00,10000.00,'Active'),(3,'Trưởng phòng',3000000.00,15000000.00,10000.00,'Active'),(4,'Giám đốc',5000000.00,25000000.00,20000.00,'Active');
/*!40000 ALTER TABLE `chucvu` ENABLE KEYS */;
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
  `NgayBatDau` datetime DEFAULT (now()),
  `NgayKetThuc` datetime DEFAULT (now()),
  `LyDo` varchar(200) DEFAULT NULL,
  `TrangThai` enum('Chưa duyệt','Đã duyệt','Từ chối') DEFAULT 'Chưa duyệt',
  `NgayGui` datetime DEFAULT (now()),
  `NguoiDuyet` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`MaDon`),
  KEY `MaNV` (`MaNV`),
  KEY `MaLoaiDon` (`MaLoaiDon`),
  CONSTRAINT `dontu_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `dontu_ibfk_2` FOREIGN KEY (`MaLoaiDon`) REFERENCES `loaidon` (`MaLoaiDon`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dontu`
--

LOCK TABLES `dontu` WRITE;
/*!40000 ALTER TABLE `dontu` DISABLE KEYS */;
INSERT INTO `dontu` VALUES (1,1,1,'2024-09-01 08:00:00','2024-10-02 17:00:00','Về quê thăm gia đình','Đã duyệt','2025-12-05 14:43:19','Trần Thị Bánh'),(2,2,2,'2024-10-05 08:00:00','2024-10-05 17:00:00','Sốt cao','Chưa duyệt','2025-12-05 14:43:19',NULL),(3,3,5,'2024-09-10 08:00:00','2024-10-12 17:00:00','Gặp khách hàng miền Bắc','Đã duyệt','2025-12-05 14:43:19','Giám đốc'),(4,4,4,'2024-10-15 08:00:00','2024-10-15 12:00:00','Đi họp phụ huynh','Từ chối','2025-12-05 14:43:19','Trần Thị Bánh'),(5,1,2,'2024-09-20 08:00:00','2024-10-20 17:00:00','Đau bụng','Chưa duyệt','2025-12-05 14:43:19',NULL);
/*!40000 ALTER TABLE `dontu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `khautru`
--

DROP TABLE IF EXISTS `khautru`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `khautru` (
  `MaKT` int NOT NULL AUTO_INCREMENT,
  `MaNV` int NOT NULL,
  `TenKhoanTru` varchar(100) NOT NULL,
  `SoTien` decimal(18,2) NOT NULL DEFAULT '0.00',
  `Ngay` date NOT NULL DEFAULT (curdate()),
  `GhiChu` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`MaKT`),
  KEY `fk_khautru_nv` (`MaNV`),
  CONSTRAINT `fk_khautru_nv` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `khautru`
--

LOCK TABLES `khautru` WRITE;
/*!40000 ALTER TABLE `khautru` DISABLE KEYS */;
INSERT INTO `khautru` VALUES (1,1,'Bảo hiểm xã hội',800000.00,'2024-09-30','Khấu trừ tháng 9'),(2,1,'Bảo hiểm y tế',200000.00,'2024-09-30',NULL),(3,2,'Bảo hiểm xã hội',750000.00,'2024-09-30',NULL),(4,3,'Bảo hiểm xã hội',700000.00,'2024-09-30',NULL),(5,4,'Bảo hiểm xã hội',650000.00,'2024-09-30',NULL),(6,5,'BHXH + BHYT + BHTN',1050000.00,'2024-10-31','Khấu trừ theo lương đóng BH'),(7,6,'BHXH + BHYT + BHTN',840000.00,'2024-10-31',NULL),(8,7,'BHXH + BHYT + BHTN',1260000.00,'2024-10-31',NULL),(9,10,'BHXH + BHYT + BHTN',2100000.00,'2024-10-31',NULL),(10,1,'BHXH Tháng 10',800000.00,'2024-10-31',NULL);
/*!40000 ALTER TABLE `khautru` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `loaidon`
--

DROP TABLE IF EXISTS `loaidon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `loaidon` (
  `MaLoaiDon` int NOT NULL AUTO_INCREMENT,
  `TenLoaiDon` varchar(100) NOT NULL,
  `MoTa` varchar(50) DEFAULT NULL,
  `CoLuong` enum('Yes','No') DEFAULT 'Yes',
  PRIMARY KEY (`MaLoaiDon`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `loaidon`
--

LOCK TABLES `loaidon` WRITE;
/*!40000 ALTER TABLE `loaidon` DISABLE KEYS */;
INSERT INTO `loaidon` VALUES (1,'Nghỉ phép năm','Nghỉ hưởng lương theo quy định năm','Yes'),(2,'Nghỉ ốm','Nghỉ có giấy xác nhận của bác sĩ','Yes'),(3,'Nghỉ thai sản','Nghỉ sinh con theo chế độ BHXH','Yes'),(4,'Nghỉ không lương','Nghỉ việc riêng tự nguyện không hưởng lương','Yes'),(5,'Công tác','Đi công tác bên ngoài công ty','Yes');
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
  `TongNgayCong` int DEFAULT '0',
  `TienLuong` decimal(18,2) DEFAULT '0.00',
  `LuongThucNhan` decimal(18,2) DEFAULT '0.00',
  `TrangThai` enum('Chưa trả','Đã trả') DEFAULT 'Chưa trả',
  PRIMARY KEY (`MaLuong`),
  KEY `MaNV` (`MaNV`),
  CONSTRAINT `luong_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `luong`
--

LOCK TABLES `luong` WRITE;
/*!40000 ALTER TABLE `luong` DISABLE KEYS */;
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
  `HoTen` varchar(100) NOT NULL,
  `NgaySinh` date DEFAULT NULL,
  `SoCCCD` varchar(20) DEFAULT NULL,
  `DienThoai` varchar(15) DEFAULT NULL,
  `TrangThai` enum('Còn làm việc','Nghỉ việc') DEFAULT 'Còn làm việc',
  `NgayVaoLam` date DEFAULT (curdate()),
  `GioiTinh` enum('Nam','Nữ') DEFAULT 'Nam',
  `MaPB` int DEFAULT NULL,
  PRIMARY KEY (`MaNV`),
  UNIQUE KEY `SoCCCD` (`SoCCCD`),
  KEY `fk_nv_pb` (`MaPB`),
  CONSTRAINT `fk_nv_pb` FOREIGN KEY (`MaPB`) REFERENCES `phongban` (`MaPB`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nhanvien`
--

LOCK TABLES `nhanvien` WRITE;
/*!40000 ALTER TABLE `nhanvien` DISABLE KEYS */;
INSERT INTO `nhanvien` VALUES (1,'Nguyễn Văn Ann','1990-05-15','012345678901','0901123456','Nghỉ việc','2020-01-10','Nam',1),(2,'Trần Thị Bánh','1993-08-21','023456789012','0902345678','Còn làm việc','2022-03-18','Nữ',2),(3,'Lê Văn C','1988-12-05','034567890123','0903456789','Còn làm việc','2019-06-01','Nam',3),(4,'Phạm Thị D','1995-11-30','045678901234','0904567890','Còn làm việc','2022-02-25','Nữ',1),(5,'Hoàng Văn Em','1996-01-10','056789012345','0905678901','Còn làm việc','2023-05-12','Nam',2),(6,'Ngô Thị Én','1998-09-09','067890123456','0906789012','Còn làm việc','2023-08-01','Nữ',1),(7,'Bùi Văn Giao','1992-04-22','078901234567','0907890123','Còn làm việc','2021-11-15','Nam',4),(8,'Đỗ Thị Hoa','1994-07-14','089012345678','0908901234','Còn làm việc','2022-01-20','Nữ',1),(9,'Lý Văn Ích','1999-12-12','090123456789','0909012345','Còn làm việc','2024-02-01','Nam',2),(10,'Phạm Văn Khôi','1985-03-30','099887766554','0909988776','Còn làm việc','2018-10-10','Nam',4);
/*!40000 ALTER TABLE `nhanvien` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nhanvien_calam`
--

DROP TABLE IF EXISTS `nhanvien_calam`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nhanvien_calam` (
  `MaNV` int NOT NULL,
  `MaCa` int NOT NULL,
  PRIMARY KEY (`MaNV`,`MaCa`),
  KEY `MaCa` (`MaCa`),
  CONSTRAINT `nhanvien_calam_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `nhanvien_calam_ibfk_2` FOREIGN KEY (`MaCa`) REFERENCES `calam` (`MaCa`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nhanvien_calam`
--

LOCK TABLES `nhanvien_calam` WRITE;
/*!40000 ALTER TABLE `nhanvien_calam` DISABLE KEYS */;
INSERT INTO `nhanvien_calam` VALUES (1,1),(2,1),(6,1);
/*!40000 ALTER TABLE `nhanvien_calam` ENABLE KEYS */;
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
  `HeSoLuongCoban` decimal(6,4) DEFAULT '1.0000',
  `HeSoPhuCapKiemNhiem` decimal(6,4) DEFAULT '0.0000',
  `LoaiChucVu` enum('Chính thức','Kiêm nhiệm') DEFAULT 'Chính thức',
  `GhiChu` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`MaNV`,`MaCV`),
  KEY `nv_cv_fk_cv` (`MaCV`),
  CONSTRAINT `nv_cv_fk_cv` FOREIGN KEY (`MaCV`) REFERENCES `chucvu` (`MaCV`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `nv_cv_fk_nv` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nhanvien_chucvu`
--

LOCK TABLES `nhanvien_chucvu` WRITE;
/*!40000 ALTER TABLE `nhanvien_chucvu` DISABLE KEYS */;
INSERT INTO `nhanvien_chucvu` VALUES (1,2,1.0000,0.0000,'Chính thức',''),(2,3,0.0000,0.1500,'Kiêm nhiệm','');
/*!40000 ALTER TABLE `nhanvien_chucvu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `phongban`
--

DROP TABLE IF EXISTS `phongban`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `phongban` (
  `MaPB` int NOT NULL AUTO_INCREMENT,
  `TenPB` varchar(100) NOT NULL,
  `MoTa` varchar(200) DEFAULT NULL,
  `MaTruongPhong` int DEFAULT NULL,
  PRIMARY KEY (`MaPB`),
  KEY `fk_pb_truongphong` (`MaTruongPhong`),
  CONSTRAINT `fk_pb_truongphong` FOREIGN KEY (`MaTruongPhong`) REFERENCES `nhanvien` (`MaNV`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `phongban`
--

LOCK TABLES `phongban` WRITE;
/*!40000 ALTER TABLE `phongban` DISABLE KEYS */;
INSERT INTO `phongban` VALUES (1,'Phòng Nhân Sự','Quản lý nhân sự và tuyển dụng',NULL),(2,'Phòng Kỹ Thuật','Phát triển và bảo trì hệ thống',NULL),(3,'Phòng Kinh Doanh','Tư vấn và bán hàng',NULL),(4,'Phòng Kế Toán','Quản lý tài chính và thu chi',NULL);
/*!40000 ALTER TABLE `phongban` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `phucap_nhanvien`
--

DROP TABLE IF EXISTS `phucap_nhanvien`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `phucap_nhanvien` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `MaNV` int NOT NULL,
  `TenPhuCap` varchar(100) NOT NULL,
  `SoTien` decimal(18,2) NOT NULL DEFAULT '0.00',
  `ApDungTuNgay` date NOT NULL,
  `ApDungDenNgay` date DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `fk_phucap_nv` (`MaNV`),
  CONSTRAINT `fk_phucap_nv` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `phucap_nhanvien`
--

LOCK TABLES `phucap_nhanvien` WRITE;
/*!40000 ALTER TABLE `phucap_nhanvien` DISABLE KEYS */;
INSERT INTO `phucap_nhanvien` VALUES (1,1,'Phụ cấp thâm niên',500000.00,'2024-01-01',NULL),(2,1,'Phụ cấp trách nhiệm',300000.00,'2024-06-01','2024-12-31'),(3,2,'Phụ cấp trách nhiệm',200000.00,'2024-01-01',NULL),(4,3,'Phụ cấp ca đêm',150000.00,'2024-03-01',NULL),(5,4,'Phụ cấp trách nhiệm',250000.00,'2024-05-01',NULL),(6,1,'Phụ cấp ăn trưa',730000.00,'2024-01-01',NULL),(7,2,'Phụ cấp ăn trưa',730000.00,'2024-01-01',NULL),(8,2,'Phụ cấp điện thoại',200000.00,'2024-01-01',NULL),(9,3,'Phụ cấp ăn trưa',730000.00,'2024-01-01',NULL),(10,4,'Phụ cấp ăn trưa',730000.00,'2024-01-01',NULL),(11,5,'Phụ cấp ăn trưa',730000.00,'2024-10-01',NULL),(12,5,'Phụ cấp dự án',1000000.00,'2024-10-01','2024-12-31'),(13,6,'Phụ cấp ăn trưa',730000.00,'2024-10-01',NULL),(14,7,'Phụ cấp ăn trưa',730000.00,'2024-10-01',NULL),(15,7,'Phụ cấp xăng xe',500000.00,'2024-10-01',NULL),(16,10,'Phụ cấp chức vụ',2000000.00,'2024-10-01',NULL),(17,10,'Phụ cấp xăng xe',1000000.00,'2024-10-01',NULL);
/*!40000 ALTER TABLE `phucap_nhanvien` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `quyenhan`
--

DROP TABLE IF EXISTS `quyenhan`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `quyenhan` (
  `MaQuyen` int NOT NULL AUTO_INCREMENT,
  `TenQuyen` varchar(100) DEFAULT NULL,
  `MoTa` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`MaQuyen`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `quyenhan`
--

LOCK TABLES `quyenhan` WRITE;
/*!40000 ALTER TABLE `quyenhan` DISABLE KEYS */;
INSERT INTO `quyenhan` VALUES (1,'DuyetDon','Quyền duyệt đơn từ nghỉ phép'),(2,'XemBaoCao','Quyền xem các báo cáo thống kê'),(3,'QuanLyNhanVien','Thêm, sửa, xóa hồ sơ nhân viên'),(4,'QuanTriHeThong','Cấu hình hệ thống, tạo tài khoản'),(5,'QuanLyChamCong','Xem và chỉnh sửa dữ liệu chấm công'),(6,'QuanLyLuong','Xem và quản lý lương');
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
  `TenDangNhap` varchar(50) NOT NULL,
  `MatKhau` varchar(256) NOT NULL,
  `VaiTro` varchar(50) DEFAULT NULL,
  `TrangThai` enum('Hoạt động','Đã khóa','Đã xóa') DEFAULT 'Hoạt động',
  PRIMARY KEY (`MaTK`),
  UNIQUE KEY `TenDangNhap` (`TenDangNhap`),
  KEY `MaNV` (`MaNV`),
  CONSTRAINT `taikhoan_ibfk_1` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `taikhoan`
--

LOCK TABLES `taikhoan` WRITE;
/*!40000 ALTER TABLE `taikhoan` DISABLE KEYS */;
INSERT INTO `taikhoan` VALUES (1,1,'nguyenann','E10ADC3949BA59ABBE56E057F20F883E','NhanVien','Hoạt động'),(2,2,'tranthibanh','E10ADC3949BA59ABBE56E057F20F883E','QuanLy','Hoạt động'),(3,3,'levanc','E10ADC3949BA59ABBE56E057F20F883E','Admin','Hoạt động'),(4,4,'phamthid','E10ADC3949BA59ABBE56E057F20F883E','NhanVien','Đã khóa'),(5,NULL,'superadmin','E10ADC3949BA59ABBE56E057F20F883E','Admin','Hoạt động'),(6,5,'hoangvanem','E10ADC3949BA59ABBE56E057F20F883E','NhanVien','Hoạt động'),(7,6,'ngothien','E10ADC3949BA59ABBE56E057F20F883E','NhanVien','Hoạt động'),(8,7,'buivangiao','E10ADC3949BA59ABBE56E057F20F883E','QuanLy','Hoạt động'),(9,8,'dothihoa','E10ADC3949BA59ABBE56E057F20F883E','NhanVien','Hoạt động'),(10,9,'lyvanich','E10ADC3949BA59ABBE56E057F20F883E','NhanVien','Hoạt động'),(11,10,'phamvankhoi','E10ADC3949BA59ABBE56E057F20F883E','QuanLy','Hoạt động');
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `taikhoan_vaitro`
--

LOCK TABLES `taikhoan_vaitro` WRITE;
/*!40000 ALTER TABLE `taikhoan_vaitro` DISABLE KEYS */;
INSERT INTO `taikhoan_vaitro` VALUES (3,1),(5,1),(2,2),(8,2),(11,2),(1,3),(4,3),(6,3),(7,3),(9,3),(10,3);
/*!40000 ALTER TABLE `taikhoan_vaitro` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `thue`
--

DROP TABLE IF EXISTS `thue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `thue` (
  `MaThue` int NOT NULL AUTO_INCREMENT,
  `MaNV` int NOT NULL,
  `TenThue` varchar(100) NOT NULL,
  `SoTien` decimal(18,2) NOT NULL DEFAULT '0.00',
  `ApDungTuNgay` date NOT NULL,
  `ApDungDenNgay` date DEFAULT NULL,
  PRIMARY KEY (`MaThue`),
  KEY `fk_thue_nv` (`MaNV`),
  CONSTRAINT `fk_thue_nv` FOREIGN KEY (`MaNV`) REFERENCES `nhanvien` (`MaNV`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `thue`
--

LOCK TABLES `thue` WRITE;
/*!40000 ALTER TABLE `thue` DISABLE KEYS */;
INSERT INTO `thue` VALUES (1,1,'Thuế thu nhập cá nhân',1200000.00,'2024-09-01','2024-09-30'),(2,2,'Thuế thu nhập cá nhân',800000.00,'2024-09-01','2024-09-30'),(3,3,'Thuế thu nhập cá nhân',1000000.00,'2024-09-01','2024-09-30'),(4,4,'Thuế thu nhập cá nhân',750000.00,'2024-09-01','2024-09-30'),(5,10,'Thuế TNCN T10/2024',1500000.00,'2024-10-01','2024-10-31'),(6,7,'Thuế TNCN T10/2024',350000.00,'2024-10-01','2024-10-31'),(7,3,'Thuế TNCN T10/2024',900000.00,'2024-10-01','2024-10-31');
/*!40000 ALTER TABLE `thue` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vaitro`
--

DROP TABLE IF EXISTS `vaitro`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vaitro` (
  `MaVaiTro` int NOT NULL AUTO_INCREMENT,
  `TenVaiTro` varchar(50) DEFAULT NULL,
  `MoTa` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`MaVaiTro`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vaitro_quyenhan`
--

LOCK TABLES `vaitro_quyenhan` WRITE;
/*!40000 ALTER TABLE `vaitro_quyenhan` DISABLE KEYS */;
INSERT INTO `vaitro_quyenhan` VALUES (1,1),(2,1),(1,2),(2,2),(3,2),(1,3);
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

-- Dump completed on 2025-12-12 22:18:05
