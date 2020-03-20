namespace Audiology.Web.ViewModels.Albums
{
    using System.Net;
    using System.Text.RegularExpressions;

    using Audiology.Data.Models;
    using Audiology.Services.Mapping;

    public class AlbumsListViewModel : IMapFrom<Album>
    {
        public string Name { get; set; }

        public string Producer { get; set; }

        public string CoverUrl { get; set; }

        public string Description { get; set; }

        public string ShortDescription
        {
            get
            {
                var content = WebUtility.HtmlDecode(Regex.Replace(this.Description, @"<[^>]+>", string.Empty));
                return content.Length > 57
                        ? content.Substring(0, 57) + "..."
                        : content;
            }
        }
    }
}
