# ************************************************************
# Sequel Pro SQL dump
# Version 4096
#
# http://www.sequelpro.com/
# http://code.google.com/p/sequel-pro/
#
# Host: localhost (MySQL 5.6.16)
# Database: Voxelgon
# Generation Time: 2014-03-20 04:12:04 +0000
# ************************************************************


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


# Dump of table elements
# ------------------------------------------------------------

DROP TABLE IF EXISTS `elements`;

CREATE TABLE `elements` (
  `atomic_number` int(3) NOT NULL DEFAULT '0',
  `symbol` varchar(3) DEFAULT '???',
  `element_name` varchar(15) DEFAULT 'impossabilium',
  `stable` tinyint(1) DEFAULT '1',
  `phase` char(1) DEFAULT 's',
  `catagory` char(1) DEFAULT 'x',
  `atomic_weight` float(6,3) DEFAULT NULL,
  `density` float(8,3) DEFAULT NULL,
  `melting_point` float(8,3) DEFAULT NULL,
  `boiling_point` float(8,3) DEFAULT NULL,
  PRIMARY KEY (`atomic_number`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

LOCK TABLES `elements` WRITE;
/*!40000 ALTER TABLE `elements` DISABLE KEYS */;

INSERT INTO `elements` (`atomic_number`, `symbol`, `element_name`, `stable`, `phase`, `catagory`, `atomic_weight`, `density`, `melting_point`, `boiling_point`)
VALUES
	(0,'Nt','Neutronium',1,'s','x',1.000,2.874,NULL,NULL),
	(1,'H','Hydrogen',1,'g','h',1.008,0.071,14.010,20.280),
	(2,'He','Helium',1,'g','j',4.003,0.147,0.950,4.216),
	(3,'Li','Lithium',1,'s','a',6.941,0.534,553.690,1118.150),
	(4,'Be','Beryllium',1,'s','b',9.012,1.848,1551.000,3243.000),
	(5,'B','Boron',1,'s','g',10.811,2.340,2573.000,3931.000),
	(6,'C','Carbon',1,'s','h',12.011,2.250,3820.000,5100.000),
	(7,'N','Nitrogen',1,'g','h',14.007,0.808,63.290,77.400),
	(8,'O','Oxygen',1,'g','h',15.999,1.149,54.800,90.190),
	(9,'F','Fluorine',1,'g','i',18.998,1.108,53.530,85.010),
	(10,'Ne','Neon',1,'g','j',20.180,1.204,48.000,27.100),
	(11,'Na','Sodium',1,'s','a',22.990,0.971,370.960,1156.100),
	(12,'Mg','Magnesium',1,'s','b',24.305,1.738,922.000,1363.000),
	(13,'Al','Aluminum',1,'s','f',26.982,2.699,933.500,2740.000),
	(14,'Si','Silicon',1,'s','g',28.085,2.330,1683.000,2628.000),
	(15,'P','Phosphorus',1,'s','h',30.974,1.820,317.300,553.000),
	(16,'S','Sulfur',1,'s','h',32.066,2.070,386.000,717.824),
	(17,'Cl','Chlorine',1,'g','i',35.453,1.560,172.200,238.600),
	(18,'Ar','Argon',1,'g','j',39.948,1.400,83.800,87.300),
	(19,'K','Potassium',1,'s','a',39.098,0.856,336.800,1047.000),
	(20,'Ca','Calcium',1,'s','b',40.078,1.550,1112.000,1757.000),
	(21,'Sc','Scandium',1,'s','e',44.956,2.990,1814.000,3104.000),
	(22,'Ti','Titanium',1,'s','e',47.880,4.540,1933.000,3560.000),
	(23,'V','Vanadium',1,'s','e',50.941,6.110,2160.000,3650.000),
	(24,'Cr','Chromium',1,'s','e',51.996,7.180,2130.000,2945.000),
	(25,'Mn','Manganese',1,'s','e',54.938,7.210,1517.000,2235.000),
	(26,'Fe','Iron',1,'s','e',55.847,7.874,1808.000,3023.000),
	(27,'Co','Cobalt',1,'s','e',58.933,8.900,1768.000,3143.000),
	(28,'Ni','Nickel',1,'s','e',58.693,8.902,1726.000,3005.000),
	(29,'Cu','Copper',1,'s','e',63.546,8.960,1356.600,2840.000),
	(30,'Zn','Zinc',1,'s','e',65.390,7.133,692.730,1180.000),
	(31,'Ga','Gallium',1,'s','f',69.723,5.910,302.930,2676.000),
	(32,'Ge','Germanium',1,'s','g',72.610,5.323,1210.600,3103.000),
	(33,'As','Arsenic',1,'s','g',74.922,5.730,1090.000,876.000),
	(34,'Se','Selenium',1,'s','h',78.960,4.790,490.000,958.100),
	(35,'Br','Bromine',1,'l','i',79.904,3.120,265.900,331.900),
	(36,'Kr','Krypton',1,'g','j',83.800,2.155,116.600,120.850),
	(37,'Rb','Rubidium',1,'l','a',85.468,1.532,312.200,961.000),
	(38,'Sr','Strontium',1,'s','b',87.620,2.540,1042.000,1657.000),
	(39,'Y','Yttrium',1,'s','e',88.906,4.470,1795.000,3611.000),
	(40,'Zr','Zirconium',1,'s','e',91.224,6.506,2125.000,4650.000),
	(41,'Nb','Niobium',1,'s','e',92.906,8.570,2741.000,5015.000),
	(42,'Mo','Molybdenum',1,'s','e',95.940,10.220,2890.000,4885.000),
	(43,'Tc','Technetium',1,'s','e',97.907,11.500,2445.000,5150.000),
	(44,'Ru','Ruthenium',1,'s','e',101.070,12.410,2583.000,4173.000),
	(45,'Rh','Rhodium',1,'s','e',102.906,12.410,2239.000,4000.000),
	(46,'Pd','Palladium',1,'s','e',106.420,12.020,1825.000,3413.000),
	(47,'Ag','Silver',1,'s','e',107.868,10.500,1235.100,2485.000),
	(48,'Cd','Cadmium',1,'s','e',112.411,8.650,594.100,1038.000),
	(49,'In','Indium',1,'s','f',114.818,7.310,429.320,2353.000),
	(50,'Sn','Tin',1,'s','f',118.710,7.310,505.100,2543.000),
	(51,'Sb','Antimony',1,'s','g',121.760,6.691,903.900,1908.000),
	(52,'Te','Tellurium',1,'s','g',127.600,6.240,722.700,1263.000),
	(53,'I','Iodine',1,'s','i',126.904,4.930,386.700,457.500),
	(54,'Xe','Xenon',1,'g','j',131.290,3.520,161.300,166.100),
	(55,'Cs','Cesium',1,'l','a',132.905,1.873,301.600,951.600),
	(56,'Ba','Barium',1,'s','b',137.327,3.500,1002.000,1910.000),
	(57,'La','Lanthanum',1,'s','c',138.905,6.150,1194.000,3730.000),
	(58,'Ce','Cerium',1,'s','c',140.115,6.757,1072.000,3699.000),
	(59,'Pr','Praseodymium',1,'s','c',140.908,6.773,1204.000,3785.000),
	(60,'Nd','Neodymium',1,'s','c',144.240,7.007,1294.000,3341.000),
	(61,'Pm','Promethium',1,'s','c',144.913,7.200,1441.000,3000.000),
	(62,'Sm','Samarium',1,'s','c',150.360,7.520,1350.000,2064.000),
	(63,'Eu','Europium',1,'s','c',151.965,5.243,1095.000,1870.000),
	(64,'Gd','Gadolinium',1,'s','c',157.250,7.900,1586.000,3539.000),
	(65,'Tb','Terbium',1,'s','c',158.925,8.229,1629.000,3296.000),
	(66,'Dy','Dysprosium',1,'s','c',162.500,8.550,1685.000,2835.000),
	(67,'Ho','Holmium',1,'s','c',164.930,8.795,1747.000,2968.000),
	(68,'Er','Erbium',1,'s','c',167.260,9.060,1802.000,3136.000),
	(69,'Tm','Thulium',1,'s','c',168.934,9.321,1818.000,2220.000),
	(70,'Yb','Ytterbium',1,'s','c',173.040,6.965,1097.000,1466.000),
	(71,'Lu','Lutetium',1,'s','e',174.967,9.840,1936.000,3668.000),
	(72,'Hf','Hafnium',1,'s','e',178.490,13.310,2503.000,5470.000),
	(73,'Ta','Tantalum',1,'s','e',180.948,16.654,3269.000,5698.000),
	(74,'W','Tungsten',1,'s','e',183.840,19.300,3680.000,5930.000),
	(75,'Re','Rhenium',1,'s','e',186.207,21.020,3453.000,5900.000),
	(76,'Os','Osmium',1,'s','e',190.230,22.570,3327.000,5300.000),
	(77,'Ir','Iridium',1,'s','e',192.220,22.420,2683.000,4403.000),
	(78,'Pt','Platinum',1,'s','e',195.080,21.450,2045.000,4100.000),
	(79,'Au','Gold',1,'s','e',196.967,19.300,1337.580,3080.000),
	(80,'Hg','Mercury',1,'l','e',200.590,13.546,234.280,629.730),
	(81,'Tl','Thallium',1,'s','f',204.383,11.850,576.600,1730.000),
	(82,'Pb','Lead',1,'s','f',207.200,11.350,600.650,2013.000),
	(83,'Bi','Bismuth',1,'s','f',208.980,9.747,544.500,1883.000),
	(84,'Po','Polonium',0,'s','g',208.982,9.320,527.000,1235.000),
	(85,'At','Astatine',0,'s','i',209.987,6.400,575.000,610.000),
	(86,'Rn','Radon',0,'g','j',222.018,4.400,202.000,211.400),
	(87,'Fr','Francium',0,'l','a',223.020,1.870,300.000,950.000),
	(88,'Ra','Radium',0,'s','b',226.025,5.500,973.000,1413.000),
	(89,'Ac','Actinium',0,'s','d',227.028,NULL,1320.000,3470.000),
	(90,'Th','Thorium',1,'s','d',232.038,11.780,2028.000,5060.000),
	(91,'Pa','Protactinium',1,'s','d',231.036,15.370,2113.000,4300.000),
	(92,'U','Uranium',1,'s','d',238.029,19.050,1405.500,4018.000),
	(93,'Np','Neptunium',0,'s','d',237.048,20.250,913.000,4175.000),
	(94,'Pu','Plutonium',0,'s','d',244.064,19.840,914.000,3505.000),
	(95,'Am','Americium',0,'s','d',243.061,13.670,1267.000,2880.000),
	(96,'Cm','Curium',0,'s','d',247.070,13.510,1340.000,NULL),
	(97,'Bk','Berkelium',0,'s','d',247.070,13.250,NULL,NULL),
	(98,'Cf','Californium',0,'s','d',251.080,15.100,900.000,NULL),
	(99,'Es','Einsteinium',0,'s','d',252.083,NULL,NULL,1130.000),
	(100,'Fm','Fermium',0,'s','d',257.095,NULL,1800.000,NULL),
	(101,'Md','Mendelevium',0,'s','d',258.100,NULL,1100.000,NULL),
	(102,'No','Nobelium',0,'s','d',259.101,NULL,1100.000,NULL),
	(103,'Lr','Lawrencium',0,'s','k',262.110,NULL,NULL,NULL),
	(104,'Rf','Rutherfordium',0,'s','k',261.000,NULL,NULL,NULL),
	(105,'Db','Dubnium',0,'s','k',262.000,NULL,NULL,NULL),
	(106,'Sg','Seaborgium',0,'s','k',266.000,NULL,NULL,NULL),
	(107,'Bh','Bohrium',0,'s','k',264.000,NULL,NULL,NULL),
	(108,'Hs','Hassium',0,'s','k',269.000,NULL,NULL,NULL),
	(109,'Mt','Meitnerium',0,'s','k',268.000,NULL,NULL,NULL),
	(110,'Ds','Darmstadtium',0,'s','k',269.000,NULL,NULL,NULL),
	(111,'Rg','Roentgenium',0,'s','k',272.000,NULL,NULL,NULL),
	(112,'Cn','Copernicium',0,'s','k',277.000,NULL,NULL,NULL),
	(113,'Ja','Japonium',0,'s','k',NULL,NULL,NULL,NULL),
	(114,'Fl','Flerovium',0,'s','k',289.000,NULL,NULL,NULL),
	(115,'Av','Asimovium',0,'s','k',NULL,NULL,NULL,NULL),
	(116,'Lv','Livermorium',0,'s','k',NULL,NULL,NULL,NULL),
	(117,'Mv','Moscovium',0,'s','k',NULL,NULL,NULL,NULL),
	(118,'Fy','Flyorium',0,'s','k',NULL,NULL,NULL,NULL);

/*!40000 ALTER TABLE `elements` ENABLE KEYS */;
UNLOCK TABLES;


# Dump of table elements_catagory
# ------------------------------------------------------------

DROP TABLE IF EXISTS `elements_catagory`;

CREATE TABLE `elements_catagory` (
  `flag` char(11) NOT NULL DEFAULT '',
  `catagory` text,
  PRIMARY KEY (`flag`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

LOCK TABLES `elements_catagory` WRITE;
/*!40000 ALTER TABLE `elements_catagory` DISABLE KEYS */;

INSERT INTO `elements_catagory` (`flag`, `catagory`)
VALUES
	('a','Alkali Metal'),
	('b','Alkali Earth Metal'),
	('c','Lanthanide'),
	('d','Actinide'),
	('e','Transition Metal'),
	('f','Poor Metal'),
	('g','Semimetal'),
	('h','Nonmetal'),
	('i','Halogen'),
	('j','Noble Gas'),
	('k','Post-transition metal'),
	('x','Exotic Element');

/*!40000 ALTER TABLE `elements_catagory` ENABLE KEYS */;
UNLOCK TABLES;


# Dump of table materials
# ------------------------------------------------------------

DROP TABLE IF EXISTS `materials`;

CREATE TABLE `materials` (
  `material_id` int(11) NOT NULL,
  `namespace` varchar(30) NOT NULL DEFAULT 'Voxelgon',
  `material_name` varchar(30) DEFAULT 'Unknown Substance',
  `material_description` text,
  `conductive` tinyint(1) DEFAULT '1',
  `magnetic` tinyint(1) DEFAULT '0',
  `organic` tinyint(1) DEFAULT '0',
  `strength` float(5,2) DEFAULT '2.00',
  `rad-shielding` float(5,3) DEFAULT '1.000' COMMENT 'halving thickness in meters',
  `radiation` float(5,1) DEFAULT '0.0',
  PRIMARY KEY (`material_id`,`namespace`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

LOCK TABLES `materials` WRITE;
/*!40000 ALTER TABLE `materials` DISABLE KEYS */;

INSERT INTO `materials` (`material_id`, `namespace`, `material_name`, `material_description`, `conductive`, `magnetic`, `organic`, `strength`, `rad-shielding`, `radiation`)
VALUES
	(0,'Voxelgon','Steel','A durable ferrous material, suitable for simple structures and light armor',1,1,0,2.00,1.000,0.0),
	(1,'Voxelgon','Titanium','A stronger, corrosion resistant material that is non-ferrous. Though expensive, it is favored my militaries for its abundance and simplicity.',1,0,0,4.00,0.950,0.0),
	(2,'Voxelgon','Lead','A very weak and heavy metal, lead is commonly used for its radiation-blocking properties. ',1,0,0,1.25,0.250,0.0);

/*!40000 ALTER TABLE `materials` ENABLE KEYS */;
UNLOCK TABLES;


# Dump of table materials_makeup
# ------------------------------------------------------------

DROP TABLE IF EXISTS `materials_makeup`;

CREATE TABLE `materials_makeup` (
  `makeup_id` int(11) NOT NULL,
  `material_id` int(11) NOT NULL DEFAULT '0',
  `namespace` varchar(30) NOT NULL DEFAULT 'voxelgon',
  `atomic_number` int(11) DEFAULT '1',
  `element_percentage` float(4,1) DEFAULT '100.0',
  PRIMARY KEY (`makeup_id`,`material_id`,`namespace`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

LOCK TABLES `materials_makeup` WRITE;
/*!40000 ALTER TABLE `materials_makeup` DISABLE KEYS */;

INSERT INTO `materials_makeup` (`makeup_id`, `material_id`, `namespace`, `atomic_number`, `element_percentage`)
VALUES
	(0,0,'voxelgon',26,95.0),
	(1,0,'voxelgon',6,4.0),
	(2,1,'voxelgon',22,99.9),
	(3,2,'voxelgon',82,99.5);

/*!40000 ALTER TABLE `materials_makeup` ENABLE KEYS */;
UNLOCK TABLES;



/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
