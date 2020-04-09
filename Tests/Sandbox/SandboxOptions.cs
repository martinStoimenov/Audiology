namespace Sandbox
{
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using CommandLine;

    [Verb("sandbox", HelpText = "Run sandbox code.")]
    public static class SandboxOptions
    {
        public static async Task Main()
        {
            var client = new HttpClient();

            var artist = "2pac";
            var song = "Better Dayz";

            var response = await client.GetAsync("https://api.lyrics.ovh/v1/" + artist + "/" + song);

            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException("Song lyrics can't be found right now");
            }
            else
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                var json = JsonDocument.Parse(jsonString);

                var lyrics = json.RootElement.GetProperty("lyrics").ToString();

                string modifiedString = Regex.Replace(lyrics, @"(\r\n)|\n|\r", "<br/>");

                Console.WriteLine(modifiedString);
            }

            //------------------------------------------------------------------------------------------------------------------------------------

            /*var kanye = await client.GetStringAsync("https://api.kanye.rest/?format=text");

            Console.WriteLine(kanye);
            Console.WriteLine("----------------------------------------------------------------------------------");

            var apiSeedsClient = new HttpClient();

            string baseUrl = "https://orion.apiseeds.com/api/music/lyric/";
            string artist = "Dababy";
            string songName = "/Suge";
            string apiKey = "?apikey=qnvwwzXwsPeyGI7KUILgQSTjlzoBywKYIp1l7KPe0al9jiwYT4qms0UzDJozxM2i";
            string search = baseUrl + artist + songName + apiKey;

            var jsonResultString = await apiSeedsClient.GetAsync(search);

            if (!jsonResultString.IsSuccessStatusCode)
            {
                throw new ArgumentException("Song lyrics can't be found right now");
            }
            else
            {
                var jsonStr = await jsonResultString.Content.ReadAsStringAsync();

                var json = JsonDocument.Parse(jsonStr);

                var lyrics = json.RootElement.GetProperty("result").GetProperty("track").GetProperty("text").ToString();

                Console.WriteLine(lyrics);
            }*/
        }
    }
}
