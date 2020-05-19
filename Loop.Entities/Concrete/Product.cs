﻿using Loop.Entities.Intermediate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Loop.Entities.Concrete
{
    public abstract class Product
    {
        public int ProductId { get; set; }

        [Required, MinLength(2), MaxLength(60)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required, MinLength(10), MaxLength(300)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Date and Time of Production")]
        public DateTime? ProductionDate { get; set; }

        [Display(Name = "Date and Time of Production")]
        public DateTime InsertionDate { get; set; } = DateTime.Now;

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
