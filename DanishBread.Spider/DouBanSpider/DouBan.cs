using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using FuckWayne.Models;
using NewLife.Log;

namespace FuckWayne.DouBan
{
    /// <summary>
    /// 豆瓣图片处理 url:https://www.dbmeinv.com/index.htm
    /// </summary>
    public class DouBan
    {

        public DouBan(string saveFolder)
        {
            this.SaveFolder = saveFolder;
            if (!saveFolder.EndsWith("\\"))
            {
                this.SaveFolder += "\\";
            }
        }

        /// <summary>
        /// 保存路径
        /// </summary>
        private string SaveFolder = "";

        /// <summary>
        /// 分类ID数据 2-胸 3-腿 4-脸 5-杂 6-臀 7-袜子
        /// </summary>
        private int[] Ids = new int[] { 2, 3, 4, 5, 6, 7 };

        /// <summary>
        /// 豆瓣地址
        /// </summary>
        private string DouBanUrl = "https://www.buxiuse.com/?cid={0}&page={1}";

        /// <summary>
        /// 已下载图片链接
        /// </summary>
        private List<string> ImageUrlList = new List<string>();

        /// <summary>
        /// 网页源代码
        /// </summary>
        /// <param name="address"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string GetUrlString(string address, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.GetEncoding("UTF-8");
            }
            var str = string.Empty;
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Encoding = encoding;
                    str = wc.DownloadString(address);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"请求{address}发生错误,原因{e.Message}");
            }

            return str;
        }

        /// <summary>
        /// 获取网页源代码并转换为IHtmlDocument
        /// </summary>
        /// <param name="address"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public IHtmlDocument GetHtmlDocument(string address, Encoding encoding = null)
        {
            var resultStr = GetUrlString(address, encoding);
            return new HtmlParser().ParseDocument(resultStr);
        }

        /// <summary>
        /// 获取指定链接下的图片(默认随机获取某个分类下的第一页图片)
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public List<Belle> GetListBelle(string url = "")
        {
            // 随机数
            var rand = new Random();

            // 豆瓣地址
            var address = string.IsNullOrWhiteSpace(url) ? "https://www.dbmeinv.com/index.htm?cid=" + Ids[rand.Next(Ids.Length)] : url;

            // 请求豆辨网
            var document = GetHtmlDocument(address);

            // 根据class获取html元素
            var cells = document.QuerySelectorAll(".panel-body li");

            // We are only interested in the text - select it with LINQ
            List<Belle> list = new List<Belle>();
            foreach (var item in cells)
            {
                var belle = new Belle
                {
                    Title = item.QuerySelector("img").GetAttribute("title"),
                    ImageUrl = item.QuerySelector("img").GetAttribute("src")
                };
                list.Add(belle);
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="savePath">保存路径</param>
        /// <param name="cid">分类ID</param>
        /// <param name="index">页码</param>
        private void Do_Task(string savePath, int cid, int index)
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            // 构造链接
            var url = string.Format(DouBanUrl, cid, index);

            // 获取图片列表
            var imageList = GetListBelle(url);

            // 日志

            string log = $"开始抓取cid ={cid} 第 {index}页";

            XTrace.WriteLine(log);

            // 开始下载
            foreach (var img in imageList)
            {
                var imgUrl = img.ImageUrl.Replace("bmiddle", "large");
                if (!ImageUrlList.Contains(imgUrl))
                {
                    var dirImageCount = Directory.GetDirectories(savePath);

                    // 子文件夹个数为0 创建第一个文件夹
                    if (dirImageCount.Length == 0)
                    {
                        Directory.CreateDirectory(savePath + "\\1");
                        dirImageCount = Directory.GetDirectories(savePath);
                    }

                    // 最后一个文件夹的图片数量
                    var imageCount = Directory.GetFiles(savePath + "\\" + dirImageCount.Length);

                    // 大于等于500时新建文件夹
                    if (imageCount.Length >= 500)
                    {
                        Directory.CreateDirectory(savePath + "\\" + (dirImageCount.Length + 1));
                    }

                    // 重新获取一下子文件夹数量
                    dirImageCount = Directory.GetDirectories(savePath);

                    using (var wc = new WebClient())
                    {
                        try
                        {
                            wc.DownloadFile(imgUrl, Path.Combine(savePath + "\\" + dirImageCount.Length, Path.GetFileName(imgUrl)));

                            log = $"{imgUrl} 下载成功";
                            XTrace.WriteLine(log);
                        }
                        catch (Exception ex)
                        {
                            log = $"{imgUrl} 下载失败 {ex.Message} 路径{ex.Source} 堆栈{ex.StackTrace}";
                            XTrace.WriteLine(log);
                        }
                    }
                    ImageUrlList.Add(img.ImageUrl);
                }
            }

            // 递归调用
            Do_Task(savePath, cid, index + 1);
        }

        /// <summary>
        /// 暴力获取所有图片....
        /// </summary>
        public void DownloadAllImage()
        {
            foreach (var cid in Ids)
            {
                // 还是开一个任务吧
                Task.Factory.StartNew(() =>
                {
                    Do_Task(this.SaveFolder + cid, cid, 1);
                });
            }
        }
    }

}
