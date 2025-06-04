using Godot;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Utility;

[GlobalClass]
[Icon("res://Assets//GodotEditor/Icons/hurtbox.png")]
public partial class HurtBox : Area2D
{
    public override void _Ready()
    {
        SafeGuard.EnsureIsConstType<Node2D>(Owner);
        SafeGuard.Ensure(CollisionLayer == 0, "Do not set the collision layer!");
        AreaEntered += HandleAreaEntered;
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
