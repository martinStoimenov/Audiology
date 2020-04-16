namespace Audiology.Web.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class MessageModel
    {
        public string User { get; set; }

        [MaxLength(1000)]
        public string Text { get; set; }
    }
}
