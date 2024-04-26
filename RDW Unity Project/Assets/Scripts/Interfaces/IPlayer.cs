
namespace Interfaces
{
    public interface IPlayer
    {
        void TakeDamage(float damageValue);
        
        float Energy { get; }

        void ActivateMoldPower();

        public UnityEngine.Events.UnityEvent GameOverEvent { get; }
    }
}