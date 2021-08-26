SELECT		so.[name] AS TRIGGER_NAME,
			USER_NAME(so.[uid]) AS TRIGGER_OWNER,
			USER_NAME(so2.[uid]) AS TABLE_SCHEMA,
			OBJECT_NAME(so.[parent_obj]) AS TABLE_NAME,
			OBJECTPROPERTY(so.id, 'ExecIsUpdateTrigger') AS IS_UPDATE,
			OBJECTPROPERTY(so.id, 'ExecIsDeleteTrigger') AS IS_DELETE,
			OBJECTPROPERTY(so.id, 'ExecIsInsertTrigger') AS IS_INSERT,
			OBJECTPROPERTY(so.id, 'ExecIsAfterTrigger') AS IS_AFTER,
			OBJECTPROPERTY(so.id, 'ExecIsInsteadOfTrigger') AS IS_INSTEAD_OF,
			OBJECTPROPERTY(so.id, 'ExecIsTriggerDisabled') AS IS_DISABLED,
			object_definition(so.id) AS [TRIGGER_CONTENT]
FROM		sysobjects AS so
INNER JOIN	sysobjects AS so2
				ON so.parent_obj = so2.Id
WHERE		so.[type] = 'TR'
