// <copyright file="SqlDatabase.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using QuickCompareModel.Models;

/// <summary>
/// Class for running database queries and building lists that detail the content of the database schema.
/// </summary>
/// <remarks>
/// Initialises a new instance of the <see cref="SqlDatabase"/> class with a connection string and setting options.
/// </remarks>
/// <param name="connectionString">The database connection string for the current instance being inspected.</param>
/// <param name="options">Collection of configuration settings.</param>
public partial class SqlDatabase(string connectionString, QuickCompareOptions options)
{
    private readonly string connectionString = connectionString;
    private readonly QuickCompareOptions options = options;

    /// <summary>
    /// Initialises a new instance of the <see cref="SqlDatabase"/> class with a connection string.
    /// </summary>
    /// <param name="connectionString">The database connection string for the current instance being inspected.</param>
    public SqlDatabase(string connectionString)
        : this(connectionString, new QuickCompareOptions())
    {
    }

    /// <summary> Handler for when the status message changes. </summary>
    public event EventHandler<StatusChangedEventArgs> LoaderStatusChanged;

    /// <summary>
    /// Gets a friendly name for the database instance, including both server name and database name.
    /// </summary>
    public string FriendlyName
    {
        get
        {
            var builder = new SqlConnectionStringBuilder(this.connectionString);
            return $"[{builder.DataSource}].[{builder.InitialCatalog}]";
        }
    }

    /// <summary> Gets or sets a list of <see cref="SqlTable"/> instances, indexed by table name. </summary>
    public Dictionary<string, SqlTable> Tables { get; set; } = [];

    /// <summary> Gets or sets a list of <see cref="SqlUserType"/> instances, indexed by name. </summary>
    public Dictionary<string, SqlUserType> UserTypes { get; set; } = [];

    /// <summary> Gets or sets a list of database views, indexed by view name. </summary>
    public Dictionary<string, string> Views { get; set; } = [];

    /// <summary> Gets or sets a list of SQL synonyms, indexed by synonym name. </summary>
    public Dictionary<string, string> Synonyms { get; set; } = [];

    /// <summary> Gets or sets a list of <see cref="SqlUserRoutine"/> instances, indexed by routine name. </summary>
    /// <remarks> User routines include views, functions and stored procedures. </remarks>
    public Dictionary<string, SqlUserRoutine> UserRoutines { get; set; } = [];

    /// <summary> Gets or sets a list of <see cref="SqlPermission"/> instances, for both roles and users. </summary>
    public List<SqlPermission> Permissions { get; set; } = [];

    /// <summary> Gets or sets a list of <see cref="SqlExtendedProperty"/> instances for the database itself. </summary>
    public List<SqlExtendedProperty> ExtendedProperties { get; set; } = [];

    /// <summary>
    /// Helper method to return embedded SQL resource by filename.
    /// </summary>
    /// <param name="queryName">Name of the SQL file without the extension.</param>
    /// <returns>SQL query text.</returns>
    public static string LoadQueryFromResource(string queryName)
    {
        var resourceName = $"{nameof(QuickCompareModel)}.{nameof(DatabaseSchema)}.Queries.{queryName}.sql";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        return stream != null ? StreamToString(stream) : string.Empty;
    }

    /// <summary>
    /// Populate the models based on the supplied connection string.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task PopulateSchemaModelAsync()
    {
        this.RaiseStatusChanged("Connecting");

        await this.LoadFullyQualifiedTableNamesAsync();
        await Task.WhenAll(this.RequiredItemTasks());
        await Task.WhenAll(this.DependentItemTasks());

        this.RaiseStatusChanged("Done");
    }

    /// <summary>
    /// Raises the status changed event.
    /// </summary>
    /// <param name="message">Status message.</param>
    protected virtual void RaiseStatusChanged(string message) =>
        this.LoaderStatusChanged?.Invoke(this, new StatusChangedEventArgs(message));

    private static string StreamToString(Stream stream)
    {
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
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
                    index.IsPrimaryKey = desc.Contains("primary key");
                    index.Clustered = !desc.Contains("nonclustered");
                    index.Unique = desc.Contains("unique");
                    index.IsUniqueKey = desc.Contains("unique key");
                    index.FileGroup = FileGroupRegex().Match(desc).Groups[1].Value;
                    break;
                default:
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
                default:
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
                    detail.ColumnDefault = dr.GetNullableString(i);
                    break;
                case "IS_NULLABLE":
                    detail.IsNullable = !dr.IsDBNull(i) && dr.GetString(i) == "YES";
                    break;
                case "DATA_TYPE":
                    detail.DataType = dr.GetString(i);
                    break;
                case "CHARACTER_MAXIMUM_LENGTH":
                    detail.CharacterMaximumLength = dr.GetNullableInt32(i);
                    break;
                case "CHARACTER_OCTET_LENGTH":
                    detail.CharacterOctetLength = dr.GetNullableInt32(i);
                    break;
                case "NUMERIC_PRECISION":
                    detail.NumericPrecision = dr.GetNullableByte(i);
                    break;
                case "NUMERIC_PRECISION_RADIX":
                    detail.NumericPrecisionRadix = dr.GetNullableInt16(i);
                    break;
                case "NUMERIC_SCALE":
                    detail.NumericScale = dr.GetNullableInt32(i);
                    break;
                case "DATETIME_PRECISION":
                    detail.DatetimePrecision = dr.GetNullableInt16(i);
                    break;
                case "CHARACTER_SET_NAME":
                    detail.CharacterSetName = dr.GetNullableString(i);
                    break;
                case "COLLATION_NAME":
                    detail.CollationName = dr.GetNullableString(i);
                    break;
                case "DOMAIN_SCHEMA":
                    detail.DomainSchema = dr.GetNullableString(i);
                    break;
                case "DOMAIN_NAME":
                    detail.DomainName = dr.GetNullableString(i);
                    break;
                case "IS_FULL_TEXT_INDEXED":
                    detail.IsFullTextIndexed = dr.GetNullableInt32AsBoolean(i);
                    break;
                case "IS_COMPUTED":
                    detail.IsComputed = dr.GetNullableInt32AsBoolean(i);
                    break;
                case "IS_IDENTITY":
                    detail.IsIdentity = dr.GetNullableInt32AsBoolean(i);
                    break;
                case "IDENTITY_SEED":
                    detail.IdentitySeed = dr.GetNullableDecimal(i);
                    break;
                case "IDENTITY_INCREMENT":
                    detail.IdentityIncrement = dr.GetNullableDecimal(i);
                    break;
                case "IS_SPARSE":
                    detail.IsSparse = dr.GetNullableInt32AsBoolean(i);
                    break;
                case "IS_COLUMN_SET":
                    detail.IsColumnSet = dr.GetNullableInt32AsBoolean(i);
                    break;
                case "IS_ROW_GUID":
                    detail.IsRowGuid = dr.GetNullableInt32AsBoolean(i);
                    break;
                default:
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
                    userType.Precision = dr.GetNullableInt32(i);
                    break;
                case "scale":
                    userType.Scale = dr.GetNullableInt32(i);
                    break;
                case "max_length":
                    userType.MaxLength = dr.GetNullableInt32(i);
                    break;
                case "is_nullable":
                    userType.IsNullable = dr.GetInt32AsBoolean(i);
                    break;
                case "collation_name":
                    userType.CollationName = dr.GetNullableString(i);
                    break;
                case "is_assembly_type":
                    userType.IsAssemblyType = dr.GetInt32AsBoolean(i);
                    break;
                default:
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
                    permission.RoleName = dr.GetNullableString(i);
                    break;
                case "PERMISSION_TYPE":
                    permission.PermissionType = dr.GetNullableString(i);
                    break;
                case "PERMISSION_STATE":
                    permission.PermissionState = dr.GetNullableString(i);
                    break;
                case "OBJECT_TYPE":
                    permission.ObjectType = dr.GetNullableString(i);
                    break;
                case "OBJECT_NAME":
                    permission.ObjectName = dr.GetNullableString(i);
                    break;
                case "COLUMN_NAME":
                    permission.ColumnName = dr.GetNullableString(i);
                    break;
                case "OBJECT_SCHEMA":
                    permission.ObjectSchema = dr.GetNullableString(i);
                    break;
                default:
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
                    property.ObjectName = dr.GetNullableString(i);
                    break;
                case "OBJECT_SCHEMA":
                    property.ObjectSchema = dr.GetNullableString(i);
                    break;
                case "COLUMN_NAME":
                    property.ColumnName = dr.GetNullableString(i);
                    break;
                case "PROPERTY_NAME":
                    property.PropertyName = dr.GetString(i);
                    break;
                case "PROPERTY_VALUE":
                    property.PropertyValue = dr.GetNullableString(i);
                    break;
                case "INDEX_NAME":
                    property.IndexName = dr.GetNullableString(i);
                    break;
                case "TABLE_NAME":
                    property.TableName = dr.GetNullableString(i);
                    break;
                case "TABLE_SCHEMA":
                    property.TableSchema = dr.GetNullableString(i);
                    break;
                default:
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
                    trigger.TriggerOwner = dr.GetNullableString(i);
                    break;
                case "TABLE_SCHEMA":
                    trigger.TableSchema = dr.GetNullableString(i);
                    break;
                case "TABLE_NAME":
                    trigger.TableName = dr.GetNullableString(i);
                    break;
                case "IS_UPDATE":
                    trigger.IsUpdate = dr.GetInt32AsBoolean(i);
                    break;
                case "IS_DELETE":
                    trigger.IsDelete = dr.GetInt32AsBoolean(i);
                    break;
                case "IS_INSERT":
                    trigger.IsInsert = dr.GetInt32AsBoolean(i);
                    break;
                case "IS_AFTER":
                    trigger.IsAfter = dr.GetInt32AsBoolean(i);
                    break;
                case "IS_INSTEAD_OF":
                    trigger.IsInsteadOf = dr.GetInt32AsBoolean(i);
                    break;
                case "IS_DISABLED":
                    trigger.IsDisabled = dr.GetInt32AsBoolean(i);
                    break;
                case "TRIGGER_CONTENT":
                    trigger.TriggerContent = dr.GetString(i);
                    break;
                default:
                    break;
            }

            i++;
        }

        return trigger;
    }

    [GeneratedRegex("located on (.*)$")] private static partial Regex FileGroupRegex();

    private Task[] RequiredItemTasks()
    {
        var tasks = new List<Task>() { this.LoadRelationsAsync(), this.LoadColumnDetailsAsync() };

        if (this.options.CompareUserTypes)
        {
            tasks.Add(this.LoadUserTypesAsync());
        }

        if (this.options.ComparePermissions)
        {
            tasks.AddRange(new Task[] { this.LoadRolePermissionsAsync(), this.LoadUserPermissionsAsync() });
        }

        if (this.options.CompareProperties)
        {
            tasks.Add(this.LoadExtendedPropertiesAsync());
        }

        if (this.options.CompareTriggers)
        {
            tasks.Add(this.LoadTriggersAsync());
        }

        if (this.options.CompareSynonyms)
        {
            tasks.Add(this.LoadSynonymsAsync());
        }

        if (this.options.CompareObjects)
        {
            tasks.AddRange(new Task[] { this.LoadViewsAsync(), this.LoadUserRoutinesAsync(), this.LoadUserRoutineDefinitionsAsync() });
        }

        if (this.options.CompareIndexes)
        {
            foreach (var fullyQualifiedTableName in this.Tables.Keys)
            {
                tasks.Add(this.LoadIndexesAsync(fullyQualifiedTableName));
            }
        }

        return [.. tasks];
    }

    private Task[] DependentItemTasks()
    {
        var tasks = new List<Task>();
        if (this.options.CompareObjects)
        {
            tasks.Add(this.LoadUserRoutineDefinitionsAsync());
        }

        if (this.options.CompareIndexes)
        {
            foreach (var fullyQualifiedTableName in this.Tables.Keys)
            {
                foreach (var index in this.Tables[fullyQualifiedTableName].Indexes)
                {
                    tasks.Add(this.LoadIncludedColumnsForIndexAsync(index));
                }
            }
        }

        return [.. tasks];
    }

    private async Task LoadFullyQualifiedTableNamesAsync()
    {
        this.RaiseStatusChanged("Reading tables");
        using var connection = new SqlConnection(this.connectionString);
        using var command = new SqlCommand(LoadQueryFromResource("TableNames"), connection);
        await connection.OpenAsync();
        using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        while (await dr.ReadAsync())
        {
            this.Tables.Add($"[{dr.GetString(0)}].[{dr.GetString(1)}]", new SqlTable());
        }
    }

    private async Task LoadIndexesAsync(string fullyQualifiedTableName)
    {
        this.RaiseStatusChanged($"Reading indexes for table {fullyQualifiedTableName}");
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

            this.Tables[fullyQualifiedTableName].Indexes.Add(index);
        }
    }

    private async Task LoadIncludedColumnsForIndexAsync(SqlIndex index)
    {
        this.RaiseStatusChanged($"Reading index included columns for {index.IndexName}");
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
        this.RaiseStatusChanged("Reading relations");
        using var connection = new SqlConnection(this.connectionString);
        using var command = new SqlCommand(LoadQueryFromResource("Relations"), connection);
        await connection.OpenAsync();
        using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        while (await dr.ReadAsync())
        {
            var relation = LoadRelation(dr);
            var fullyQualifiedChildTable = relation.ChildTable.PrependSchemaName(relation.ChildSchema);

            if (this.Tables.TryGetValue(fullyQualifiedChildTable, out var table))
            {
                table.Relations.Add(relation);
            }
        }
    }

    private async Task LoadColumnDetailsAsync()
    {
        this.RaiseStatusChanged("Reading column details");
        using var connection = new SqlConnection(this.connectionString);
        using var command = new SqlCommand(LoadQueryFromResource("ColumnDetails"), connection);
        await connection.OpenAsync();
        using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        while (await dr.ReadAsync())
        {
            var detail = LoadColumnDetail(dr);
            var fullyQualifiedTableName = detail.TableName.PrependSchemaName(detail.TableSchema);

            if (this.Tables.TryGetValue(fullyQualifiedTableName, out var table))
            {
                table.ColumnDetails.Add(detail);
            }
        }
    }

    private async Task LoadUserTypesAsync()
    {
        this.RaiseStatusChanged("Reading user types");
        using var connection = new SqlConnection(this.connectionString);
        using var command = new SqlCommand(LoadQueryFromResource("UserTypes"), connection);
        await connection.OpenAsync();
        using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        while (await dr.ReadAsync())
        {
            var userType = LoadUserType(dr);
            this.UserTypes.Add(userType.CustomTypeName.PrependSchemaName(userType.SchemaName), userType);
        }
    }

    private async Task LoadRolePermissionsAsync()
    {
        this.RaiseStatusChanged("Reading role permissions");
        using var connection = new SqlConnection(this.connectionString);
        using var command = new SqlCommand(LoadQueryFromResource("RolePermissions"), connection);
        await connection.OpenAsync();
        using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        while (await dr.ReadAsync())
        {
            this.Permissions.Add(LoadPermission(dr));
        }
    }

    private async Task LoadUserPermissionsAsync()
    {
        this.RaiseStatusChanged("Reading user permissions");
        using var connection = new SqlConnection(this.connectionString);
        using var command = new SqlCommand(LoadQueryFromResource("UserPermissions"), connection);
        await connection.OpenAsync();
        using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        while (await dr.ReadAsync())
        {
            this.Permissions.Add(LoadPermission(dr));
        }
    }

    private async Task LoadExtendedPropertiesAsync()
    {
        this.RaiseStatusChanged("Reading extended properties");
        using var connection = new SqlConnection(this.connectionString);
        using var command = new SqlCommand(LoadQueryFromResource("ExtendedProperties"), connection);
        await connection.OpenAsync();
        using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        while (await dr.ReadAsync())
        {
            this.ExtendedProperties.Add(LoadExtendedProperty(dr));
        }
    }

    private async Task LoadTriggersAsync()
    {
        this.RaiseStatusChanged("Reading triggers");
        using var connection = new SqlConnection(this.connectionString);
        using var command = new SqlCommand(LoadQueryFromResource("Triggers"), connection);
        await connection.OpenAsync();
        using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        while (await dr.ReadAsync())
        {
            var trigger = LoadTrigger(dr);
            var fullyQualifiedTableName = trigger.TableName.PrependSchemaName(trigger.TableSchema);

            if (this.Tables.TryGetValue(fullyQualifiedTableName, out var table))
            {
                table.Triggers.Add(trigger);
            }
        }
    }

    private async Task LoadSynonymsAsync()
    {
        this.RaiseStatusChanged("Reading synonyms");
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

            this.Synonyms.Add(name, def);
        }
    }

    private async Task LoadViewsAsync()
    {
        this.RaiseStatusChanged("Reading views");
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

            this.Views.Add(name.PrependSchemaName(schema), def);
        }
    }

    private async Task LoadUserRoutinesAsync()
    {
        this.RaiseStatusChanged("Reading user routines");
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
                    default:
                        break;
                }

                i++;
            }

            this.UserRoutines.Add(name.PrependSchemaName(schema), routine);
        }
    }

    private async Task LoadUserRoutineDefinitionsAsync()
    {
        using var connection = new SqlConnection(this.connectionString);
        using var command = new SqlCommand(LoadQueryFromResource("UserRoutineDefinitions"), connection);
        command.Parameters.Add("@routinename", SqlDbType.VarChar, 128);
        var sb = new StringBuilder();
        foreach (var routine in this.UserRoutines.Keys)
        {
            this.RaiseStatusChanged($"Reading routine definition {Array.IndexOf([.. this.UserRoutines.Keys], routine) + 1} of {this.UserRoutines.Count}");

            command.Parameters["@routinename"].Value = routine.GetObjectName();
            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await dr.ReadAsync())
            {
                sb.Append(dr.GetString(0));
            }

            this.UserRoutines[routine].RoutineDefinition = sb.ToString();
            sb.Clear();
        }
    }
}
