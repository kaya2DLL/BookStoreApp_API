using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObject
{
    public abstract record BookDtoForManipulation
    {
        [Required(ErrorMessage ="title is requried Field")]
        [MinLength(2, ErrorMessage ="Title must consist of at least 2 characters")]
        [MaxLength(50, ErrorMessage = "Title must consist of at macimum 50 characters")]
        public string Title { get; init; }
        [Required(ErrorMessage = "Title is requried Field")]
        [Range(10,1000)]
        public decimal Price { get; init; }
    }
}
