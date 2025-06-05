using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GodotUtilities;
using GodotStrict.Types.Traits;
using GodotStrict.Types;
using GensokyoSurvivors.Core.Interface.Lens;
using System.Linq;

[GlobalClass]
[UseAutowiring]
[Icon("res://Assets/GodotEditor/Icons/factory.png")]
public partial class EnemySpawner : Node
{
	private const float C_SPAWN_MARGIN = 10f;

	[Autowired("id-unit-layer")]
	Scanner<LMother> mUnitLayerRef;

	[Autowired("id-game-camera")]
	Scanner<LBoundsInfo2D> mCameraBoundsRef;

	[Autowired("Spawns")]
	Node mSpawnedCollection;

	public override void _Ready()
	{
		__PerformDependencyInjection();

		SafeGuard.EnsureNotNull(mSpawnedCollection, "the 'Spawns' node shall contain a list of UnitDesignToken that describes which entities to pick from when spawning");
		SafeGuard.Ensure(mTimer.WaitTime != 0);

		mSpawnList = mSpawnedCollection
						.GetChildren()
						.Cast<UnitDesignToken>()
						.ToArray();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (mTimer.Tick(delta))
		{
			DoSpawnNewEnemy();
			mTimer.Reset();
		}
	}

	private void DoSpawnNewEnemy()
	{
		if (mCameraBoundsRef.Unavailable(out var cameraInfo)) return;
		if (mUnitLayerRef.Unavailable(out var unitLayer)) return;

		var bounds = cameraInfo.GetBounds();
		
		// make sure to offset the position by the camera's position in the world, because the bounds are screen coords.
		var randomOrigin = Chance.RandomSpotRectangleShell(bounds, C_SPAWN_MARGIN) + cameraInfo.GlobalPosition;

		var randomEnemy = Chance.RandomCollectionItem(mSpawnList);
		MobUnit unit = randomEnemy.DoInstantiateNew();
		unitLayer.TryHost(unit);

		// ParamInit
		unit.GlobalPosition = randomOrigin;
	}

	[Export]
	public float MySpawnDelay
	{
		get
		{
			return mSpawnDelay;
		}

		set
		{
			mSpawnDelay = value;
			mTimer.ResetWithCustomTime(value);
		}
	}

	float mSpawnDelay;
	LiteTimer mTimer;

	UnitDesignToken[] mSpawnList;

	// Run dependency injection before _Ready() is called.
}
