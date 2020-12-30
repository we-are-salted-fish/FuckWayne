using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuckWayne.Models
{
    /// <summary>
    /// 图片信息
    /// </summary>
    public class Belle
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 多张图片(来源地址+图片地址)
        /// </summary>
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
