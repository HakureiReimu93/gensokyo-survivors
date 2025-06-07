using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/manager.png")]
public partial class SessionSignalBus : Node
{
    [Signal]
    public delegate void SessionTimeExpiredEventHandler();

    public void BroadcastSessionTimeExpired()
    {
        EmitSignal(SignalName.SessionTimeExpired);
    }

    public static Option<SessionSignalBus> SingletonInstance
    {
        get
        {
            if (mSingletonInstance is null)
            {
                return Option<SessionSignalBus>.None;
            }
            else
            {
                return Option<SessionSignalBus>.Ok(mSingletonInstance);
            }
        }
    }
    private static SessionSignalBus mSingletonInstance;

    public override void _EnterTree()
    {
        SafeGuard.Ensure(mSingletonInstance is null, $"cannot have 2 or more of singleton {nameof(SessionSignalBus)}");
        mSingletonInstance = this;
    }
}
