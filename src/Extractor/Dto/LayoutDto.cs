using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Extractor.Dto
{
    public class Layout
    {
        public string Config { get; set; }
        public string Filters { get; set; }
        public int Id { get; set; }
        public int LayoutOptimization { get; set; }
        public IList<Pod> Pods { get; set; }
        public IList<object> PublicCustomVisuals { get; set; }
        public int ReportId { get; set; }
        public IList<Resourcepackage> ResourcePackages { get; set; }
        public IList<Section> Sections { get; set; }
        public string Theme { get; set; }
    }

    public class Pod
    {
        public string BoundSection { get; set; }
        public string Config { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Resourcepackage
    {
        public bool Disabled { get; set; }
        public int Id { get; set; }
        public IList<Item> Items { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int ResourcePackageId { get; set; }
        public int ResourcePackageItemBlobInfoId { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
    }

    public class Section
    {
        public string Config { get; set; }
        public string DisplayName { get; set; }
        public int DisplayOption { get; set; }
        public string Filters { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public int Ordinal { get; set; }
        public IList<Visualcontainer> VisualContainers { get; set; }
        public int Width { get; set; }
    }

    public class Visualcontainer
    {
        public string Config { get; set; }
        public string Filters { get; set; }
        public float Height { get; set; }
        public long Id { get; set; }
        public float Width { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int Z { get; set; }
        public string DataTransforms { get; set; }
        public string Query { get; set; }
        public int TabOrder { get; set; }
    }


    public class Filter
    {
        public string DisplayName { get; set; }
        public object Expression { get; set; }
        public int HowCreated { get; set; }
        public string Name { get; set; }
        public int Ordinal { get; set; }
        public string Type { get; set; }
    }


    public class Config
    {
        public string Name { get; set; }
        public object Layouts { get; set; }
        public object SingleVisual { get; set; }
    }


    public class DataTransforms
    {
        public object Objects { get; set; }
        public object ProjectionOrdering { get; set; }
        public object QueryMetadata { get; set; }
        public IList<object> VisualElements { get; set; }
        public IList<object> Selects { get; set; }
    }

    public class Query
    {
        public IList<object> Commands { get; set; }
    }
}
