using Godot;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Utility;
using GensokyoSurvivors.Core.Interface;
using GodotStrict.Types;

[GlobalClass]
[Icon("res://Assets//GodotEditor/Icons/hurtbox.png")]
public partial class HurtBox : Area2D, IFactionMember
{
    public override void _Ready()
    {
        SafeGuard.EnsureIsConstType<IKillable>(Owner);

        mGraceTimer = new LiteTimer(MyGracePeriod);

        SafeGuard.Ensure(CollisionLayer == 0, "Do not set the collision layer!");
        AreaEntered += HandleAreaEntered;

        (Owner as IKillable).OnDie(Deactivate);
    }

    private void Deactivate()
    {
        Callable.From(() => ProcessMode = ProcessModeEnum.Disabled).CallDeferred();
        Visible = false;
    }

    private void HandleAreaEntered(Area2D other)
    {
        if (other is not HitBox hitBox) return;
        hitBox.OnCollidedWith(this);

        var damage = hitBox.MyDamageOnHit;

        if (damage > 0)
        {
            EmitSignal(SignalName.MyTakeRawDamage, hitBox.MyDamageOnHit);
        }

        Callable.From(() => SetMonitoring(false)).CallDeferred();
        mGraceTimer.ResetWithCustomTime(MyGracePeriod);
    }

    public override void _Process(double delta)
    {
        if (mGraceTimer.Tick(delta))
        {
            Callable.From(() => SetMonitoring(true)).CallDeferred();
        }
    }

    [Export]
    public FactionEnum MyFaction
    {
        get
        {
            return mFaction;
        }
        set
        {
            mFaction = value;
            CollisionMask = FactionUtil.LayerFromFaction(mFaction);
        }
    }

    [Export]
    float MyGracePeriod { get; set; } = 1f;

    [Signal]
    public delegate void MyTakeRawDamageEventHandler(float pRawDamage);

    FactionEnum mFaction;
    LiteTimer mGraceTimer;
}
