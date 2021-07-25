using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MemeMarketeerApp.Models.Common.Authentication
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }

        [Display(Name = "Created On")]
        public DateTime? CreatedOn { get; set; }
    }
}
