// ReSharper disable VirtualMemberCallInConstructor
namespace Audiology.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Audiology.Data.Common.Models;
    using Audiology.Data.Models.Enumerations;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
            this.UsersAlbums = new HashSet<UsersAlbum>();
        }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        // Extended props
        [Required]
        [MaxLength(25)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(25)]
        public string LastName { get; set; }

        public string ProfilePicUrl { get; set; }

        public Gender Gender { get; set; }

        public int? PlaylistId { get; set; }

        public virtual Playlist Playlist { get; set; }

        public int? FavouritesId { get; set; }

        public virtual Favourites Favourites { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Birthday { get; set; }

        public virtual ICollection<UsersAlbum> UsersAlbums { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
    }
}
