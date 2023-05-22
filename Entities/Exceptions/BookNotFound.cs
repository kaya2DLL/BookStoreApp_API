namespace Entities.Eceptions
{
    public abstract partial class NotFoundException
    {
        public sealed class BookNotFoundExcepiton : NotFoundException
        {
            public BookNotFoundExcepiton(int id) 
                :base($"The Book with id :{id} could not found")
            {
            }
        }
    }
}
