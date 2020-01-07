CREATE DATABASE  IF NOT EXISTS `queuing_system` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `queuing_system`;
-- MySQL dump 10.13  Distrib 8.0.11, for Win64 (x86_64)
--
-- Host: localhost    Database: queuing_system
-- ------------------------------------------------------
-- Server version	8.0.11

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
 SET NAMES utf8 ;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `display_settings`
--

DROP TABLE IF EXISTS `display_settings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `display_settings` (
  `id` int(11) NOT NULL,
  `terminal_name_fontsize` double DEFAULT NULL,
  `queue_number_fontsize` double DEFAULT NULL,
  `terminal_width` double DEFAULT NULL,
  `terminal_height` double DEFAULT NULL,
  `header_background_color` varchar(255) DEFAULT NULL,
  `header_title_color` varchar(255) DEFAULT NULL,
  `header_title_fontsize` double DEFAULT NULL,
  `header_date_color` varchar(255) DEFAULT NULL,
  `header_date_fontsize` double DEFAULT NULL,
  `header_time_color` varchar(255) DEFAULT NULL,
  `header_time_fontsize` double DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `display_settings_id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `display_settings`
--

LOCK TABLES `display_settings` WRITE;
/*!40000 ALTER TABLE `display_settings` DISABLE KEYS */;
/*!40000 ALTER TABLE `display_settings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `task`
--

DROP TABLE IF EXISTS `task`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `task` (
  `id` int(11) NOT NULL,
  `type` tinyint(255) DEFAULT NULL,
  `terminal_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `task_terminal_idx` (`terminal_id`),
  CONSTRAINT `task_terminal` FOREIGN KEY (`terminal_id`) REFERENCES `terminal` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `task`
--

LOCK TABLES `task` WRITE;
/*!40000 ALTER TABLE `task` DISABLE KEYS */;
/*!40000 ALTER TABLE `task` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `terminal`
--

DROP TABLE IF EXISTS `terminal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `terminal` (
  `id` int(11) NOT NULL,
  `name` varchar(255) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL,
  `title_color` varchar(255) DEFAULT NULL,
  `number_color` varchar(255) DEFAULT NULL,
  `background_color` varchar(255) DEFAULT NULL,
  `active` tinyint(4) DEFAULT NULL,
  `sorting` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `terminal`
--

LOCK TABLES `terminal` WRITE;
/*!40000 ALTER TABLE `terminal` DISABLE KEYS */;
/*!40000 ALTER TABLE `terminal` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `terminal_queue`
--

DROP TABLE IF EXISTS `terminal_queue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `terminal_queue` (
  `id` int(11) NOT NULL,
  `transaction_id` int(11) DEFAULT NULL,
  `terminal_id` int(11) DEFAULT NULL,
  `transaction_queue_id` int(11) DEFAULT NULL,
  `is_done` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `terminal_queue_transaction_idx` (`transaction_id`),
  KEY `terminal_queue_terminal_idx` (`terminal_id`),
  KEY `terminal_queue_transaction_queue_idx` (`transaction_queue_id`),
  CONSTRAINT `terminal_queue_terminal` FOREIGN KEY (`terminal_id`) REFERENCES `terminal` (`id`),
  CONSTRAINT `terminal_queue_transaction` FOREIGN KEY (`transaction_id`) REFERENCES `transaction` (`id`),
  CONSTRAINT `terminal_queue_transaction_queue` FOREIGN KEY (`transaction_queue_id`) REFERENCES `transaction_queue` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `terminal_queue`
--

LOCK TABLES `terminal_queue` WRITE;
/*!40000 ALTER TABLE `terminal_queue` DISABLE KEYS */;
/*!40000 ALTER TABLE `terminal_queue` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `terminal_transaction`
--

DROP TABLE IF EXISTS `terminal_transaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `terminal_transaction` (
  `id` int(11) NOT NULL,
  `terminal_id` int(11) DEFAULT NULL,
  `priority_level` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `terminal_transaction_terminal_idx` (`terminal_id`),
  CONSTRAINT `terminal_transaction_terminal` FOREIGN KEY (`terminal_id`) REFERENCES `terminal` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `terminal_transaction`
--

LOCK TABLES `terminal_transaction` WRITE;
/*!40000 ALTER TABLE `terminal_transaction` DISABLE KEYS */;
/*!40000 ALTER TABLE `terminal_transaction` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `transaction`
--

DROP TABLE IF EXISTS `transaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `transaction` (
  `id` int(11) NOT NULL,
  `name` varchar(255) DEFAULT NULL,
  `prefix` varchar(255) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL,
  `active` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `transaction`
--

LOCK TABLES `transaction` WRITE;
/*!40000 ALTER TABLE `transaction` DISABLE KEYS */;
/*!40000 ALTER TABLE `transaction` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `transaction_queue`
--

DROP TABLE IF EXISTS `transaction_queue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `transaction_queue` (
  `id` int(11) NOT NULL,
  `terminal_id` int(11) DEFAULT NULL,
  `queue_number` int(11) DEFAULT NULL,
  `date_time` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `transaction_queue_terminal_idx` (`terminal_id`),
  CONSTRAINT `transaction_queue_terminal` FOREIGN KEY (`terminal_id`) REFERENCES `terminal` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `transaction_queue`
--

LOCK TABLES `transaction_queue` WRITE;
/*!40000 ALTER TABLE `transaction_queue` DISABLE KEYS */;
/*!40000 ALTER TABLE `transaction_queue` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'queuing_system'
--

--
-- Dumping routines for database 'queuing_system'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-01-07 17:29:37
