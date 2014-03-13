using Microsoft.WindowsAzure.Storage.Table;

namespace WorkerRole1
{
    public class Link : TableEntity
    {
        public Link(string domain, string url, string title)
        {
            this.PartitionKey = domain;
            this.RowKey = url;
            this.Title = title;
        }

        public Link() { }

        public string Title { get; set; }
    }
}
