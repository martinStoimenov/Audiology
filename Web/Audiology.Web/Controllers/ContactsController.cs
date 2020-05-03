namespace Audiology.Web.Controllers
{
    using System.Threading.Tasks;
    using Audiology.Services.Messaging;
    using Audiology.Web.ViewModels.Contact;
    using Microsoft.AspNetCore.Mvc;

    public class ContactsController : Controller
    {
        private readonly IEmailSender emailSender;

        public ContactsController(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            return this.View();
        }

        public async Task<IActionResult> Send(ContactFormViewModel input)
        {
            var combinedNames = input.FirstName + " " + input.LastName;
            input.Content = "Sender:" + input.Email + "\n\r" + input.Content;
            await this.emailSender.SendEmailAsync("audiology@abv.bg", combinedNames, "audiology@abv.bg", input.Subject, input.Content);

            return this.View("Success");
        }
    }
}