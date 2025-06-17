using Godot;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using static GodotStrict.Types.Coroutine.AdventureExtensions;
using Adventure = System.Collections.Generic.IEnumerator<GodotStrict.Types.Coroutine.BaseSoon>;
using GodotStrict.Traits;
using GodotUtilities;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/script.png")]
public partial class HitBox : Node
{	
	public override void _Ready()
	{
		依赖注入();
	}
}
