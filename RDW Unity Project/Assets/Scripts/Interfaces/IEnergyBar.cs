namespace Interfaces
{
    public interface IEnergyBar
    {
        float CurrentEnergy { get; }

        void TakeDamage(float dmg);
    }
}