using DotNetCore5CRUD.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNetCore5CRUD.ViewModels
{
    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(250)]
        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public bool InStock { get; set; }

        [Display(Name = "Select image...")]
        public byte[] Poster { get; set; }

        [Display(Name = "Tag")]
        public byte TagId { get; set; }

        public IEnumerable<Tag> Tags { get; set; }
    }
}