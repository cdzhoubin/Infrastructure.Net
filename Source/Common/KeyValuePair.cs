using System;
using System.Collections.Generic;
using System.Text;

namespace Zhoubin.Infrastructure.Common
{
    /// <summary>
    /// 字典数据，用于存储键值对
    /// </summary>
    public class KeyValue
    {
        /// <summary>
        /// 键，用于标识
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值，用于显示
        /// </summary>
        public string Value { get; set; }
    }
}
