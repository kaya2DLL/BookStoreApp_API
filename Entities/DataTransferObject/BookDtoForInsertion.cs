using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObject
{
    public record BookDtoForInsertion : BookDtoForManipulation
    {
        [Required(ErrorMessage ="Category id is required")]
        public int CategoryId { get; init; }
    }
}
