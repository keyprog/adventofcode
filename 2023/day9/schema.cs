// Generated by https://quicktype.io

namespace QuickType
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Visualizer
    {
        [JsonProperty("definitions")]
        public Definitions Definitions { get; set; }

        [JsonProperty("anyOf")]
        public AnyOf[] AnyOf { get; set; }
    }

    public partial class AnyOf
    {
        [JsonProperty("$ref")]
        public string Ref { get; set; }
    }

    public partial class Definitions
    {
        [JsonProperty("TreeVisualizationData")]
        public TreeVisualizationData TreeVisualizationData { get; set; }

        [JsonProperty("TreeNode")]
        public TreeNode TreeNode { get; set; }

        [JsonProperty("TreeNodeItem")]
        public TreeNodeItem TreeNodeItem { get; set; }

        [JsonProperty("AstTreeVisualizationData")]
        public AstTreeVisualizationData AstTreeVisualizationData { get; set; }

        [JsonProperty("AstTreeNode")]
        public AstTreeNode AstTreeNode { get; set; }

        [JsonProperty("AstTreeNodeItem")]
        public AstTreeNodeItem AstTreeNodeItem { get; set; }

        [JsonProperty("GraphvizDotVisualizationData")]
        public GraphvizDotVisualizationData GraphvizDotVisualizationData { get; set; }

        [JsonProperty("GraphVisualizationData")]
        public GraphVisualizationData GraphVisualizationData { get; set; }

        [JsonProperty("GraphNode")]
        public GraphNode GraphNode { get; set; }

        [JsonProperty("GraphEdge")]
        public GraphEdge GraphEdge { get; set; }

        [JsonProperty("GridVisualizationData")]
        public GridVisualizationData GridVisualizationData { get; set; }

        [JsonProperty("ImageVisualizationData")]
        public ImageVisualizationData ImageVisualizationData { get; set; }

        [JsonProperty("MonacoTextVisualizationData")]
        public MonacoTextVisualizationData MonacoTextVisualizationData { get; set; }

        [JsonProperty("LineColumnRange")]
        public LineColumnRange LineColumnRange { get; set; }

        [JsonProperty("LineColumnPosition")]
        public LineColumnPosition LineColumnPosition { get; set; }

        [JsonProperty("MonacoTextDiffVisualizationData")]
        public MonacoTextDiffVisualizationData MonacoTextDiffVisualizationData { get; set; }

        [JsonProperty("TableVisualizationData")]
        public TableVisualizationData TableVisualizationData { get; set; }

        [JsonProperty("PlotlyVisualizationData")]
        public PlotlyVisualizationData PlotlyVisualizationData { get; set; }

        [JsonProperty("SimpleTextVisualizationData")]
        public SimpleTextVisualizationData SimpleTextVisualizationData { get; set; }

        [JsonProperty("SvgVisualizationData")]
        public SvgVisualizationData SvgVisualizationData { get; set; }
    }

    public partial class AstTreeNode
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public AstTreeNodeProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] AstTreeNodeRequired { get; set; }
    }

    public partial class AstTreeNodeProperties
    {
        [JsonProperty("children")]
        public EdgesClass Children { get; set; }

        [JsonProperty("items")]
        public EdgesClass Items { get; set; }

        [JsonProperty("segment")]
        public PuneHedgehog Segment { get; set; }

        [JsonProperty("isMarked")]
        public PuneHedgehog IsMarked { get; set; }

        [JsonProperty("span")]
        public Span Span { get; set; }
    }

    public partial class EdgesClass
    {
        [JsonProperty("type")]
        public PurpleType Type { get; set; }

        [JsonProperty("items")]
        public AnyOf Items { get; set; }
    }

    public partial class PuneHedgehog
    {
        [JsonProperty("type")]
        public FluffyType Type { get; set; }
    }

    public partial class Span
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public SpanProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] SpanRequired { get; set; }
    }

    public partial class SpanProperties
    {
        [JsonProperty("start")]
        public PuneHedgehog Start { get; set; }

        [JsonProperty("length")]
        public PuneHedgehog Length { get; set; }
    }

    public partial class AstTreeNodeItem
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public AstTreeNodeItemProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] AstTreeNodeItemRequired { get; set; }
    }

    public partial class AstTreeNodeItemProperties
    {
        [JsonProperty("text")]
        public PuneHedgehog Text { get; set; }

        [JsonProperty("emphasis")]
        public PurpleEmphasis Emphasis { get; set; }
    }

    public partial class PurpleEmphasis
    {
        [JsonProperty("anyOf")]
        public EmphasisAnyOf[] AnyOf { get; set; }
    }

    public partial class EmphasisAnyOf
    {
        [JsonProperty("enum", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Enum { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public FluffyType? Type { get; set; }
    }

    public partial class AstTreeVisualizationData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public AstTreeVisualizationDataProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] AstTreeVisualizationDataRequired { get; set; }
    }

    public partial class AstTreeVisualizationDataProperties
    {
        [JsonProperty("kind")]
        public PurpleKind Kind { get; set; }

        [JsonProperty("root")]
        public AnyOf Root { get; set; }

        [JsonProperty("text")]
        public PuneHedgehog Text { get; set; }

        [JsonProperty("fileName")]
        public PuneHedgehog FileName { get; set; }
    }

    public partial class PurpleKind
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public PurpleProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] KindRequired { get; set; }
    }

    public partial class PurpleProperties
    {
        [JsonProperty("ast")]
        public Ast Ast { get; set; }

        [JsonProperty("tree")]
        public Ast Tree { get; set; }

        [JsonProperty("text")]
        public Ast Text { get; set; }
    }

    public partial class Ast
    {
        [JsonProperty("enum")]
        public bool[] Enum { get; set; }
    }

    public partial class GraphEdge
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public GraphEdgeProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] GraphEdgeRequired { get; set; }
    }

    public partial class GraphEdgeProperties
    {
        [JsonProperty("from")]
        public PuneHedgehog From { get; set; }

        [JsonProperty("to")]
        public PuneHedgehog To { get; set; }

        [JsonProperty("label")]
        public PuneHedgehog Label { get; set; }

        [JsonProperty("id")]
        public PuneHedgehog Id { get; set; }

        [JsonProperty("color")]
        public PuneHedgehog Color { get; set; }

        [JsonProperty("style")]
        public Style Style { get; set; }
    }

    public partial class Style
    {
        [JsonProperty("oneOf")]
        public StyleOneOf[] OneOf { get; set; }
    }

    public partial class StyleOneOf
    {
        [JsonProperty("enum")]
        public string[] Enum { get; set; }
    }

    public partial class GraphNode
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public GraphNodeProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] GraphNodeRequired { get; set; }
    }

    public partial class GraphNodeProperties
    {
        [JsonProperty("id")]
        public PuneHedgehog Id { get; set; }

        [JsonProperty("label")]
        public PuneHedgehog Label { get; set; }

        [JsonProperty("color")]
        public PuneHedgehog Color { get; set; }

        [JsonProperty("shape")]
        public Style Shape { get; set; }
    }

    public partial class GraphVisualizationData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public GraphVisualizationDataProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] GraphVisualizationDataRequired { get; set; }
    }

    public partial class GraphVisualizationDataProperties
    {
        [JsonProperty("kind")]
        public FluffyKind Kind { get; set; }

        [JsonProperty("nodes")]
        public EdgesClass Nodes { get; set; }

        [JsonProperty("edges")]
        public EdgesClass Edges { get; set; }
    }

    public partial class FluffyKind
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public FluffyProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] KindRequired { get; set; }
    }

    public partial class FluffyProperties
    {
        [JsonProperty("graph")]
        public Ast Graph { get; set; }
    }

    public partial class GraphvizDotVisualizationData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public GraphvizDotVisualizationDataProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] GraphvizDotVisualizationDataRequired { get; set; }
    }

    public partial class GraphvizDotVisualizationDataProperties
    {
        [JsonProperty("kind")]
        public TentacledKind Kind { get; set; }

        [JsonProperty("text")]
        public PuneHedgehog Text { get; set; }
    }

    public partial class TentacledKind
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public TentacledProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] KindRequired { get; set; }
    }

    public partial class TentacledProperties
    {
        [JsonProperty("dotGraph")]
        public Ast DotGraph { get; set; }
    }

    public partial class GridVisualizationData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public GridVisualizationDataProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] GridVisualizationDataRequired { get; set; }
    }

    public partial class GridVisualizationDataProperties
    {
        [JsonProperty("kind")]
        public StickyKind Kind { get; set; }

        [JsonProperty("columnLabels")]
        public ColumnLabels ColumnLabels { get; set; }

        [JsonProperty("rows")]
        public PurpleRows Rows { get; set; }

        [JsonProperty("markers")]
        public Markers Markers { get; set; }
    }

    public partial class ColumnLabels
    {
        [JsonProperty("type")]
        public PurpleType Type { get; set; }

        [JsonProperty("items")]
        public ColumnLabelsItems Items { get; set; }
    }

    public partial class ColumnLabelsItems
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public StickyProperties Properties { get; set; }

        [JsonProperty("required")]
        public object[] ItemsRequired { get; set; }
    }

    public partial class StickyProperties
    {
        [JsonProperty("label")]
        public PuneHedgehog Label { get; set; }
    }

    public partial class StickyKind
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public IndigoProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] KindRequired { get; set; }
    }

    public partial class IndigoProperties
    {
        [JsonProperty("grid")]
        public Ast Grid { get; set; }
    }

    public partial class Markers
    {
        [JsonProperty("type")]
        public PurpleType Type { get; set; }

        [JsonProperty("items")]
        public MarkersItems Items { get; set; }
    }

    public partial class MarkersItems
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public IndecentProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] ItemsRequired { get; set; }
    }

    public partial class IndecentProperties
    {
        [JsonProperty("id")]
        public PuneHedgehog Id { get; set; }

        [JsonProperty("row")]
        public PuneHedgehog Row { get; set; }

        [JsonProperty("column")]
        public PuneHedgehog Column { get; set; }

        [JsonProperty("rows")]
        public PuneHedgehog Rows { get; set; }

        [JsonProperty("columns")]
        public PuneHedgehog Columns { get; set; }

        [JsonProperty("label")]
        public PuneHedgehog Label { get; set; }

        [JsonProperty("color")]
        public PuneHedgehog Color { get; set; }
    }

    public partial class PurpleRows
    {
        [JsonProperty("type")]
        public PurpleType Type { get; set; }

        [JsonProperty("items")]
        public PurpleItems Items { get; set; }
    }

    public partial class PurpleItems
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public HilariousProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] ItemsRequired { get; set; }
    }

    public partial class HilariousProperties
    {
        [JsonProperty("label")]
        public PuneHedgehog Label { get; set; }

        [JsonProperty("columns")]
        public Columns Columns { get; set; }
    }

    public partial class Columns
    {
        [JsonProperty("type")]
        public PurpleType Type { get; set; }

        [JsonProperty("items")]
        public ColumnsItems Items { get; set; }
    }

    public partial class ColumnsItems
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public AmbitiousProperties Properties { get; set; }

        [JsonProperty("required")]
        public object[] ItemsRequired { get; set; }
    }

    public partial class AmbitiousProperties
    {
        [JsonProperty("content")]
        public PuneHedgehog Content { get; set; }

        [JsonProperty("tag")]
        public Base64Data Tag { get; set; }

        [JsonProperty("color")]
        public PuneHedgehog Color { get; set; }
    }

    public partial class Base64Data
    {
        [JsonProperty("type")]
        public FluffyType Type { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class ImageVisualizationData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public ImageVisualizationDataProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] ImageVisualizationDataRequired { get; set; }
    }

    public partial class ImageVisualizationDataProperties
    {
        [JsonProperty("kind")]
        public IndigoKind Kind { get; set; }

        [JsonProperty("base64Data")]
        public Base64Data Base64Data { get; set; }
    }

    public partial class IndigoKind
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public CunningProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] KindRequired { get; set; }
    }

    public partial class CunningProperties
    {
        [JsonProperty("imagePng")]
        public Ast ImagePng { get; set; }
    }

    public partial class LineColumnPosition
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public LineColumnPositionProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] LineColumnPositionRequired { get; set; }
    }

    public partial class LineColumnPositionProperties
    {
        [JsonProperty("line")]
        public Base64Data Line { get; set; }

        [JsonProperty("column")]
        public Base64Data Column { get; set; }
    }

    public partial class LineColumnRange
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public LineColumnRangeProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] LineColumnRangeRequired { get; set; }
    }

    public partial class LineColumnRangeProperties
    {
        [JsonProperty("start")]
        public End Start { get; set; }

        [JsonProperty("end")]
        public End End { get; set; }
    }

    public partial class End
    {
        [JsonProperty("$ref")]
        public string Ref { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class MonacoTextDiffVisualizationData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public MonacoTextDiffVisualizationDataProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] MonacoTextDiffVisualizationDataRequired { get; set; }
    }

    public partial class MonacoTextDiffVisualizationDataProperties
    {
        [JsonProperty("kind")]
        public IndecentKind Kind { get; set; }

        [JsonProperty("text")]
        public Base64Data Text { get; set; }

        [JsonProperty("otherText")]
        public Base64Data OtherText { get; set; }

        [JsonProperty("fileName")]
        public Base64Data FileName { get; set; }
    }

    public partial class IndecentKind
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public MagentaProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] KindRequired { get; set; }
    }

    public partial class MagentaProperties
    {
        [JsonProperty("text")]
        public Ast Text { get; set; }
    }

    public partial class MonacoTextVisualizationData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public MonacoTextVisualizationDataProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] MonacoTextVisualizationDataRequired { get; set; }
    }

    public partial class MonacoTextVisualizationDataProperties
    {
        [JsonProperty("kind")]
        public IndecentKind Kind { get; set; }

        [JsonProperty("text")]
        public Base64Data Text { get; set; }

        [JsonProperty("decorations")]
        public Decorations Decorations { get; set; }

        [JsonProperty("jsonSchemas")]
        public JsonSchemas JsonSchemas { get; set; }

        [JsonProperty("fileName")]
        public Base64Data FileName { get; set; }
    }

    public partial class Decorations
    {
        [JsonProperty("type")]
        public PurpleType Type { get; set; }

        [JsonProperty("items")]
        public DecorationsItems Items { get; set; }
    }

    public partial class DecorationsItems
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public FriskyProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] ItemsRequired { get; set; }
    }

    public partial class FriskyProperties
    {
        [JsonProperty("range")]
        public Range Range { get; set; }

        [JsonProperty("label")]
        public PuneHedgehog Label { get; set; }
    }

    public partial class Range
    {
        [JsonProperty("oneOf")]
        public AnyOf[] OneOf { get; set; }
    }

    public partial class JsonSchemas
    {
        [JsonProperty("type")]
        public PurpleType Type { get; set; }

        [JsonProperty("items")]
        public JsonSchemasItems Items { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class JsonSchemasItems
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public MischievousProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] ItemsRequired { get; set; }
    }

    public partial class MischievousProperties
    {
        [JsonProperty("schema")]
        public Schema Schema { get; set; }
    }

    public partial class Schema
    {
        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class PlotlyVisualizationData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public PlotlyVisualizationDataProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] PlotlyVisualizationDataRequired { get; set; }
    }

    public partial class PlotlyVisualizationDataProperties
    {
        [JsonProperty("kind")]
        public HilariousKind Kind { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("layout")]
        public Layout Layout { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("type")]
        public PurpleType Type { get; set; }

        [JsonProperty("items")]
        public DataItems Items { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class DataItems
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public BraggadociousProperties Properties { get; set; }

        [JsonProperty("required")]
        public object[] ItemsRequired { get; set; }
    }

    public partial class BraggadociousProperties
    {
        [JsonProperty("text")]
        public Text Text { get; set; }

        [JsonProperty("xaxis")]
        public PuneHedgehog Xaxis { get; set; }

        [JsonProperty("yaxis")]
        public PuneHedgehog Yaxis { get; set; }

        [JsonProperty("x")]
        public ZClass X { get; set; }

        [JsonProperty("y")]
        public ZClass Y { get; set; }

        [JsonProperty("z")]
        public ZClass Z { get; set; }

        [JsonProperty("cells")]
        public Cells Cells { get; set; }

        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("domain")]
        public Domain Domain { get; set; }

        [JsonProperty("type")]
        public Style Type { get; set; }

        [JsonProperty("mode")]
        public Style Mode { get; set; }
    }

    public partial class Cells
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public CellsProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] CellsRequired { get; set; }
    }

    public partial class CellsProperties
    {
        [JsonProperty("values")]
        public Values Values { get; set; }
    }

    public partial class Values
    {
        [JsonProperty("type")]
        public PurpleType Type { get; set; }

        [JsonProperty("items")]
        public ZClass Items { get; set; }
    }

    public partial class ZClass
    {
        [JsonProperty("oneOf")]
        public XOneOf[] OneOf { get; set; }
    }

    public partial class XOneOf
    {
        [JsonProperty("type")]
        public PurpleType Type { get; set; }

        [JsonProperty("items")]
        public OneOfItems Items { get; set; }
    }

    public partial class OneOfItems
    {
        [JsonProperty("oneOf", NullValueHandling = NullValueHandling.Ignore)]
        public PurpleOneOf[] OneOf { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public PurpleType? Type { get; set; }

        [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
        public ItemsItems Items { get; set; }
    }

    public partial class ItemsItems
    {
        [JsonProperty("oneOf")]
        public PurpleOneOf[] OneOf { get; set; }
    }

    public partial class PurpleOneOf
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public FluffyType? Type { get; set; }

        [JsonProperty("enum", NullValueHandling = NullValueHandling.Ignore)]
        public object[] Enum { get; set; }
    }

    public partial class Domain
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public DomainProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] DomainRequired { get; set; }
    }

    public partial class DomainProperties
    {
        [JsonProperty("x")]
        public XElement X { get; set; }

        [JsonProperty("y")]
        public XElement Y { get; set; }
    }

    public partial class XElement
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
        public PuneHedgehog Items { get; set; }
    }

    public partial class Header
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public HeaderProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] HeaderRequired { get; set; }
    }

    public partial class HeaderProperties
    {
        [JsonProperty("values")]
        public ZClass Values { get; set; }
    }

    public partial class Text
    {
        [JsonProperty("oneOf")]
        public XElement[] OneOf { get; set; }
    }

    public partial class HilariousKind
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public Properties1 Properties { get; set; }

        [JsonProperty("required")]
        public string[] KindRequired { get; set; }
    }

    public partial class Properties1
    {
        [JsonProperty("plotly")]
        public Ast Plotly { get; set; }
    }

    public partial class Layout
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public LayoutProperties Properties { get; set; }

        [JsonProperty("required")]
        public object[] LayoutRequired { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class LayoutProperties
    {
        [JsonProperty("title")]
        public PuneHedgehog Title { get; set; }
    }

    public partial class SimpleTextVisualizationData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public SimpleTextVisualizationDataProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] SimpleTextVisualizationDataRequired { get; set; }
    }

    public partial class SimpleTextVisualizationDataProperties
    {
        [JsonProperty("kind")]
        public IndecentKind Kind { get; set; }

        [JsonProperty("text")]
        public PuneHedgehog Text { get; set; }
    }

    public partial class SvgVisualizationData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public SvgVisualizationDataProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] SvgVisualizationDataRequired { get; set; }
    }

    public partial class SvgVisualizationDataProperties
    {
        [JsonProperty("kind")]
        public AmbitiousKind Kind { get; set; }

        [JsonProperty("text")]
        public Base64Data Text { get; set; }
    }

    public partial class AmbitiousKind
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public Properties2 Properties { get; set; }

        [JsonProperty("required")]
        public string[] KindRequired { get; set; }
    }

    public partial class Properties2
    {
        [JsonProperty("svg")]
        public Ast Svg { get; set; }
    }

    public partial class TableVisualizationData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public TableVisualizationDataProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] TableVisualizationDataRequired { get; set; }
    }

    public partial class TableVisualizationDataProperties
    {
        [JsonProperty("kind")]
        public CunningKind Kind { get; set; }

        [JsonProperty("rows")]
        public FluffyRows Rows { get; set; }
    }

    public partial class CunningKind
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public Properties3 Properties { get; set; }

        [JsonProperty("required")]
        public string[] KindRequired { get; set; }
    }

    public partial class Properties3
    {
        [JsonProperty("table")]
        public Ast Table { get; set; }
    }

    public partial class FluffyRows
    {
        [JsonProperty("type")]
        public PurpleType Type { get; set; }

        [JsonProperty("items")]
        public FluffyItems Items { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class FluffyItems
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public Properties4 Properties { get; set; }

        [JsonProperty("required")]
        public object[] ItemsRequired { get; set; }
    }

    public partial class Properties4
    {
    }

    public partial class TreeNode
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public TreeNodeProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] TreeNodeRequired { get; set; }
    }

    public partial class TreeNodeProperties
    {
        [JsonProperty("children")]
        public PurpleChildren Children { get; set; }

        [JsonProperty("items")]
        public PurpleChildren Items { get; set; }

        [JsonProperty("segment")]
        public Base64Data Segment { get; set; }

        [JsonProperty("isMarked")]
        public Base64Data IsMarked { get; set; }
    }

    public partial class PurpleChildren
    {
        [JsonProperty("type")]
        public PurpleType Type { get; set; }

        [JsonProperty("items")]
        public AnyOf Items { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class TreeNodeItem
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public TreeNodeItemProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] TreeNodeItemRequired { get; set; }
    }

    public partial class TreeNodeItemProperties
    {
        [JsonProperty("text")]
        public Base64Data Text { get; set; }

        [JsonProperty("emphasis")]
        public FluffyEmphasis Emphasis { get; set; }
    }

    public partial class FluffyEmphasis
    {
        [JsonProperty("anyOf")]
        public EmphasisAnyOf[] AnyOf { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class TreeVisualizationData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public TreeVisualizationDataProperties Properties { get; set; }

        [JsonProperty("required")]
        public string[] TreeVisualizationDataRequired { get; set; }
    }

    public partial class TreeVisualizationDataProperties
    {
        [JsonProperty("kind")]
        public MagentaKind Kind { get; set; }

        [JsonProperty("root")]
        public AnyOf Root { get; set; }
    }

    public partial class MagentaKind
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public Properties5 Properties { get; set; }

        [JsonProperty("required")]
        public string[] KindRequired { get; set; }
    }

    public partial class Properties5
    {
        [JsonProperty("tree")]
        public Ast Tree { get; set; }
    }

    public enum PurpleType { Array };

    public enum FluffyType { Boolean, Number, String };

    public partial class Visualizer
    {
        public static Visualizer FromJson(string json) => JsonConvert.DeserializeObject<Visualizer>(json, QuickType.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Visualizer self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                PurpleTypeConverter.Singleton,
                FluffyTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class PurpleTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(PurpleType) || t == typeof(PurpleType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "array")
            {
                return PurpleType.Array;
            }
            throw new Exception("Cannot unmarshal type PurpleType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (PurpleType)untypedValue;
            if (value == PurpleType.Array)
            {
                serializer.Serialize(writer, "array");
                return;
            }
            throw new Exception("Cannot marshal type PurpleType");
        }

        public static readonly PurpleTypeConverter Singleton = new PurpleTypeConverter();
    }

    internal class FluffyTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(FluffyType) || t == typeof(FluffyType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "boolean":
                    return FluffyType.Boolean;
                case "number":
                    return FluffyType.Number;
                case "string":
                    return FluffyType.String;
            }
            throw new Exception("Cannot unmarshal type FluffyType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (FluffyType)untypedValue;
            switch (value)
            {
                case FluffyType.Boolean:
                    serializer.Serialize(writer, "boolean");
                    return;
                case FluffyType.Number:
                    serializer.Serialize(writer, "number");
                    return;
                case FluffyType.String:
                    serializer.Serialize(writer, "string");
                    return;
            }
            throw new Exception("Cannot marshal type FluffyType");
        }

        public static readonly FluffyTypeConverter Singleton = new FluffyTypeConverter();
    }
}