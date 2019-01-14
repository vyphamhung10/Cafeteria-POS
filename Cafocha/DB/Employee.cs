//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cafocha.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            this.OrderNotes = new HashSet<OrderNote>();
            this.SalaryNotes = new HashSet<SalaryNote>();
            this.StockIns = new HashSet<StockIn>();
            this.StockOuts = new HashSet<StockOut>();
            this.WorkingHistories = new HashSet<WorkingHistory>();
        }
    
        public string emp_id { get; set; }
        public string emp_code { get; set; }
        public string manager { get; set; }
        public string username { get; set; }
        public string pass { get; set; }
        public string name { get; set; }
        public System.DateTime birth { get; set; }
        public System.DateTime startday { get; set; }
        public int hour_wage { get; set; }
        public string addr { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int emp_role { get; set; }
        public int deleted { get; set; }
    
        public virtual AdminRe AdminRe { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderNote> OrderNotes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalaryNote> SalaryNotes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockIn> StockIns { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockOut> StockOuts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkingHistory> WorkingHistories { get; set; }
    }
}