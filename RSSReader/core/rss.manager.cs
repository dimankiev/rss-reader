using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace RSSReader.core
{
    public class RSSReadResult
    {
        public RSSReadResult(string name, string url)
        { Name = name; Url = url; }
        public bool Success { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public SyndicationFeed Feed { get; set; }
        public Exception Message { get; set; }
        public void StatusSet(bool isSuccess, Exception message)
        { Success = isSuccess; Message = message; }
    }
    
    public class RSSManager
    {
        public RSSManager() {}

        public RSSReadResult Fetch(string feedName, string feedUrl)
        {
            RSSReadResult result = new RSSReadResult(feedName, feedUrl);
            try
            {
                using (var reader = XmlReader.Create(feedUrl)) result.Feed = SyndicationFeed.Load(reader);
                result.StatusSet(true, null);
            }
            catch (Exception err)
            {
                result.StatusSet(false, err);
            }

            return result;
        }

        public Dictionary<string, RSSReadResult> FetchMany(List<string[]> feedParams)
        {
            Dictionary<string, RSSReadResult> fetched = new Dictionary<string, RSSReadResult>();
            Parallel.ForEach(feedParams, currentFeed =>
            {
                RSSReadResult feed = Fetch(currentFeed[0], currentFeed[1]);
                if (!feed.Success) 
                { throw new Exception($"{feed.Name} ({feed.Url}) {feed.Message}"); }
                fetched.Add(currentFeed[0], feed);
            });
            return fetched;
        }
    }
}