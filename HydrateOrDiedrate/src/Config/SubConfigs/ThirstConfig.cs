using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HydrateOrDiedrate.Config.SubConfigs;

public class ThirstConfig 
{
    //TODO: stuff should really be renamed to hydration in some places , but for now we will keep it as thirst to avoid breaking compatibility too much.

    /// <summary>
    /// Master switch for the entire thirst/dehydration system
    /// </summary>
    [DefaultValue(true)]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// The upper bound of the player’s thirst gauge
    /// </summary>
    [Range(100d, 10000d)]
    [DefaultValue(1500f)]
    public float MaxThirst { get; set; } = 1500f;

    /// <summary>
    /// Base rate (units/tick) at which thirst increases under normal conditions
    /// (this will be affected by other multipliers depending on the circumstances)
    /// </summary>
    [Category("Thirst Decay")]
    [Range(0.1d, double.PositiveInfinity)]
    [DefaultValue(10f)]
    public float ThirstDecayRate { get; set; } = 10f;

    /// <summary>
    /// Maximum multiplier applied to ThirstDecayRate under extreme conditions
    /// (So the thirst decay rate after applying all multipliers will be capped at ThirstDecayRate * ThirstDecayRateMax)
    /// </summary>
    [Category("Thirst Decay")]
    [Range(1d, double.PositiveInfinity)]
    [DefaultValue(5f)]
    public float ThirstDecayRateMax { get; set; } = 5f;

    /// <summary>
    /// Scales the thirst reduction delay you get from drinking drinks
    /// Note: other parts of the equation are not configurable yet and final delay is capped at 600 seconds.
    /// </summary>
    [Category("Thirst Decay")]
    [Range(0d, double.PositiveInfinity)]
    [DefaultValue(1d)]
    public float HydrationLossDelayMultiplierNormalized { get; set; } = 1f; //TODO: the other parts of this equation should really be configurable as well.

    /// <summary>
    /// Multiplier for thirst decay when the player is sprinting.
    /// </summary>
    [Category("Thirst Decay")]
    [Range(1d, double.PositiveInfinity)]
    [DefaultValue(1.5d)]
    public float SprintThirstMultiplier { get; set; } = 1.5f;
    
    /// <summary>
    /// Multiplier for thirst decay when the player is idle.
    /// </summary>
    [Category("Thirst Decay")]
    [DisplayFormat(DataFormatString = "P")]
    [Range(0.01d, 1d)]
    [DefaultValue(0.25f)]
    public float IdleThirstModifier { get; set; } = 0.25f;

    /// <summary>
    /// Damage dealt to the player each interval once fully dehydrated
    /// (set to 0 to disable thirst damage entirely)
    /// </summary>
    [Category("Damage")]
    [Range(0d, double.PositiveInfinity)]
    [DefaultValue(1f)]
    public float ThirstDamage { get; set; } = 1f;

    /// <summary>
    /// How much damage a player takes from drinking boiling water.
    /// (set to 0 to disable boiling water damage entirely)
    /// </summary>
    [Category("Damage")]
    [Range(0d, double.PositiveInfinity)]
    [DefaultValue(5.0d)]
    public float BoilingWaterDamage { get; set; } = 5.0f;

    /// <summary>
    /// The maximum speed penalty a player can get from thirst.
    /// </summary>
    [Category("Penalty")]
    [DisplayFormat(DataFormatString = "P")]
    [Range(0d, 1d)]
    [DefaultValue(0.3f)]
    public float MaxMovementSpeedPenalty { get; set; } = 0.3f;

    /// <summary>
    /// From which thirst level the player starts getting a movement speed penalty.
    /// Set to -1 to disable the movement speed penalty entirely.
    /// (penalty will increase gradually as hydration drops even further)
    /// </summary>
    [Category("Penalty")]
    [Range(-1d, 10000d)] //TODO: should probably be a percentage of MaxThirst so you can't just forget about and have it always be active because you lowered max thirst
    [DefaultValue(600f)]
    public float MovementSpeedPenaltyThreshold { get; set; } = 600f;

    /// <summary>
    /// Percentage of the thirst gauge that will be filled after the player respawns.
    /// </summary>
    [Category("Penalty")]
    [DisplayFormat(DataFormatString = "P")]
    [Range(0d, 1d)]
    [DefaultValue(0.5f)]
    public float ThirstPercentageOnRespawn { get; set; } = 0.5f;
}
