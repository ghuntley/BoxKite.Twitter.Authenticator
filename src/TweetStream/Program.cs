using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter;
using BoxKite.Twitter.Models;
using Newtonsoft.Json;

namespace TweetStream
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task t = MainAsync(args);
            t.Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            const string filename = "credentials.json";

            var json = File.ReadAllText(filename);
            var credentials = JsonConvert.DeserializeObject<TwitterCredentials>(json);

            var session = new UserSession(credentials, new DesktopPlatformAdaptor());
            var user = await session.GetVerifyCredentials();

            if (user.OK)
            {
                Console.WriteLine(credentials.ScreenName + " is authorised to use BoxKite.Twitter.");

                var stream = session.StartSearchStream(track: "#yolo");
                stream.FoundTweets.Subscribe(tweet =>
                {
                    Console.WriteLine(String.Format("ScreenName: {0}, Tweet: {1}", tweet.User.ScreenName, tweet.Text));
                });
                stream.Start();
            }
            else
            {
                Console.WriteLine("Credentials found in {0} were invalid, please run the Authenticator and copy over the resulting output.", filename);
            }

            Thread.Sleep(Timeout.Infinite);
        }
    }
}