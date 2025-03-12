﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.DataAccess.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(255)]
        public string Title { get; set; } = null!;

        public int Year { get; set; }

        [MaxLength(255)]
        public string Director { get; set; } = null!;

        public string Synopsis { get; set; } = null!;

        public int Length { get; set; }

        public byte[] Image { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Screening> Screenings { get; set; } = [];
    }
}
