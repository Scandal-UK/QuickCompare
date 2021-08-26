SELECT		[USER_NAME] = ISNULL(memberprinc.[name], ulogin.[name]),
			[USER_TYPE] =
				CASE memberprinc.[type]
				WHEN 'S' THEN 'SQL User'
				WHEN 'U' THEN 'Windows User' END,
			[ROLE_NAME] = roleprinc.[name],
			[PERMISSION_TYPE] = perm.[permission_name],
			[PERMISSION_STATE] = perm.[state_desc],
			[OBJECT_TYPE] = ISNULL(obj.type_desc, perm.class_desc),
			[OBJECT_NAME] = OBJECT_NAME(perm.major_id),
			[COLUMN_NAME] = col.[name],
			[OBJECT_SCHEMA] = OBJECT_SCHEMA_NAME(perm.major_id)
FROM		sys.database_role_members members
JOIN		sys.database_principals roleprinc
				ON roleprinc.[principal_id] = members.[role_principal_id]
JOIN		sys.database_principals memberprinc
				ON memberprinc.[principal_id] = members.[member_principal_id]
LEFT JOIN	sys.login_token ulogin
				ON memberprinc.[sid] = ulogin.[sid]
LEFT JOIN	sys.database_permissions perm
				ON perm.[grantee_principal_id] = roleprinc.[principal_id]
LEFT JOIN	sys.columns col
				ON col.[object_id] = perm.major_id AND col.[column_id] = perm.[minor_id]
LEFT JOIN	sys.objects obj
				ON perm.[major_id] = obj.[object_id]
