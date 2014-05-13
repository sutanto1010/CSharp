using System;
using System.Linq;

namespace PluralsightDownloader
{
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Fiddler;

    class PluralSight
    {
        private string url, localPath;
        public PluralSight(string url, string localPath)
        {
            this.url = url;
            this.localPath = localPath;
        }

        public void DownloadFile()
        {
            var webClient = new WebClient();
            webClient.DownloadFileAsync(new Uri(url), localPath);
        }

    }
    public class VideoDownloader : IAutoTamper, IFiddlerExtension
    {


        private string rootPath;


        public VideoDownloader()
        {
            this.rootPath = @"E:\PluralSight";

        }

        public void OnLoad()
        {
            //MessageBox.Show("OnLoad");
        }

        public void OnBeforeUnload()
        {
            //MessageBox.Show("OnBeforeUnload");
        }

        public void AutoTamperRequestBefore(Session oSession)
        {
            if (oSession.uriContains("pluralsight.com"))
            {
                if (oSession.uriContains(".mp4"))
                {
                    var temp = oSession.url.Split('/');
                    var author = rootPath + "\\" + temp[4];
                    //this.createFolder(author);
                    string chapter;
                    var title = rootPath + "\\" + this.GetTitle(temp[5], out chapter);
                    chapter = title + "\\" + chapter;
                    this.createFolder(title);
                    this.createFolder(chapter);
                    var file = chapter + "\\" + temp[6] + ".mp4";
                    if (!File.Exists(file))
                    {
                        var pluralSight = new PluralSight("http://" + oSession.url, file);
                        var thread = new Thread(new ThreadStart(pluralSight.DownloadFile));
                        thread.Start();
                    }
                }
            }
        }



        private void createFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }


        private string GetTitle(string candidatTitle, out string chapter)
        {
            var temp = candidatTitle.Split('-');
            var title = string.Empty;
            bool stop = false;
            int i = 0;
            for (i = 0; i < temp.Length && !stop; i++)
            {
                if (Regex.IsMatch(temp[i], "m[0-9][0-9]*"))
                {
                    stop = true;
                }
            }

            title = string.Join(" ", temp.Take(i - 1).ToArray()).TrimEnd();
            var numberTemp = temp[i - 1].Substring(1);
            int number = -1;
            if (Int32.TryParse(numberTemp, out number))
                chapter = String.Format("{0:00}", number) + " " + string.Join(" ", temp.Skip(i).ToArray()).TrimEnd();
            else
                chapter = string.Join(" ", temp.Skip(i - 1).ToArray()).TrimEnd().Substring(1);

            return title;
        }

        public void AutoTamperRequestAfter(Session oSession)
        {
            //MessageBox.Show("AutoTamperRequestAfter: "+ oSession.url);
        }

        public void AutoTamperResponseBefore(Session oSession)
        {
            //MessageBox.Show("AutoTamperResponseBefore: "+ oSession.url);
        }

        public void AutoTamperResponseAfter(Session oSession)
        {
            //MessageBox.Show("AutoTamperResponseAfter: "+ oSession.url);
        }

        public void OnBeforeReturningError(Session oSession)
        {
            //MessageBox.Show("OnBeforeReturningError: "+ oSession.url);
        }
    }
}
