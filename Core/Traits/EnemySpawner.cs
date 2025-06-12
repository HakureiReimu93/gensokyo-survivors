using Godot;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GodotUtilities;
using GodotStrict.Types.Traits;
using GodotStrict.Types;
using GensokyoSurvivors.Core.Interface.Lens;
using System.Linq;
using GodotStrict.Traits;

[GlobalClass]
[UseAutowiring]
[Icon("res://Assets/GodotEditor/Icons/factory.png")]
public partial class EnemySpawner : Node
{
	[Autowired("id-player")]
	private Scanner<LInfo2D> mPlayerRef;

	[Autowired("id-unit-layer")]
	private Scanner<LMother> mUnitLayerRef;

	[Autowired("id-game-camera")]
	private Scanner<LBoundsInfo2D> mCameraBoundsRef;

	[Autowired("Spawns")]
	private Node mSpawnedCollection;

	public override void _Ready()
	{
		__PerformDependencyInjection();

		SafeGuard.EnsureNotNull(mSpawnedCollection, "The 'Spawns' node shall contain a list of UnitDesignToken that describes which entities to pick from when spawning");
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
		if (mPlayerRef.Unavailable(out _)) return;

		var bounds = cameraInfo.GetBounds();
		bounds.Position = cameraInfo.GlobalPosition - (bounds.Size / 2);

		// Make sure to offset the position by the camera's position in the world, because the bounds are screen coords.
		var randomOrigin = Calculate.RandomSpotRectangleShell(bounds, C_SPAWN_MARGIN);

		// host a random enemy
		var randomEnemy = Calculate.RandomCollectionItem(mSpawnList);
		MobUnit unit = randomEnemy.DoInstantiateNew();
		unitLayer.TryHost(unit);

		unit.GlobalPosition = randomOrigin;
	}

	/// <summary>
	/// The delay between spawns in seconds.
	/// </summary>
	[Export]
	public float MySpawnDelay
	{
		get
		{
			return mSpawnDelay;
		}
		set
		{
			SafeGuard.Ensure(value > 0);

			mTimer.ResetWithCustomTime(value);
			mSpawnDelay = value;
		}
	}

	private UnitDesignToken[] mSpawnList;
	private float mSpawnDelay;

	/// <summary>
	/// The timer for the spawn delay.
	/// </summary>
	private LiteTimer mTimer;

	/// <summary>
	/// The margin between spawned enemies in pixels.
	/// </summary>
	private const float C_SPAWN_MARGIN = 10f;
}