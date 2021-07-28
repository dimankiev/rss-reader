using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using RSSReader.core;

namespace RSSReader.cli
{
    public class DisplayManager
    {
        private bool isSingle { get; set; }
        private RSSReadResult currentFeed { get; set; }
        private Dictionary<string, RSSReadResult> allFeeds { get; set; }
        private List<string> allFeedsNames { get; set; }
        public DisplayManager(RSSReadResult feed)
        {
            isSingle = true;
            currentFeed = feed;
        }

        public DisplayManager(Dictionary<string, RSSReadResult> feeds)
        {
            isSingle = false;
            allFeeds = feeds;
            allFeedsNames = new List<string>(feeds.Keys);
        }

        private void SetCurrentFeed()
        {
            if (isSingle) return;
            currentFeed = allFeeds.First().Value;
        }
        
        public void Display()
        {
            if (!isSingle) SetCurrentFeed();
            int articleNo = 0;
            int feedNo = 0;
            List<SyndicationItem> articles = new List<SyndicationItem>(currentFeed.Feed.Items);
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Up/Down Arrow Keys to navigate through feed articles list");
                if (!isSingle) Console.WriteLine("Left/Right Arrow Keys to navigate through feeds list");
                
                Console.WriteLine("Press Q to quit feed viewer...\n\n");
                Console.WriteLine($"Current feed: {currentFeed.Name} ({currentFeed.Feed.Title.Text.Trim()})");
                Console.WriteLine($"Description:\n{currentFeed.Feed.Description.Text.Trim()}\n");
                if (articles.Count > 0)
                {
                    Console.WriteLine($"Article No. {articleNo + 1} of {articles.Count}:");
                    Console.WriteLine($"{articles[articleNo].Title.Text.Trim()}\n");
                    Console.WriteLine($"Date: {articles[articleNo].PublishDate.ToString()}");
                    Console.WriteLine($"\n{articles[articleNo].Summary.Text.Trim()}\n");
                    new List<SyndicationLink>(articles[articleNo].Links).ForEach(link =>
                    {
                        Console.WriteLine($"{link.Uri.ToString().Trim()}");
                    }); 
                }
                else
                {
                    Console.WriteLine("No articles found!");
                }
                
                ConsoleKeyInfo pressed = Console.ReadKey();
                if (pressed.Key == ConsoleKey.UpArrow && articleNo + 1 < articles.Count)
                    articleNo += 1;
                if (pressed.Key == ConsoleKey.DownArrow && articleNo - 1 >= 0)
                    articleNo -= 1;
                if (!isSingle)
                {
                    int previousFeedNo = feedNo;
                    if (pressed.Key == ConsoleKey.LeftArrow && !isSingle && feedNo - 1 >= 0)
                        feedNo -= 1;
                    if (pressed.Key == ConsoleKey.RightArrow && !isSingle && feedNo + 1 < allFeedsNames.Count)
                        feedNo += 1;
                    if (previousFeedNo != feedNo)
                    {
                        currentFeed = allFeeds[allFeedsNames[feedNo]];
                        articles = new List<SyndicationItem>(currentFeed.Feed.Items);
                        articleNo = 0;
                    }
                }
                
                if (pressed.Key == ConsoleKey.Q)
                    break;
            }
            Console.Clear();
        }
    }
}