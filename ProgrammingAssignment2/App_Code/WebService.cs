using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;


//Hudson Poissant

/// <summary>
/// This webservice provides words for query suggestion.
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
public class WebService : System.Web.Services.WebService {
    
    public static Trie words;

    /// <summary>
    /// Constructs a new WebService.
    /// </summary>
    public WebService () {
    }

    

    /// <summary>
    /// This WebMethod adds all of the Wikipedia titles from a file to be retrieved at a later time.
    /// </summary>
    [WebMethod]
    public void AddWords()
    {
        words = new Trie();
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference("http://hudpoi459storage.blob.core.windows.net/hudson/");
        CloudBlockBlob blockBlob = container.GetBlockBlobReference("wikipedia.txt");
        try
        {
            using (var stream = blockBlob.OpenRead())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        words.Insert(line);
                    }
                }
            }
        }
        catch (OutOfMemoryException)
        {
            GC.Collect();
        }
    }

    /// <summary>
    /// This WebMethod gets word for the query suggestion feature.
    /// </summary>
    /// <param name="word">Can be an entire word or part of a word. This 
    /// parameter acts as the starting point for the WebService to start 
    /// outputing data.</param>
    /// <returns>10 words based on word parameter</returns>
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetWords(string word) {
        List<string> wordList = new List<string>();
        if (!word.Equals(""))
        {
            wordList = words.WordsThatStartWith(word);
        }
        return new JavaScriptSerializer().Serialize(wordList);
    }
}
