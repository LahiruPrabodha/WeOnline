CREATE DATABASE `xsdevx` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE xsdevx;
CREATE TABLE `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `email` varchar(45) NOT NULL,
  `nickname` text NOT NULL,
  `password` text NOT NULL,
  `banned` varchar(5) NOT NULL,
  `role` varchar(45) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `email_UNIQUE` (`email`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;