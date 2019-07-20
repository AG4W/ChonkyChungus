using UnityEngine;

public class AttackCommand : ActorCommand
{
    int _hitRoll;

    int _damageRoll;
    int _criticalDamageRoll;

    bool _isHit = false;
    bool _isCriticalSuccess = false;
    bool _isCriticalFailure = false;

    Actor _attacked;

    public AttackCommand(Actor attacker, Actor attacked, Item item) : base(attacker)
    {
        _attacked = attacked;
        _hitRoll = Synched.Dice(20);
        _damageRoll = Synched.Dice(item.damageType.damage);
        _criticalDamageRoll = Synched.Dice(6);

        _isHit = _hitRoll <= attacker.data.GetAttribute(AttributeType.Accuracy).value;

        if (_isHit)
            _isCriticalSuccess = _hitRoll == 1;
        else
            _isCriticalFailure = _hitRoll == 20;
    }

    public override void Execute()
    {
        base.actor.transform.LookAt(_attacked.transform.position, Vector3.up);
        base.actor.OnAttack();

        GlobalEvents.Raise(
            GlobalEvent.PopupRequested, 
            base.actor.transform.position + Vector3.up * 1.5f,
            (_isHit ? "Hit!" : "Miss!") + "\nRequired: " + base.actor.data.GetAttribute(AttributeType.Accuracy).value + ", Rolled: " + _hitRoll);

        if (_isHit)
        {
            _attacked.data.GetVital(VitalType.Health).Update(_isCriticalSuccess ? -(_damageRoll + _criticalDamageRoll) : -_damageRoll);
            _attacked.OnAttacked();

            AudioManager.PlayOneshot(_attacked.data.damageSFX.Random(), _attacked.transform.position + Vector3.up * 1.8f, .05f, .15f, .75f, 1.25f);
            GlobalEvents.Raise(
                GlobalEvent.PopupRequested,
                _attacked.transform.position + Vector3.up * 1.5f,
                _isCriticalSuccess ? "<b><color=red>" + (-_damageRoll).ToString() + " + " + (-_criticalDamageRoll).ToString() + "</color></b>" : "<color=red>" + (-_damageRoll).ToString() + "</color>");
        }

        base.actor.data.GetVital(VitalType.Stamina).Update(-1);
    }
}
