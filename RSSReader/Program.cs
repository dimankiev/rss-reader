﻿using System;
using System.Collections.Generic;
using RSSReader.core;

namespace RSSReader
{
    class Program
    {
        private static List<List<string>> commands = new List<List<string>>()
        {
            // First array is for checking that entered command actually exists
            new List<string>() {"add", "read", "edit", "remove", "help", "exit"},
            // Next arrays are for commands description. Used in "help" command
            new List<string>() {"add [feed name]", "Add new RSS Feed to the app. Name is required"},
            new List<string>() {"read [feed name]", "Read one RSS Feed. Leave name empty to read all feeds."},
            new List<string>() {"edit [feed name]", "Edit a RSS Feed in your config."},
            new List<string>() {"remove [feed name]", "Remove one RSS Feed from the app."},
            new List<string>() {"help", "Display available commands."},
            new List<string>() {"exit", "Save the configuration and exit the program."}
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
            Console.WriteLine("RSS Feed Reader v1.0.0");
            Console.WriteLine("Enter 'help' command to get help\n");
            ConfigManager config = new ConfigManager();
            RssManager manager = new RssManager();
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
                    if (feedUrl == null)
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
                else if (parsedCommand[0] == "exit")
                {
                    Console.WriteLine("Config is saved!\nBye-bye!");
                    return;
                }
            }
        }
    }
}