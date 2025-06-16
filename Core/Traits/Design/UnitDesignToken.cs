using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/tag.png")]
public partial class UnitDesignToken : Node
{
	[Export]
	PackedScene MySubjectScene;

	[Export(PropertyHint.Range,"0,2")]
	public float MySpawnWeight { get; set; }

	[Export(PropertyHint.Range, "0,8")]
	public uint MyDifficulty { get; set; }

	public override void _Ready()
	{
	}

	public MobUnit DoInstantiateNew()
	{
		SafeGuard.EnsureCanInstantiate(MySubjectScene);
		var instantiatedAsMobUnit = MySubjectScene.InstantiateOrNull<MobUnit>();
		SafeGuard.Ensure(instantiatedAsMobUnit is not null);

		return instantiatedAsMobUnit;
	}
	
}
