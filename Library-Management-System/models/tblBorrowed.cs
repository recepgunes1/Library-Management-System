//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Library_Management_System.models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblBorrowed
    {
        public int brwID { get; set; }
        public Nullable<int> brwBkID { get; set; }
        public Nullable<int> brwUsrID { get; set; }
        public System.DateTime brwPurchaseDate { get; set; }
        public Nullable<bool> brwSitutation { get; set; }
        public System.DateTime brwRedemptionDate { get; set; }
    
        public virtual tblBook tblBook { get; set; }
        public virtual tblUser tblUser { get; set; }
    }
}
