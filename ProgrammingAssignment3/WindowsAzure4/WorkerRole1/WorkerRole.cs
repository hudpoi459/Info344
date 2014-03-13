using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;


namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private string started;
        private static CloudTable pageTable;
        private static HashSet<string> alreadyAdded;
        private static HashSet<string> disallow;

        public override void Run()
        {
            Thread.Sleep(1000);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue cmdQ = queueClient.GetQueueReference("cmdq");
            cmdQ.CreateIfNotExists();
            CloudQueueMessage cmd = cmdQ.GetMessage();
            cmdQ.CreateIfNotExists();
            cmd = cmdQ.GetMessage();
            while (cmd == null)
            {
                cmd = cmdQ.PeekMessage();
                if (cmd != null)
                {
                }
                Thread.Sleep(1000);
            }
            started = cmd.AsString;
            while (true)
            {
                if (started.Equals("start"))                                
                {
                    CloudQueue urlQ = queueClient.GetQueueReference("pagequeue");
                    if (urlQ.PeekMessage() != null)                         
                    {                                                       
                        CloudQueueMessage queued = urlQ.GetMessage();      
                        string currentUrl = queued.AsString;
                        alreadyAdded.Add(currentUrl);                       
                        urlQ.DeleteMessage(queued);                        
                        WebClient getContent = new WebClient();
                        try
                        {
                            string pageContent = getContent.DownloadString(currentUrl);
                            if (currentUrl.Contains("robots.txt"))
                            {
                                foreach (Match disallowedPath in Regex.Matches(pageContent, "(?<=disallow: ).*"))
                                {
                                    disallow.Add(disallowedPath.ToString());
                                }
                            }
                            if (currentUrl.Contains(".htm"))
                            { 
                                string title = Regex.Match(pageContent, @"<title[^>]*>(.*?)</title>").Groups[1].Value;
                                title = Regex.Replace(title, @"\s*<.*?>\s*", "", RegexOptions.Singleline);
                                TableOperation insert = TableOperation.Insert(new Link("cnn", HttpUtility.UrlEncode(currentUrl), title));
                                pageTable.Execute(insert);
                            }
                            MatchCollection matches;
                            if (currentUrl.Contains(".htm"))                                                                
                            {
                                matches = Regex.Matches(pageContent, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);
                            }
                            else
                            {
                                matches = Regex.Matches(pageContent, @"(http(.*)(cnn.).*(xml|html|htm))");
                            }
                            foreach (Match urlFind in matches)
                            {
                                if (started.Equals("start"))
                                {
                                    string processingUrl = urlFind.ToString();
                                    if (!inDisallow(processingUrl))
                                    {
                                        if (!processingUrl.StartsWith("#"))
                                        {
                                            if (Regex.IsMatch(processingUrl, "(<a.*?>.*?</a>)"))
                                            {
                                                string value = urlFind.Groups[1].Value;
                                                Match m2 = Regex.Match(value, @"href=\""(.*?)\""", RegexOptions.Singleline);
                                                processingUrl = m2.Groups[1].Value;
                                            }
                                            if (processingUrl.StartsWith("/"))
                                            {
                                                processingUrl = "http://www.cnn.com" + processingUrl;
                                            }
                                        }
                                        if (Regex.IsMatch(processingUrl, @"^[\S]*$"))
                                        {
                                            if (!alreadyAdded.Contains(processingUrl))
                                            {
                                                processingUrl = Regex.Match(processingUrl, @"^http.*[.]cnn[.][\S]*$").ToString();
                                                if (!string.IsNullOrWhiteSpace(processingUrl))
                                                {
                                                    string robots;
                                                    if (processingUrl.EndsWith(".com"))
                                                    {
                                                        robots = processingUrl + "/robots.txt";
                                                        alreadyAdded.Add(robots);
                                                        urlQ.AddMessage(new CloudQueueMessage(robots));
                                                    }
                                                    if (processingUrl.EndsWith(".com/"))
                                                    {
                                                        robots = processingUrl + "robots.txt";
                                                        alreadyAdded.Add(robots);
                                                        urlQ.AddMessage(new CloudQueueMessage(robots));
                                                    }
                                                    alreadyAdded.Add(processingUrl);
                                                    urlQ.AddMessage(new CloudQueueMessage(processingUrl));
                                                }
                                            }
                                        }
                                    }
                                    cmd = cmdQ.GetMessage();
                                    if (cmd != null)                                            // update state
                                    {
                                        started = cmd.AsString;
                                        cmdQ.DeleteMessage(cmd);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                else                                                        
                {
                    cmd = cmdQ.GetMessage();
                    if (cmd != null)
                    {
                        started = cmd.AsString;
                        cmdQ.DeleteMessage(cmd);
                    }
                }
            }
        }

        private bool inDisallow(string url)
        {
            foreach(string disallowed in disallow) {
                if (url.IndexOf(disallowed) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool OnStart()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            pageTable = tableClient.GetTableReference("indexedpages");
            ServicePointManager.DefaultConnectionLimit = 12;
            disallow = new HashSet<string>();
            alreadyAdded = new HashSet<string>();
            return base.OnStart();
        }
    }
}
