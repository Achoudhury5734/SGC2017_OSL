//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OSL.AdminPortal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Donation
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public Nullable<long> RecipientId { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> Expiration { get; set; }
        public string Status { get; set; }
        public int Amount { get; set; }
        public long DonorId { get; set; }
        public System.DateTime Updated { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime StatusUpdated { get; set; }
        public string PictureUrl { get; set; }
    }
}
