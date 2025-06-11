using Godot;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Interface;
using System.Collections.Generic;
using GodotStrict.Types;
using System.Linq;

/// <summary>
/// <code>
/// This class provides functionality for picking the closest unit to a protagonist.
/// It allows setting the maximum range within which units can be considered as victims,
/// and selecting a specific protagonist and a collection of antagonists.
/// </code>
/// </summary>
[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class PickClosestUnit : Node, IAdversarialUnitPicker
{
    public override void _Ready()
    {
        SafeGuard.Ensure(MyMaxRange != 0, "Set the max range of this spawn strategy.");
        SafeGuard.EnsureIsConstType<IAbilitySpawner>(GetParent());
    }

    /// <summary>
    /// Computes the closest unit to the protagonist within the specified maximum range.
    /// </summary>
    public Option<MobUnit> ComputeUnitVictim()
    {
        if (mProtagonist.Available(out var protagonist) is false) return default;

        var sortedEnemies = mAntagonists
            .ToList()
            .Where(antagonist => antagonist.GlobalPosition.DistanceSquaredTo(protagonist.GlobalPosition) <= MyMaxRange * MyMaxRange)
            .OrderBy(antagonist => antagonist.GlobalPosition.DistanceSquaredTo(protagonist.GlobalPosition));

        if (sortedEnemies.Any())
        {
            return sortedEnemies.First();
        }
        else
        {
            return Option<MobUnit>.None;
        }
    }

    /// <summary>
    /// Sets the protagonist to be used for unit selection.
    /// </summary>
    /// <param name="protagonist">The protagonist to use.</param>
    /// <returns>A reference to this object, allowing method chaining.</returns>
    public IAdversarialUnitPicker WithProtagonist(MobUnit protagonist)
    {
        mProtagonist = protagonist;
        return this;
    }

    /// <summary>
    /// Sets the collection of antagonists to be used for unit selection.
    /// </summary>
    /// <param name="antagonists">The collection of antagonists.</param>
    /// <returns>A reference to this object, allowing method chaining.</returns>
    public IAdversarialUnitPicker WithAntagonists(IEnumerable<MobUnit> antagonists)
    {
        mAntagonists = antagonists;
        return this;
    }

    [Export(PropertyHint.Range, "0,500")]
    public float MyMaxRange { get; set; }

    private Option<MobUnit> mProtagonist = default;
    private IEnumerable<MobUnit> mAntagonists = [];
}