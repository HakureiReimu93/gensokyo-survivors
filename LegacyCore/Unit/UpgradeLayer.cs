using Godot;
using static GodotStrict.Helpers.Logging.StrictLog;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using System.Linq;
using GensokyoSurvivors.Core.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class UpgradeLayer : Node
{
	public override void _Ready()
	{
		SafeGuard.EnsureIsConstType<PlayerControl>(GetParent());
		SafeGuard.EnsureNotNull(MyFallbackUpgrade);
	}

	// big problem because this is going to need to accomodate every single upgrade.
	public void HostUpgrade(UpgradeMetaData pWhichUpgrade)
	{
		if (!mNonUniqueUpgradeCount.TryAdd(pWhichUpgrade, 1u))
		{
			mNonUniqueUpgradeCount[pWhichUpgrade] += 1u;
		}

		this.LogAny($"Upgrade has been hosted: {pWhichUpgrade.MyDisplayName}");

		if (GetOwner() is INewUpgradeDispatcher dispatcher)
		{
			dispatcher.SendNewUpgrade(
				pWhichUpgrade,
				new ReadOnlyDictionary<UpgradeMetaData, uint>(mNonUniqueUpgradeCount)
			);
		}
		else
		{
			this.LogWarn("cannot accept new upgrade, as the owner is not a dispatcher.");
		}
		
		if (pWhichUpgrade.MyStackType == SkillStackType.OnlyOne)
		{
			MyPool.Remove(pWhichUpgrade);
		}
	}

	public UpgradeMetaData[] DoGetNextUpgrades_3()
	{
		// Get 3 upgrades simply by getting them from the pool.
		// no removing duplicates yet
		SafeGuard.EnsureNonempty(MyPool);
		return Calculate.RandomCollectionItems(MyPool, 3);
	}

	public bool TryGetUpgradeMetaDataFrom(StringName pID, out UpgradeMetaData upgrade)
	{
		upgrade = MyPool.Where(umd => umd.MyID == pID).FirstOrDefault();

		return upgrade == null;
	}

	[Export]
	Godot.Collections.Array<UpgradeMetaData> MyPool { get; set; }

	[Export]
	UpgradeMetaData MyFallbackUpgrade { get; set; }

	Dictionary<UpgradeMetaData, uint> mNonUniqueUpgradeCount = new();
}
