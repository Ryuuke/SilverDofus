/*
Navicat MySQL Data Transfer

Source Server         : AJAX
Source Server Version : 50524
Source Host           : localhost:3306
Source Database       : silvergame

Target Server Type    : MYSQL
Target Server Version : 50524
File Encoding         : 65001

Date: 2013-07-23 20:28:39
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for `characters`
-- ----------------------------
DROP TABLE IF EXISTS `characters`;
CREATE TABLE `characters` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `classe` int(11) NOT NULL,
  `level` int(11) NOT NULL,
  `color1` int(11) NOT NULL DEFAULT '-1',
  `color2` int(11) NOT NULL DEFAULT '-1',
  `color3` int(11) NOT NULL DEFAULT '-1',
  `skin` int(11) NOT NULL,
  `sex` int(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of characters
-- ----------------------------
INSERT INTO `characters` VALUES ('1', 'afop', '9', '1', '-1', '-1', '-1', '90', '0');
INSERT INTO `characters` VALUES ('2', 'ejigovi', '2', '1', '12902402', '14730242', '16379102', '21', '1');
INSERT INTO `characters` VALUES ('3', 'ihedope', '8', '1', '3786054', '-1', '-1', '81', '1');
INSERT INTO `characters` VALUES ('4', 'ejor', '8', '1', '-1', '-1', '-1', '80', '0');
INSERT INTO `characters` VALUES ('5', 'ocanex', '5', '1', '-1', '-1', '-1', '50', '0');
INSERT INTO `characters` VALUES ('6', '-Ryuuke-', '7', '1', '5212335', '-1', '-1', '70', '0');
INSERT INTO `characters` VALUES ('7', 'oxowigeq', '7', '1', '5212335', '-1', '-1', '70', '0');
