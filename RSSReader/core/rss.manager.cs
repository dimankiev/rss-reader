using System;
using System.Data;
using System.ServiceModel.Syndication;
using System.Xml;

namespace RSSReader.core
{
    public class RSSReadResult
    {
        public bool Success { get; set; }
        public SyndicationFeed Feed { get; set; }
        public Exception Message { get; set; }
        public void StatusSet(bool isSuccess, Exception message)
        {
            Success = isSuccess;
            Message = message;
        }
    }
    
    public class RssManager
    {
        public RssManager() {}

        public RSSReadResult Fetch(string url)
        {
            RSSReadResult result = new RSSReadResult();
            try
            {
                using (var reader = XmlReader.Create(url)) result.Feed = SyndicationFeed.Load(reader);
                result.StatusSet(true, null);
            }
            catch (Exception err)
            {
                result.StatusSet(false, err);
            }

            return result;
        }
    }
}