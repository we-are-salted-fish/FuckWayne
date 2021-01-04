using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using DouBanSpider;
using FuckWayne.Models;
using NewLife.Log;

namespace FuckWayne.DouBan
{
    /// <summary>
    /// douban imgs handler url:https://www.dbmeinv.com/index.htm
    /// </summary>
    public class DouBan
    {
        public DouBan(string saveFolder, List<string> Categorys)
        {
            this._saveFolder = saveFolder;
            this._categorys = Categorys;
            if (!saveFolder.EndsWith("\\"))
            {
                this._saveFolder += "\\";
            }
        }

        private string _saveFolder = "";

        /// <summary>
        /// Category 2-胸 3-腿 4-脸 5-杂 6-臀 7-袜子
        /// </summary>
        private List<string> _categorys;

        /// <summary>
        /// DouBan Url
        /// </summary>
        private string _douBanUrl = "https://www.buxiuse.com/?cid={0}&page={1}";

        /// <summary>
        /// downloaded url list
        /// </summary>
        private List<string> _imageUrlList = new List<string>();

        /// <summary>
        /// Get Page Source
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string GetUrlString(string url, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.GetEncoding("UTF-8");
            }
            var str = string.Empty;
            try
            {
                str = HttpHelper.GetStringAsync(url, encoding).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"request error{url},tips：{e.Message}");
            }

            return str;
        }

        /// <summary>
        /// PageSource Parse IHtmlDocument
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public IHtmlDocument GetHtmlDocument(string url, Encoding encoding = null)
        {
            var resultStr = GetUrlString(url, encoding);
            return new HtmlParser().ParseDocument(resultStr);
        }

        /// <summary>
        /// 获取指定链接下的图片(默认随机获取某个分类下的第一页图片)
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public List<Belle> GetListBelle(string url)
        {
            // send request
            var document = GetHtmlDocument(url);

            // get document by class
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

        private void Do_Task(string savePath, string cid, int index)
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            var url = string.Format(_douBanUrl, cid, index);

            var imageList = GetListBelle(url);

            var log = $"start download cid ={cid}  NO.{index}";
            XTrace.WriteLine(log);

            // start download
            foreach (var img in imageList)
            {
                var imgUrl = img.ImageUrl.Replace("bmiddle", "large");
                if (!_imageUrlList.Contains(imgUrl))
                {
                    var dirImageCount = Directory.GetDirectories(savePath);

                    // child folder counts= then create first folder
                    if (dirImageCount.Length == 0)
                    {
                        Directory.CreateDirectory(savePath + "\\1");
                        dirImageCount = Directory.GetDirectories(savePath);
                    }

                    // last child folder imgs count
                    var imageCount = Directory.GetFiles(savePath + "\\" + dirImageCount.Length);

                    // imgs count>=500 then create new folder
                    if (imageCount.Length >= 500)
                    {
                        Directory.CreateDirectory(savePath + "\\" + (dirImageCount.Length + 1));
                    }

                    // reload child folder counts
                    dirImageCount = Directory.GetDirectories(savePath);

                    try
                    {
                        var imgStream = HttpHelper.GetStreamAsync(imgUrl).Result;
                        using (var fileStream = File.Create(Path.Combine(savePath + "\\" + dirImageCount.Length, Path.GetFileName(imgUrl))))
                        {
                            fileStream.Write(imgStream);
                        }

                        log = $"{imgUrl} downloaded success";
                        XTrace.WriteLine(log);

                        _imageUrlList.Add(img.ImageUrl);
                    }
                    catch (Exception ex)
                    {
                        log = $"{imgUrl} downloaded error {ex.Message} Source:{ex.Source} StackTrace:{ex.StackTrace}";
                        XTrace.WriteLine(log);
                    }
                }
            }

            // recursion
            Do_Task(savePath, cid, index + 1);
        }

        /// <summary>
        /// get all img....
        /// </summary>
        public void DownloadAllImage()
        {
            foreach (var cid in _categorys)
            {
                Task.Factory.StartNew(() =>
                {
                    Do_Task(this._saveFolder + cid, cid, 1);
                });
            }
        }
    }
}
