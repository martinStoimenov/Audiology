namespace Audiology.Web.ViewModels.Administration.Dashboard
{
    using Audiology.Data.Models;
    using Audiology.Services.Mapping;

    public class SongDropDownViewModel : IMapFrom<Song>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
