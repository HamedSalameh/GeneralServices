using GeneralServices.Helpers;
using GeneralServices.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Linq;

namespace GeneralServices.Components
{
    public class EntityMapper
    {
        #region Private Properties
        private static EntityMapper _instance;
        private static string _connectionString;
        private static bool _EntityMapperInitialized;
        #endregion

        #region Public Properties
        public string ConnectionString
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception(string.Format("{0} : ConnectionString value cannot be null or empty", Reflection.GetCurrentMethodName()));
                }
                _connectionString = value;
            }
            get
            {
                return _connectionString;
            }
        }

        public bool IsEntityMapperInitialized
        {
            get
            {
                return _EntityMapperInitialized;
            }
        }
        #endregion

        #region Singleton
        private EntityMapper()
        {
            // do all initalizations here
            _connectionString = string.Empty;
            _EntityMapperInitialized = false;
        }

        public static EntityMapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EntityMapper();
                }
                return _instance;
            }
        } 
        #endregion

        private Dictionary<string, int> InitializeEntityMapping(string connectionString)
        {
            Dictionary<string, int> dicDomainMapping = new Dictionary<string, int>();
            DataTable dtDomainMapping = EntityMapperDBHelper.loadDomainEntityMapping(connectionString);
            if (dtDomainMapping == null || (dtDomainMapping.Rows != null && dtDomainMapping.Rows.Count == 0))
            {
                // Mapping does not exist or empty
                // create new mapping
                Initialize(connectionString);
            }
            else
            {
                // convert into dictionary
                dicDomainMapping = dtDomainMapping.AsEnumerable().ToDictionary(dr => dr.Field<string>("EntityTypeName"), dr => dr.Field<int>("EntityTypeID"));
            }

            return dicDomainMapping;
        }

        private EntityTypeLookup CreateEntityMap(Type EntityType)
        {
            EntityTypeLookup etl = null;

            if (EntityType != null && EntityType != typeof(object))
            {
                string typeName = EntityType.FullName;

                etl = new EntityTypeLookup()
                {
                    EntityTypeName = typeName,
                    EntityTypeID = General.calculateClassHash(EntityType).Value,
                    EntityProperties = new List<EntityPropertyLookup>()
                };

                PropertyInfo[] props = EntityType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (PropertyInfo p in props)
                {
                    EntityPropertyLookup epl = new EntityPropertyLookup()
                    {
                        EntityPropertyID = Helpers.General.calculateSingleFieldHash(p).Value,
                        EntityPropertyName = p.Name,
                        EntityTypeLookupID = etl.ID,
                        EntityType = etl
                    };

                    etl.EntityProperties.Add(epl);
                }
            }


            return etl;
        }

        private void Initialize(string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    EntityMapperDBHelper.createMappingTables(connection);

                    EntityMapperDBHelper.createHelperUserDefinedTypes(connection);

                    EntityMapperDBHelper.createHelperStoredProcedures(connection);

                    connection.Close();
                }
            }
            catch (Exception sqlEx)
            {
                throw sqlEx;
            }

        }

        /// <summary>
        /// Scans an assembly for any class types, then generates a mapping in a given database for all the assembly classes
        /// </summary>
        /// <param name="connectionString">A string representing the connection string to a SQL based database</param>
        /// <param name="domainModelAssemblyName">The name of the assembly containing the classes to map</param>
        public void CreateEntityMapping(string domainModelAssemblyName)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception(string.Format("{0} : Connection string must not be empty.", Reflection.GetCurrentMethodName()));
            }
            if (string.IsNullOrEmpty(domainModelAssemblyName))
            {
                throw new Exception(string.Format("{0} : domain model assembly name must not be empty.", Reflection.GetCurrentMethodName()));
            }


            Dictionary<string, int> dicDomainMapping = new Dictionary<string, int>(); ;
            // Try get the domain types from the given assembly
            Type[] domainModelTypes = Reflection.GetDomainTypes(domainModelAssemblyName);

            if (domainModelTypes != null && domainModelTypes.Length > 0)
            {
                try
                {
                    // Load existing domain mapping, if any, otherwise create a new one
                    dicDomainMapping = InitializeEntityMapping(_connectionString);
                    if (dicDomainMapping != null)
                    {
                        int existingHash = 0;
                        foreach (Type domainModelType in domainModelTypes)
                        {
                            // Build type lookup (hash)
                            EntityTypeLookup entityMap = CreateEntityMap(domainModelType);
                            // Compare the existing hash with the new one
                            dicDomainMapping.TryGetValue(entityMap.EntityTypeName, out existingHash);
                            if (existingHash != entityMap.EntityTypeID)
                            {
                                bool actionResult = false;
                                // change is detected, remap the entity into DB
                                if (existingHash != 0)
                                {
                                    // Existing entity - remove the old mapping
                                    actionResult = EntityMapperDBHelper.RemoveEntityMapping(existingHash, _connectionString);
                                }
                                // in case we succeed in removing old mapping, continue and create a new one
                                if (actionResult || existingHash == 0)
                                {
                                    actionResult = EntityMapperDBHelper.SaveEntityMapping(_connectionString, entityMap);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to build domain mapping dicionary");
                    }

                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }
        }

        public int GetEntityTypeLookupID(Type EntityType)
        {
            int entityID = 0;

            try
            {
                EntityMapperDBHelper.GetEntityTypeLookupID(EntityType, ConnectionString);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

            return entityID;
        }
    }
}
