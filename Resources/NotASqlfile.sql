CREATE TABLE `MOOOOOOOOOOOO` (
  `atomic_number`   INTEGER(3)  NOT NULL DEFAULT '0',
  `symbol`          TEXT(3)     DEFAULT '???',
  `element_name`    TEXT(15)    DEFAULT 'impossabilium',
  `stable`          INTEGER(1)  DEFAULT '1',
  `phase`           TEXT(1)     DEFAULT 's',
  `catagory`        TEXT(1)     DEFAULT 'x',
  `atomic_weight`   REAL(6,3),
  `density`         REAL(8,3),
  `melting_point`   REAL(8,3),
  `boiling_point`   REAL(8,3),
  `sql_path`        TEXT(100),

  PRIMARY KEY (`atomic_number`)
);

