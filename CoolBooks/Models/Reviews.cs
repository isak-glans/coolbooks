namespace CoolBooks.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    public partial class Reviews
    {
        public Reviews()
        {
            Created = DateTime.Now;
            IsDeleted = false;
        }

        public int Id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int BookId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(4000)]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [Range(1, 5)]
        public byte? Rating { get; set; }
        
        public DateTime Created { get; set; }

        public bool IsDeleted { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }        

        public virtual Books Books { get; set; }
    }
}
