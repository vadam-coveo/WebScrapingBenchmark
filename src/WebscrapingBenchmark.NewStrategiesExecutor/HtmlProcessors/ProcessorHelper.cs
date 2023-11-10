using System.Text.RegularExpressions;
using AngleSharp.Dom;
using Coveo.Connectors.Utilities.Web.WebScraping;
using HtmlAgilityPack;

namespace WebscrapingBenchmark.NewStrategiesExecutor.HtmlProcessors
{
    internal class ProcessorHelper
    {

        /// <summary>
        /// Extract the value of the HTML node if it is an attribute or text node, or the node's HTML
        /// content if it is an element.
        /// </summary>
        public static string GetValueOrElement(IElement htmlNode, IPathInfo pathInfo)
        {
            string value;
            if (htmlNode.NodeType == NodeType.Text)
                value = htmlNode.NodeValue;
            else if (pathInfo.IsText)
                value = htmlNode.TextContent;
            else if (pathInfo.IsAttribute)
                value = htmlNode.GetAttribute(pathInfo.AttributeName);
            else
                value = htmlNode.OuterHtml;

            value = CleanHtml(value);

            return value;
        }

        /// <summary>
        /// Extract the value of the HTML node if it is an attribute or text node, or the node's HTML
        /// content if it is an element.
        /// </summary>
        public static string GetValueOrElement(HtmlNode htmlNode, IPathInfo pathInfo)
        {
            string value;
            if (htmlNode.NodeType == HtmlNodeType.Text)
                value = htmlNode.InnerText;
            else if (pathInfo.IsText)
                value = htmlNode.InnerText;
            else if (pathInfo.IsAttribute)
                value = htmlNode.GetAttributeValue(pathInfo.AttributeName, null);
            else
                value = htmlNode.OuterHtml;

            value = CleanHtml(value);

            return value;
        }

        public static IPathInfo GetPathInfo(Selector selector)
        {
            if (selector.Type == SelectorType.CSS)
            {
                return CssPath.Create(selector.Path);
            }
            return XpathInfo.Create(selector.Path);
        }

        /// <summary>
        /// Removes all trailing whitespaces and de-entitize an HTML string.
        /// </summary>
        /// <param name="html">The HTML string.</param>
        /// <returns>A cleaned HTML string.</returns>
        public static string CleanHtml(string html)
        {
            return !string.IsNullOrEmpty(html) ? HtmlEntity.DeEntitize(html).Trim() : html;
        }


        internal interface IPathInfo
        {
            public string Path { get;  }

            /// <summary>
            /// Whether the path accesses the text elements or not.
            /// </summary>
            public bool IsText { get; }

            /// <summary>
            /// Whether the path select a specific attribute or not. If true, the attribute selected is set
            /// is the AttributeName property.
            /// </summary>
            public bool IsAttribute { get; }

            /// <summary>
            /// The name of the attribute to return, if the IsAttribute property is set to true.
            /// </summary>
            public string AttributeName { get; }
        }

        internal sealed class XpathInfo : IPathInfo
        {
            private const string GROUP_NAME_PATH = "path";
            private const string GROUP_NAME_ATTRIBUTE = "attributeName";

            private static readonly Regex _textElementRegex = new Regex(string.Format("^(?<{0}>.+)\\/text\\(.{{1,}}$", GROUP_NAME_PATH),
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            private static readonly Regex _attributeElementRegex =
                new Regex(string.Format(@"^(?<{0}>.+)\/@(?<{1}>.*)$", GROUP_NAME_PATH, GROUP_NAME_ATTRIBUTE),
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);


            /// <summary>
            /// The normalized path.
            /// </summary>
            public string Path { get; private set; }

            /// <summary>
            /// Whether the path accesses the text elements or not.
            /// </summary>
            public bool IsText { get; private set; }

            /// <summary>
            /// Whether the path select a specific attribute or not. If true, the attribute selected is set
            /// is the AttributeName property.
            /// </summary>
            public bool IsAttribute { get; private set; }

            /// <summary>
            /// The name of the attribute to return, if the IsAttribute property is set to true.
            /// </summary>
            public string AttributeName { get; private set; }

            public XpathInfo()
            {

            }

            public static XpathInfo Create(string path)
            {
                var instance = new XpathInfo();

                // Normalize the path.
                if (path != null && _textElementRegex.IsMatch(path))
                {
                    instance.IsText = true;
                    instance.Path = _textElementRegex.Match(path).Groups[GROUP_NAME_PATH].Value;
                }
                else if (path != null && _attributeElementRegex.IsMatch(path))
                {
                    var attrMatch = _attributeElementRegex.Match(path);
                    instance.IsAttribute = true;
                    instance.AttributeName = attrMatch.Groups[GROUP_NAME_ATTRIBUTE].Value;
                    instance.Path = attrMatch.Groups[GROUP_NAME_PATH].Value;
                }
                else
                {
                    // No special pseudo-element detected, the path is not modified.
                    instance.Path = path;
                }

                return instance;
            }
        }

        /// <summary>
        /// Provides an object representation of a CSS selector path.
        /// </summary>
        internal sealed class CssPath : IPathInfo
        {
            private const string GROUP_NAME_PATH = "path";
            private const string GROUP_NAME_ATTRIBUTE = "attributeName";

            private static readonly Regex _textElementRegex = new Regex(string.Format("^(?<{0}>.+)::text$", GROUP_NAME_PATH),
                                                                        RegexOptions.IgnoreCase | RegexOptions.Compiled);

            private static readonly Regex _attributeElementRegex =
                new Regex(string.Format(@"^(?<{0}>.+)::attr\((?<{1}>.*)\)$", GROUP_NAME_PATH, GROUP_NAME_ATTRIBUTE),
                          RegexOptions.IgnoreCase | RegexOptions.Compiled);

            /// <summary>
            /// The normalized path.
            /// </summary>
            public string Path { get; private set; }

            /// <summary>
            /// Whether the path accesses the text elements or not.
            /// </summary>
            public bool IsText { get; private set; }

            /// <summary>
            /// Whether the path select a specific attribute or not. If true, the attribute selected is set
            /// is the AttributeName property.
            /// </summary>
            public bool IsAttribute { get; private set; }

            /// <summary>
            /// The name of the attribute to return, if the IsAttribute property is set to true.
            /// </summary>
            public string AttributeName { get; private set; }

            /// <summary>
            /// Private constructor. Use <see cref="Create" /> method to create a new instance.
            /// </summary>
            private CssPath()
            { }

            /// <summary>
            /// Creates a new instance of <see cref="CssPath" /> using a string path.
            /// </summary>
            /// <param name="path">The path.</param>
            public static CssPath Create(string path)
            {
                var instance = new CssPath();

                // Normalize the path.
                if (path != null && _textElementRegex.IsMatch(path))
                {
                    instance.IsText = true;
                    instance.Path = _textElementRegex.Match(path).Groups[GROUP_NAME_PATH].Value;
                }
                else if (path != null && _attributeElementRegex.IsMatch(path))
                {
                    var attrMatch = _attributeElementRegex.Match(path);
                    instance.IsAttribute = true;
                    instance.AttributeName = attrMatch.Groups[GROUP_NAME_ATTRIBUTE].Value;
                    instance.Path = attrMatch.Groups[GROUP_NAME_PATH].Value;
                }
                else
                {
                    // No special pseudo-element detected, the path is not modified.
                    instance.Path = path;
                }

                return instance;
            }
        }
    }
}
