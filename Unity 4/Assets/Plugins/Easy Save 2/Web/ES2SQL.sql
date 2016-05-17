
--
-- Table structure for table `es2data`
--
SET foreign_key_checks = 0;
CREATE TABLE IF NOT EXISTS `es2data` (
  `tag` varchar(100) NOT NULL DEFAULT '',
  `data` mediumblob NOT NULL,
  `fileId` int(11) NOT NULL,
  PRIMARY KEY (`tag`,`fileId`),
  KEY `fileId` (`fileId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `es2files`
--

CREATE TABLE IF NOT EXISTS `es2files` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `filename` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `filename` (`filename`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `es2data`
--
ALTER TABLE `es2data`
  ADD CONSTRAINT `es2data_ibfk_1` FOREIGN KEY (`fileId`) REFERENCES `es2files` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

SET foreign_key_checks = 1;