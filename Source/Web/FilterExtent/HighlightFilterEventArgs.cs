using System;

namespace Zhoubin.Infrastructure.Web.FilterExtent
{
    /// <summary>
    /// Used by a custom filter to alter the value of the output.
    /// </summary>
    /// <author>Yvan Rodrigues</author>
    /// <copyright>© 2011 <a href="http://two-red-cells.com">Red Cell Innovation Inc.</a></copyright>
    /// <license>Provided under the terms of the <a href="http://www.codeproject.com/info/cpol10.aspx">Code Project Open License</a>.</license>
    public sealed class HighlightFilterEventArgs : EventArgs
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterEventArgs"/> class.
        /// </summary>
        internal HighlightFilterEventArgs(string needle, string haystack)
        {
            Needle = needle;
            Haystack = haystack;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the haystack.
        /// </summary>
        /// <value>
        /// The haystack.
        /// </value>
        public string Haystack { get; set; }

        /// <summary>
        /// Gets or sets the needle.
        /// </summary>
        /// <value>
        /// The needle.
        /// </value>
        public string Needle { get; set; }
        #endregion
    }
}