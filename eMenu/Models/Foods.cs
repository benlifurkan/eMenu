namespace eMenu.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Foods
    {
        public Guid foodsID { get; set; }

        public Guid menuID { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        [StringLength(250)]
        public string description { get; set; }

        [Column(TypeName = "money")]
        public decimal price { get; set; }

        [StringLength(200)]
        public string imagePath { get; set; }

        public virtual Menus Menus { get; set; }
    }
}
