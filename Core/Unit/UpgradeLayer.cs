using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using System.Linq;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class UpgradeLayer : Node
{
	public override void _Ready()
	{
		SafeGuard.EnsureIsConstType<PlayerControl>(GetParent());
		SafeGuard.EnsureNotNull(MyFallbackUpgrade);
	}

	public UpgradeMetaData[] DoGetNextUpgrades_3()
	{
		// Get 3 upgrades simply by getting them from the pool.
		// no removing duplicates yet
		SafeGuard.EnsureNonempty(MyPool);
		var items = Calculate.RandomCollectionItems(MyPool, 3);
		return [.. items];
	}

	// big problem because this is going to need to accomodate every single upgrade.
	public void HostUpgrade(UpgradeMetaData pWhichUpgrade)
	{
		this.LogAny($"Upgrade has been hosted: {pWhichUpgrade.MyDisplayName}");
	}

	[Export]
	Godot.Collections.Array<UpgradeMetaData> MyPool { get; set; }

	[Export]
	UpgradeMetaData MyFallbackUpgrade { get; set; }
}
