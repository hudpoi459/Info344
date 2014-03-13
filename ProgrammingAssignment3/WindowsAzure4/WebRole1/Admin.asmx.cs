using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Diagnostics;
using System.Web.Services;
using Microsoft.WindowsAzure.Storage.Table;

namespace WebRole1
{
    /// <summary>
    /// Summary description for Admin
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Admin : System.Web.Services.WebService
    {
        public static string StartCrawling()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable pageTable = tableClient.GetTableReference("indexedpages");
            while (pageTable.Exists())
            {
                System.Threading.Thread.Sleep(1000);
            }
            pageTable.CreateIfNotExists();
            Debug.WriteLine("Now in StartCrawling");
            CloudQueue cmdQ = queueClient.GetQueueReference("cmdq");
            cmdQ.CreateIfNotExists();
            CloudQueue urlQ = queueClient.GetQueueReference("pagequeue");
            urlQ.CreateIfNotExists();
            urlQ.Clear();
            urlQ.AddMessage(new CloudQueueMessage("http://www.cnn.com/robots.txt"));
            Debug.WriteLine("THIS IS THE MESSAGE ADDED: " + urlQ.PeekMessage().AsString);
            cmdQ.Clear();
            string message = "start";
            CloudQueueMessage cmd = new CloudQueueMessage(message);
            cmdQ.AddMessage(cmd);
            return message;
        }

        public static void StopCrawling()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue cmdQ = queueClient.GetQueueReference("cmdq");
            CloudQueue urlQ = queueClient.GetQueueReference("pagequeue");
            cmdQ.CreateIfNotExists();
            urlQ.CreateIfNotExists();
            cmdQ.Clear();
            cmdQ.AddMessage(new CloudQueueMessage("stop"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable pageTable = tableClient.GetTableReference("indexedpages");
            pageTable.DeleteIfExists();
            while (pageTable.Exists())
            {
                System.Threading.Thread.Sleep(1000);
            }
            urlQ.Clear();
        }
    }
}
