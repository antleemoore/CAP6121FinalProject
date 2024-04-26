namespace Interfaces
{
    public interface IGrabbable
    {
        bool Select(IGrabber parent);
        bool Throw();
    }
}