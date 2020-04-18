namespace Audiology.Web.ViewModels.Administration.Dashboard
{
    using System.Collections.Generic;

    using Audiology.Data.Models;

    public class IndexViewModel
    {
        public IEnumerable<ApplicationUser> Artists { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }

        public int? SongId { get; set; }

        public IEnumerable<SongDropDownViewModel> Songs { get; set; }
    }
}
