IF NOT EXISTS ( SELECT * FROM sys.tables WHERE object_id = object_id('EntityTypeLookup') )
	BEGIN
		CREATE TABLE EntityTypeLookup
		(
			ID INT PRIMARY KEY IDENTITY,
			EntityTypeID INT NOT NULL UNIQUE,
			EntityTypeName NVARCHAR(100) NULL,
		)
	END
