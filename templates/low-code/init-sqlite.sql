-- ----------------------------
-- Table structure for app
-- ----------------------------
DROP TABLE IF EXISTS `app`;
CREATE TABLE `app` (
  `Id` char(36) NOT NULL,
  `Name` varchar(20) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
);

-- ----------------------------
-- Table structure for app_recycle
-- ----------------------------
DROP TABLE IF EXISTS `app_recycle`;
CREATE TABLE `app_recycle` (
  `Id` char(36) NOT NULL,
  `Name` varchar(20) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
);