using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class UpgradeLayer : Node
{
	[Export]
	UpgradeMetaData[] MyPool { get; set; }

	[Export]
	UpgradeMetaData MyFallbackChoice { get; set; }

	public override void _Ready()
	{
		SafeGuard.EnsureIsConstType<PlayerControl>(GetParent());
		SafeGuard.EnsureNotNull(MyFallbackChoice);
	}

	public Tuple<UpgradeMetaData, UpgradeMetaData, UpgradeMetaData> DoGet3Upgrades()
	{
		SafeGuard.EnsureNonempty(MyPool);

		return new Tuple<UpgradeMetaData, UpgradeMetaData, UpgradeMetaData>(
			MyFallbackChoice,
			MyFallbackChoice,
			MyFallbackChoice
		);
	}
}
