SELECT			class_desc AS PROPERTY_TYPE,
				o.[name] AS OBJECT_NAME,
				SCHEMA_NAME(o.[schema_id]) AS OBJECT_SCHEMA,
				c.[name] AS COLUMN_NAME,
				ep.[name] AS PROPERTY_NAME,
				[value] AS PROPERTY_VALUE,
				t.[name] AS TABLE_NAME,
				s.[name] AS INDEX_NAME,
				SCHEMA_NAME(t.[schema_id]) AS TABLE_SCHEMA
FROM			sys.extended_properties AS ep
LEFT OUTER JOIN	sys.objects AS o
					ON o.object_id = ep.major_id
LEFT OUTER JOIN	sys.tables AS t
					ON t.object_id = ep.major_id
LEFT OUTER JOIN	sys.columns AS c
					ON c.object_id = ep.major_id AND c.column_id = ep.minor_id AND ep.class = 1
LEFT OUTER JOIN	sys.indexes AS s
					ON s.object_id = ep.major_id AND s.index_id = ep.minor_id
WHERE			(ep.name <> 'microsoft_database_tools_support')
AND				((NOT t.[name] IS NULL AND NOT c.[name] IS NULL AND ep.[name] = 'MS_Description') OR ep.[name] NOT LIKE 'MS_%')
