using Godot;
using GodotUtilities;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/script.png")]
public partial class AccelDecel : Node
{	
	public override void _Ready()
	{
		依赖注入();
	}
}
