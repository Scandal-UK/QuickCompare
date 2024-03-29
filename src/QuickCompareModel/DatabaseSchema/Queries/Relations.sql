﻿SELECT DISTINCT
		R1.RELATION_NAME,
		R1.CHILD_SCHEMA,
		R1.CHILD_TABLE,
		STUFF(
			(
				SELECT ', ' + R2.CHILD_COLUMN
				FROM
					(
						SELECT Child.CONSTRAINT_NAME AS RELATION_NAME,
						Child.COLUMN_NAME AS CHILD_COLUMN,
						Child.ORDINAL_POSITION
						FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
						INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE Child
						ON Child.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG AND Child.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA AND Child.CONSTRAINT_NAME = RC.CONSTRAINT_NAME
					) R2
				WHERE R2.RELATION_NAME = R1.RELATION_NAME
				ORDER BY R2.ORDINAL_POSITION FOR XML PATH(''), TYPE
		).value('.','VARCHAR(MAX)'),1,2,'') AS CHILD_COLUMNS,
		R1.UNIQUE_CONSTRAINT_NAME,
		R1.PARENT_SCHEMA,
		R1.PARENT_TABLE,
		STUFF(
			(
				SELECT ', ' + R2.PARENT_COLUMN
				FROM
					(
						SELECT Child.CONSTRAINT_NAME AS RELATION_NAME,
						Parent.COLUMN_NAME AS PARENT_COLUMN,
						Parent.ORDINAL_POSITION
						FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
						INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE Child ON Child.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG AND Child.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA AND Child.CONSTRAINT_NAME = RC.CONSTRAINT_NAME
						INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE Parent ON Parent.CONSTRAINT_CATALOG = RC.UNIQUE_CONSTRAINT_CATALOG AND Parent.CONSTRAINT_SCHEMA = RC.UNIQUE_CONSTRAINT_SCHEMA AND Parent.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME AND Parent.ORDINAL_POSITION = Child.ORDINAL_POSITION
					) R2
				WHERE R2.RELATION_NAME = R1.RELATION_NAME
				ORDER BY R2.ORDINAL_POSITION FOR XML PATH(''), TYPE
		).value('.','VARCHAR(MAX)'),1,2,'') AS PARENT_COLUMNS,
		R1.UPDATE_RULE,
		R1.DELETE_RULE
		FROM
			(
				SELECT Child.CONSTRAINT_NAME AS RELATION_NAME,
				Child.CONSTRAINT_SCHEMA AS CHILD_SCHEMA,
				Child.TABLE_NAME AS CHILD_TABLE,
				Child.COLUMN_NAME AS CHILD_COLUMN,
				Parent.CONSTRAINT_NAME AS UNIQUE_CONSTRAINT_NAME,
				Parent.CONSTRAINT_SCHEMA AS PARENT_SCHEMA,
				Parent.TABLE_NAME AS PARENT_TABLE,
				Parent.COLUMN_NAME AS PARENT_COLUMN,
				RC.UPDATE_RULE,
				RC.DELETE_RULE
				FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
				INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE Child ON Child.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG AND Child.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA AND Child.CONSTRAINT_NAME = RC.CONSTRAINT_NAME
				INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE Parent ON Parent.CONSTRAINT_CATALOG = RC.UNIQUE_CONSTRAINT_CATALOG AND Parent.CONSTRAINT_SCHEMA = RC.UNIQUE_CONSTRAINT_SCHEMA AND Parent.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME AND Parent.ORDINAL_POSITION = Child.ORDINAL_POSITION
			) R1