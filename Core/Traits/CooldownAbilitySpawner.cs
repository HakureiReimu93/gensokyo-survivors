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

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/factory.png")]
public partial class CooldownAbilitySpawner : Node, IAbilitySpawner
{
	[Export]
	PackedScene MySkillToSpawn { get; set; }

	[Export(PropertyHint.Range, "0,10")]
	float MySpawnDelay
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
	float mSpawnDelay;

	LiteTimer mTimer;

	Scanner<LMother> mSkillLayerRef;
	Scanner<LInfo2D> mPlayerRef;
	IAbilitySpawnStrategy mSpawnStrategy;

	Node2D mOwner;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SafeGuard.EnsureNotNull(MySkillToSpawn);
		SafeGuard.EnsureFloatsNotEqual(MySpawnDelay, 0f, "delay cannot be 0");
		SafeGuard.Ensure(Owner is Node2D, "Cannot take advantage of certain spawn strategies without Position reference");

		//TODO: Ensure owner is a player, because this spawner only works on players.

		mOwner = Owner as Node2D;

		mSkillLayerRef = ExpectFromGroup<LMother>("id-skill-layer");
		mPlayerRef = ExpectFromGroup<LInfo2D>("id-player");

		mSpawnStrategy = this.Require<IAbilitySpawnStrategy>();
	}


	public override void _PhysicsProcess(double delta)
	{
		if (mTimer.Tick(delta))
		{
			mTimer.Reset();
			SpawnNext();
		}
	}

	private void SpawnNext()
	{
		if (mSkillLayerRef.Unavailable(out var skillLayerInfo)
					|| mPlayerRef.Unavailable(out var playerInfo)) return;

		var spawnOriginMaybe = mSpawnStrategy
			.WithProtagonistPosition(playerInfo.GlobalPosition)
			.WithAntagonistPositions(
				GetTree()
					.GetNodesInGroup("id-enemy")
					.Cast<Node2D>()
					.Select(node => node.GlobalPosition)
			)
			.ComputeSpawnOrigin();

		if (spawnOriginMaybe.Unavailable(out Vector2 spawnOrigin)) return;

		var instantiated = MySkillToSpawn.InstantiateOrNull<Node2D>();
		SafeGuard.EnsureIsConstType<IPhysicalSkill>(instantiated);

		skillLayerInfo.TryHost(instantiated);
		instantiated.GlobalPosition = spawnOrigin;
	}
}
