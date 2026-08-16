using Entities.Grid;
using Events.Combat;
using MessagePipe;
using R3;
using UnityEngine;

namespace Systems.Combat
{
    public class CombatManager
    {
        private readonly IPublisher<DamageDealtEvent> _damagePublisher;

        public CombatManager(IPublisher<DamageDealtEvent> damagePublisher)
        {
            _damagePublisher = damagePublisher;
        }

        public void ResolveMeleeAttack(CharacterEntity attacker, CharacterEntity defender)
        {
            bool isBackstab = attacker.Direction.Value == defender.Direction.Value;

            if (isBackstab)
            {
                int damage = Mathf.RoundToInt(attacker.Attack.Value * 1.5f);
                ApplyDamage(defender, damage);

                _damagePublisher.Publish(new DamageDealtEvent(defender, damage, isCritical: true));
            }
            else
            {
                int damage = attacker.Attack.Value;
                ApplyDamage(defender, damage);
                _damagePublisher.Publish(new DamageDealtEvent(defender, damage, isCritical: false));

                if (defender.CurrentHealth.Value > 0)
                {
                    int counterDamage = defender.Attack.Value;
                    ApplyDamage(attacker, counterDamage);
                    _damagePublisher.Publish(new DamageDealtEvent(attacker, counterDamage, isCritical: false));
                }
            }
        }

        private void ApplyDamage(CharacterEntity target, int amount)
        {
            target.CurrentHealth.Value = Mathf.Max(0, target.CurrentHealth.Value - amount);

            if (target.CurrentHealth.Value == 0)
            {
                target.State.Value = CharacterState.Death;
                target.Destroyed.Execute(Unit.Default);
            }
        }
    }
}