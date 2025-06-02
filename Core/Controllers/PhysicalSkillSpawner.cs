using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Traits;
using GodotStrict.Types;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using System;
using GodotStrict.Types.Traits;
using GensokyoSurvivors.Core.Controllers;
using GensokyoSurvivors.Core.Interface;
using System.Linq;

public partial class PhysicalSkillSpawner : Node
{
	[Export]
	PackedScene MySkillToSpawn { get; set; }

	[Export]
	SkillSpawnOrigin MySpawnStrategy;

	[Export(PropertyHint.Range, "0,10")]
	float MySpawnDelay
	{
		get
		{
			return mTimer.WaitTime;
		}
		set
		{
			SafeGuard.Ensure(value > 0);
			mTimer.ResetWithCustomTime(value);
		}
	}

	LiteTimer mTimer;

	Scanner<LMother> mSkillLayerRef;
	Scanner<LInfo2D> mPlayerRef;

	Node2D mOwner;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SafeGuard.EnsureNotNull(MySkillToSpawn);
		SafeGuard.EnsureFloatsNotEqual(MySpawnDelay, 0f, "delay cannot be 0");
		SafeGuard.Ensure(Owner is Node2D, "Cannot take advantage of certain spawn strategies without Position reference");

		mOwner = Owner as Node2D;

		mSkillLayerRef = ExpectFromGroup<LMother>("id-skill-layer");
		mPlayerRef = ExpectFromGroup<LInfo2D>("id-player");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (mTimer.Tick(delta))
		{
			mTimer.Reset();
			if (mSkillLayerRef.Available(out var skillLayerRefInfo) &&
				TryGetSkillSpawnOrigin(out var skillOrigin))
			{
				SafeGuard.EnsureIsConstType<IPhysicalSkill>(MySkillToSpawn);
				var instantiated = MySkillToSpawn.InstantiateOrNull<Node2D>();
				skillLayerRefInfo.TryHost(instantiated);
				instantiated.GlobalPosition = skillOrigin;
			}
		}
	}

	private bool TryGetSkillSpawnOrigin(out Vector2 pSkillSpawnOrigin)
	{
		pSkillSpawnOrigin = default;
		switch (MySpawnStrategy)
		{
			case SkillSpawnOrigin.ON_PLAYER:
				if (!mPlayerRef.Available(out var playerRef)) return false;
				pSkillSpawnOrigin = playerRef.GlobalPosition;
				return true;
			case SkillSpawnOrigin.ON_NEAREST_ENEMY:
				// TODO: multi-scanner.
				var enemies = GetTree().GetNodesInGroup("id-enemy").Cast<Node2D>();
				var closestEnemy = enemies
					.ToList()
					.OrderBy(enemy => enemy.GlobalPosition.DistanceSquaredTo(mOwner.GlobalPosition))
					.FirstOrDefault();

				if (closestEnemy is null) return false;

				pSkillSpawnOrigin = closestEnemy.GlobalPosition;
				return true;
			case SkillSpawnOrigin.ON_RANDOM_ENEMY:
				var enemyList = GetTree().GetNodesInGroup("id-enemy").Cast<Node2D>();
				var randomEnemy = enemyList
					.ToList()
					.OrderBy(x => Random.Shared.Next())
					.FirstOrDefault();

				if (randomEnemy is null) return false;

				pSkillSpawnOrigin = randomEnemy.GlobalPosition;

				return true;
			default:
				return false;
		}
	}
}
