namespace Interfaces
{
    public interface IGrabber
    {
        void Clear();
        UnityEngine.Transform GetTransform();
        bool Throw();
        bool Grab(IGrabbable obj);
    }
}