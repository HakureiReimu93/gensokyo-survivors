using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;
using GodotStrict.Types.Traits;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GensokyoSurvivors.Core.Interface;
using System.Linq;
using GodotUtilities;
using System.Collections.ObjectModel;

/// <summary>
/// Spawns skills for the current player.
/// </summary>
[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/factory.png")]
[UseAutowiring]
public partial class CooldownAbilitySpawner : Node, IAbilitySpawner, INewUpgradeSubject
{

    [Autowired("id-skill-layer")]
    private Scanner<LMother> mSkillLayerRef;

    [Autowired]
    private Option<IAdversarialUnitPicker> mVictimPicker;

    [Autowired]
    private Option<IPickPointArountProtagonist> mPlayerPointPicker;

    public override void _Ready()
    {
        __PerformDependencyInjection();

        SafeGuard.EnsureNotNull(MySkillToSpawn);
        SafeGuard.EnsureFloatsNotEqual(MySpawnDelay, 0f, "delay cannot be 0");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (mTimer.Tick(delta))
        {
            mTimer.ResetWithCustomTime(MySpawnDelay * mSpawnDelayMultiplier);
            SpawnNext();
        }
    }

    private void SpawnNext()
    {
        if (mSkillLayerRef.Unavailable(out var skillLayerInfo)) return;

        var playerRef = GetTree().GetFirstNodeInGroup("id-player");
        SafeGuard.EnsureIsConstType<MobUnit>(playerRef);

        if (mVictimPicker.Available(out var victimPicker))
        {
            var pickResult = victimPicker
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
        else if (mPlayerPointPicker.Available(out var ppp))
        {
            var pickedPosition = ppp.WithProtagonist(playerRef as MobUnit)
                                    .ComputeOffset();
            
            var instantiated = MySkillToSpawn.InstantiateOrNull<Node2D>();
            SafeGuard.EnsureIsConstType<IPhysicalSkill>(instantiated);

            // Set the position for the ability.
            // do this first because some abilities read Global Position on _Ready
            instantiated.GlobalPosition = pickedPosition;

            skillLayerInfo.TryHost(instantiated);

        }
        
    }

    public void ConsiderNewUpgrade(UpgradeMetaData pMeta, ReadOnlyDictionary<UpgradeMetaData, uint> pAllUpgrades)
    {
        if (pMeta.MyID == cDecreaseCooldownUpgradeID)
        {
            mSpawnDelayMultiplier *= 1 - 0.10f;
        }
    }


    [Export]
    public PackedScene MySkillToSpawn { get; set; }

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

    private float mSpawnDelayMultiplier = 1f;

    private float mSpawnDelay;

    private LiteTimer mTimer;

    readonly StringName cDecreaseCooldownUpgradeID = new("100_000");
}