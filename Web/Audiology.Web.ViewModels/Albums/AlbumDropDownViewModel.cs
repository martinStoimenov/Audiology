namespace Audiology.Web.ViewModels.Albums
{
    using Audiology.Data.Models;
    using Audiology.Services.Mapping;

    public class AlbumDropDownViewModel : IMapFrom<Album>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
