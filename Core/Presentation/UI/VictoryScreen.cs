using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types.Coroutine;
using GodotUtilities;
using System;
using static GodotStrict.Types.Coroutine.AdventureExtensions;

using Adventure = System.Collections.Generic.IEnumerator<GodotStrict.Types.Coroutine.BaseSoon>;

[UseAutowiring]
public partial class VictoryScreen : CanvasLayer
{
    [Autowired]
    AnimationPlayer mAnim;

    public override void _Ready()
    {
        __PerformDependencyInjection();

        SafeGuard.EnsureNotNull(MyQuitButton);
        SafeGuard.EnsureNotNull(MyRestartButton);

        MyQuitButton.Disabled = true;
        MyRestartButton.Disabled = true;
    }

    public void ShowAndInvokeChosen(
        Action pQuit,
        Action pRestart,
        Action pUnblock
    )
    {
        mQuitContinuation = pQuit;
        mRestartContinuation = pRestart;
        mUnblock = pUnblock;

        this.StartAdventure(ShowAndWaitForUserChoice);
    }

    public void DescribeAsDefeat()
    {
        MyTitle.Text = "Defeat";
    }

    public void DescribeAsVictory()
    {
        MyTitle.Text = "Victory";
    }

    private Adventure ShowAndWaitForUserChoice()
    {
        yield return WaitTillAnimationFinish(mAnim, "fade-in");
        yield return WaitTillAnimationFinish(mAnim, "fly-in");

        UnionSoon<Action> buttonToContinuation = new UnionSoon<Action>();

        MyQuitButton.Disabled = false;
        MyRestartButton.Disabled = false;

        buttonToContinuation.AddSuspendedResult(
            new ButtonClickSoon(MyQuitButton), mQuitContinuation
        );
        buttonToContinuation.AddSuspendedResult(
            new ButtonClickSoon(MyRestartButton), mRestartContinuation
        );
        yield return buttonToContinuation;

        MyQuitButton.Disabled = true;
        MyRestartButton.Disabled = true;

        yield return WaitTillAnimationFinish(mAnim, "fly-out");
        yield return WaitTillAnimationFinish(mAnim, "fade-out");

        QueueFree();
        mUnblock?.Invoke();

        // invoke either quit or restart
        buttonToContinuation.Result?.Invoke();
    }

    [Export]
    Button MyQuitButton;

    [Export]
    Button MyRestartButton;

    [Export]
    Label MyTitle;
    
    private Action mQuitContinuation;
    private Action mRestartContinuation;
    private Action mUnblock;
}
