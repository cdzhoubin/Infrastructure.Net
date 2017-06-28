using System;

namespace Zhoubin.Infrastructure.Web.FilterExtent
{
    /// <summary>
    /// Used by a custom filter to alter the value of the output.
    /// </summary>
    /// <author>Yvan Rodrigues</author>
    /// <copyright>© 2011 <a href="http://two-red-cells.com">Red Cell Innovation Inc.</a></copyright>
    /// <license>Provided under the terms of the <a href="http://www.codeproject.com/info/cpol10.aspx">Code Project Open License</a>.</license>
    public sealed class FilterEventArgs : EventArgs
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterEventArgs"/> class.
        /// </summary>
        internal FilterEventArgs(string value)
        {
            Value = value;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
        #endregion
    }
}