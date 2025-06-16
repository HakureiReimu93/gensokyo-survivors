using GensokyoSurvivors.Core.Interface;
using Godot;
using GodotStrict.Traits;
using GodotStrict.Types;
using GodotUtilities;
using System;

using Adventure = System.Collections.Generic.IEnumerator<GodotStrict.Types.Coroutine.BaseSoon>;
using static GodotStrict.Types.Coroutine.AdventureExtensions;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types.Traits;
using GodotStrict.Helpers.Logging;
using GodotStrict.Helpers;

[UseAutowiring]
public partial class AxeAbilityUnit : Node2D, IPhysicalSkill
{
    [Autowired("id-player")]
    Scanner<LInfo2D> mPlayerRef;

    [Autowired]
    AnimationPlayer mAnim;

    [Autowired]
    HitBox mHitBox;

    [Autowired("ReleaseOnDie")]
    Option<Node2D> mEmitOnDie;

    [Autowired("id-effect-layer")]
    Scanner<LMother> mEffectLayer;

    public override void _Ready()
    {
        __PerformDependencyInjection();
 
        if (mPlayerRef.Available(out var player))
        {
            mInitialAngle = GlobalPosition.AngleToPoint(player.GlobalPosition);
            mInitialDistance = GlobalPosition.DistanceTo(player.GlobalPosition);
        }

        Visible = false;
        this.StartAdventure(DoSwingMotion);
    }

    public override void _Process(double delta)
    {
        if (mRequestedDie.WasTriggered()) return;
        if (!mCompletedIntro.WasTriggered()) return;

        if (!mPlayerRef.Available(out var player))
        {
            this.LogWarn("Could not obtain player ref; dying on purpose.");
            Die();
            return;
        }

        if (mTrackTimer.Tick(delta))
        {
            Die();
            return;
        }

        var parameter = 1 - (mTrackTimer.TimeLeft / mTrackTimer.WaitTime);
        var radAngleAdd = Mathf.Lerp(0, Calculate.Pi360, parameter);
        var distanceAdd = Mathf.Lerp(0, MyRange, parameter);

        var destVector = player.GlobalPosition
                            + Vector2.Left.Rotated(radAngleAdd + mInitialAngle) * (distanceAdd + mInitialDistance);

        GlobalPosition = GlobalPosition.MoveToward(destVector, (float)delta * 300f);

    }

    private void Die()
    {
        if (mRequestedDie.Ever()) return;

        this.StartAdventure(DoDieMotion);
    }

    private Adventure DoDieMotion()
    {
        mHitBox.QueueFree();
        yield return WaitTillAnimationFinish(mAnim, "die");

        if (mEffectLayer.Available(out var layer) &&
            mEmitOnDie.Available(out var emitOnDie))
        {
            RemoveChild(emitOnDie);
            layer.TryHost(emitOnDie);

            emitOnDie.GlobalPosition = GlobalPosition;
        }

        QueueFree();
    }

    private Adventure DoSwingMotion()
    {
        Visible = true;
        Callable.From(() => mHitBox.Monitorable = false).CallDeferred();
        yield return WaitTillAnimationFinish(mAnim, "spawn");
        Callable.From(() => mHitBox.Monitorable = true).CallDeferred();
        yield return WaitTillAnimationFinish(mAnim, "spin");

        mAnim.Play("idle");
        mTrackTimer = new LiteTimer(MyDuration);
        mCompletedIntro.SetTrue();
    }

    public void OnEnemyHit(MobUnit enemy) { }

    public void OnEnemyChosen(MobUnit enemy) { }

    [Export]
    public int MyRange { get; set; }

    [Export(PropertyHint.Range, "1,6")]
    public float MyDuration { get; set; } = 1;

    private float mInitialDistance;
    private float mInitialAngle;

    private LiteTimer mTrackTimer;
    private TriggerFlag mCompletedIntro;
    private TriggerFlag mRequestedDie;
}
