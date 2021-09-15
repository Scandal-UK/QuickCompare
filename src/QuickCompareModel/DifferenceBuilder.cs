﻿namespace QuickCompareModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using QuickCompareModel.DatabaseDifferences;
    using QuickCompareModel.DatabaseSchema;
    using QuickCompareModel.DatabaseSchema.Enums;

    /// <summary>
    /// Class responsible for building a set of differences between two database instances.
    /// </summary>
    public class DifferenceBuilder : IDifferenceBuilder
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="DifferenceBuilder"/> class.
        /// </summary>
        /// <param name="options">Option settings for the database comparison.</param>
        public DifferenceBuilder(IOptions<QuickCompareOptions> options) =>
            this.Options = options.Value ?? throw new ArgumentNullException(nameof(options));

        /// <summary>
        /// Initialises a new instance of the <see cref="DifferenceBuilder"/> class with ready <see cref="SqlDatabase"/> instances.
        /// </summary>
        /// <param name="options">Option settings for the database comparison.</param>
        /// <param name="database1">Instance of <see cref="SqlDatabase"/> representing the first database to compare.</param>
        /// <param name="database2">Instance of <see cref="SqlDatabase"/> representing the second database to compare.</param>
        public DifferenceBuilder(IOptions<QuickCompareOptions> options, SqlDatabase database1, SqlDatabase database2)
            : this(options)
        {
            this.Database1 = database1;
            this.Database2 = database2;
        }

        /// <inheritdoc/>
        public QuickCompareOptions Options { get; set; }

        /// <inheritdoc/>
        public SqlDatabase Database1 { get; set; }

        /// <inheritdoc/>
        public SqlDatabase Database2 { get; set; }

        /// <inheritdoc/>
        public event EventHandler<StatusChangedEventArgs> ComparisonStatusChanged;

        /// <inheritdoc/>
        public Differences Differences { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, (string, string)> DefinitionDifferences { get; set; } = new Dictionary<string, (string, string)>();

        /// <inheritdoc/>
        public async Task BuildDifferencesAsync()
        {
            if (Database1 == null)
            {
                await LoadDatabaseSchemas();
            }

            Differences = new Differences
            {
                Database1 = this.Database1.FriendlyName,
                Database2 = this.Database2.FriendlyName,
            };

            RaiseStatusChanged("Inspecting differences");

            if (this.Options.CompareProperties)
            {
                InspectDatabaseExtendedProperties();
            }

            if (this.Options.ComparePermissions)
            {
                InspectDatabasePermissions();
            }

            InspectTables();
            InspectTableDifferences();

            if (this.Options.CompareUserTypes)
            {
                InspectUserTypes();
            }

            if (this.Options.CompareSynonyms)
            {
                InspectSynonyms();
            }

            if (this.Options.CompareObjects)
            {
                InspectViews();
                InspectRoutines();
            }

            RaiseStatusChanged("Difference inspection completed...");
        }

        protected virtual void RaiseStatusChanged(string message) =>
            this.ComparisonStatusChanged?.Invoke(this, new StatusChangedEventArgs(message));

        protected virtual void RaiseStatusChanged(string message, DatabaseInstance databaseInstance) =>
            this.ComparisonStatusChanged?.Invoke(this, new StatusChangedEventArgs(message, databaseInstance));

        private async Task LoadDatabaseSchemas()
        {
            if (string.IsNullOrEmpty(this.Options.ConnectionString1) || string.IsNullOrEmpty(this.Options.ConnectionString2))
            {
                throw new InvalidOperationException("Connection strings must be set");
            }

            Database1 = new SqlDatabase(this.Options.ConnectionString1, this.Options);
            Database2 = new SqlDatabase(this.Options.ConnectionString2, this.Options);

            if (Database1.FriendlyName.ToLower() == Database2.FriendlyName.ToLower())
            {
                throw new InvalidOperationException("Connection strings must target different database instances");
            }

            Database1.LoaderStatusChanged += (object sender, StatusChangedEventArgs e) =>
                RaiseStatusChanged(e.StatusMessage, DatabaseInstance.Database1);

            Database2.LoaderStatusChanged += (object sender, StatusChangedEventArgs e) =>
                RaiseStatusChanged(e.StatusMessage, DatabaseInstance.Database2);

            await Task.WhenAll(Database1.PopulateSchemaModelAsync(), Database2.PopulateSchemaModelAsync());
        }

        private void InspectDatabaseExtendedProperties()
        {
            foreach (var property1 in Database1.ExtendedProperties.Where(x => x.Type == PropertyObjectType.Database))
            {
                var diff = new ExtendedPropertyDifference(true, false);

                var property2 = Database2.ExtendedProperties.Where(x => x.FullId == property1.FullId).FirstOrDefault();
                if (property2 != null)
                {
                    diff.ExistsInDatabase2 = true;
                    diff.Value1 = property1.PropertyValue;
                    diff.Value2 = property2.PropertyValue;
                }

                Differences.ExtendedPropertyDifferences.Add(property1.PropertyName, diff);
            }

            foreach (var property2 in Database2.ExtendedProperties.Where(x => x.Type == PropertyObjectType.Database && !Differences.ExtendedPropertyDifferences.ContainsKey(x.PropertyName)))
            {
                Differences.ExtendedPropertyDifferences.Add(property2.PropertyName, new ExtendedPropertyDifference(false, true));
            }
        }

        private void InspectDatabasePermissions()
        {
            foreach (var permission1 in Database1.Permissions.Where(x => x.Type == PermissionObjectType.Database))
            {
                Differences.PermissionDifferences.Add(
                    permission1.ToString(),
                    new BaseDifference(true, Database2.Permissions.Exists(x => x.FullId == permission1.FullId)));
            }

            foreach (var permission2 in Database2.Permissions.Where(x => x.Type == PermissionObjectType.Database && !Differences.PermissionDifferences.ContainsKey(x.ToString())))
            {
                Differences.PermissionDifferences.Add(permission2.ToString(), new BaseDifference(false, true));
            }
        }

        private void InspectTables()
        {
            foreach (var table1 in Database1.Tables.Keys)
            {
                Differences.TableDifferences.Add(table1, new TableDifference(true, Database2.Tables.Keys.Any(x => x == table1)));
            }

            foreach (var table2 in Database2.Tables.Keys.Where(x => !Differences.TableDifferences.ContainsKey(x)))
            {
                Differences.TableDifferences.Add(table2, new TableDifference(false, true));
            }
        }

        private void InspectTableDifferences()
        {
            foreach (var fullyQualifiedTableName in Differences.TableDifferences.Keys.Where(x => Differences.TableDifferences[x].ExistsInBothDatabases))
            {
                if (this.Options.CompareColumns)
                {
                    InspectTableColumns(fullyQualifiedTableName);
                }

                if (this.Options.CompareIndexes)
                {
                    InspectIndexes(fullyQualifiedTableName);
                }

                if (this.Options.CompareRelations)
                {
                    InspectRelations(fullyQualifiedTableName);
                }

                if (this.Options.ComparePermissions)
                {
                    InspectTablePermissions(fullyQualifiedTableName);
                }

                if (this.Options.CompareProperties)
                {
                    InspectTableProperties(fullyQualifiedTableName);
                }

                if (this.Options.CompareTriggers)
                {
                    InspectTriggers(fullyQualifiedTableName);
                }
            }
        }

        private void InspectTableColumns(string fullyQualifiedTableName)
        {
            foreach (var column1 in Database1.Tables[fullyQualifiedTableName].ColumnDetails)
            {
                var diff = new ItemWithPropertiesDifference(true, false);

                var column2 = Database2.Tables[fullyQualifiedTableName].ColumnDetails.Where(x => x.ColumnName == column1.ColumnName).FirstOrDefault();
                if (column2 != null)
                {
                    InspectColumns(fullyQualifiedTableName, diff, column1, column2);
                }

                Differences.TableDifferences[fullyQualifiedTableName].ColumnDifferences.Add(column1.ColumnName, diff);
            }

            foreach (var column2 in Database2.Tables[fullyQualifiedTableName].ColumnDetails.Where(x => !Differences.TableDifferences[fullyQualifiedTableName].ColumnDifferences.ContainsKey(x.ColumnName)))
            {
                Differences.TableDifferences[fullyQualifiedTableName].ColumnDifferences.Add(column2.ColumnName, new ItemWithPropertiesDifference(false, true));
            }
        }

        private void InspectColumns(string fullyQualifiedTableName, ItemWithPropertiesDifference diff, SqlColumnDetail column1, SqlColumnDetail column2)
        {
            if (this.Options.CompareOrdinalPositions && column2.OrdinalPosition != column1.OrdinalPosition)
            {
                diff.Differences.Add($"has different ordinal position - is {column1.OrdinalPosition} in database 1 and is {column2.OrdinalPosition} in database 2");
            }

            if (column2.ColumnDefault != column1.ColumnDefault)
            {
                diff.Differences.Add($"has different default value - is {column1.ColumnDefault} in database 1 and is {column2.ColumnDefault} in database 2");
            }

            if (column2.IsNullable != column1.IsNullable)
            {
                diff.Differences.Add($"is {(column1.IsNullable ? string.Empty : "not ")}allowed null in database 1 and is {(column2.IsNullable ? string.Empty : "not ")}allowed null in database 2");
            }

            if (column2.DataType != column1.DataType)
            {
                diff.Differences.Add($"has different data type - is {column1.DataType} in database 1 and is {column2.DataType} in database 2");
            }

            if (column2.CharacterMaximumLength != column1.CharacterMaximumLength)
            {
                diff.Differences.Add($"has different max length - is {(column1.CharacterMaximumLength.HasValue ? column1.CharacterMaximumLength.Value.ToString("n0") : "NULL")} in database 1 and is {(column2.CharacterMaximumLength.HasValue ? column2.CharacterMaximumLength.Value.ToString("n0") : "NULL")} in database 2");
            }

            if (column2.CharacterOctetLength != column1.CharacterOctetLength)
            {
                diff.Differences.Add($"has different character octet length - is {(column1.CharacterOctetLength.HasValue ? column1.CharacterOctetLength.Value.ToString("n0") : "NULL")} in database 1 and is {(column2.CharacterOctetLength.HasValue ? column2.CharacterOctetLength.Value.ToString("n0") : "NULL")} in database 2");
            }

            if (column2.NumericPrecision != column1.NumericPrecision)
            {
                diff.Differences.Add($"has different numeric precision - is {(column1.NumericPrecision.HasValue ? column1.NumericPrecision.Value.ToString() : "NULL")} in database 1 and is {(column2.NumericPrecision.HasValue ? column2.NumericPrecision.Value.ToString() : "NULL")} in database 2");
            }

            if (column2.NumericPrecisionRadix != column1.NumericPrecisionRadix)
            {
                diff.Differences.Add($"has different numeric precision radix - is {(column1.NumericPrecisionRadix.HasValue ? column1.NumericPrecisionRadix.Value.ToString() : "NULL")} in database 1 and is {(column2.NumericPrecisionRadix.HasValue ? column2.NumericPrecisionRadix.Value.ToString() : "NULL")} in database 2");
            }

            if (column2.NumericScale != column1.NumericScale)
            {
                diff.Differences.Add($"has different numeric scale - is {(column1.NumericScale.HasValue ? column1.NumericScale.Value.ToString() : "NULL")} in database 1 and is {(column2.NumericScale.HasValue ? column2.NumericScale.Value.ToString() : "NULL")} in database 2");
            }

            if (column2.DatetimePrecision != column1.DatetimePrecision)
            {
                diff.Differences.Add($"has different datetime precision - is {(column1.DatetimePrecision.HasValue ? column1.DatetimePrecision.Value.ToString() : "NULL")} in database 1 and is {(column2.DatetimePrecision.HasValue ? column2.DatetimePrecision.Value.ToString() : "NULL")} in database 2");
            }

            if (column2.CharacterSetName != column1.CharacterSetName)
            {
                diff.Differences.Add($"has different character set - is {(string.IsNullOrEmpty(column1.CharacterSetName) ? "NULL" : column1.CharacterSetName)} in database 1 and is {(string.IsNullOrEmpty(column2.CharacterSetName) ? "NULL" : column2.CharacterSetName)} in database 2");
            }

            if (column2.CollationName != column1.CollationName)
            {
                diff.Differences.Add($"has different collation - is {(string.IsNullOrEmpty(column1.CollationName) ? "NULL" : column1.CollationName)} in database 1 and is {(string.IsNullOrEmpty(column2.CollationName) ? "NULL" : column2.CollationName)} in database 2");
            }

            if (column2.IsFullTextIndexed != column1.IsFullTextIndexed)
            {
                diff.Differences.Add($"is{(column1.IsFullTextIndexed ? string.Empty : " not")} full-text indexed in database 1 and is{(column2.IsFullTextIndexed ? string.Empty : " not")} full-text indexed in database 2");
            }

            if (column2.IsComputed != column1.IsComputed)
            {
                diff.Differences.Add($"is{(column1.IsComputed ? string.Empty : " not")} computed in database 1 and is{(column2.IsComputed ? string.Empty : " not")} computed in database 2");
            }

            if (column2.IsIdentity != column1.IsIdentity)
            {
                diff.Differences.Add($"is{(column1.IsIdentity ? string.Empty : " not")} an identity column in database 1 and is{(column2.IsIdentity ? string.Empty : " not")} an identity column in database 2");
            }

            if (column2.IsIdentity && column1.IsIdentity)
            {
                if (column2.IdentitySeed != column1.IdentitySeed)
                {
                    diff.Differences.Add($"has different identity seed - is [{column1.IdentitySeed}] in database 1 and is [{column2.IdentitySeed}] in database 2");
                }

                if (column2.IdentityIncrement != column1.IdentityIncrement)
                {
                    diff.Differences.Add($"has different identity increment - is [{column1.IdentityIncrement}] in database 1 and is [{column2.IdentityIncrement}] in database 2");
                }
            }

            if (column2.IsSparse != column1.IsSparse)
            {
                diff.Differences.Add($"is{(column1.IsSparse ? string.Empty : " not")} sparse in database 1 and is{(column2.IsSparse ? string.Empty : " not")} sparse in database 2");
            }

            if (column2.IsColumnSet != column1.IsColumnSet)
            {
                diff.Differences.Add($"is{(column1.IsColumnSet ? string.Empty : " not")} a column-set in database 1 and is{(column2.IsColumnSet ? string.Empty : " not")} a column-set in database 2");
            }

            if (column2.IsRowGuid != column1.IsRowGuid)
            {
                diff.Differences.Add($"is{(column1.IsRowGuid ? string.Empty : " not")} a row-guid in database 1 and is{(column2.IsRowGuid ? string.Empty : " not")} a row-guid in database 2");
            }

            if (column2.DomainName.PrependSchemaName(column2.DomainSchema) != column1.DomainName.PrependSchemaName(column1.DomainSchema))
            {
                diff.Differences.Add($"has different custom datatype - is {(string.IsNullOrEmpty(column1.DomainName) ? "NULL" : column1.DomainName.PrependSchemaName(column1.DomainSchema))} in database 1 and is {(string.IsNullOrEmpty(column2.DomainName) ? "NULL" : column2.DomainName.PrependSchemaName(column2.DomainSchema))} in database 2");
            }

            if (Database2.Tables[fullyQualifiedTableName].ColumnHasUniqueIndex(column2.ColumnName) != Database1.Tables[fullyQualifiedTableName].ColumnHasUniqueIndex(column1.ColumnName))
            {
                diff.Differences.Add($"{(Database1.Tables[fullyQualifiedTableName].ColumnHasUniqueIndex(column1.ColumnName) ? "has" : "does not have")} a unique constraint in database 1 and {(Database2.Tables[fullyQualifiedTableName].ColumnHasUniqueIndex(column2.ColumnName) ? "has" : "does not have")} a unique constraint in database 2");
            }

            if (this.Options.CompareProperties)
            {
                InspectColumnProperties(fullyQualifiedTableName, column2.ColumnName, diff);
            }

            diff.ExistsInDatabase2 = true;
        }

        private void InspectColumnProperties(string fullyQualifiedTableName, string columnName, ItemWithPropertiesDifference columnDiff)
        {
            var hasFoundColumn1Description = false;

            foreach (var property1 in Database1.ExtendedProperties)
            {
                var propertyTableName = property1.TableName.PrependSchemaName(property1.TableSchema);
                if (property1.Type == PropertyObjectType.TableColumn && propertyTableName == fullyQualifiedTableName && property1.ColumnName == columnName)
                {
                    var diff = new ExtendedPropertyDifference(true, false);

                    var property2 = Database2.ExtendedProperties.Where(x => x.FullId == property1.FullId).FirstOrDefault();
                    if (property2 != null)
                    {
                        diff.ExistsInDatabase2 = true;
                        diff.Value1 = property1.PropertyValue;
                        diff.Value2 = property2.PropertyValue;
                    }

                    if (property1.PropertyName == "MS_Description")
                    {
                        hasFoundColumn1Description = true;
                        if (!diff.ExistsInDatabase2)
                        {
                            columnDiff.Differences.Add("description exists in database 1 and does not exist in database 2");
                        }
                        else if (diff.Value1 != diff.Value2)
                        {
                            columnDiff.Differences.Add($"has different description - is [{diff.Value1}] in database 1 and is [{diff.Value2}] in database 2");
                        }
                    }
                    else
                    {
                        columnDiff.ExtendedPropertyDifferences.Add(property1.PropertyName, diff);
                    }
                }
            }

            foreach (var property2 in Database2.ExtendedProperties.Where(x => x.Type == PropertyObjectType.TableColumn && x.TableName.PrependSchemaName(x.TableSchema) == fullyQualifiedTableName && x.ColumnName == columnName))
            {
                if (property2.PropertyName == "MS_Description")
                {
                    if (!hasFoundColumn1Description)
                    {
                        columnDiff.Differences.Add("description exists in database 2 and does not exist in database 1");
                    }
                }
                else if (!columnDiff.ExtendedPropertyDifferences.ContainsKey(property2.PropertyName))
                {
                    columnDiff.ExtendedPropertyDifferences.Add(property2.PropertyName, new ExtendedPropertyDifference(false, true));
                }
            }
        }

        private void InspectIndexes(string fullyQualifiedTableName)
        {
            foreach (var index1 in Database1.Tables[fullyQualifiedTableName].Indexes)
            {
                var diff = new ItemWithPropertiesDifference(true, false, index1.ItemType);

                var index2 = Database2.Tables[fullyQualifiedTableName].Indexes.Where(x => x.FullId == index1.FullId).FirstOrDefault();
                if (index2 != null)
                {
                    if (index2.Clustered != index1.Clustered)
                    {
                        diff.Differences.Add($"has different clustering - is{(index1.Clustered ? string.Empty : " not")} clustered in database 1 and is{(index2.Clustered ? string.Empty : " not")} clustered in database 2");
                    }

                    if (index2.Unique != index1.Unique)
                    {
                        diff.Differences.Add($"has different uniqueness - is{(index1.Unique ? string.Empty : " not")} unique in database 1 and is{(index2.Unique ? string.Empty : " not")} unique in database 2");
                    }

                    if (index2.IsUniqueKey != index1.IsUniqueKey)
                    {
                        diff.Differences.Add($"has different type - {(index1.IsUniqueKey ? "unique key" : "index")} in database 1 and {(index2.Unique ? string.Empty : " not")} in database 2");
                    }

                    if (index2.IsPrimaryKey != index1.IsPrimaryKey)
                    {
                        diff.Differences.Add($"has different primary - is{(index1.IsPrimaryKey ? string.Empty : " not")} a primary key in database 1 and is{(index2.IsPrimaryKey ? string.Empty : " not")} a primary key in database 2");
                    }

                    if (index2.FileGroup != index1.FileGroup)
                    {
                        diff.Differences.Add($"has different filegroup - [{index1.FileGroup}] in database 1 and [{index2.FileGroup}] in database 2");
                    }

                    if (index2.ColumnsToString != index1.ColumnsToString)
                    {
                        foreach (var column in index1.Columns.Keys)
                        {
                            if (index2.Columns.TryGetValue(column, out bool value))
                            {
                                if (index1.Columns[column] != value)
                                {
                                    diff.Differences.Add($"[{column}] has different ordering - {(index1.Columns[column] ? "a" : "de")}scending on database 1 and {(value ? "a" : "de")}scending on database 2");
                                }
                            }
                            else
                            {
                                diff.Differences.Add($"[{column}] column does not exist in database 2 index");
                            }
                        }

                        foreach (var column in index2.Columns.Keys.Where(x => !index1.Columns.ContainsKey(x)))
                        {
                            diff.Differences.Add($"[{column}] column does not exist in database 1 index");
                        }
                    }

                    if (index2.IncludedColumnsToString != index1.IncludedColumnsToString)
                    {
                        foreach (var column in index1.IncludedColumns.Keys)
                        {
                            if (index2.IncludedColumns.TryGetValue(column, out bool value))
                            {
                                if (index1.IncludedColumns[column] != value)
                                {
                                    diff.Differences.Add($"[{column}] \"included column\" has different ordering - {(index1.IncludedColumns[column] ? "a" : "de")}scending on database 1 and {(value ? "a" : "de")}scending on database 2");
                                }
                            }
                            else
                            {
                                diff.Differences.Add($"[{column}] \"included column\" does not exist in database 2 index");
                            }
                        }

                        foreach (var column in index2.IncludedColumns.Keys.Where(x => !index1.IncludedColumns.ContainsKey(x)))
                        {
                            diff.Differences.Add($"[{column}] \"included column\" does not exist in database 1 index");
                        }
                    }

                    if (this.Options.CompareProperties)
                    {
                        InspectIndexProperties(fullyQualifiedTableName, index2.IndexName, diff);
                    }

                    diff.ExistsInDatabase2 = true;
                }

                Differences.TableDifferences[fullyQualifiedTableName].IndexDifferences.Add(index1.IndexName, diff);
            }


            foreach (var index in Database2.Tables[fullyQualifiedTableName].Indexes.Where(x =>
                !Differences.TableDifferences[fullyQualifiedTableName].IndexDifferences.ContainsKey(x.IndexName)))
            {
                Differences.TableDifferences[fullyQualifiedTableName].IndexDifferences.Add(index.IndexName, new ItemWithPropertiesDifference(false, true, index.ItemType));
            }
        }

        private void InspectIndexProperties(string fullyQualifiedTableName, string indexName, ItemWithPropertiesDifference indexDiff)
        {
            foreach (var property1 in Database1.ExtendedProperties)
            {
                var propertyTableName = property1.TableName.PrependSchemaName(property1.TableSchema);
                if (property1.Type == PropertyObjectType.Index && propertyTableName == fullyQualifiedTableName && property1.IndexName == indexName)
                {
                    var diff = new ExtendedPropertyDifference(true, false);
                    var property2 = Database2.ExtendedProperties.Where(x => x.FullId == property1.FullId).FirstOrDefault();
                    if (property2 != null)
                    {
                        diff.ExistsInDatabase2 = true;
                        diff.Value1 = property1.PropertyValue;
                        diff.Value2 = property2.PropertyValue;
                    }

                    indexDiff.ExtendedPropertyDifferences.Add(property1.PropertyName, diff);
                }
            }

            foreach (var property in Database2.ExtendedProperties.Where(x =>
                x.PropertyType == "INDEX" &&
                x.TableName.PrependSchemaName(x.TableSchema) == fullyQualifiedTableName &&
                x.IndexName == indexName &&
                !indexDiff.ExtendedPropertyDifferences.ContainsKey(x.PropertyName)))
            {
                indexDiff.ExtendedPropertyDifferences.Add(property.PropertyName, new ExtendedPropertyDifference(false, true));
            }
        }

        private void InspectRelations(string fullyQualifiedTableName)
        {
            foreach (var relation1 in Database1.Tables[fullyQualifiedTableName].Relations)
            {
                var diff = new ItemDifference(true, false);

                var relation2 = Database2.Tables[fullyQualifiedTableName].Relations.Where(x => x.RelationName == relation1.RelationName).FirstOrDefault();
                if (relation2 != null)
                {
                    if (relation2.ChildColumns != relation1.ChildColumns)
                    {
                        diff.Differences.Add($"has different child column list - is \"{relation1.ChildColumns}\" in database 1 and is \"{relation2.ChildColumns}\" in database 2");
                    }

                    if (relation2.ParentColumns != relation1.ParentColumns)
                    {
                        diff.Differences.Add($"has different parent column list - is \"{relation1.ParentColumns}\" in database 1 and is \"{relation2.ParentColumns}\" in database 2");
                    }

                    if (relation2.DeleteRule != relation1.DeleteRule)
                    {
                        diff.Differences.Add($"has different delete rule - is \"{relation1.DeleteRule}\" in database 1 and is \"{relation2.DeleteRule}\" in database 2");
                    }

                    if (relation2.UpdateRule != relation1.UpdateRule)
                    {
                        diff.Differences.Add($"has different update rule - is \"{relation1.UpdateRule}\" in database 1 and is \"{relation2.UpdateRule}\" in database 2");
                    }

                    diff.ExistsInDatabase2 = true;
                }

                Differences.TableDifferences[fullyQualifiedTableName].RelationshipDifferences.Add(relation1.RelationName, diff);
            }

            foreach (var relation in Database2.Tables[fullyQualifiedTableName].Relations.Where(x => !Differences.TableDifferences[fullyQualifiedTableName].RelationshipDifferences.ContainsKey(x.RelationName)))
            {
                Differences.TableDifferences[fullyQualifiedTableName].RelationshipDifferences.Add(relation.RelationName, new ItemDifference(false, true));
            }
        }

        private void InspectTablePermissions(string fullyQualifiedTableName)
        {
            foreach (var permission1 in Database1.Permissions)
            {
                var permissionTableName = permission1.ObjectName.PrependSchemaName(permission1.ObjectSchema);
                if (permission1.Type == PermissionObjectType.UserTable && permissionTableName == fullyQualifiedTableName)
                {
                    Differences.TableDifferences[fullyQualifiedTableName].PermissionDifferences.Add(
                        permission1.ToString(),
                        new BaseDifference(true, Database2.Permissions.Exists(x => x.FullId == permission1.FullId)));
                }
            }

            foreach (var permission in Database2.Permissions.Where(x =>
                x.ObjectType == "USER_TABLE" &&
                x.ObjectName.PrependSchemaName(x.ObjectSchema) == fullyQualifiedTableName &&
                !Differences.TableDifferences[fullyQualifiedTableName].PermissionDifferences.ContainsKey(x.ToString())))
            {
                Differences.TableDifferences[fullyQualifiedTableName].PermissionDifferences.Add(permission.ToString(), new ExtendedPropertyDifference(false, true));
            }
        }

        private void InspectTableProperties(string fullyQualifiedTableName)
        {
            foreach (var property1 in Database1.ExtendedProperties)
            {
                var propertyTableName = property1.TableName.PrependSchemaName(property1.TableSchema);
                if (property1.Type == PropertyObjectType.Table && propertyTableName == fullyQualifiedTableName)
                {
                    var diff = new ExtendedPropertyDifference(true, false);

                    var property2 = Database2.ExtendedProperties.Where(x => x.FullId == property1.FullId).FirstOrDefault();
                    if (property2 != null)
                    {
                        diff.ExistsInDatabase2 = true;
                        diff.Value1 = property1.PropertyValue;
                        diff.Value2 = property2.PropertyValue;
                    }

                    Differences.TableDifferences[fullyQualifiedTableName].ExtendedPropertyDifferences.Add(property1.PropertyName, diff);
                }
            }

            foreach (var property in Database2.ExtendedProperties.Where(x =>
                x.ColumnName == null &&
                x.TableName.PrependSchemaName(x.TableSchema) == fullyQualifiedTableName &&
                !Differences.TableDifferences[fullyQualifiedTableName].ExtendedPropertyDifferences.ContainsKey(x.PropertyName)))
            {
                Differences.TableDifferences[fullyQualifiedTableName].ExtendedPropertyDifferences.Add(property.PropertyName, new ExtendedPropertyDifference(false, true));
            }
        }

        private void InspectTriggers(string fullyQualifiedTableName)
        {
            foreach (var trigger1 in Database1.Tables[fullyQualifiedTableName].Triggers)
            {
                var diff = new ItemDifference(true, false);

                var trigger2 = Database2.Tables[fullyQualifiedTableName].Triggers.Where(x => x.TriggerName == trigger1.TriggerName && x.TableName.PrependSchemaName(x.TableSchema) == fullyQualifiedTableName).FirstOrDefault();
                if (trigger2 != null)
                {
                    if (trigger2.FileGroup != trigger1.FileGroup)
                    {
                        diff.Differences.Add($"has different filegroup - is {trigger1.FileGroup} in database 1 and is {trigger2.FileGroup} in database 2");
                    }

                    if (trigger2.TriggerOwner != trigger1.TriggerOwner)
                    {
                        diff.Differences.Add($"has different owner - is {trigger1.TriggerOwner} in database 1 and is {trigger2.TriggerOwner} in database 2");
                    }

                    if (trigger2.IsUpdate != trigger1.IsUpdate)
                    {
                        diff.Differences.Add($"has different update - is {(trigger1.IsUpdate ? string.Empty : "not ")}update in database 1 and is {(trigger2.IsUpdate ? string.Empty : "not ")}update in database 2");
                    }

                    if (trigger2.IsDelete != trigger1.IsDelete)
                    {
                        diff.Differences.Add($"has different delete - is {(trigger1.IsDelete ? string.Empty : "not ")}delete in database 1 and is {(trigger2.IsDelete ? string.Empty : "not ")}delete in database 2");
                    }

                    if (trigger2.IsInsert != trigger1.IsInsert)
                    {
                        diff.Differences.Add($"has different insert - is {(trigger1.IsInsert ? string.Empty : "not ")}insert in database 1 and is {(trigger2.IsInsert ? string.Empty : "not ")}insert in database 2");
                    }

                    if (trigger2.IsAfter != trigger1.IsAfter)
                    {
                        diff.Differences.Add($"has different after - is {(trigger1.IsAfter ? string.Empty : "not ")}after in database 1 and is {(trigger2.IsAfter ? string.Empty : "not ")}after in database 2");
                    }

                    if (trigger2.IsInsteadOf != trigger1.IsInsteadOf)
                    {
                        diff.Differences.Add($"has different instead-of - is {(trigger1.IsInsteadOf ? string.Empty : "not ")}instead-of in database 1 and is {(trigger2.IsInsteadOf ? string.Empty : "not ")}instead-of in database 2");
                    }

                    if (trigger2.IsDisabled != trigger1.IsDisabled)
                    {
                        diff.Differences.Add($"has different disabled - is {(trigger1.IsDisabled ? string.Empty : "not ")}disabled in database 1 and is {(trigger2.IsDisabled ? string.Empty : "not ")}disabled in database 2");
                    }

                    if (BaseDifference.CleanDefinitionText(trigger1.TriggerContent, true) != BaseDifference.CleanDefinitionText(trigger2.TriggerContent, true))
                    {
                        diff.Differences.Add("definitions are different");
                    }

                    diff.ExistsInDatabase2 = true;
                }

                Differences.TableDifferences[fullyQualifiedTableName].TriggerDifferences.Add(trigger1.TriggerName, diff);
            }

            foreach (var trigger2 in Database2.Tables[fullyQualifiedTableName].Triggers.Where(x => !Differences.TableDifferences[fullyQualifiedTableName].TriggerDifferences.ContainsKey(x.TriggerName)))
            {
                Differences.TableDifferences[fullyQualifiedTableName].TriggerDifferences.Add(trigger2.TriggerName, new ItemDifference(false, true));
            }
        }

        private void InspectUserTypes()
        {
            foreach (var userTypeName in Database1.UserTypes.Keys)
            {
                var diff = new ItemDifference(true, false);

                var userType = Database2.UserTypes.Where(x => x.Key == userTypeName).FirstOrDefault();
                if (!userType.Equals(default(KeyValuePair<string, SqlUserType>)))
                {
                    var item1 = Database1.UserTypes[userTypeName];
                    var item2 = Database2.UserTypes[userTypeName];

                    if (item1.UnderlyingTypeName != item2.UnderlyingTypeName)
                    {
                        diff.Differences.Add($"has different underlying type - is {item1.UnderlyingTypeName} in database 1 and is {item2.UnderlyingTypeName} in database 2");
                    }

                    if (item1.Precision != item2.Precision)
                    {
                        diff.Differences.Add($"has different precision - is {(item1.Precision.HasValue ? item1.Precision.Value.ToString() : "NULL")} in database 1 and is {(item2.Precision.HasValue ? item2.Precision.Value.ToString() : "NULL")} in database 2");
                    }

                    if (item1.Scale != item2.Scale)
                    {
                        diff.Differences.Add($"has different scale - is {(item1.Scale.HasValue ? item1.Scale.Value.ToString() : "NULL")} in database 1 and is {(item2.Scale.HasValue ? item2.Scale.Value.ToString() : "NULL")} in database 2");
                    }

                    if (item1.MaxLength != item2.MaxLength)
                    {
                        diff.Differences.Add($"has different max length - is {(item1.MaxLength.HasValue ? item1.MaxLength.Value.ToString() : "NULL")} in database 1 and is {(item2.MaxLength.HasValue ? item2.MaxLength.Value.ToString() : "NULL")} in database 2");
                    }

                    if (item1.IsNullable != item2.IsNullable)
                    {
                        diff.Differences.Add($"is{(item1.IsNullable ? "" : " not")} nullable in database 1 and is{(item2.IsNullable ? "" : " not")} nullable in database 2");
                    }

                    if (item1.CollationName != item2.CollationName)
                    {
                        diff.Differences.Add($"has different collation - is {(string.IsNullOrEmpty(item1.CollationName) ? "NULL" : item1.CollationName)} in database 1 and is {(string.IsNullOrEmpty(item2.CollationName) ? "NULL" : item2.CollationName)} in database 2");
                    }

                    if (item1.IsAssemblyType != item2.IsAssemblyType)
                    {
                        diff.Differences.Add($"is{(item1.IsAssemblyType ? "" : " not")} assembly type in database 1 and is{(item2.IsAssemblyType ? "" : " not")} assembly type in database 2");
                    }

                    diff.ExistsInDatabase2 = true;
                }

                Differences.UserTypeDifferences.Add(userTypeName, diff);
            }

            foreach (var userType in Database2.UserTypes.Where(x => !Differences.UserTypeDifferences.ContainsKey(x.Key)))
            {
                Differences.UserTypeDifferences.Add(userType.Key, new ItemDifference(false, true));
            }
        }

        private void InspectSynonyms()
        {
            foreach (var synonymName in Database1.Synonyms.Keys)
            {
                var diff = new DatabaseObjectDifference(true, false);

                var synonym = Database2.Synonyms.Where(x => x.Key == synonymName).FirstOrDefault();
                if (!synonym.Equals(default(KeyValuePair<string, string>)))
                {
                    if (this.Options.CompareProperties)
                    {
                        InspectObjectProperties(synonymName, diff);
                    }

                    if (this.Options.ComparePermissions)
                    {
                        InspectObjectPermissions(synonymName, PermissionObjectType.Synonym, diff);
                    }

                    diff.ObjectDefinition1 = Database1.Synonyms[synonymName];
                    diff.ObjectDefinition2 = Database2.Synonyms[synonymName];

                    diff.ExistsInDatabase2 = true;
                }

                Differences.SynonymDifferences.Add(synonymName, diff);
            }

            foreach (var synonym in Database2.Synonyms.Where(x => !Differences.SynonymDifferences.ContainsKey(x.Key)))
            {
                Differences.SynonymDifferences.Add(synonym.Key, new DatabaseObjectDifference(false, true));
            }
        }

        private void InspectViews()
        {
            foreach (var viewName in Database1.Views.Keys)
            {
                var diff = new DatabaseObjectDifference(true, false);

                var view = Database2.Views.Where(x => x.Key == viewName).FirstOrDefault();
                if (!view.Equals(default(KeyValuePair<string, string>)))
                {
                    if (this.Options.CompareProperties)
                    {
                        InspectObjectProperties(viewName, diff);
                    }

                    if (this.Options.ComparePermissions)
                    {
                        InspectObjectPermissions(viewName, PermissionObjectType.View, diff);
                    }

                    diff.ObjectDefinition1 = Database1.Views[viewName];
                    diff.ObjectDefinition2 = view.Value;

                    if (diff.DefinitionsAreDifferent)
                    {
                        DefinitionDifferences.Add(viewName, (diff.ObjectDefinition1, diff.ObjectDefinition2));
                    }

                    diff.ExistsInDatabase2 = true;
                }

                Differences.ViewDifferences.Add(viewName, diff);
            }

            foreach (var view in Database2.Views.Where(x => !Differences.ViewDifferences.ContainsKey(x.Key)))
            {
                Differences.ViewDifferences.Add(view.Key, new DatabaseObjectDifference(false, true));
            }
        }

        private void InspectRoutines()
        {
            foreach (var routineName in Database1.UserRoutines.Keys)
            {
                var diff = new DatabaseObjectDifference(true, false);
                var isFunction = Database1.UserRoutines[routineName].RoutineType.ToLower() == "function";

                var routine = Database2.UserRoutines.Where(x => x.Key == routineName).FirstOrDefault();
                if (!routine.Equals(default(KeyValuePair<string, SqlUserRoutine>)))
                {
                    if (this.Options.CompareProperties)
                    {
                        InspectObjectProperties(routineName, diff);
                    }

                    if (this.Options.ComparePermissions)
                    {
                        InspectObjectPermissions(routineName, isFunction ? PermissionObjectType.SqlFunction : PermissionObjectType.SqlStoredProcedure, diff);
                    }

                    diff.ObjectDefinition1 = Database1.UserRoutines[routineName].RoutineDefinition;
                    diff.ObjectDefinition2 = routine.Value.RoutineDefinition;

                    if (diff.DefinitionsAreDifferent)
                    {
                        DefinitionDifferences.Add(routineName, (diff.ObjectDefinition1, diff.ObjectDefinition2));
                    }

                    diff.ExistsInDatabase2 = true;
                }

                if (isFunction)
                {
                    Differences.FunctionDifferences.Add(routineName, diff);
                }
                else
                {
                    Differences.StoredProcedureDifferences.Add(routineName, diff);
                }
            }

            foreach (var routineName in Database2.UserRoutines.Keys)
            {
                if (Database2.UserRoutines[routineName].RoutineType.ToLower() == "function")
                {
                    if (!Differences.FunctionDifferences.ContainsKey(routineName))
                    {
                        Differences.FunctionDifferences.Add(routineName, new DatabaseObjectDifference(false, true));
                    }
                }
                else
                {
                    if (!Differences.StoredProcedureDifferences.ContainsKey(routineName))
                    {
                        Differences.StoredProcedureDifferences.Add(routineName, new DatabaseObjectDifference(false, true));
                    }
                }
            }
        }

        private void InspectObjectProperties(string fullyQualifiedObjectName, DatabaseObjectDifference objectDiff)
        {
            foreach (var property1 in Database1.ExtendedProperties)
            {
                var propertyObjectName = property1.ObjectName.PrependSchemaName(property1.ObjectSchema);
                if (property1.Type == PropertyObjectType.Routine && propertyObjectName == fullyQualifiedObjectName)
                {
                    var diff = new ExtendedPropertyDifference(true, false);

                    var property2 = Database2.ExtendedProperties.Where(x => x.FullId == property1.FullId).FirstOrDefault();
                    if (property2 != null)
                    {
                        diff.ExistsInDatabase2 = true;
                        diff.Value1 = property1.PropertyValue;
                        diff.Value2 = property2.PropertyValue;
                    }

                    objectDiff.ExtendedPropertyDifferences.Add(property1.PropertyName, diff);
                }
            }

            foreach (var property2 in Database2.ExtendedProperties)
            {
                var propertyObjectName = property2.ObjectName.PrependSchemaName(property2.ObjectSchema);
                if (property2.Type == PropertyObjectType.Routine &&
                    propertyObjectName == fullyQualifiedObjectName &&
                    !objectDiff.ExtendedPropertyDifferences.ContainsKey(property2.PropertyName))
                {
                    objectDiff.ExtendedPropertyDifferences.Add(property2.PropertyName, new ExtendedPropertyDifference(false, true));
                }
            }
        }

        private void InspectObjectPermissions(string fullyQualifiedObjectName, PermissionObjectType objectType, DatabaseObjectDifference objectDiff)
        {
            foreach (var permission1 in Database1.Permissions)
            {
                var permissionObjectName = permission1.ObjectName.PrependSchemaName(permission1.ObjectSchema);
                if (permission1.Type == objectType && permissionObjectName == fullyQualifiedObjectName)
                {
                    objectDiff.PermissionDifferences.Add(
                        permission1.ToString(),
                        new BaseDifference(true, Database2.Permissions.Exists(x => x.FullId == permission1.FullId)));
                }
            }

            foreach (var permission2 in Database2.Permissions)
            {
                var permissionObjectName = permission2.ObjectName.PrependSchemaName(permission2.ObjectSchema);
                if (permission2.Type == objectType &&
                    permissionObjectName == fullyQualifiedObjectName &&
                    !objectDiff.PermissionDifferences.ContainsKey(permission2.ToString()))
                {
                    objectDiff.PermissionDifferences.Add(permission2.ToString(), new BaseDifference(false, true));
                }
            }
        }
    }
}
