﻿using Loop.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Loop.Entities
{
    public class Post
    {
        public int PostId { get; set; }

        [Required, MinLength(2), MaxLength(300)]
        [Display(Name = "Post Title")]
        public string Title { get; set; }

        [Required, MinLength(10), MaxLength(5000)]
        [Display(Name = "Main Content")]
        public string Text { get; set; }
        [Required]
        [Display(Name = "Date and Time of Posting")]
        public DateTime PostDate { get; set; } 

        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<Reply> Replies { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

       
    }
}
