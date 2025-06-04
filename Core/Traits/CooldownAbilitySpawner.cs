using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;
using GodotStrict.Types.Traits;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GensokyoSurvivors.Core.Interface;
using System.Linq;
using GodotUtilities;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/factory.png")]
[UseAutowiring]
public partial class CooldownAbilitySpawner : Node, IAbilitySpawner
{
	[Autowired("id-skill-layer")]
	Scanner<LMother> mSkillLayerRef;

	[Autowired]
	IAdversarialUnitPicker mVictimPicker;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		__PerformDependencyInjection();

		SafeGuard.EnsureNotNull(MySkillToSpawn);
		SafeGuard.EnsureFloatsNotEqual(MySpawnDelay, 0f, "delay cannot be 0");
		SafeGuard.Ensure(Owner is Node2D, "Cannot take advantage of certain spawn strategies without Position reference");

		//TODO: Ensure owner is a player, because this spawner only works on players.
		mOwner = Owner as Node2D;
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
		if (mSkillLayerRef.Unavailable(out var skillLayerInfo)) return;

		var playerRef = GetTree().GetFirstNodeInGroup("id-player");
		SafeGuard.EnsureIsConstType<MobUnit>(playerRef);

		var pickResult = mVictimPicker
			.WithProtagonist(playerRef as MobUnit)
			.WithAntagonists(
				GetTree()
					.GetNodesInGroup("id-enemy")
					.Cast<MobUnit>()
			)
			.ComputeUnitVictim();

		if (pickResult.Unavailable(out MobUnit victim)) return;

		var instantiated = MySkillToSpawn.InstantiateOrNull<Node2D>();
		SafeGuard.EnsureIsConstType<IPhysicalSkill>(instantiated);

		skillLayerInfo.TryHost(instantiated);

		// Set the position for the ability.
		instantiated.GlobalPosition = victim.GlobalPosition;

		// Set up the skill
		var skill = instantiated as IPhysicalSkill;
		skill.OnEnemyChosen(victim);
	}

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

	Node2D mOwner;
	LiteTimer mTimer;

	

}