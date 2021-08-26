SELECT		[text]
FROM		syscomments
WHERE		(id = OBJECT_ID(@routinename))
ORDER BY	colid
-- todo: consider using OBJECT_DEFINITION instead
