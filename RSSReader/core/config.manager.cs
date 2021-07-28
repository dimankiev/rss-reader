using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RSSReader.core
{
    public class ConfigManager
    {
        private XDocument _configFile;
        private readonly string _configFilePath;
        public ConfigManager()
        {
            Directory.CreateDirectory("configs");
            _configFilePath = Path.Join("configs", "app.config.xml");
            LoadConfig();
        }

        private void LoadConfig()
        {
            if (!File.Exists(_configFilePath))
            {
                _configFile = new XDocument(
                    new XDeclaration("1.1.0", "utf-8", "yes"),
                    new XElement("RSSFeeds")
                    );
                _configFile.Save(_configFilePath);
            }
            _configFile = XDocument.Load(_configFilePath);
            // Verify integrity
            if (_configFile.Element("RSSFeeds") == null)
            {
                File.Delete(_configFilePath);
                LoadConfig();
            }
        }

        public string AppendFeed(string Name, string Link)
        {
            // Check whether that Name is already used
            bool isAvailable = false;
            try
            {
                List<string[]> feeds = GetAllFeeds();
                string[] feed = feeds.Single(feedParameters => feedParameters[0] == Name);
            }
            catch (Exception err)
            {
                if (err is InvalidOperationException) isAvailable = true;
                if (err is ArgumentNullException) return "ConfigMalformed_RSSFeedsNotFound";
            }

            if (!isAvailable) return "That feed name is already occupied";
            XElement feedsContainer = _configFile.Element("RSSFeeds");
            if (feedsContainer == null) return "ConfigMalformed_RSSFeedsNotFound";
            feedsContainer.Add(new XElement(
                    "Feed", 
                    new XAttribute("Name", Name),
                    new XAttribute("Link", Link)
                    )
                );
            _configFile.Save(_configFilePath);
            return "Success";
        }

        public string GetFeedLink(string FeedName)
        {
            XElement feed;
            try
            {
                feed = _configFile.Elements("RSSFeeds")
                    .Single(element => element.Attribute("Name").Value == FeedName);
            }
            catch
            {
                return "FeedNotFound";
            }
            XAttribute feedLink = feed.Attribute("Link");
            if (feedLink == null) return "FeedLinkNotFound";
            return feedLink.Value;
        }

        public List<string[]> GetAllFeeds()
        {
            IEnumerable<XElement> feedElements = _configFile.Descendants("Feed");
            List<string[]> feeds = new List<string[]>();
            foreach (XElement feedElement in feedElements)
            {
                try
                {
                    feeds.Add(new string[2]
                    {
                        feedElement.Attribute("Name").Value,
                        feedElement.Attribute("Link").Value
                    });
                }
                catch
                {
                    throw new Exception("Some of feeds attributes are inaccessible");
                }
            }

            return feeds;
        }

        public string EditFeed(string FeedName, string AttrName, string AttrValue)
        {
            XElement feed;
            try
            {
                feed = _configFile.Elements("RSSFeeds")
                    .Single(element => element.Attribute("Name").Value == FeedName);
            }
            catch
            {
                return "FeedNotFound";
            }
            XAttribute feedAttribute = feed.Attribute(AttrName);
            if (feedAttribute == null) return "AttrNotFound";
            feedAttribute.Value = AttrValue;
            _configFile.Save(_configFilePath);
            return "Success";
        }

        public string RemoveFeed(string FeedName)
        {
            XElement feed;
            try
            {
                feed = _configFile.Element("RSSFeeds")
                    .Elements("Feed")
                    .Single(
                        element => 
                            element.Attribute("Name").Value == FeedName
                        );
            }
            catch
            {
                return "FeedNotFound";
            }
            feed.Remove();
            _configFile.Save(_configFilePath);
            return "Success";
        }
    }
}