using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

namespace Extractor.Tests;

public sealed class Tests
{
    [Test]
    public void TestMeasure()
    {
        var extracts = GetExtracts("Measure.json");
        var expectedExtracts = new List<Extract>
        {
            new Extract(
                file: new File(
                    relativePath: "tables/table1/measures",
                    fileName: "Measure1.dax"
                ),
                content: "1 + 2"
            )
        };

        extracts.Should().BeEquivalentTo(expectedExtracts);
    }
    
    [Test]
    public void TestColumn()
    {
        var extracts = GetExtracts("Column.json");
        var expectedExtracts = new List<Extract>
        {
            new Extract(
                file: new File(
                    relativePath: "tables/table1/columns",
                    fileName: "HelloColumn.dax"
                ),
                content: "hello"
            )
        };

        extracts.Should().BeEquivalentTo(expectedExtracts);
    }
    
    [Test]
    public void TestColumnWithDisplayFolder()
    {
        var extracts = GetExtracts("ColumnWithDisplayFolder.json");
        var expectedExtracts = new List<Extract>
        {
            new Extract(
                file: new File(
                    relativePath: "tables/table1/f1_f2_f3/columns",
                    fileName: "Column 2 _ 3 _ 4.dax"
                ),
                content: "\"Custom column\""
            )
        };

        extracts.Should().BeEquivalentTo(expectedExtracts);
    }
    
    [Test]
    public void TestPartition()
    {
        var extracts = GetExtracts("Partition.json");
        var expectedExtracts = new List<Extract>
        {
            new Extract(
                file: new File(
                    relativePath: "tables/table1/partitions",
                    fileName: "table1-id.calculated"
                ),
                content: "Row(\"Column\", BLANK())"
            )
        };

        extracts.Should().BeEquivalentTo(expectedExtracts);
    }
    
    [Test]
    public void TestExpression()
    {
        var extracts = GetExtracts("Expression.json");
        var expectedExtracts = new List<Extract>
        {
            new Extract(
                file: new File(
                    relativePath: "expressions",
                    fileName: "Db-hostname.m"
                ),
                content: "null meta [IsParameterQuery=true, Type=\"Text\", IsParameterQueryRequired=true]"
            )
        };

        extracts.Should().BeEquivalentTo(expectedExtracts);
    }

    private IEnumerable<Extract> GetExtracts(string fileName)
    {
        using var reader = new StreamReader(GetDataPath(fileName));
        var schema = Program.GetSchema(reader);
        return Extractor.GetSchemaExtracts(schema);
    }

    private string GetDataPath(string fileName)
    {
        return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Data", fileName);
    }
}