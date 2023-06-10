namespace Entities.Eceptions
{
    public abstract partial class NotFoundException
    {
        public sealed class CategoryNotFoundException : NotFoundException
        {
           public CategoryNotFoundException(int id) : base($"Category with id : {id} could not found.")
            {
            }
        }
    }
}
