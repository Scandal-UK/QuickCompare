// <copyright file="SqlTrigger.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema
{
    public class SqlTrigger
    {
        public string FileGroup { get; set; }

        public string TriggerName { get; set; }

        public string TriggerOwner { get; set; }

        public string TableSchema { get; set; }

        public string TableName { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsDelete { get; set; }

        public bool IsInsert { get; set; }

        public bool IsAfter { get; set; }

        public bool IsInsteadOf { get; set; }

        public bool IsDisabled { get; set; }

        public string TriggerContent { get; set; }
    }
}
