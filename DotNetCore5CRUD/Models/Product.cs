using System.ComponentModel.DataAnnotations;

namespace DotNetCore5CRUD.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public bool InStock { get; set; }

        [Required]
        public byte[] Image { get; set; }

        public byte TagId { get; set; }

        public Tag Tag { get; set; }
    }
}
