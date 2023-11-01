using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebScrapingBenchmark.Framework.Scrapers
{
    internal class CSSHelper
    {

        /// <summary>
        /// Provides an object representation of a CSS selector path.
        /// </summary>
        private sealed class CssPath
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
