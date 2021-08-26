SELECT		ROUTINE_NAME,
			ROUTINE_SCHEMA,
			ROUTINE_TYPE
FROM		INFORMATION_SCHEMA.ROUTINES
WHERE		(NOT (SPECIFIC_NAME LIKE 'dt_%'))
AND			(NOT (SPECIFIC_NAME = 'fn_diagramobjects'))
AND			(NOT (SPECIFIC_NAME = 'sp_dropdiagram'))
AND			(NOT (SPECIFIC_NAME = 'sp_alterdiagram'))
AND			(NOT (SPECIFIC_NAME = 'sp_renamediagram'))
AND			(NOT (SPECIFIC_NAME = 'sp_creatediagram'))
AND			(NOT (SPECIFIC_NAME = 'sp_helpdiagramdefinition'))
AND			(NOT (SPECIFIC_NAME = 'sp_helpdiagrams'))
AND			(NOT (SPECIFIC_NAME = 'sp_upgraddiagrams'))
ORDER BY	ROUTINE_NAME