namespace Audiology.Web.Controllers
{
    using System.Threading.Tasks;

    using Audiology.Web.ViewModels.Contact;
    using Microsoft.AspNetCore.Mvc;

    public class ContactsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return this.View();
        }

        public async Task<IActionResult> Send(ContactFormViewModel input)
        {


            return this.Redirect("/"); // email received page
        }
    }
}