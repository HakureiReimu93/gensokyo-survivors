using System;
using GensokyoSurvivors.Core.Utility.Adventure;
using Godot;
using GodotStrict.Types;

namespace GensokyoSurvivors.Core.Utility;

public class AnimSoon : ISoon
{
    Option<AnimationPlayer> animPlayerRef;
    StringName targetAnim;

    SoonStatus status = SoonStatus.InProgress;

    Action onComplete;
    Action onReject;
    Action onCompleteOrReject;

    public AnimSoon(AnimationPlayer _anim, StringName _targetAnim)
    {
        animPlayerRef = _anim;
        targetAnim = _targetAnim;
        _anim.AnimationChanged += HandleChange;
        _anim.AnimationFinished += HandleComplete;
    }

    private void HandleComplete(StringName animName)
    {
        if (animPlayerRef.Available(out var animPlayer) && animName == targetAnim)
        {
            status = SoonStatus.Completed;
            onComplete?.Invoke();
            onCompleteOrReject?.Invoke();
            animPlayerRef = Option<AnimationPlayer>.None;
            animPlayer.AnimationChanged -= HandleChange;
            animPlayer.AnimationFinished -= HandleComplete;
        }
    }

    private void HandleChange(StringName oldName, StringName newName)
    {
        if (animPlayerRef.Available(out var animPlayer) && newName != targetAnim)
        {
            status = SoonStatus.Rejected;
            onReject?.Invoke();
            onCompleteOrReject?.Invoke();
            animPlayerRef = Option<AnimationPlayer>.None;
            animPlayer.AnimationChanged -= HandleChange;
            animPlayer.AnimationFinished -= HandleComplete;
        }
    }

    public bool IsCompleted
    {
        get
        {
            return status == SoonStatus.Completed;
        }
    }

    public bool IsRejected
    {
        get
        {
            return status == SoonStatus.Rejected;
        }
    }

    public void GetResult()
    {
    }

    public void OnCompleted(Action continuation)
    {
        onComplete = continuation;
    }

    public void OnRejected(Action failure)
    {
        onReject = failure;
    }

  public void Finally(Action final)
  {
    throw new NotImplementedException();
  }
}
