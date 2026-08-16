using Entities.Grid;

namespace Events.Combat
{
    public readonly struct HeroDiedEvent 
    {
        public readonly HeroEntity Hero;
        public HeroDiedEvent(HeroEntity hero)
        { 
            Hero = hero;
        }
    }

    public readonly struct HeroSpawnedEvent
    {
        public readonly HeroEntity Hero;
        public HeroSpawnedEvent(HeroEntity hero)    
        {
            Hero = hero;
        }
    }

    public readonly struct DamageDealtEvent
    {
        public readonly CharacterEntity Target;
        public readonly int Damage;
        public readonly bool IsCritical;

        public DamageDealtEvent(CharacterEntity target,
                                int damage,
                                bool isCritical)
        {
            Target = target;
            Damage = damage;
            IsCritical = isCritical;
        }
    }
}
