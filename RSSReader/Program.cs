using System;
using System.Collections.Generic;
using RSSReader.core;
using RSSReader.cli;

namespace RSSReader
{
    class Program
    {
        private static List<List<string>> commands = new List<List<string>>()
        {
            // First array is for checking that entered command actually exists
            new List<string>() {"add", "read", "edit", "remove", "help", "exit"},
            // Next arrays are for commands description. Used in "help" command
            new List<string>() {"add [feed name]\t\t", "\tAdd new RSS Feed to the app. Name is required"},
            new List<string>() {"read [feed name]\t", "\tRead one RSS Feed. Leave name empty to read all feeds."},
            new List<string>() {"edit [feed name]\t", "\tEdit a RSS Feed in your config."},
            new List<string>() {"remove [feed name]\t", "\tRemove one RSS Feed from the app."},
            new List<string>() {"help\t\t\t", "\tDisplay available commands."},
            new List<string>() {"exit\t\t\t", "\tSave the configuration and exit the program."}
        };

        private static void GetHelp(string command)
        {
            int descriptionIndex = commands[0].FindIndex(cmd => cmd == command);
            Console.WriteLine($"{commands[descriptionIndex + 1][0]}\t-\t{commands[descriptionIndex + 1][1]}");
        }

        private static void GetHelp()
        {
            for (int index = 1; index < commands.Count; index++)
                Console.WriteLine($"{commands[index][0]}\t-\t{commands[index][1]}");
        }
        static void Main(string[] args)
        {
            Console.WriteLine("RSS Feed Reader v1.2.3 by dimankiev");
            Console.WriteLine("Enter 'help' command to get help\n");
            ConfigManager config = new ConfigManager();
            RSSManager manager = new RSSManager();
            while (true)
            {
                Console.Write("RSSReader >> ");
                string command = Console.ReadLine();
                // Incorrect command checks and parsing
                if (command is null) 
                { Console.WriteLine("Command wasn't entered"); continue; }
                string[] parsedCommand = command.Trim().Split(' ');
                if (!commands[0].Contains(parsedCommand[0])) 
                { Console.WriteLine("Command isn't recognized"); continue; }
                parsedCommand[0] = parsedCommand[0].ToLower();
                // Actual RSS Reader manipulation
                if (parsedCommand[0] == "add")
                {
                    if (parsedCommand.Length < 2)
                    { GetHelp(parsedCommand[0]); continue; }

                    Console.Write($"Feed({parsedCommand[1]}) URL: ");
                    string feedUrl = Console.ReadLine();
                    if (feedUrl == null || feedUrl.Trim().Length < 11) // 11 symbols minimum: http://o.io
                    { Console.WriteLine("Bad url is entered!"); continue; }

                    feedUrl = feedUrl.Trim().ToLower();
                    string result = config.AppendFeed(parsedCommand[1], feedUrl);
                    Console.WriteLine($"Feed '{parsedCommand[1]}' add result: {result}");
                }
                else if (parsedCommand[0] == "remove")
                {
                    if (parsedCommand.Length < 2)
                    { GetHelp(parsedCommand[0]); continue; }

                    string result = config.RemoveFeed(parsedCommand[1]);
                    Console.WriteLine($"Feed '{parsedCommand[1]}' remove result: {result}");
                }
                else if (parsedCommand[0] == "edit")
                {
                    if (parsedCommand.Length < 2)
                    { GetHelp(parsedCommand[0]); continue; }

                    bool exists = config.FeedExists(parsedCommand[1]);
                    if (!exists)
                    { Console.WriteLine("Feed does not exist"); continue; }

                    Console.WriteLine("Leave a field empty to save parameters previous value");
                    
                    Console.Write("RSSFeed Editor [Name]: ");
                    string newNameSetResult = "", newName = Console.ReadLine();
                    if (newName is not null && newName.Trim().Length >= 1)
                        newNameSetResult = config.EditFeed(parsedCommand[1], "Name", newName);
                    else newName = parsedCommand[1];
                    
                    Console.Write("RSSFeed Editor [Feed URL]: ");
                    string newUrlSetResult = "", newUrl = Console.ReadLine();
                    if (newUrl is not null && newUrl.Trim().Length > 11)
                        newUrlSetResult = config.EditFeed(newName, "Link", newUrl);

                    if (newNameSetResult == "Success" && newUrlSetResult == "Success")
                    { Console.WriteLine($"Feed '{newName}' was edited successfully!"); }
                    else
                    {
                        Console.WriteLine("Following messages were returned while editing the feed:");
                        Console.WriteLine($"Editing name: {newNameSetResult}\nEditing URL: {newUrlSetResult}");
                    }
                }
                else if (parsedCommand[0] == "read")
                {
                    try
                    {
                        DisplayManager dm;
                        if (parsedCommand.Length < 2)
                        {
                            var allFeeds = config.GetAllFeeds();
                            
                            if (allFeeds.Count < 1)
                            { Console.WriteLine("There is no feeds added!"); continue; }
                            
                            var fetchedFeeds = manager.FetchMany(allFeeds);
                            dm = new DisplayManager(fetchedFeeds);
                            dm.Display();
                        }
                        else
                        {
                            if (!config.FeedExists(parsedCommand[1]))
                            {
                                Console.WriteLine("Feed does not exist!");
                                continue;
                            }

                            var feedUrl = config.GetFeedLink(parsedCommand[1]);
                            var fetchedFeed = manager.Fetch(parsedCommand[1], feedUrl);
                            dm = new DisplayManager(fetchedFeed);
                            dm.Display();
                        }
                    }
                    catch (Exception error)
                    {
                        Console.Clear();
                        Console.WriteLine($"Following error has been occured while reading feed:\n{error}");
                    }
                }
                else if (parsedCommand[0] == "help")
                {
                    Console.WriteLine("Help for RSSReader");
                    for (int i = 1; i < commands.Count; i++)
                        Console.WriteLine($"{commands[i][0]}-{commands[i][1]}");
                }
                else if (parsedCommand[0] == "exit")
                {
                    Console.WriteLine("Config is saved!\nBye-bye!");
                    return;
                }
            }
        }
    }
}