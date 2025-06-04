using Godot;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Interface;
using System.Collections.Generic;
using GodotStrict.Types;
using System.Linq;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class PickClosestUnit : Node, IAdversarialUnitPicker
{
	public override void _Ready()
	{
		SafeGuard.Ensure(MyMaxRange != 0, "Set the max range of this spawn strategy.");
		SafeGuard.EnsureIsConstType<IAbilitySpawner>(GetParent());
	}

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

	public IAdversarialUnitPicker WithProtagonist(MobUnit protagonist)
	{
		mProtagonist = protagonist;
		return this;
	}

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
