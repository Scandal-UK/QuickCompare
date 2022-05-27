// <copyright file="SqlRelation.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace QuickCompareModel.DatabaseSchema
{
    public class SqlRelation
    {
        public string RelationName { get; set; }

        public string ChildSchema { get; set; }

        public string ChildTable { get; set; }

        public string ChildColumns { get; set; }

        public string UniqueConstraintName { get; set; }

        public string ParentSchema { get; set; }

        public string ParentTable { get; set; }

        public string ParentColumns { get; set; }

        public string UpdateRule { get; set; }

        public string DeleteRule { get; set; }
    }
}
