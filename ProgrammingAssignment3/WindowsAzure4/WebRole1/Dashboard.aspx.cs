using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Diagnostics;

namespace WebRole1
{

    public partial class Dashboard : System.Web.UI.Page
    {
        string status = "idle";

        protected void Page_Load(object sender, EventArgs e)
        {
            PerformanceCounter cpuCounter = new PerformanceCounter();
            PerformanceCounter ramCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            System.Threading.Thread.Sleep(1000);
            cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            Response.Write("Worker State: " + status + "<br>");
            Response.Write("CPU Utilization: " + cpuCounter.NextValue() + " <br>");
            Response.Write("RAM Utilization: " + ramCounter.NextValue() + " <br>");
            Response.Write("Urls Crawled: " + " <br>");
            Response.Write("Last 10 URLs crawled" + " <br>");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue urlQ = queueClient.GetQueueReference("pagequeue");
            urlQ.CreateIfNotExists();
            int? messageCount = urlQ.ApproximateMessageCount;
            Response.Write(messageCount.ToString());
            Debug.WriteLine("ApproximateMessageCount: " + urlQ.ApproximateMessageCount);
            Response.Write("Size of Queue: " + urlQ.ApproximateMessageCount + " <br>");
            Response.Write("Size of Index: " + " <br>");
            Response.Write("Errors and URLs: " + " <br>");
        }

        public void start(object sender, EventArgs e)
        {
            status = "started";
            Admin.StartCrawling();

        }

        public void stop(object sender, EventArgs e)
        {
            status = "stopped";
            Admin.StopCrawling();
        }
    }
}