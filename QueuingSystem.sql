-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               10.3.13-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Version:             9.5.0.5196
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Dumping database structure for queuing_system
DROP DATABASE IF EXISTS `queuing_system`;
CREATE DATABASE IF NOT EXISTS `queuing_system` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `queuing_system`;

-- Dumping structure for table queuing_system.display_settings
DROP TABLE IF EXISTS `display_settings`;
CREATE TABLE IF NOT EXISTS `display_settings` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
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

-- Dumping data for table queuing_system.display_settings: ~0 rows (approximately)
DELETE FROM `display_settings`;
/*!40000 ALTER TABLE `display_settings` DISABLE KEYS */;
/*!40000 ALTER TABLE `display_settings` ENABLE KEYS */;

-- Dumping structure for table queuing_system.task
DROP TABLE IF EXISTS `task`;
CREATE TABLE IF NOT EXISTS `task` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `type` tinyint(255) DEFAULT NULL,
  `terminal_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `task_terminal_idx` (`terminal_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- Dumping data for table queuing_system.task: ~0 rows (approximately)
DELETE FROM `task`;
/*!40000 ALTER TABLE `task` DISABLE KEYS */;
INSERT INTO `task` (`id`, `type`, `terminal_id`) VALUES
	(1, 4, 1),
	(3, 4, 1),
	(5, 4, 1);
/*!40000 ALTER TABLE `task` ENABLE KEYS */;

-- Dumping structure for table queuing_system.terminal
DROP TABLE IF EXISTS `terminal`;
CREATE TABLE IF NOT EXISTS `terminal` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL,
  `title_color` varchar(255) DEFAULT NULL,
  `number_color` varchar(255) DEFAULT NULL,
  `background_color` varchar(255) DEFAULT NULL,
  `active` tinyint(4) DEFAULT NULL,
  `sorting` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- Dumping data for table queuing_system.terminal: ~0 rows (approximately)
DELETE FROM `terminal`;
/*!40000 ALTER TABLE `terminal` DISABLE KEYS */;
INSERT INTO `terminal` (`id`, `name`, `description`, `title_color`, `number_color`, `background_color`, `active`, `sorting`) VALUES
	(1, 'Sample', '', '#FFFFFFFF', '#FFFFFFFF', '#FF1F00C1', 1, 0);
/*!40000 ALTER TABLE `terminal` ENABLE KEYS */;

-- Dumping structure for table queuing_system.terminal_queue
DROP TABLE IF EXISTS `terminal_queue`;
CREATE TABLE IF NOT EXISTS `terminal_queue` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `transaction_id` int(11) DEFAULT NULL,
  `terminal_id` int(11) DEFAULT NULL,
  `transaction_queue_id` int(11) DEFAULT NULL,
  `is_done` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `terminal_queue_transaction_idx` (`transaction_id`),
  KEY `terminal_queue_terminal_idx` (`terminal_id`),
  KEY `terminal_queue_transaction_queue_idx` (`transaction_queue_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table queuing_system.terminal_queue: ~0 rows (approximately)
DELETE FROM `terminal_queue`;
/*!40000 ALTER TABLE `terminal_queue` DISABLE KEYS */;
/*!40000 ALTER TABLE `terminal_queue` ENABLE KEYS */;

-- Dumping structure for table queuing_system.terminal_transaction
DROP TABLE IF EXISTS `terminal_transaction`;
CREATE TABLE IF NOT EXISTS `terminal_transaction` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `terminal_id` int(11) DEFAULT NULL,
  `priority_level` tinyint(4) DEFAULT NULL,
  `transaction_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `terminal_transaction_terminal_idx` (`terminal_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table queuing_system.terminal_transaction: ~0 rows (approximately)
DELETE FROM `terminal_transaction`;
/*!40000 ALTER TABLE `terminal_transaction` DISABLE KEYS */;
/*!40000 ALTER TABLE `terminal_transaction` ENABLE KEYS */;

-- Dumping structure for table queuing_system.transaction
DROP TABLE IF EXISTS `transaction`;
CREATE TABLE IF NOT EXISTS `transaction` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) DEFAULT NULL,
  `prefix` varchar(255) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL,
  `active` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table queuing_system.transaction: ~0 rows (approximately)
DELETE FROM `transaction`;
/*!40000 ALTER TABLE `transaction` DISABLE KEYS */;
/*!40000 ALTER TABLE `transaction` ENABLE KEYS */;

-- Dumping structure for table queuing_system.transaction_queue
DROP TABLE IF EXISTS `transaction_queue`;
CREATE TABLE IF NOT EXISTS `transaction_queue` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `terminal_id` int(11) DEFAULT NULL,
  `queue_number` int(11) DEFAULT NULL,
  `date_time` datetime DEFAULT NULL,
  `transaction_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `transaction_queue_terminal_idx` (`terminal_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- Dumping data for table queuing_system.transaction_queue: ~0 rows (approximately)
DELETE FROM `transaction_queue`;
/*!40000 ALTER TABLE `transaction_queue` DISABLE KEYS */;
INSERT INTO `transaction_queue` (`id`, `terminal_id`, `queue_number`, `date_time`, `transaction_id`) VALUES
	(1, 1, 1, NULL, NULL);
/*!40000 ALTER TABLE `transaction_queue` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
