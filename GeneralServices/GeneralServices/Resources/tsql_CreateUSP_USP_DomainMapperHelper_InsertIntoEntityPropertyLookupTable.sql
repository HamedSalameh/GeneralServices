IF EXISTS (select * from sys.procedures where object_id = object_id(('USP_DomainMapperHelper_InsertIntoEntityPropertyLookupTable')))
BEGIN
	DROP PROCEDURE USP_DomainMapperHelper_InsertIntoEntityPropertyLookupTable
END
GO

CREATE PROCEDURE USP_DomainMapperHelper_InsertIntoEntityPropertyLookupTable
	@dtEntityProperties UDT_DomainMapperHelper_EntityProperties READONLY
AS
	BEGIN
		INSERT INTO EntityPropertyLookup
		SELECT * FROM @dtEntityProperties
	END
