using Godot;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using Adventure = System.Collections.Generic.IEnumerator<GodotStrict.Types.Coroutine.BaseSoon>;
using static GodotStrict.Types.Coroutine.AdventureExtensions;
using GodotStrict.Traits;
using GodotUtilities;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/script.png")]
public partial class MovementStyle : Node, 
{
	public override void _Ready()
	{
		__PerformDependencyInjection();
	}
	
	public void ConsiderSample(int test)
	{
	}
	
	public MovementStyle Entity => this;
}

public interface ITestChannel: ILens<MovementStyle>
{
	public void ConsiderTest(int test);
}
