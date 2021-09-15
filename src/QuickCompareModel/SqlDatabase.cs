namespace QuickCompareModel
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using QuickCompareModel.DatabaseSchema;

    /// <summary>
    /// Class for running database queries and building lists that detail the content of the database schema.
    /// </summary>
    public class SqlDatabase
    {
        private readonly string connectionString;
        private readonly QuickCompareOptions options;

        /// <summary>
        /// Initialises a new instance of the <see cref="SqlDatabase"/> class with a connection string and setting options.
        /// </summary>
        /// <param name="connectionString">The database connection string for the current instance being inspected.</param>
        /// <param name="options">Collection of configuration settings.</param>
        public SqlDatabase(string connectionString, QuickCompareOptions options)
        {
            this.connectionString = connectionString;
            this.options = options;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="SqlDatabase"/> class with a connection string.
        /// </summary>
        /// <param name="connectionString">The database connection string for the current instance being inspected.</param>
        public SqlDatabase(string connectionString)
            : this(connectionString, new QuickCompareOptions())
        {
        }

        /// <summary>
        /// Friendly name for the database instance, including both server name and database name.
        /// </summary>
        public string FriendlyName
        {
            get
            {
                var builder = new SqlConnectionStringBuilder(this.connectionString);
                return $"[{builder.DataSource}].[{builder.InitialCatalog}]";
            }
        }

        /// <summary> Handler for when the status message changes. </summary>
        public event EventHandler<StatusChangedEventArgs> LoaderStatusChanged;

        /// <summary> Gets or sets a list of <see cref="SqlTable"/> instances, indexed by table name. </summary>
        public Dictionary<string, SqlTable> Tables { get; set; } = new Dictionary<string, SqlTable>();

        /// <summary> Gets or sets a list of <see cref="SqlUserType"/> instances, indexed by name. </summary>
        public Dictionary<string, SqlUserType> UserTypes { get; set; } = new Dictionary<string, SqlUserType>();

        /// <summary> Gets or sets a list of database views, indexed by view name. </summary>
        public Dictionary<string, string> Views { get; set; } = new Dictionary<string, string>();

        /// <summary> Gets or sets a list of SQL synonyms, indexed by synonym name. </summary>
        public Dictionary<string, string> Synonyms { get; set; } = new Dictionary<string, string>();

        /// <summary> Gets or sets a list of <see cref="SqlUserRoutine"/> instances, indexed by routine name. </summary>
        /// <remarks> User routines include views, functions and stored procedures. </remarks>
        public Dictionary<string, SqlUserRoutine> UserRoutines { get; set; } = new Dictionary<string, SqlUserRoutine>();

        /// <summary> Gets or sets a list of <see cref="SqlPermission"/> instances, for both roles and users. </summary>
        public List<SqlPermission> Permissions { get; set; } = new List<SqlPermission>();

        /// <summary> Gets or sets a list of <see cref="SqlExtendedProperty"/> instances for the database itself. </summary>
        public List<SqlExtendedProperty> ExtendedProperties { get; set; } = new List<SqlExtendedProperty>();

        /// <summary>
        /// Populate the models based on the supplied connection string.
        /// </summary>
        public async Task PopulateSchemaModelAsync()
        {
            RaiseStatusChanged("Connecting");

            await LoadFullyQualifiedTableNamesAsync();

            await Task.WhenAll(RequiredItemTasks());

            await Task.WhenAll(DependentItemTasks());
        }

        /// <summary>
        /// Helper method to return embedded SQL resource by filename.
        /// </summary>
        /// <param name="queryName">Name of the SQL file without the extension.</param>
        /// <returns>SQL query text.</returns>
        public static string LoadQueryFromResource(string queryName)
        {
            var resourceName = $"{nameof(QuickCompareModel)}.{nameof(DatabaseSchema)}.Queries.{queryName}.sql";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            return stream != null ? new StreamReader(stream).ReadToEnd() : string.Empty;
        }

        protected virtual void RaiseStatusChanged(string message) =>
            this.LoaderStatusChanged?.Invoke(this, new StatusChangedEventArgs(message));

        private Task[] RequiredItemTasks()
        {
            var tasks = new List<Task>() { LoadRelationsAsync(), LoadColumnDetailsAsync() };

            if (options.CompareUserTypes)
            {
                tasks.Add(LoadUserTypesAsync());
            }

            if (options.ComparePermissions)
            {
                tasks.AddRange(new Task[] { LoadRolePermissionsAsync(), LoadUserPermissionsAsync() });
            }

            if (options.CompareProperties)
            {
                tasks.Add(LoadExtendedPropertiesAsync());
            }

            if (options.CompareTriggers)
            {
                tasks.Add(LoadTriggersAsync());
            }

            if (options.CompareSynonyms)
            {
                tasks.Add(LoadSynonymsAsync());
            }

            if (options.CompareObjects)
            {
                tasks.AddRange(new Task[] { LoadViewsAsync(), LoadUserRoutinesAsync(), LoadUserRoutineDefinitionsAsync() });
            }

            if (options.CompareIndexes)
            {
                foreach (var fullyQualifiedTableName in Tables.Keys)
                {
                    tasks.Add(LoadIndexesAsync(fullyQualifiedTableName));
                }
            }

            return tasks.ToArray();
        }

        private Task[] DependentItemTasks()
        {
            var tasks = new List<Task>();
            if (options.CompareObjects)
            {
                tasks.Add(LoadUserRoutineDefinitionsAsync());
            }

            if (options.CompareIndexes)
            {
                foreach (var fullyQualifiedTableName in Tables.Keys)
                {
                    foreach (var index in Tables[fullyQualifiedTableName].Indexes)
                    {
                        tasks.Add(LoadIncludedColumnsForIndexAsync(index));
                    }
                }
            }

            return tasks.ToArray();
        }

        private async Task LoadFullyQualifiedTableNamesAsync()
        {
            RaiseStatusChanged("Reading tables");
            using var connection = new SqlConnection(this.connectionString);
            using var command = new SqlCommand(LoadQueryFromResource("TableNames"), connection);
            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await dr.ReadAsync())
            {
                Tables.Add($"[{dr.GetString(0)}].[{dr.GetString(1)}]", new SqlTable());
            }
        }

        private async Task LoadIndexesAsync(string fullyQualifiedTableName)
        {
            RaiseStatusChanged($"Reading indexes for table {fullyQualifiedTableName}");
            using var connection = new SqlConnection(this.connectionString);
            using var command = new SqlCommand("sp_helpindex", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@objname", fullyQualifiedTableName);

            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await dr.ReadAsync())
            {
                var index = LoadIndex(dr);
                index.TableSchema = fullyQualifiedTableName.GetSchemaName();
                index.TableName = fullyQualifiedTableName.GetObjectName();

                Tables[fullyQualifiedTableName].Indexes.Add(index);
            }
        }

        private async Task LoadIncludedColumnsForIndexAsync(SqlIndex index)
        {
            RaiseStatusChanged($"Reading index included columns for {index.IndexName}");
            using var connection = new SqlConnection(this.connectionString);
            using var command = new SqlCommand(LoadQueryFromResource("IncludedColumnsForIndex"), connection);
            command.Parameters.AddWithValue("@TableName", index.TableName);
            command.Parameters.AddWithValue("@IndexName", index.IndexName);
            command.Parameters.AddWithValue("@TableSchema", index.TableSchema);

            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await dr.ReadAsync())
            {
                if (dr.GetBoolean(3))
                {
                    index.IncludedColumns.Add(dr.GetString(1), dr.GetBoolean(2));
                }
            }
        }

        private async Task LoadRelationsAsync()
        {
            RaiseStatusChanged("Reading relations");
            using var connection = new SqlConnection(this.connectionString);
            using var command = new SqlCommand(LoadQueryFromResource("Relations"), connection);
            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await dr.ReadAsync())
            {
                var relation = LoadRelation(dr);
                var fullyQualifiedChildTable = relation.ChildTable.PrependSchemaName(relation.ChildSchema);

                if (Tables.ContainsKey(fullyQualifiedChildTable))
                {
                    Tables[fullyQualifiedChildTable].Relations.Add(relation);
                }
            }
        }

        private async Task LoadColumnDetailsAsync()
        {
            RaiseStatusChanged("Reading column details");
            using var connection = new SqlConnection(this.connectionString);
            using var command = new SqlCommand(LoadQueryFromResource("ColumnDetails"), connection);
            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await dr.ReadAsync())
            {
                var detail = LoadColumnDetail(dr);
                var fullyQualifiedTableName = detail.TableName.PrependSchemaName(detail.TableSchema);

                if (Tables.ContainsKey(fullyQualifiedTableName))
                {
                    Tables[fullyQualifiedTableName].ColumnDetails.Add(detail);
                }
            }
        }

        private async Task LoadUserTypesAsync()
        {
            RaiseStatusChanged("Reading user types");
            using var connection = new SqlConnection(this.connectionString);
            using var command = new SqlCommand(LoadQueryFromResource("UserTypes"), connection);
            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await dr.ReadAsync())
            {
                var userType = LoadUserType(dr);
                UserTypes.Add(userType.CustomTypeName.PrependSchemaName(userType.SchemaName), userType);
            }
        }

        private async Task LoadRolePermissionsAsync()
        {
            RaiseStatusChanged("Reading role permissions");
            using var connection = new SqlConnection(this.connectionString);
            using var command = new SqlCommand(LoadQueryFromResource("RolePermissions"), connection);
            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await dr.ReadAsync())
            {
                Permissions.Add(LoadPermission(dr));
            }
        }

        private async Task LoadUserPermissionsAsync()
        {
            RaiseStatusChanged("Reading user permissions");
            using var connection = new SqlConnection(this.connectionString);
            using var command = new SqlCommand(LoadQueryFromResource("UserPermissions"), connection);
            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await dr.ReadAsync())
            {
                Permissions.Add(LoadPermission(dr));
            }
        }

        private async Task LoadExtendedPropertiesAsync()
        {
            RaiseStatusChanged("Reading extended properties");
            using var connection = new SqlConnection(this.connectionString);
            using var command = new SqlCommand(LoadQueryFromResource("ExtendedProperties"), connection);
            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await dr.ReadAsync())
            {
                ExtendedProperties.Add(LoadExtendedProperty(dr));
            }
        }

        private async Task LoadTriggersAsync()
        {
            RaiseStatusChanged("Reading triggers");
            using var connection = new SqlConnection(this.connectionString);
            using var command = new SqlCommand(LoadQueryFromResource("Triggers"), connection);
            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await dr.ReadAsync())
            {
                var trigger = LoadTrigger(dr);
                var fullyQualifiedTableName = trigger.TableName.PrependSchemaName(trigger.TableSchema);

                if (Tables.ContainsKey(fullyQualifiedTableName))
                {
                    Tables[fullyQualifiedTableName].Triggers.Add(trigger);
                }
            }
        }

        private async Task LoadSynonymsAsync()
        {
            RaiseStatusChanged("Reading synonyms");
            using var connection = new SqlConnection(this.connectionString);
            using var command = new SqlCommand(LoadQueryFromResource("Synonyms"), connection);
            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await dr.ReadAsync())
            {
                var i = 0;
                var name = string.Empty;
                var def = string.Empty;
                while (i < dr.FieldCount)
                {
                    switch (dr.GetName(i))
                    {
                        case "SYNONYM_NAME":
                            name = dr.GetString(i);
                            break;
                        case "BASE_OBJECT_NAME":
                            def = dr.GetString(i);
                            break;
                    }

                    i++;
                }

                Synonyms.Add(name, def);
            }
        }

        private async Task LoadViewsAsync()
        {
            RaiseStatusChanged("Reading views");
            using var connection = new SqlConnection(this.connectionString);
            using var command = new SqlCommand(LoadQueryFromResource("Views"), connection);
            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await dr.ReadAsync())
            {
                var i = 0;
                var name = string.Empty;
                var schema = string.Empty;
                var def = string.Empty;
                while (i < dr.FieldCount)
                {
                    switch (dr.GetName(i))
                    {
                        case "VIEW_NAME":
                            name = dr.GetString(i);
                            break;
                        case "TABLE_SCHEMA":
                            schema = dr.GetString(i);
                            break;
                        case "VIEW_DEFINITION":
                            def = dr.GetString(i);
                            break;
                    }

                    i++;
                }

                Views.Add(name.PrependSchemaName(schema), def);
            }
        }

        private async Task LoadUserRoutinesAsync()
        {
            RaiseStatusChanged("Reading user routines");
            using var connection = new SqlConnection(this.connectionString);
            using var command = new SqlCommand(LoadQueryFromResource("UserRoutines"), connection);
            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await dr.ReadAsync())
            {
                var routine = new SqlUserRoutine();
                var name = string.Empty;
                var schema = string.Empty;
                var i = 0;
                while (i < dr.FieldCount)
                {
                    switch (dr.GetName(i))
                    {
                        case "ROUTINE_NAME":
                            name = dr.GetString(i);
                            break;
                        case "ROUTINE_SCHEMA":
                            schema = dr.GetString(i);
                            break;
                        case "ROUTINE_TYPE":
                            routine.RoutineType = dr.GetString(i);
                            break;
                    }

                    i++;
                }

                UserRoutines.Add(name.PrependSchemaName(schema), routine);
            }
        }

        private async Task LoadUserRoutineDefinitionsAsync()
        {
            using var connection = new SqlConnection(this.connectionString);
            using var command = new SqlCommand(LoadQueryFromResource("UserRoutineDefinitions"), connection);
            command.Parameters.Add("@routinename", SqlDbType.VarChar, 128);
            foreach (var routine in UserRoutines.Keys)
            {
                RaiseStatusChanged($"Reading routine definition {Array.IndexOf(UserRoutines.Keys.ToArray(), routine) + 1} of {UserRoutines.Count}");

                command.Parameters["@routinename"].Value = routine.GetObjectName();
                await connection.OpenAsync();
                using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    UserRoutines[routine].RoutineDefinition += dr.GetString(0);
                }
            }
        }

        private static SqlIndex LoadIndex(SqlDataReader dr)
        {
            var index = new SqlIndex();
            string desc;
            var i = 0;
            while (i < dr.FieldCount)
            {
                switch (dr.GetName(i))
                {
                    case "index_name":
                        index.IndexName = dr.GetString(i);
                        break;
                    case "index_keys":
                        index.SetColumnsFromString(dr.GetString(i));
                        break;
                    case "index_description":
                        desc = dr.GetString(i);
                        index.IsPrimaryKey = desc.IndexOf("primary key") >= 0;
                        index.Clustered = desc.IndexOf("nonclustered") == -1;
                        index.Unique = desc.IndexOf("unique") > 0;
                        index.IsUniqueKey = desc.IndexOf("unique key") > 0;
                        index.FileGroup = Regex.Match(desc, "located on  (.*)$").Groups[1].Value;
                        break;
                }
                i++;
            }

            return index;
        }

        private static SqlRelation LoadRelation(SqlDataReader dr)
        {
            var relation = new SqlRelation();
            var i = 0;
            while (i < dr.FieldCount)
            {
                switch (dr.GetName(i))
                {
                    case "RELATION_NAME":
                        relation.RelationName = dr.GetString(i);
                        break;
                    case "CHILD_SCHEMA":
                        relation.ChildSchema = dr.GetString(i);
                        break;
                    case "CHILD_TABLE":
                        relation.ChildTable = dr.GetString(i);
                        break;
                    case "CHILD_COLUMNS":
                        relation.ChildColumns = dr.GetString(i);
                        break;
                    case "UNIQUE_CONSTRAINT_NAME":
                        relation.UniqueConstraintName = dr.GetString(i);
                        break;
                    case "PARENT_SCHEMA":
                        relation.ParentSchema = dr.GetString(i);
                        break;
                    case "PARENT_TABLE":
                        relation.ParentTable = dr.GetString(i);
                        break;
                    case "PARENT_COLUMNS":
                        relation.ParentColumns = dr.GetString(i);
                        break;
                    case "UPDATE_RULE":
                        relation.UpdateRule = dr.GetString(i);
                        break;
                    case "DELETE_RULE":
                        relation.DeleteRule = dr.GetString(i);
                        break;
                }

                i++;
            }

            return relation;
        }

        private static SqlColumnDetail LoadColumnDetail(SqlDataReader dr)
        {
            var i = 0;
            var detail = new SqlColumnDetail();
            while (i < dr.FieldCount)
            {
                switch (dr.GetName(i))
                {
                    case "TABLE_SCHEMA":
                        detail.TableSchema = dr.GetString(i);
                        break;
                    case "TABLE_NAME":
                        detail.TableName = dr.GetString(i);
                        break;
                    case "COLUMN_NAME":
                        detail.ColumnName = dr.GetString(i);
                        break;
                    case "ORDINAL_POSITION":
                        detail.OrdinalPosition = dr.GetInt32(i);
                        break;
                    case "COLUMN_DEFAULT":
                        if (!dr.IsDBNull(i))
                        {
                            detail.ColumnDefault = dr.GetString(i);
                        }
                        break;
                    case "IS_NULLABLE":
                        if (!dr.IsDBNull(i))
                        {
                            detail.IsNullable = dr.GetString(i) == "YES";
                        }
                        break;
                    case "DATA_TYPE":
                        detail.DataType = dr.GetString(i);
                        break;
                    case "CHARACTER_MAXIMUM_LENGTH":
                        if (!dr.IsDBNull(i))
                        {
                            detail.CharacterMaximumLength = dr.GetInt32(i);
                        }
                        break;
                    case "CHARACTER_OCTET_LENGTH":
                        if (!dr.IsDBNull(i))
                        {
                            detail.CharacterOctetLength = dr.GetInt32(i);
                        }
                        break;
                    case "NUMERIC_PRECISION":
                        if (!dr.IsDBNull(i))
                        {
                            detail.NumericPrecision = dr.GetByte(i);
                        }
                        break;
                    case "NUMERIC_PRECISION_RADIX":
                        if (!dr.IsDBNull(i))
                        {
                            detail.NumericPrecisionRadix = dr.GetInt16(i);
                        }
                        break;
                    case "NUMERIC_SCALE":
                        if (!dr.IsDBNull(i))
                        {
                            detail.NumericScale = dr.GetInt32(i);
                        }
                        break;
                    case "DATETIME_PRECISION":
                        if (!dr.IsDBNull(i))
                        {
                            detail.DatetimePrecision = dr.GetInt16(i);
                        }
                        break;
                    case "CHARACTER_SET_NAME":
                        if (!dr.IsDBNull(i))
                        {
                            detail.CharacterSetName = dr.GetString(i);
                        }
                        break;
                    case "COLLATION_NAME":
                        if (!dr.IsDBNull(i))
                        {
                            detail.CollationName = dr.GetString(i);
                        }
                        break;
                    case "DOMAIN_SCHEMA":
                        if (!dr.IsDBNull(i))
                        {
                            detail.DomainSchema = dr.GetString(i);
                        }
                        break;
                    case "DOMAIN_NAME":
                        if (!dr.IsDBNull(i))
                        {
                            detail.DomainName = dr.GetString(i);
                        }
                        break;
                    case "IS_FULL_TEXT_INDEXED":
                        if (!dr.IsDBNull(i))
                        {
                            detail.IsFullTextIndexed = dr.GetInt32(i) == 1;
                        }
                        break;
                    case "IS_COMPUTED":
                        if (!dr.IsDBNull(i))
                        {
                            detail.IsComputed = dr.GetInt32(i) == 1;
                        }
                        break;
                    case "IS_IDENTITY":
                        if (!dr.IsDBNull(i))
                        {
                            detail.IsIdentity = dr.GetInt32(i) == 1;
                        }
                        break;
                    case "IDENTITY_SEED":
                        if (!dr.IsDBNull(i))
                        {
                            detail.IdentitySeed = dr.GetDecimal(i);
                        }
                        break;
                    case "IDENTITY_INCREMENT":
                        if (!dr.IsDBNull(i))
                        {
                            detail.IdentityIncrement = dr.GetDecimal(i);
                        }
                        break;
                    case "IS_SPARSE":
                        if (!dr.IsDBNull(i))
                        {
                            detail.IsSparse = dr.GetInt32(i) == 1;
                        }
                        break;
                    case "IS_COLUMN_SET":
                        if (!dr.IsDBNull(i))
                        {
                            detail.IsColumnSet = dr.GetInt32(i) == 1;
                        }
                        break;
                    case "IS_ROW_GUID":
                        if (!dr.IsDBNull(i))
                        {
                            detail.IsRowGuid = dr.GetInt32(i) == 1;
                        }
                        break;
                }

                i++;
            }

            return detail;
        }

        private static SqlUserType LoadUserType(SqlDataReader dr)
        {
            var i = 0;
            var userType = new SqlUserType();
            while (i < dr.FieldCount)
            {
                switch (dr.GetName(i))
                {
                    case "custom_type_name":
                        userType.CustomTypeName = dr.GetString(i);
                        break;
                    case "schema_name":
                        userType.SchemaName = dr.GetString(i);
                        break;
                    case "underlying_type_name":
                        userType.UnderlyingTypeName = dr.GetString(i);
                        break;
                    case "precision":
                        if (!dr.IsDBNull(i))
                        {
                            userType.Precision = dr.GetInt32(i);
                        }
                        break;
                    case "scale":
                        if (!dr.IsDBNull(i))
                        {
                            userType.Scale = dr.GetInt32(i);
                        }
                        break;
                    case "max_length":
                        if (!dr.IsDBNull(i))
                        {
                            userType.MaxLength = dr.GetInt32(i);
                        }
                        break;
                    case "is_nullable":
                        userType.IsNullable = dr.GetInt32(i) == 1;
                        break;
                    case "collation_name":
                        if (!dr.IsDBNull(i))
                        {
                            userType.CollationName = dr.GetString(i);
                        }
                        break;
                    case "is_assembly_type":
                        userType.IsAssemblyType = dr.GetInt32(i) == 1;
                        break;
                }

                i++;
            }

            return userType;
        }

        private static SqlPermission LoadPermission(SqlDataReader dr)
        {
            var i = 0;
            var permission = new SqlPermission();
            while (i < dr.FieldCount)
            {
                switch (dr.GetName(i))
                {
                    case "USER_NAME":
                        permission.UserName = dr.GetString(i);
                        break;
                    case "ROLE_NAME":
                        if (!dr.IsDBNull(i))
                        {
                            permission.RoleName = dr.GetString(i);
                        }
                        break;
                    case "PERMISSION_TYPE":
                        if (!dr.IsDBNull(i))
                        {
                            permission.PermissionType = dr.GetString(i);
                        }
                        break;
                    case "PERMISSION_STATE":
                        if (!dr.IsDBNull(i))
                        {
                            permission.PermissionState = dr.GetString(i);
                        }
                        break;
                    case "OBJECT_TYPE":
                        if (!dr.IsDBNull(i))
                        {
                            permission.ObjectType = dr.GetString(i);
                        }
                        break;
                    case "OBJECT_NAME":
                        if (!dr.IsDBNull(i))
                        {
                            permission.ObjectName = dr.GetString(i);
                        }
                        break;
                    case "COLUMN_NAME":
                        if (!dr.IsDBNull(i))
                        {
                            permission.ColumnName = dr.GetString(i);
                        }
                        break;
                    case "OBJECT_SCHEMA":
                        if (!dr.IsDBNull(i))
                        {
                            permission.ObjectSchema = dr.GetString(i);
                        }
                        break;
                }

                i++;
            }

            return permission;
        }

        private static SqlExtendedProperty LoadExtendedProperty(SqlDataReader dr)
        {
            var i = 0;
            var property = new SqlExtendedProperty();
            while (i < dr.FieldCount)
            {
                switch (dr.GetName(i))
                {
                    case "PROPERTY_TYPE":
                        property.PropertyType = dr.GetString(i);
                        break;
                    case "OBJECT_NAME":
                        if (!dr.IsDBNull(i))
                        {
                            property.ObjectName = dr.GetString(i);
                        }
                        break;
                    case "OBJECT_SCHEMA":
                        if (!dr.IsDBNull(i))
                        {
                            property.ObjectSchema = dr.GetString(i);
                        }
                        break;
                    case "COLUMN_NAME":
                        if (!dr.IsDBNull(i))
                        {
                            property.ColumnName = dr.GetString(i);
                        }
                        break;
                    case "PROPERTY_NAME":
                        property.PropertyName = dr.GetString(i);
                        break;
                    case "PROPERTY_VALUE":
                        if (!dr.IsDBNull(i))
                        {
                            property.PropertyValue = dr.GetString(i);
                        }
                        break;
                    case "INDEX_NAME":
                        if (!dr.IsDBNull(i))
                        {
                            property.IndexName = dr.GetString(i);
                        }
                        break;
                    case "TABLE_NAME":
                        if (!dr.IsDBNull(i))
                        {
                            property.TableName = dr.GetString(i);
                        }
                        break;
                    case "TABLE_SCHEMA":
                        if (!dr.IsDBNull(i))
                        {
                            property.TableSchema = dr.GetString(i);
                        }
                        break;
                }

                i++;
            }

            return property;
        }

        private static SqlTrigger LoadTrigger(SqlDataReader dr)
        {
            var i = 0;
            var trigger = new SqlTrigger();
            while (i < dr.FieldCount)
            {
                switch (dr.GetName(i))
                {
                    case "TRIGGER_NAME":
                        trigger.TriggerName = dr.GetString(i);
                        break;
                    case "TRIGGER_OWNER":
                        if (!dr.IsDBNull(i))
                        {
                            trigger.TriggerOwner = dr.GetString(i);
                        }
                        break;
                    case "TABLE_SCHEMA":
                        if (!dr.IsDBNull(i))
                        {
                            trigger.TableSchema = dr.GetString(i);
                        }
                        break;
                    case "TABLE_NAME":
                        if (!dr.IsDBNull(i))
                        {
                            trigger.TableName = dr.GetString(i);
                        }
                        break;
                    case "IS_UPDATE":
                        trigger.IsUpdate = dr.GetInt32(i) == 1;
                        break;
                    case "IS_DELETE":
                        trigger.IsDelete = dr.GetInt32(i) == 1;
                        break;
                    case "IS_INSERT":
                        trigger.IsInsert = dr.GetInt32(i) == 1;
                        break;
                    case "IS_AFTER":
                        trigger.IsAfter = dr.GetInt32(i) == 1;
                        break;
                    case "IS_INSTEAD_OF":
                        trigger.IsInsteadOf = dr.GetInt32(i) == 1;
                        break;
                    case "IS_DISABLED":
                        trigger.IsDisabled = dr.GetInt32(i) == 1;
                        break;
                    case "TRIGGER_CONTENT":
                        trigger.TriggerContent = dr.GetString(i);
                        break;
                }

                i++;
            }

            return trigger;
        }
    }
}
