//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAM1
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string CellPhone { get; set; }
        public bool IsAdmin { get; set; }
        public int AddressId { get; set; }
        public string AuthenticationCode { get; set; }
        public System.DateTime RegistrationDate { get; set; }
    
        public virtual Address Address { get; set; }
    }
}
