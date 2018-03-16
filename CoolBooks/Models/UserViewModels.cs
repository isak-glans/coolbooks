using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoolBooks.Models
{
    public class UserDetailViewModel
    {             
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime? Birthdate { get; set; }
        public byte[] Picture { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Info { get; set; }
        public string DisplayName { get; set; }
        public DateTime Created { get; set; }
        public bool IsDeleted { get; set; }
        public string UserId { get; set; }
        
        public bool isMine { get; set; }        
    }

    public class editUserViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime? Birthdate { get; set; }        
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }        
        public string Info { get; set; }
        public string DisplayName { get; set; }        
    }
}