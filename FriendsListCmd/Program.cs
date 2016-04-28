using FriendsListCmd.api;
using FriendsListCmd.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace FriendsListCmd
{
    internal class Program
    {
        public static XElement AccountVerify { get; set; }
        public static XElement FriendsList { get; set; }

        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            Console.WriteLine("Write email");
            Console.Write("> ");
            var email = Console.ReadLine();
            Console.WriteLine("Write password");
            Console.Write("> ");
            var password = Console.ReadLine();

            var webClient = new WebClient();
            AccountVerify = XElement.Parse(webClient.DownloadString($"http://realmofthemadgodhrd.appspot.com/account/verify?guid={email}&password={password}"));
            
            if (AccountVerify.Elements("Error").Any())
            {
                Console.WriteLine("Not a valid account");
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Valid account");
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("Available commands: \"download\", \"data\", \"exit\"");

            var running = true;
            var hasParsed = false;
            var Accounts = new List<Account>();

            while (running)
            {
                Console.Write("> ");
                switch (Console.ReadLine())
                {
                    case "download":
                        var sw = new Stopwatch();
                        sw.Start();
                        Console.Write("Downloading friends list...");
                        FriendsList =
                            XElement.Parse(
                                webClient.DownloadString(
                                    $"http://realmofthemadgodhrd.appspot.com/friends/getList?guid={email}&password={password}"));
                        Console.WriteLine($" completed in {sw.ElapsedMilliseconds} ms");
                        sw.Reset();
                        sw.Start();
                        Console.Write("Parsing data...");
                        Accounts.AddRange(FriendsList.XPathSelectElements("//Account").Select(_ => new Account(_)));
                        Console.WriteLine($" completed in {sw.ElapsedMilliseconds} ms");
                        sw.Stop();
                        hasParsed = true;
                        break;

                    case "data":
                        if (!hasParsed)
                        {
                            Console.WriteLine("Download first!");
                            break;
                        }
                        Console.WriteLine("-- General --");
                        Console.WriteLine($"Friends: {Accounts.Count}");
                        Console.WriteLine("-- Total Fame --");
                        Console.WriteLine($"Most Total Fame: {StringUtil.FormatNumber(Accounts.OrderByDescending(_ => _.Stats.TotalFame).First().Stats.TotalFame)} ({Accounts.OrderByDescending(_ => _.Stats.TotalFame).First().Name})");
                        Console.WriteLine($"Least Total Fame: {StringUtil.FormatNumber(Accounts.OrderByDescending(_ => _.Stats.TotalFame).Last().Stats.TotalFame)} ({Accounts.OrderByDescending(_ => _.Stats.TotalFame).Last().Name})");
                        Console.WriteLine($"Combined Total Fame: {StringUtil.FormatNumber(Accounts.Sum(_ => _.Stats.TotalFame))}");
                        Console.WriteLine($"Average Total Fame: {StringUtil.FormatNumber((int)Accounts.Average(_ => _.Stats.TotalFame))}");
                        Console.WriteLine("-- Stars --");
                        Console.WriteLine($"Combined Stars: {StringUtil.FormatNumber(Accounts.Sum(_ => _.Stats.Stars))}");
                        Console.WriteLine($"Average Stars: {StringUtil.FormatNumber((int)Accounts.Average(_ => _.Stats.Stars))}");
                        Console.WriteLine("-- Last Seen Characters --");
                        Console.WriteLine($"Most Experience: {StringUtil.FormatNumber(Accounts.OrderByDescending(_ => _.Character.Experience).First().Character.Experience)} ({Accounts.OrderByDescending(_ => _.Character.Experience).First().Name})");
                        if (
                            Accounts.Count(
                                x =>
                                    x.Character.Experience ==
                                    Accounts.OrderByDescending(y => y.Character.Experience).Last().Character.Experience) >
                            1)
                        {
                            var sb =
                                new StringBuilder(
                                    $"Least Experience: {StringUtil.FormatNumber(Accounts.OrderByDescending(_ => _.Character.Experience).Last().Character.Experience)} (");
                            var iterations = 0;
                            foreach (var acc in Accounts.Where(
                                     x =>
                                         x.Character.Experience ==
                                         Accounts.OrderByDescending(y => y.Character.Experience)
                                                 .Last()
                                                 .Character.Experience))
                            {
                                if (iterations != 0)
                                    sb.Append(", ");
                                sb.Append($"{acc.Name}");
                                iterations++;
                            }
                            sb.Append(")");
                            Console.WriteLine(sb.ToString());
                        }
                        else
                            Console.WriteLine($"Least Experience: {StringUtil.FormatNumber(Accounts.OrderByDescending(_ => _.Character.Experience).Last().Character.Experience)} ({Accounts.OrderByDescending(_ => _.Character.Experience).Last().Name})");
                        Console.WriteLine($"Combined Experience: {StringUtil.FormatNumber(Accounts.Sum(_ => _.Character.Experience))}");
                        Console.WriteLine($"Average Experience: {StringUtil.FormatNumber((int)Accounts.Average(_ => _.Character.Experience))}");
                        Console.WriteLine($"Most Fame: {StringUtil.FormatNumber(Accounts.OrderByDescending(_ => _.Character.CurrentFame).First().Character.CurrentFame)} ({Accounts.OrderByDescending(_ => _.Character.CurrentFame).First().Name})");
                        if (
                            Accounts.Count(
                                x =>
                                    x.Character.CurrentFame ==
                                    Accounts.OrderByDescending(y => y.Character.CurrentFame).Last().Character.CurrentFame) >
                            1)
                        {
                            var sb =
                                new StringBuilder(
                                    $"Least Fame: {StringUtil.FormatNumber(Accounts.OrderByDescending(_ => _.Character.CurrentFame).Last().Character.CurrentFame)} (");
                            var iterations = 0;
                            foreach (var acc in Accounts.Where(
                                     x =>
                                         x.Character.CurrentFame ==
                                         Accounts.OrderByDescending(y => y.Character.CurrentFame)
                                                 .Last()
                                                 .Character.CurrentFame))
                            {
                                if (iterations != 0)
                                    sb.Append(", ");
                                sb.Append($"{acc.Name}");
                                iterations++;
                            }
                            sb.Append(")");
                            Console.WriteLine(sb.ToString());
                        }
                        else
                            Console.WriteLine($"Least Fame: {StringUtil.FormatNumber(Accounts.OrderByDescending(_ => _.Character.CurrentFame).Last().Character.CurrentFame)} ({Accounts.OrderByDescending(_ => _.Character.CurrentFame).Last().Name})");
                        Console.WriteLine($"Combined Fame: {StringUtil.FormatNumber(Accounts.Sum(_ => _.Character.CurrentFame))}");
                        Console.WriteLine($"Average Fame: {StringUtil.FormatNumber((int)Accounts.Average(_ => _.Character.CurrentFame))}");
                        break;

                    default:
                        Console.WriteLine("Available commands: \"download\", \"stop\", \"data\"");
                        break;

                    case "exit":
                        running = false;
                        break;
                }
            }

            Console.ReadLine();
        }
    }
}