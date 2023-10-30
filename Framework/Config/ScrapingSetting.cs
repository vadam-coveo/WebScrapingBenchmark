using Newtonsoft.Json;

namespace WebScrapingBenchmark.Framework.Config
{
        /// <summary>
        /// Class that contains the scraping settings for one or more specific pages and/or sub-items.
        /// When applied to a sub-item, the selectors are applied on the sub-item's element by default.
        /// </summary>
        public class ScrapingSetting
        {
            /// <summary>
            /// Defines the name of the ScrapingSetting.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Defines for which pages and sub items this config should be used.
            /// </summary>
            [JsonProperty("for")]
            public Restriction Restriction { get; set; }

            /// <summary>
            /// Specify additional metadatas that will be retrieved from the web page using specific
            /// selectors. Each key in the dictionary represents a single metadata.
            /// </summary>
            public IDictionary<string, Selector> Metadata { get; set; }

            /// <summary>
            /// Defines parts of the web page to be removed using selectors.
            /// </summary>
            public IEnumerable<Selector> Exclude { get; set; }

            /// <summary>
            /// A Dictionary of sub-item type and the associated selector. This defines how to
            /// retrieve sub-items for each sub-item types.
            /// </summary>
            public IDictionary<string, Selector> SubItems { get; set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            public ScrapingSetting()
            {
                Restriction = new Restriction();
                Metadata = new Dictionary<string, Selector>();
                Exclude = new List<Selector>();
                SubItems = new Dictionary<string, Selector>();
            }
        }

        /// <summary>
        /// Class that represents for which pages and sub-items a specific <see cref="ScrapingSetting" /> should be applied.
        /// </summary>
        public class Restriction
        {
            /// <summary>
            /// Apply this config to only the pages which match these regex.
            /// </summary>
            public IEnumerable<string> Urls { get; set; }

            /// <summary>
            /// Apply this config to pages that have been tagged as one of these types.
            /// </summary>
            public IEnumerable<string> Types { get; set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            public Restriction()
            {
                Urls = new List<string>();
                Types = new List<string>();
            }
        }
}
