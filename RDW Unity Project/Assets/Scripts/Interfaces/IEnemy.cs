namespace Interfaces
{
    public interface IEnemy : IGrabbable
    {
        // returns enemy remaining health
        int Slash(int dmg);
        int Stab(int dmg);

        int ShurikenHit(int dmg);

        void Parry();

        void Mold();
        
        bool PlayerDetected { get; set; }
    }
}