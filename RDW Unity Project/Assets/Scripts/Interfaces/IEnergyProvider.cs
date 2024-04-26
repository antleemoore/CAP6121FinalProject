namespace Interfaces
{
    public interface IEnergyProvider
    {
        /// <summary>
        /// Energy that the Provider gives to the Ninja - Read Only
        /// </summary>
        int EnergyValue { get; }
    }
}