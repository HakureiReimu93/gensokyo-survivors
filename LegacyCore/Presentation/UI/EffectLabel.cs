using Godot;
using GodotStrict.Helpers.Guard;
using System;

public partial class EffectLabel : Node2D
{
    public override void _Ready()
    {
        SafeGuard.EnsureNotNull(mLabel);
    }

    public void ShowMe(string pText)
    {
        SafeGuard.EnsureFalse(pText == String.Empty);

        var tween = CreateTween();
        var scaleTween = CreateTween();
        
        tween.TweenProperty(this, "global_position", GlobalPosition + (Vector2.Up * 16), 0.3f)
             .SetEase(Tween.EaseType.Out)
             .SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(this, "global_position", GlobalPosition + (Vector2.Up * 48), 0.4f)
             .SetEase(Tween.EaseType.In)
             .SetTrans(Tween.TransitionType.Cubic);
        tween.Finished += QueueFree;

        scaleTween.TweenProperty(this, "scale", Vector2.One * 1.2f, 0.3f)
             .SetEase(Tween.EaseType.Out)
             .SetTrans(Tween.TransitionType.Elastic);
        scaleTween.TweenProperty(this, "scale", Vector2.Zero, 0.4f)
             .SetEase(Tween.EaseType.In)
             .SetTrans(Tween.TransitionType.Cubic);


        mLabel.Text = pText;
    }

    [Export]
    Label mLabel;
}
