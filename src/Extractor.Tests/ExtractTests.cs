using Extractor.Dto;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
    public void TestPartitionWithMultilineExpression()
    {
        var extracts = GetExtracts("PartitionWithMultilineSourceExpression.json");
        var expectedExtracts = new List<Extract>
        {
            new Extract(
                file: new File(
                    relativePath: "tables/table1/partitions",
                    fileName: "table1-id.calculated"
                ),
                content: "line1" + Environment.NewLine + "line2"
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

    private static IEnumerable<Extract> GetExtracts(string fileName)
    {
        using var reader = new StreamReader(GetDataPath(fileName));
        var schema = Program.GetSchema(reader);
        return Extractor.GetSchemaExtracts(schema);
    }

    private static IEnumerable<Extract> GetLayoutExtracts(string fileName)
    {
        using var reader = new StreamReader(GetDataPath(fileName));
        var layout = Program.GetLayout(reader);
        return Extractor.GetLayoutExtracts(layout);
    }

    private static string GetDataPath(string fileName)
    {
        return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Data", fileName);
    }

    [Test]
    public void ExtractRelationshipsTest()
    {
        var extracts = GetExtracts("Relationships.json");
        var relationship = new
        {
            fromColumn = "EnterpriseKey",
            fromTable = "bill",
            name = "9a41faf5-26a4-48f8-8d2b-942a4f2aac0d",
            toColumn = "EnterpriseKey",
            toTable = "enterprise"
        };

        var expectedExtracts = new List<Extract>
        {
            new Extract(
                file: new File(
                    relativePath: "relationships",
                    fileName: "9a41faf5-26a4-48f8-8d2b-942a4f2aac0d"
                ),
                content: JsonConvert.SerializeObject(relationship, Formatting.Indented)
            )
        };

        extracts.Should().BeEquivalentTo(expectedExtracts);
    }


    [Test]
    public void ExtractRolesTest()
    {
        var extracts = GetExtracts("Roles.json");

        var expectedExtracts = new List<Extract>
        {
            new Extract(
                file: new File(
                    relativePath: "roles",
                    fileName: "User"
                ),
                content: @"'enterprise'[EnterpriseKey] IN CALCULATETABLE(
VALUES(user_enterprise_access[EnterpriseKey]),
FILTER(user_enterprise_access,
LOWER(user_enterprise_access[UserId]) = LOWER(USERPRINCIPALNAME()))
)
'bill'[EnterpriseKey] IN CALCULATETABLE(
VALUES(user_enterprise_access[EnterpriseKey]),
FILTER(user_enterprise_access,
LOWER(user_enterprise_access[UserId]) = LOWER(USERPRINCIPALNAME()))
)
'account_company'[ChainKey] IN CALCULATETABLE(
VALUES(user_enterprise_access[ChainKey]),
FILTER(user_enterprise_access,
LOWER(user_enterprise_access[UserId]) = LOWER(USERPRINCIPALNAME()))
)
'account'[ChainKey] IN CALCULATETABLE(
VALUES(user_enterprise_access[ChainKey]),
FILTER(user_enterprise_access,
LOWER(user_enterprise_access[UserId]) = LOWER(USERPRINCIPALNAME()))
)"
            )
        };

        extracts.Should().BeEquivalentTo(expectedExtracts);
    }

    private static List<dynamic> GetFilterMock()
    {
        return new List<dynamic>()
        {
            new {
                DisplayName = "Relative date (empty is today)",
                Expression = new {
                  Column = new {
                    Expression = new {
                      SourceRef = new {
                        Entity = "calender"
                      }
                    },
                    Property = "Date"
                  }
                },
                HowCreated = 1,
                Name = "Filter99db7c6e4bdb6ad6b7cf",
                Type = "Categorical"
              }
        };
    }

    [Test]
    public void ExtractAllPagesFilterTest()
    {
        var extracts = GetLayoutExtracts("AllPagesFilter.json");

        var expectedExtracts = new List<Extract>
        {
            new Extract(
            new File("report", "AllPagesFilter.json"),
                JsonConvert.SerializeObject(GetFilterMock(), Formatting.Indented)
            )
        };

        extracts.Should().BeEquivalentTo(expectedExtracts);
    }

    [Test]
    public void ExtractPageFiltersTest()
    {
        var extract = GetLayoutExtracts("PageFilter.json").FirstOrDefault(x => x.File?.FileName == "Filters");

        var expectedExtract =
            new Extract(
                new File($"report/AgingReport", "Filters"),
                JsonConvert.SerializeObject(GetFilterMock(), Formatting.Indented)
            );


        extract.Should().BeEquivalentTo(expectedExtract);
    }

    [Test]
    public void ExtractVisualContainerFiltersTest()
    {
        var extract = GetLayoutExtracts("VisualContainerFilter.json").FirstOrDefault(x => x?.File?.FileName == "Filters");

        var expectedExtract =
            new Extract(
                new File("report/AgingReport/Report_53caaf51ea569a44b22e", "Filters"),
                JsonConvert.SerializeObject(GetFilterMock(), Formatting.Indented)
            );

        extract.Should().BeEquivalentTo(expectedExtract);
    }

    [Test]
    public void ExtractPageConfigTest()
    {
        var extract = GetLayoutExtracts("PageConfig.json").FirstOrDefault(x => x.File?.FileName == "Config");

        dynamic expectedConfigObject = "{\"relationships\":[{\"source\":\"b93c2b8a314195409ea9\",\"target\":\"ad40d3af784e006b4b3c\",\"type\":1}],\"objects\":{\"outspacePane\":[{\"properties\":{\"width\":{\"expr\":{\"Literal\":{\"Value\":\"190L\"}}}}}]},\"filterSortOrder\":3}";

        var expectedExtract =
            new Extract(
                new File("report/AgingReport", "Config"),
                JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(expectedConfigObject), Formatting.Indented)
            );

        extract.Should().BeEquivalentTo(expectedExtract);
    }

    [Test]
    public void ExtractVisualContainerConfigTest()
    {
        var extract = GetLayoutExtracts("VisualContainerConfig.json").FirstOrDefault(x => x.File?.RelativePath == "report/AgingReport/53caaf51ea569a44b22e" && x.File?.FileName == "Config");

        dynamic expectedConfigObject = "{\"name\":\"53caaf51ea569a44b22e\",\"layouts\":[{\"id\":0,\"position\":{\"x\":16,\"y\":12,\"z\":0,\"width\":1408,\"height\":674,\"tabOrder\":0}}],\"singleVisual\":{\"visualType\":\"basicShape\",\"drillFilterOtherVisuals\":true,\"objects\":{\"fill\":[{\"properties\":{\"fillColor\":{\"solid\":{\"color\":{\"expr\":{\"ThemeDataColor\":{\"ColorId\":0,\"Percent\":0}}}}},\"transparency\":{\"expr\":{\"Literal\":{\"Value\":\"0D\"}}},\"show\":{\"expr\":{\"Literal\":{\"Value\":\"true\"}}}}}]}}}";

        var expectedExtract =
            new Extract(
                new File("report/AgingReport/53caaf51ea569a44b22e", "Config"),
                JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(expectedConfigObject), Formatting.Indented)
            );

        extract.Should().BeEquivalentTo(expectedExtract);
    }

    [Test]
    public void ExtractVisualQueryTest()
    {
        var extract = GetLayoutExtracts("VisualContainerQuery.json").FirstOrDefault(x => x.File?.FileName == "Query");

        dynamic expectedConfigObject = "{\"Commands\":[{\"SemanticQueryDataShapeCommand\":{\"Query\":{\"Version\":2,\"From\":[{\"Name\":\"_\",\"Entity\":\"_core custom measures\",\"Type\":0},{\"Name\":\"b\",\"Entity\":\"bill_dimensions_table\",\"Type\":0}],\"Select\":[{\"Measure\":{\"Expression\":{\"SourceRef\":{\"Source\":\"_\"}},\"Property\":\"LastRefresh\"},\"Name\":\"_core custom measures.LastRefresh\"}],\"Where\":[{\"Condition\":{\"In\":{\"Expressions\":[{\"Column\":{\"Expression\":{\"SourceRef\":{\"Source\":\"b\"}},\"Property\":\"aging_table Fields\"}}],\"Values\":[[{\"Literal\":{\"Value\":\"'''account''[AccountType]'\"}}],[{\"Literal\":{\"Value\":\"'''account''[OrganisationName]'\"}}],[{\"Literal\":{\"Value\":\"'''account''[CustomerName]'\"}}]]}}}]},\"Binding\":{\"Primary\":{\"Groupings\":[{\"Projections\":[0]}]},\"DataReduction\":{\"DataVolume\":3,\"Primary\":{\"Top\":{}}},\"Version\":1},\"ExecutionMetricsKind\":1}}]}";

        var expectedExtract =
            new Extract(
                new File("report/AgingReport/InfobarLastRefresh_fce5faeee5156c92d78d", "Query"),
                JsonConvert.SerializeObject(JsonConvert.DeserializeObject<Query>(expectedConfigObject), Formatting.Indented)
            );

        extract.Should().BeEquivalentTo(expectedExtract);
    }

    [Test]
    public void ExtractVisualDataTransformTest()
    {
        var extract = GetLayoutExtracts("VisualContainerDataTransform.json").FirstOrDefault(x => x.File?.FileName == "DataTransforms");

        dynamic expectedConfigObject = "{\"objects\":{\"labels\":[{\"properties\":{\"color\":{\"solid\":{\"color\":{\"expr\":{\"Literal\":{\"Value\":\"'#101B2C'\"}}}}},\"fontSize\":{\"expr\":{\"Literal\":{\"Value\":\"'23'\"}}},\"fontFamily\":{\"expr\":{\"Literal\":{\"Value\":\"'''Segoe UI Bold'', wf_segoe-ui_bold, helvetica, arial, sans-serif'\"}}}}}],\"categoryLabels\":[{\"properties\":{\"show\":{\"expr\":{\"Literal\":{\"Value\":\"true\"}}},\"color\":{\"solid\":{\"color\":{\"expr\":{\"Literal\":{\"Value\":\"'#6b7989'\"}}}}},\"fontSize\":{\"expr\":{\"Literal\":{\"Value\":\"'11'\"}}}}}]},\"projectionOrdering\":{\"Values\":[0]},\"queryMetadata\":{\"Select\":[{\"Restatement\":\"Total amount\",\"Name\":\"_core custom measures.AmountReceivableTotal\",\"Type\":1,\"Format\":\"0.00\"}]},\"visualElements\":[{\"DataRoles\":[{\"Name\":\"Values\",\"Projection\":0,\"isActive\":false}]}],\"selects\":[{\"displayName\":\"Total amount\",\"format\":\"0.00\",\"queryName\":\"_core custom measures.AmountReceivableTotal\",\"roles\":{\"Values\":true},\"type\":{\"category\":null,\"underlyingType\":259},\"expr\":{\"Measure\":{\"Expression\":{\"SourceRef\":{\"Entity\":\"_core custom measures\"}},\"Property\":\"AmountReceivableTotal\"}}}]}";

        var expectedExtract =
            new Extract(
                new File("report/AgingReport/TopcardsTotalAmount_7911f72ae58bd0ce629e", "DataTransforms"),
                JsonConvert.SerializeObject(JsonConvert.DeserializeObject<DataTransforms>(expectedConfigObject), Formatting.Indented)
            );

        extract.Should().BeEquivalentTo(expectedExtract);
    }
}