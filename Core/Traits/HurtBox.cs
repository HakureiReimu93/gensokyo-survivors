using Godot;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Utility;
using GensokyoSurvivors.Core.Interface;

[GlobalClass]
[Icon("res://Assets//GodotEditor/Icons/hurtbox.png")]
public partial class HurtBox : Area2D, IFactionMember
{
    public override void _Ready()
    {
        SafeGuard.EnsureIsConstType<IKillable>(Owner);
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
        SafeGuard.Ensure(other is HitBox);
        HitBox hitBox = other as HitBox;
        hitBox.OnCollidedWith(this);

        var damage = hitBox.MyDamageOnHit;

        if (damage > 0)
        {
            EmitSignal(SignalName.MyTakeRawDamage, hitBox.MyDamageOnHit);
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

    [Signal]
    public delegate void MyTakeRawDamageEventHandler(float pRawDamage);

    FactionEnum mFaction;
}
