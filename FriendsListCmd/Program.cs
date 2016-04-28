using FriendsListCmd.api;
using System;
using System.Collections.Generic;
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
        private static bool isValidAcc;
        public static string CharList { get; set; }
        public static XElement FriendsList { get; set; }

        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            Console.WriteLine("Write email");
            var email = Console.ReadLine();
            Console.WriteLine("Write password");
            var password = Console.ReadLine();

            var webClient = new WebClient();
            CharList = webClient.DownloadString($"http://realmofthemadgodhrd.appspot.com/char/list?guid={email}&password={password}");

            var text = new StringReader(CharList);
            var xml = XElement.Load(text);

            isValidAcc = xml.FirstAttribute.Value != "1";

            if (!isValidAcc)
            {
                Console.WriteLine("Not a valid account");
                Console.ReadLine();
                return;
            }

            text.Dispose();

            Console.WriteLine("Valid account");
            Console.WriteLine("Press enter");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("Available commands: \"download\", \"stop\", \"data\"");

            var running = true;
            var hasParsed = false;
            var Accounts = new List<Account>();

            while (running)
            {
                Console.Write("> ");
                switch (Console.ReadLine())
                {
                    case "download":
                        Console.WriteLine("Downloading friends list");
                        FriendsList = XElement.Parse(webClient.DownloadString($"http://realmofthemadgodhrd.appspot.com/friends/getList?guid={email}&password={password}"));
                        Console.WriteLine("Done");
                        Console.WriteLine("Parsing data");
                        Accounts.AddRange(FriendsList.XPathSelectElements("//Account").Select(_ => new Account(_)));
                        Console.WriteLine("Done");
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
                        Console.WriteLine($"Most Total Fame: {Accounts.OrderByDescending(_ => _.Stats.TotalFame).First().Stats.TotalFame} ({Accounts.OrderByDescending(_ => _.Stats.TotalFame).First().Name})");
                        Console.WriteLine($"Least Total Fame: {Accounts.OrderByDescending(_ => _.Stats.TotalFame).Last().Stats.TotalFame} ({Accounts.OrderByDescending(_ => _.Stats.TotalFame).Last().Name})");
                        Console.WriteLine($"Combined Total Fame: {Accounts.Sum(_ => _.Stats.TotalFame)}");
                        Console.WriteLine($"Average Total Fame: {(int)Accounts.Average(_ => _.Stats.TotalFame)}");
                        Console.WriteLine("-- Stars --");
                        Console.WriteLine($"Combined Stars: {Accounts.Sum(_ => _.Stats.Stars)}");
                        Console.WriteLine($"Average Stars: {(int)Accounts.Average(_ => _.Stats.Stars)}");
                        Console.WriteLine("-- Last Seen Characters --");
                        Console.WriteLine($"Most Experience: {Accounts.OrderByDescending(_ => _.Character.Experience).First().Character.Experience} ({Accounts.OrderByDescending(_ => _.Character.Experience).First().Name})");
                        if (
                            Accounts.Count(
                                x =>
                                    x.Character.Experience ==
                                    Accounts.OrderByDescending(y => y.Character.Experience).Last().Character.Experience) >
                            1)
                        {
                            var sb =
                                new StringBuilder(
                                    $"Least Experience: {Accounts.OrderByDescending(_ => _.Character.Experience).Last().Character.Experience} (");
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
                            Console.WriteLine(
                                $"Least Experience: {Accounts.OrderByDescending(_ => _.Character.Experience).Last().Character.Experience} ({Accounts.OrderByDescending(_ => _.Character.Experience).Last().Name})");
                        Console.WriteLine($"Combined Experience: {Accounts.Sum(_ => _.Character.Experience)}");
                        Console.WriteLine($"Average Experience: {(int)Accounts.Average(_ => _.Character.Experience)}");
                        Console.WriteLine($"Most Fame: {Accounts.OrderByDescending(_ => _.Character.CurrentFame).First().Character.CurrentFame} ({Accounts.OrderByDescending(_ => _.Character.CurrentFame).First().Name})");
                        if (
                            Accounts.Count(
                                x =>
                                    x.Character.CurrentFame ==
                                    Accounts.OrderByDescending(y => y.Character.CurrentFame).Last().Character.CurrentFame) >
                            1)
                        {
                            var sb =
                                new StringBuilder(
                                    $"Least Fame: {Accounts.OrderByDescending(_ => _.Character.CurrentFame).Last().Character.CurrentFame} (");
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
                            Console.WriteLine(
                                $"Least Fame: {Accounts.OrderByDescending(_ => _.Character.CurrentFame).Last().Character.CurrentFame} ({Accounts.OrderByDescending(_ => _.Character.CurrentFame).Last().Name})");
                        Console.WriteLine($"Combined Fame: {Accounts.Sum(_ => _.Character.CurrentFame)}");
                        Console.WriteLine($"Average Fame: {(int)Accounts.Average(_ => _.Character.CurrentFame)}");
                        break;

                    default:
                        Console.WriteLine("Available commands: \"download\", \"stop\", \"data\"");
                        break;

                    case "stop":
                        running = false;
                        break;
                }
            }

            Console.ReadLine();
        }
    }
}