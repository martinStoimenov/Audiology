﻿namespace Audiology.Web.Areas.Identity.Pages.Account
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Audiology.Data.Models;
    using Audiology.Data.Models.Enumerations;
    using Audiology.Services.Mapping;
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Logging;

    [AllowAnonymous]
    public class RegisterModel : PageModel, IMapFrom<ApplicationUser>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly Cloudinary cloudinary;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            Cloudinary cloudinary)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
            this._emailSender = emailSender;
            this.cloudinary = cloudinary;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [MaxLength(256)]
            public string Username { get; set; }

            [Required]
            [MaxLength(50)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [MaxLength(50)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            public IFormFile ProfilePicUrl { get; set; }

            [MaxLength(600)]
            [Display(Name = "Instagram profile url")]
            public string InstagramUrl { get; set; }

            [MaxLength(600)]
            [Display(Name = "Facebook profile url")]
            public string FacebookUrl { get; set; }

            [MaxLength(600)]
            [Display(Name = "Twitter profile url")]
            public string TwitterUrl { get; set; }

            [MaxLength(600)]
            [Display(Name = "You Tube profile url")]
            public string YouTubeUrl { get; set; }

            [MaxLength(600)]
            [Display(Name = "Soundcloud profile url")]
            public string SondcloudUrl { get; set; }

            [DataType(DataType.Date)]
            public DateTime? Birthday { get; set; }

            [Required]
            public Gender Gender { get; set; }

            [Required]
            [Display(Name = "Select Role")]
            public RolesEnum SelectedRole { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            this.ReturnUrl = returnUrl;
            this.ExternalLogins = (await this._signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            this.ExternalLogins = (await this._signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (this.ModelState.IsValid)
            {
                var imageUrl = await this.ImageUpload(this.Input.ProfilePicUrl);
                var user = new ApplicationUser { UserName = this.Input.Username, Email = this.Input.Email, FirstName = this.Input.FirstName, LastName = this.Input.LastName, ProfilePicUrl = imageUrl, Birthday = this.Input.Birthday, Gender = this.Input.Gender, InstagramUrl = this.Input.InstagramUrl, FacebookUrl = this.Input.FacebookUrl, TwitterUrl = this.Input.TwitterUrl, YouTubeUrl = this.Input.YouTubeUrl, SondcloudUrl = this.Input.SondcloudUrl };
                var result = await this._userManager.CreateAsync(user, this.Input.Password);
                if (result.Succeeded)
                {
                    this._logger.LogInformation("User created a new account with password.");

                    var code = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = this.Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: this.Request.Scheme);

                    await this._emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (this.Input.SelectedRole.ToString() == "Artist" | this.Input.SelectedRole.ToString() == "User")
                    {
                        // Add user to selected role
                        await this._userManager.AddToRoleAsync(user, this.Input.SelectedRole.ToString());
                    }

                    if (this._userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return this.RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        await this._signInManager.SignInAsync(user, isPersistent: false);
                        return this.LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }

        public async Task<string> ImageUpload(IFormFile image)
        {
            string imagePath = string.Empty;
            byte[] destinationImage;

            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                destinationImage = memoryStream.ToArray();
            }

            using (var destinationStream = new MemoryStream(destinationImage))
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(image.FileName, destinationStream),
                    Folder = "Audiology/Profile pictures",
                };
                var uploadResult = this.cloudinary.UploadAsync(uploadParams);
                imagePath = uploadResult.Result.Uri.AbsoluteUri;
            }

            return imagePath;
        }
    }
}
