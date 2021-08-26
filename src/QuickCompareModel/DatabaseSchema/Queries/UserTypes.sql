SELECT	t1.[name] AS [custom_type_name],
		SCHEMA_NAME(t1.[schema_id]) AS [schema_name],
		t2.[name] AS [underlying_type_name],
		t1.[precision],
		t1.[scale],
		t1.[max_length],
		t1.[is_nullable],
		t1.[collation_name],
		t1.[is_assembly_type]
FROM sys.types t1
JOIN sys.types t2 ON t2.[system_type_id] = t1.[system_type_id] AND t2.[is_user_defined] = 0
WHERE t1.[is_user_defined] = 1 AND t2.[name] <> 'sysname'
ORDER BY t1.[name]
