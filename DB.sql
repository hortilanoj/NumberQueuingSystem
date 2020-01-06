CREATE DATABASE  IF NOT EXISTS `queuingsystem` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `queuingsystem`;
-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: localhost    Database: queuingsystem
-- ------------------------------------------------------
-- Server version	5.6.36-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `clients`
--

DROP TABLE IF EXISTS `clients`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `clients` (
  `ClientId` bigint(20) NOT NULL AUTO_INCREMENT,
  `ClientTypeId` bigint(20) DEFAULT NULL,
  `Firstname` longtext CHARACTER SET utf8mb4,
  `Middlename` longtext CHARACTER SET utf8mb4,
  `Lastname` longtext CHARACTER SET utf8mb4,
  `Address` longtext CHARACTER SET utf8mb4,
  `ContactNos` longtext CHARACTER SET utf8mb4,
  PRIMARY KEY (`ClientId`),
  KEY `FK_Clients_ClientTypes` (`ClientTypeId`),
  CONSTRAINT `FK_Clients_ClientTypes` FOREIGN KEY (`ClientTypeId`) REFERENCES `clienttypes` (`ClientTypeId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=10039 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clients`
--

LOCK TABLES `clients` WRITE;
/*!40000 ALTER TABLE `clients` DISABLE KEYS */;
INSERT INTO `clients` VALUES (10012,1,'sdad','da','ada','adassssssss','adadasssssdsds'),(10013,1,'Jovanie','Camariosa','Hortilano','',''),(10014,10004,'da','ada','dad','dad','dad'),(10015,10004,'adsadada','sdad','adad','da','adad'),(10016,10004,'sdad','dad','dada','adad','dada'),(10017,10005,'fsfsf','fsfs','sfsf','sfs','sfs'),(10018,1,'d','ada','ad','dad','ada'),(10019,1,'sdad','dadad','da','ada','adada'),(10020,10004,'sda','adad','dad','dadad','da'),(10021,10004,'dad','da','adad','dad','dada'),(10022,1,'dsa','ad','dada','d','adad'),(10023,1,'ada','adaa','dad','dada','ada'),(10024,10004,'dddd','dd','ddd','ddd','ddd'),(10025,1,'df','fsfsf','fs','sfsf','fsfs'),(10026,1,'dfs','sfsf','fsfs','sfs','sfsf'),(10027,1,'fgdg','gd','dgdg','gdgd','dgd'),(10028,10005,'ada','adada','sda','dadadada','adad'),(10029,10004,'sssss','sss','sssss','ssssss',''),(10030,10005,'sssaaaaaaaaaaaaaa','aaaaaaaaaaaaaaa','aaaaaaaaaaa','aaaaaaaaaaa','aaaaaaaaaaa'),(10031,10005,'qqqqq','qqqq','qqqqqq','',''),(10032,10004,'eeee','eeee','eeeee','',''),(10033,1,'ssssss','sss','ssssssssssssss','sss','ss'),(10034,10005,'fffffffffffffff','ffffffffffffffffff','ffffffffffffffffffffffffffffffff','',''),(10035,1,'ccccccccccccccccccccccc','ccccccccccccccc','cccccccccccccccccccc','',''),(10036,10004,'Rita','Bendol','Hortilano','',''),(10037,10004,'gdgd','dgdg','gd','dgdg','gdg'),(10038,1,'sdadad','dad','dad','dada','ada');
/*!40000 ALTER TABLE `clients` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `clienttypes`
--

DROP TABLE IF EXISTS `clienttypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `clienttypes` (
  `ClientTypeId` bigint(20) NOT NULL AUTO_INCREMENT,
  `ClientTypeName` longtext CHARACTER SET utf8mb4,
  `ClientTypeDescription` longtext CHARACTER SET utf8mb4,
  `ClientTypeActive` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`ClientTypeId`)
) ENGINE=InnoDB AUTO_INCREMENT=10006 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clienttypes`
--

LOCK TABLES `clienttypes` WRITE;
/*!40000 ALTER TABLE `clienttypes` DISABLE KEYS */;
INSERT INTO `clienttypes` VALUES (1,'Examination','',1),(10004,'Certification of Eligibility','',1),(10005,'Correction Of Eligibility','',1);
/*!40000 ALTER TABLE `clienttypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `queues`
--

DROP TABLE IF EXISTS `queues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `queues` (
  `QueueId` bigint(20) NOT NULL AUTO_INCREMENT,
  `TerminalId` bigint(20) DEFAULT NULL,
  `ClientId` bigint(20) DEFAULT NULL,
  `Purpose` varchar(50) CHARACTER SET utf8mb4 DEFAULT NULL,
  `IsCurrent` tinyint(1) DEFAULT NULL,
  `OrderId` int(11) DEFAULT NULL,
  `IsHold` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`QueueId`),
  KEY `FK_Queues_Terminals` (`TerminalId`),
  KEY `FK_Queues_Clients` (`ClientId`),
  CONSTRAINT `FK_Queues_Clients` FOREIGN KEY (`ClientId`) REFERENCES `clients` (`ClientId`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_Queues_Terminals` FOREIGN KEY (`TerminalId`) REFERENCES `terminals` (`TerminalId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=10052 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `queues`
--

LOCK TABLES `queues` WRITE;
/*!40000 ALTER TABLE `queues` DISABLE KEYS */;
INSERT INTO `queues` VALUES (10042,2,10030,'aaaaaaaaaaaaaaa',1,13,0),(10044,2,10031,'',0,15,0),(10050,1,10037,'gdgdgdg',1,21,0),(10051,1,10038,'adadadadad',0,22,0);
/*!40000 ALTER TABLE `queues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `terminals`
--

DROP TABLE IF EXISTS `terminals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `terminals` (
  `TerminalId` bigint(20) NOT NULL AUTO_INCREMENT,
  `TerminalName` varchar(50) CHARACTER SET utf8mb4 DEFAULT NULL,
  `TerminalDescription` varchar(50) CHARACTER SET utf8mb4 DEFAULT NULL,
  `TerminalPort` int(11) DEFAULT NULL,
  `TerminalFrontdesk` tinyint(1) DEFAULT NULL,
  `TerminalActive` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`TerminalId`)
) ENGINE=InnoDB AUTO_INCREMENT=10004 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `terminals`
--

LOCK TABLES `terminals` WRITE;
/*!40000 ALTER TABLE `terminals` DISABLE KEYS */;
INSERT INTO `terminals` VALUES (1,'DR. ROLLIN','sdsdsdsd',1905,0,1),(2,'DR. DAISY','sddsdsdsd',1803,0,1),(3,'FRONTDESK','sdsdsdsd',1857,1,1),(10002,'COUTER 1','',1540,0,0);
/*!40000 ALTER TABLE `terminals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'queuingsystem'
--

--
-- Dumping routines for database 'queuingsystem'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-05-28 16:27:08
