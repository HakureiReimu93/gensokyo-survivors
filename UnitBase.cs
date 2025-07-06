using Godot;
using GodotStrict.Helpers.Guard;
using GodotUtilities;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/script.png")]
public partial class UnitBase : CharacterBody2D
{
	public override void _Ready()
	{
		依赖注入();
		SafeGuard.Ensure(MyMovementSpeed > 0);
	}
	
	[Export(PropertyHint.Range, "0,900")]
	public float MyMovementSpeed { get; set; }
}
