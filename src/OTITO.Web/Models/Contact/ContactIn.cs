using System;
using System.ComponentModel.DataAnnotations;

namespace OTITO.Web.Models.Contact
{
    public class ContactIn
    {
        [Required]
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
        public bool sent { get; set; }
    }
}
