using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;
using GodotStrict.Types.Traits;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GensokyoSurvivors.Core.Interface;
using System.Linq;
using GodotUtilities;

/// <summary>
/// Spawns skills for the current player.
/// </summary>
[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/factory.png")]
[UseAutowiring]
public partial class CooldownAbilitySpawner : Node, IAbilitySpawner
{
    /// <summary>
    /// The layer upon which to spawn the ability
    /// </summary>
    [Autowired("id-skill-layer")]
    private Scanner<LMother> mSkillLayerRef;

    /// <summary>
    /// The unit picker strategy node.
    /// </summary>
    [Autowired]
    private IAdversarialUnitPicker mVictimPicker;

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

    /// <summary>
    /// Called every physics frame.
    /// </summary>
    public override void _PhysicsProcess(double delta)
    {
        if (mTimer.Tick(delta))
        {
            mTimer.Reset();
            SpawnNext();
        }
    }

	/// <summary>
	/// Pick an antagonist and spawn the skill on the skill layer
	/// </summary>
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

    /// <summary>
    /// The packed scene with the skill to spawn.
    /// </summary>
    [Export]
    public PackedScene MySkillToSpawn { get; set; }

    /// <summary>
    /// The delay between spawns in seconds.
    /// </summary>
    [Export(PropertyHint.Range, "0,10")]
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

    private float mSpawnDelay;

    /// <summary>
    /// The owner node.
    /// </summary>
    private Node2D mOwner;

    /// <summary>
    /// The timer for the spawn delay.
    /// </summary>
    private LiteTimer mTimer;
}