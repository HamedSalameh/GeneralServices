IF NOT EXISTS (select * from sys.types where name = 'UDT_DomainMapperHelper_EntityProperties')
BEGIN

	CREATE TYPE UDT_DomainMapperHelper_EntityProperties AS TABLE(
		EntityPropertyID int NOT NULL,
		EntityPropertyName NVARCHAR(100) NOT NULL,
		EntityTypeID int NOT NULL
	)
END
