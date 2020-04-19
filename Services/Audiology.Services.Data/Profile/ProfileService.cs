namespace Audiology.Services.Data.Profile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class ProfileService : IProfileService
    {
        private readonly IRepository<ApplicationUser> repository;

        public ProfileService(IRepository<ApplicationUser> repository)
        {
            this.repository = repository;
        }

        public async Task<T> GetUserAsync<T>(string userId)
        {
            var user = await this.repository.All().Where(u => u.Id == userId).To<T>().FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentNullException("Requested user wasn't found.");
            }

            return user;
        }

        public async Task<ApplicationUser> GetUserAsync(string userId)
        {
            var user = await this.repository.All().Where(u => u.Id == userId).FirstOrDefaultAsync();

            return user;
        }

        public async Task<string> GetArtistDescription(string artist)
        {
            var intro = string.Empty;

            var wikiClient = new HttpClient();

            var searchTerm = Regex.Replace(artist, "( )+", "%20");

            var baseUrl = $"http://en.wikipedia.org/w/api.php?action=query&format=json&list=search&srsearch={searchTerm}";

            var result = await wikiClient.GetAsync(baseUrl);
            if (!result.IsSuccessStatusCode)
            {
                return intro;
                throw new ArgumentNullException("Error occured while trying to fetch artist info from wiki.");
            }
            else
            {
                var jsonStr = await result.Content.ReadAsStringAsync();

                var json = JsonDocument.Parse(jsonStr);

                var titles = json.RootElement.GetProperty("query").GetProperty("search");

                var title = titles[0].GetProperty("title").ToString();
                var shortDesc = titles[0].GetProperty("snippet").ToString();
                var pageId = titles[0].GetProperty("pageid").ToString();

                if (title.Length > 2)
                {
                    var htmlPageUrl = $"https://en.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro&exhtml&redirects=1&titles={searchTerm}";

                    var html = await wikiClient.GetStringAsync(htmlPageUrl);

                    var pageContent = JsonDocument.Parse(html);

                    intro = pageContent.RootElement.GetProperty("query").GetProperty("pages").GetProperty(pageId).GetProperty("extract").ToString();
                }

                return intro;
            }
        }
    }
}
