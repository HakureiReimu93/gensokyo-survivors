using Godot;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GodotUtilities;
using GodotStrict.Types.Traits;
using GodotStrict.Types;
using GensokyoSurvivors.Core.Interface.Lens;
using System.Linq;
using GodotStrict.Traits;
using GodotStrict.Helpers.Logging;

[GlobalClass]
[UseAutowiring]
[Icon("res://Assets/GodotEditor/Icons/factory.png")]
public partial class EnemySpawner : Node, IDifficultyIncreasedSubject
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

		mOriginalSpawnDelay = MySpawnDelay;
	}

	public override void _Process(double delta)
	{
		if (mAcquiredArenaSession.Never())
		{
			ArenaSession.SingletonInstance?.AddDifficultyIncreaseRecipient(this);
		}

		base._Process(delta);
		if (mTimer.Tick(delta))
		{
			DoSpawnNewEnemy();
			mTimer.ResetWithCustomTime(MySpawnDelay - mDecreaseToSpawnDelay);
		}
	}

	private void DoSpawnNewEnemy()
	{
		if (mCameraBoundsRef.Unavailable(out var cameraInfo)) return;
		if (mUnitLayerRef.Unavailable(out var unitLayer)) return;
		if (mPlayerRef.Unavailable(out _)) return;

		// Make sure to offset the position by the camera's position in the world, because the bounds are screen coords.
		var randomSpawnPoint = Calculate.RandomSpotRectangleShell(cameraInfo.GetBounds(), SPAWN_MARGIN);

		var spaceState = PhysicsServer2D.SpaceGetDirectState(cameraInfo.GetWorld2D().Space);

		bool foundEmptySpot = false;
		Vector2 cameraCenter = cameraInfo.GlobalPosition;

		for (int i = 0; i < 4; i++)
		{
			Vector2 displacement = (randomSpawnPoint - cameraCenter).Rotated(Calculate.Pi90 * i);
			Vector2 destination = displacement + cameraCenter;

			// Ensure enemies cannot spawn in walls
			var queryParameters = PhysicsRayQueryParameters2D.Create(
				cameraCenter,
				destination,
				1 << 0
			);
			var queryResult = spaceState.IntersectRay(queryParameters);

			if (queryResult.Count == 0)
			{
				foundEmptySpot = true;
				randomSpawnPoint = destination;
				break;
			}
		}

		if (foundEmptySpot is false) return;

		// host a random enemy
		var allWeightsSum = mSpawnList.Aggregate(0f, (sum, udt) => sum + udt.MySpawnWeight);
		var randomChanceNum = Calculate.Random() * allWeightsSum;
		var currentBracketSum = 0f;
		var chosen = mSpawnList.SkipWhile(delegate (UnitDesignToken udt)
		{
			bool result = true;
			if (randomChanceNum <= currentBracketSum)
			{
				result = false;
			}
			currentBracketSum += udt.MySpawnWeight;
			return result;
		});
		var randomEnemy = Calculate.RandomCollectionItem(mSpawnList);
		MobUnit unit = randomEnemy.DoInstantiateNew();
		unitLayer.TryHost(unit);

		unit.GlobalPosition = randomSpawnPoint;
	}

	public void ConsiderNewDifficulty(uint pDifficulty)
	{
		MySpawnDelay = Mathf.Remap(pDifficulty, 0, 8, mOriginalSpawnDelay, 1f);
		mCurrentDifficulty = pDifficulty;
		this.LogAny(MySpawnDelay);
	}

	[Export(PropertyHint.Range, "0.1,8")]
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
	private float mDecreaseToSpawnDelay = 0f;
	private float mOriginalSpawnDelay;

	private uint mCurrentDifficulty = 0;

	private LiteTimer mTimer;
	private const float SPAWN_MARGIN = 10f;

	private TriggerFlag mAcquiredArenaSession;
}