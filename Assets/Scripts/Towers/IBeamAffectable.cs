public interface IBeamAffectable
{
    void TakeBeamDamage(float amount);
    void ApplySlow(float factor);
    void RemoveSlow();
    void MarkForExplosion();
}
