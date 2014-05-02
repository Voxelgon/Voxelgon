DROP TABLE IF EXISTS `resources`;

CREATE TABLE `resources` (
  `path`            TEXT(100)NOT NULL,
  `filename`        TEXT(30) DEFAULT NULL,
  `extension`       TEXT(10) DEFAULT NULL,

  PRIMARY KEY (`path`)
);

DROP TABLE IF EXISTS `elements`;

CREATE TABLE `elements` (
  `atomic_number`   INTEGER(3) NOT  NULL DEFAULT '0',
  `symbol`          TEXT(3)     DEFAULT '???',
  `element_name`    TEXT(15)    DEFAULT 'impossabilium',
  `stable`          INTEGER(1)  DEFAULT '1',
  `phase`           TEXT(1)     DEFAULT 's',
  `catagory`        TEXT(1)     DEFAULT 'x',
  `atomic_weight`   REAL(6,3)   DEFAULT NULL,
  `density`         REAL(8,3)   DEFAULT NULL,
  `melting_point`   REAL(8,3)   DEFAULT NULL,
  `boiling_point`   REAL(8,3)   DEFAULT NULL,
  `path`            TEXT(100)   DEFAULT '@path',

  PRIMARY KEY (`atomic_number`)
);

DROP TABLE IF EXISTS `elements_catagory`;

CREATE TABLE `elements_catagory` (
  `flag`            TEXT(11)    NOT NULL DEFAULT '',
  `catagory`        TEXT,

  PRIMARY KEY (`flag`)
);

DROP TABLE IF EXISTS `materials`;

CREATE TABLE `materials` (
  `material_id`     INTEGER(11)     NOT NULL,
  `namespace`       TEXT(30) NOT NULL DEFAULT 'Voxelgon',
  `material_name`   TEXT(30) DEFAULT 'Unknown Substance',
  `material_desc`   TEXT,
  `conductive`      INTEGER(1)  DEFAULT '1',
  `magnetic`        INTEGER(1)  DEFAULT '0',
  `organic`         INTEGER(1)  DEFAULT '0',
  `strength`        REAL(5,2)   DEFAULT '2.00',
  `rad-shielding`   REAL(5,3)   DEFAULT '1.000',
  `radiation`       REAL(5,1)   DEFAULT '0.0',
  `path`            TEXT(100)   DEFAULT '@path',

  PRIMARY KEY (`material_id`,`namespace`)
);

DROP TABLE IF EXISTS `materials_makeup`;

CREATE TABLE `materials_makeup` (
  `makeup_id`       INTEGER(11)     NOT NULL,
  `material_id`     INTEGER(11)     NOT NULL DEFAULT '0',
  `namespace`       TEXT(30)        NOT NULL DEFAULT 'voxelgon',
  `atomic_number`   INTEGER(11)     DEFAULT '1',
  `element_perc`    REAL(4,1)       DEFAULT '100.0',
  `path`            TEXT(100)       DEFAULT NULL,

  PRIMARY KEY (`makeup_id`,`material_id`,`namespace`)
);

DROP TABLE IF EXISTS `meshes`;

CREATE TABLE `meshes` (
  `mesh_id`        INTEGER(11)      NOT NULL,
  `mesh_path`      TEXT(100)        DEFAULT NULL,
  `mesh_name`      TEXT(30)         DEFAULT 'unnamed mesh',

  PRIMARY KEY (`mesh_id`)
);

DROP TABLE IF EXISTS `meshes_vertices`;

CREATE TABLE `meshes_vertices` (
  `vertex_id`     INTEGER(11)       NOT NULL,
  `vertex_x`      REAL              NOT NULL,
  `vertex_y`      REAL              NOT NULL,
  `vertex_z`      REAL              NOT NULL,

  PRIMARY KEY (`vertex_id`)
);


