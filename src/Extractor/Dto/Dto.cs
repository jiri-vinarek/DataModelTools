using System;
using System.Collections.Generic;

namespace PowerBiTools.Dto
{
    public class DataAccessOptions
    {
        public bool LegacyRedirects { get; set; }
        public bool ReturnErrorValuesAsNull { get; set; }
    }

    public class AttributeHierarchy
    {
        public string State { get; set; }
        public DateTime ModifiedTime { get; set; }
        public DateTime RefreshedTime { get; set; }
    }

    public class Annotation
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime ModifiedTime { get; set; }
    }

    public class DefaultHierarchy
    {
        public string Table { get; set; }
        public string Hierarchy { get; set; }
    }

    public class Variation
    {
        public string Name { get; set; }
        public string Relationship { get; set; }
        public DefaultHierarchy DefaultHierarchy { get; set; }
        public bool IsDefault { get; set; }
    }

    public class Column
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool IsHidden { get; set; }
        public bool IsUnique { get; set; }
        public bool IsKey { get; set; }
        public bool IsNullable { get; set; }
        public DateTime ModifiedTime { get; set; }
        public DateTime StructureModifiedTime { get; set; }
        public DateTime RefreshedTime { get; set; }
        public AttributeHierarchy AttributeHierarchy { get; set; }
        public string SourceColumn { get; set; }
        public string SummarizeBy { get; set; }
        public IList<Annotation> Annotations { get; set; }
        public string FormatString { get; set; }
        public bool? IsNameInferred { get; set; }
        public bool? IsDataTypeInferred { get; set; }
        public string DataCategory { get; set; }
        public string Expression { get; set; }
        public string SortByColumn { get; set; }
        public IList<Variation> Variations { get; set; }
        public string DisplayFolder { get; set; }
    }

    public class Source
    {
        public string Type { get; set; }
        public string Expression { get; set; }
    }

    public class Partition
    {
        public string Name { get; set; }
        public string Mode { get; set; }
        public string State { get; set; }
        public DateTime ModifiedTime { get; set; }
        public DateTime RefreshedTime { get; set; }
        public Source Source { get; set; }
    }

    public class Level
    {
        public string Name { get; set; }
        public int Ordinal { get; set; }
        public string Column { get; set; }
        public DateTime ModifiedTime { get; set; }
    }
    
    public class Hierarchy
    {
        public string Name { get; set; }
        public string State { get; set; }
        public DateTime ModifiedTime { get; set; }
        public DateTime StructureModifiedTime { get; set; }
        public DateTime RefreshedTime { get; set; }
        public IList<Level> Levels { get; set; }
        public IList<Annotation> Annotations { get; set; }
    }

    public class Measure
    {
        public string Name { get; set; }
        public string Expression { get; set; }
        public string FormatString { get; set; }
        public string DataType { get; set; }
        public DateTime ModifiedTime { get; set; }
        public DateTime StructureModifiedTime { get; set; }
        public IList<Annotation> Annotations { get; set; }
        public string DisplayFolder { get; set; }
    }

    public class Table
    {
        public string Name { get; set; }
        public DateTime ModifiedTime { get; set; }
        public DateTime StructureModifiedTime { get; set; }
        public IList<Column> Columns { get; set; }
        public IList<Partition> Partitions { get; set; }
        public IList<Annotation> Annotations { get; set; }
        public bool? IsHidden { get; set; }
        public bool? IsPrivate { get; set; }
        public IList<Hierarchy> Hierarchies { get; set; }
        public bool? ShowAsVariationsOnly { get; set; }
        public IList<Measure> Measures { get; set; }
    }

    public class Relationship
    {
        public string Name { get; set; }
        public string FromTable { get; set; }
        public string FromColumn { get; set; }
        public string ToTable { get; set; }
        public string ToColumn { get; set; }
        public string JoinOnDateBehavior { get; set; }
        public string State { get; set; }
        public DateTime ModifiedTime { get; set; }
        public DateTime RefreshedTime { get; set; }
    }

    public class Content
    {
        public string Version { get; set; }
        public string Language { get; set; }
        public string DynamicImprovement { get; set; }
    }

    public class LinguisticMetadata
    {
        // In some reports Content contains XML and sometimes JSON.
        // Disabled to skip parsing.
        // public Content Content { get; set; }
        // public string ContentType { get; set; }
        // public DateTime ModifiedTime { get; set; }
    }

    public class Culture
    {
        public string Name { get; set; }
        public DateTime ModifiedTime { get; set; }
        public DateTime StructureModifiedTime { get; set; }
        public LinguisticMetadata LinguisticMetadata { get; set; }
    }

    public class Model
    {
        public string Culture { get; set; }
        public DataAccessOptions DataAccessOptions { get; set; }
        public string DefaultPowerBIDataSourceVersion { get; set; }
        public string SourceQueryCulture { get; set; }
        public DateTime ModifiedTime { get; set; }
        public DateTime StructureModifiedTime { get; set; }
        public IList<Table> Tables { get; set; }
        public IList<Relationship> Relationships { get; set; }
        public IList<Culture> Cultures { get; set; }
        public IList<Annotation> Annotations { get; set; }
    }

    public class DataModelSchema
    {
        public string Name { get; set; }
        public int CompatibilityLevel { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime LastSchemaUpdate { get; set; }
        public DateTime LastProcessed { get; set; }
        public Model Model { get; set; }
    }
}