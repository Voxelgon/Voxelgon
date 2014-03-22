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
);

DROP TABLE IF EXISTS `elements_catagory`;

CREATE TABLE `elements_catagory` (
  `flag` char(11) NOT NULL DEFAULT '',
  `catagory` text,
  PRIMARY KEY (`flag`)
);

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
  `rad-shielding` float(5,3) DEFAULT '1.000',
  `radiation` float(5,1) DEFAULT '0.0',
  PRIMARY KEY (`material_id`,`namespace`)
);

DROP TABLE IF EXISTS `materials_makeup`;

CREATE TABLE `materials_makeup` (
  `makeup_id` int(11) NOT NULL,
  `material_id` int(11) NOT NULL DEFAULT '0',
  `namespace` varchar(30) NOT NULL DEFAULT 'voxelgon',
  `atomic_number` int(11) DEFAULT '1',
  `element_percentage` float(4,1) DEFAULT '100.0',
  PRIMARY KEY (`makeup_id`,`material_id`,`namespace`)
)


