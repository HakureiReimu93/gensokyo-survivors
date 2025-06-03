using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Utility;
using System.Diagnostics.CodeAnalysis;
using GensokyoSurvivors.Core.Interface;
using GodotStrict.Types;

[GlobalClass]
[Icon("res://Assets//GodotEditor/Icons/script.png")]
public partial class HurtBox : Area2D
{
    #region exports
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
    FactionEnum mFaction;

    #endregion

    #region signals

    [Signal]
    public delegate void MyTakeRawDamageEventHandler(float pRawDamage);

    #endregion

    public override void _Ready()
    {
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
}
