using Godot;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/manager.png")]
public partial class SessionSignalBus : Node
{
    [Signal]
    public delegate void SessionTimeExpiredEventHandler();

    public void BroadcastSessionTimeExpired()
    {
    }
}
