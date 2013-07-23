/*
Navicat MySQL Data Transfer

Source Server         : AJAX
Source Server Version : 50524
Source Host           : localhost:3306
Source Database       : silveremu

Target Server Type    : MYSQL
Target Server Version : 50524
File Encoding         : 65001

Date: 2013-07-23 20:28:31
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for `accounts`
-- ----------------------------
DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(255) NOT NULL,
  `pass` varchar(255) NOT NULL,
  `pseudo` varchar(255) NOT NULL,
  `question` varchar(255) NOT NULL,
  `reponse` varchar(255) NOT NULL,
  `connected` tinyint(1) NOT NULL,
  `bannedUntil` datetime DEFAULT NULL,
  `gmLevel` int(11) NOT NULL,
  `subscription` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of accounts
-- ----------------------------
INSERT INTO `accounts` VALUES ('1', 'Ryuu', 'lol', 'Ryuuke', 'delete?', 'delete', '0', '2013-07-20 09:50:48', '0', '2013-07-25 12:29:05');
INSERT INTO `accounts` VALUES ('2', 'Ryuuke', 'lool', 'Ryuu', 'noob ?', 'OUIIII', '0', null, '5', null);

-- ----------------------------
-- Table structure for `characters`
-- ----------------------------
DROP TABLE IF EXISTS `characters`;
CREATE TABLE `characters` (
  `accountId` int(11) NOT NULL,
  `gameServerId` int(11) NOT NULL,
  `characterName` varchar(255) NOT NULL,
  PRIMARY KEY (`accountId`,`gameServerId`,`characterName`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of characters
-- ----------------------------
INSERT INTO `characters` VALUES ('1', '1', '-Ryuuke-');
INSERT INTO `characters` VALUES ('1', '1', 'afop');
INSERT INTO `characters` VALUES ('1', '1', 'ejigovi');
INSERT INTO `characters` VALUES ('1', '1', 'ejor');
INSERT INTO `characters` VALUES ('1', '1', 'ihedope');
INSERT INTO `characters` VALUES ('1', '1', 'ocanex');
INSERT INTO `characters` VALUES ('1', '1', 'oxowigeq');

-- ----------------------------
-- Table structure for `gameservers`
-- ----------------------------
DROP TABLE IF EXISTS `gameservers`;
CREATE TABLE `gameservers` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ip` varchar(255) NOT NULL,
  `port` int(11) NOT NULL,
  `ServerKey` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of gameservers
-- ----------------------------
INSERT INTO `gameservers` VALUES ('1', '127.0.0.1', '5555', 'lolxdmdrlol');
INSERT INTO `gameservers` VALUES ('2', '127.0.0.1', '5555', 'loool');
