namespace Entities.Eceptions
{
    public abstract partial class NotFoundException : Exception
    {
        protected NotFoundException( string message):base(message)
        {

        }
    }
}
