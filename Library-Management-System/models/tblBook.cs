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
    
    public partial class tblBook
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblBook()
        {
            this.tblBorroweds = new HashSet<tblBorrowed>();
        }
    
        public int bkID { get; set; }
        public string bkName { get; set; }
        public string bkAuthor { get; set; }
        public System.DateTime bkPublishedDate { get; set; }
        public int bkPages { get; set; }
        public byte bkAmount { get; set; }
        public string bkSummary { get; set; }
        public int bkCategoryID { get; set; }
    
        public virtual tblCategory tblCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBorrowed> tblBorroweds { get; set; }
    }
}
