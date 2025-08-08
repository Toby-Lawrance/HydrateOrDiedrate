namespace HydrateOrDiedrate.Thirst;

/// <summary>
/// Interface for EntityBehaviors that allowes influencing the thirst rate of an entity.
/// </summary>
public interface IThirstRateModifier
{
    public float OnThirstRateCalculate(float currentModifier);
}
