using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace WebScrapingBenchmark.Framework.Config
{
    /// <summary>
    /// Represents the possible selector types used for scraping.
    /// </summary>
    public enum SelectorType
    {
        CSS,
        XPATH
    }

    /// <summary>
    /// Class representing a single selector used to retrieve specific parts of a web page.
    /// </summary>
    public class Selector
    {
        /// <summary>
        /// The chosen <see cref="SelectorType" />.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SelectorType Type { get; set; }

        /// <summary>
        /// The actual selector value or path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Whether or not this selector should be applied from the root of the currently scraped page.
        /// </summary>
        /// <remarks>
        /// By default, selectors are relative, thus if used in a sub-item, they will start from the
        /// sub-item's html element.
        /// </remarks>
        public bool IsAbsolute { get; set; }

        /// <summary>
        /// Whether or not to evaluate this selector as a relative Url, returning true if the selector returns an element.
        /// </summary>
        public bool IsRelativeUrl { get; set; }

        /// <summary>
        /// Whether or not to evaluate this selector as a boolean, returning true if the selector returns an element.
        /// </summary>
        public bool IsBoolean { get; set; }

        /// <summary>
        /// Whether or not to remove duplicates if the selector returns more than one element.
        /// </summary>
        public bool RemoveDuplicates { get; set; }

        /// <inheritDoc />
        public override string ToString()
        {
            return string.Format("{{ \"Type\": \"{0}\", \"Path\": \"{1}\", \"IsAbsolute\": {2}, \"IsBoolean\": {3}, \"RemoveDuplicates\": {4} }}",
                Type,
                Path,
                IsAbsolute,
                IsBoolean,
                RemoveDuplicates);
        }
    }
}
