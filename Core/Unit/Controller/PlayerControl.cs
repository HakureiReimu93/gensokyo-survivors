using GensokyoSurvivors.Core.Interface;
using Godot;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/brain.png")]
public partial class PlayerControl : Node, IMobUnitInput
{
	public Vector2 GetNormalMovement() =>
		Input.GetVector("move_left",
						"move_right",
						"move_up",
						"move_down");
}
