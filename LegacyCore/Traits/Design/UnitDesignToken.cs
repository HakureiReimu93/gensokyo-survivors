using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GodotStrict.AliasTypes;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/tag.png")]
public partial class UnitDesignToken : Node
{
	[Export]
	PackedScene MySubjectScene;

	[Export(PropertyHint.Range,"0,2")]
	public float MySpawnWeight { get; set; }

	[Export(PropertyHint.Range, "0,8")]
	public int MyDifficulty { get; set; }

	public normal CalculateSpawnChance(int pDifficulty, int maxDifficultyDifference)
	{
		float fallOff = 1f / maxDifficultyDifference * Math.Abs(MyDifficulty - pDifficulty);
		return new normal(MyDifficulty - fallOff);
	}

	public MobUnit DoInstantiateNew()
	{
		SafeGuard.EnsureCanInstantiate(MySubjectScene);
		var instantiatedAsMobUnit = MySubjectScene.InstantiateOrNull<MobUnit>();
		SafeGuard.Ensure(instantiatedAsMobUnit is not null);

		return instantiatedAsMobUnit;
	}
	
}
