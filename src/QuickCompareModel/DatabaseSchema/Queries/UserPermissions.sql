SELECT		[USER_NAME] = ISNULL(ulogin.[name], princ.[name]),
			[USER_TYPE] =
				CASE princ.[type]
				WHEN 'S' THEN 'SQL User'
				WHEN 'U' THEN 'Windows User' END,
			[ROLE_NAME] = null,
			[PERMISSION_TYPE] = perm.[permission_name],
			[PERMISSION_STATE] = perm.[state_desc],
			[OBJECT_TYPE] = ISNULL(obj.type_desc, perm.class_desc),
			[OBJECT_NAME] = OBJECT_NAME(perm.major_id),
			[COLUMN_NAME] = col.[name],
			[OBJECT_SCHEMA] = OBJECT_SCHEMA_NAME(perm.major_id)
FROM		sys.database_principals princ
LEFT JOIN	sys.login_token ulogin
				ON princ.[sid] = ulogin.[sid]
LEFT JOIN	sys.database_permissions perm
				ON perm.[grantee_principal_id] = princ.[principal_id]
LEFT JOIN	sys.columns col
				ON col.[object_id] = perm.major_id AND col.[column_id] = perm.[minor_id]
LEFT JOIN	sys.objects obj
				ON perm.[major_id] = obj.[object_id]
WHERE		princ.[type] in ('S','U')
