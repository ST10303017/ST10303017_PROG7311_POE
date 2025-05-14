/*
Calwyn Govender
ST10303017
PROG7311
(OpenAI, 2025)
(Troelsen & Japikse, 2022)
*/

using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10303017_PROG7311_POE.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Product> Products { get; set; }

        public ApplicationUser()
        {
            Products = new HashSet<Product>();
        }
    }
}