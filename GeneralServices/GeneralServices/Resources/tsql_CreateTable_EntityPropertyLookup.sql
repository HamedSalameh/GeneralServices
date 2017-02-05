IF NOT EXISTS ( SELECT * FROM sys.tables WHERE object_id = object_id('EntityPropertyLookup') )
	BEGIN
		CREATE TABLE EntityPropertyLookup
		(
			ID INT IDENTITY PRIMARY KEY,
			EntityPropertyID INT NOT NULL,
			EntityPropertyName NVARCHAR(100) NULL,
			EntityTypeID INT NOT NULL,
		
			CONSTRAINT FK_EntityType FOREIGN KEY (EntityTypeID) REFERENCES EntityTypeLookup(EntityTypeID)
			
		)
	END



	