using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Zhoubin.Infrastructure.Web.FilterExtent
{
    /// <summary>
    /// HighlightFilter wraps a string specified by a regular expression with additional markup if it is found in a stream.
    /// </summary>
    /// <author>Yvan Rodrigues</author>
    /// <copyright>© 2011 <a href="http://two-red-cells.com">Red Cell Innovation Inc.</a></copyright>
    /// <license>Provided under the terms of the <a href="http://www.codeproject.com/info/cpol10.aspx">Code Project Open License</a>.</license>
    public class HighlightFilter : FilterStream
    {
        #region Initialization
        /// <summary>
        /// 初始化 <see cref="HighlightFilter"/> 类.
        /// </summary>
        /// <param name="response">输出请求.</param>
        /// <param name="needle">高亮字符串.</param>
        public HighlightFilter(HttpResponse response, string needle)
            : base(response)
        {
            Needle = needle;
            IsHtml5 = true;
            MatchCase = false;
            MatchWholeWords = false;
            UseRegex = false;
            WordBoundary = @"\b";
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the needle should be considered a regular expression.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the needle should be considered a regular expression; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// The default value is false.
        /// </remarks>
        public bool UseRegex { get; set; }

        /// <summary>
        /// Gets or sets the opening tag that is wrapped around the found text.
        /// </summary>
        /// <value>The opening tag.</value>
        /// <remarks>
        /// <![CDATA[ The default value is <span class="highlight"> if Html5 = false, or <mark> if Html5 = true. ]]>
        /// Use of OpenTag and IsHtml5 is mutually exclusive.
        /// </remarks>
        public string OpenTag { get; set; }

        /// <summary>
        /// Gets or sets the closing tag that is wrapped around the found text.
        /// </summary>
        /// <value>
        /// The closing tag.
        /// </value>
        /// <remarks>
        /// <![CDATA[ The default value is </span> if Html5 = false, or </mark> if Html5 = true. ]]>
        /// Use of CloseTag and IsHtml5 is mutually exclusive.
        /// </remarks>
        public string CloseTag { get; set; }

        private bool _html5;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="HighlightFilter"/> should add Html5 markup.
        /// </summary>
        /// <value>
        ///   <c>true</c> if HTML5; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        public bool IsHtml5
        {
            get { return _html5; }
            set
            {
                _html5 = value;
                if (value) { OpenTag = "<mark>"; CloseTag = "</mark>"; }
                else { OpenTag = "<span class=\"highlight\">"; CloseTag = "</span>"; }
            }
        }

        /// <summary>
        /// Gets or sets the needle.
        /// </summary>
        /// <value>
        /// The needle.
        /// </value>
        public string Needle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to match case.
        /// </summary>
        /// <value>
        ///   <c>true</c> to match case; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// The default value is false.
        /// </remarks>
        public bool MatchCase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to only match whole words.
        /// </summary>
        /// <value>
        ///   <c>true</c> to only match whole words; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// The default value is false.
        /// </remarks>
        public bool MatchWholeWords { get; set; }

        /// <summary>
        /// Gets or sets the word boundary.
        /// </summary>
        /// <value>
        /// The word boundary as a regular expression character class.
        /// </value>
        public string WordBoundary { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the filter is highlighting.
        /// </summary>
        public event EventHandler<HighlightFilterEventArgs> Highlighting;
        #endregion

        #region Methods
        /// <summary>
        /// Filters the specified string.
        /// </summary>
        /// <param name="haystack">The haystack.</param>
        /// <returns>
        /// The filtered haystack.
        /// </returns>
        protected override string OnFilter(string haystack)
        {
            // Invoke events.
            if (Highlighting != null)
                Highlighting(this, new HighlightFilterEventArgs(Needle, haystack));

            // Pass-through empty searches.
            if (string.IsNullOrEmpty(Needle))
                return base.OnFilter(haystack);

            // If UseRegex is false, escape the needle so that it's used literally.
            string needle = UseRegex ? Needle : Regex.Escape(Needle);
            // Define Regex groups.
            needle = "(?<Needle>" + needle + ")";
            if (MatchWholeWords) needle = "(?<BoundA>" + WordBoundary + ")" + needle + "(?<BoundB>" + WordBoundary + ")";
            needle = "(?<=>[^<>]*?)" + needle + "(?=[^<>]*?<)";

            // Make note of the locations of elements that we don't want to break.
            var ignore = Regex.Matches(haystack, @"(?<=<(title|head|script|style)( [^>]*?)?>).*(?=</\1>)", RegexOptions.Singleline);

            // Let Regex do the dirty work with case sensitivity.
            var options = RegexOptions.Singleline;
            if (!MatchCase) options |= RegexOptions.IgnoreCase;

            // Define our replacer callback.
            MatchEvaluator replacer = match =>
            {
                // Make sure not to break code.
                if (ignore.Cast<Match>().Any(m => match.Index >= m.Index && match.Index <= m.Index + m.Length))
                {
                    return match.Groups["Needle"].Value;
                }

                string a = match.Groups["BoundA"] == null ? string.Empty : match.Groups["BoundA"].Value;
                string b = match.Groups["BoundB"] == null ? string.Empty : match.Groups["BoundB"].Value;
                return a + OpenTag + match.Groups["Needle"].Value + CloseTag + b;
            };

            // Make it so.
            string filtered = Regex.Replace(haystack, needle, replacer, options);
            return base.OnFilter(filtered);
        }
        #endregion
    }
}