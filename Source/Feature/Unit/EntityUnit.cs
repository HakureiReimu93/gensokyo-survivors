using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using Adventure = System.Collections.Generic.IEnumerator<GodotStrict.Types.Coroutine.BaseSoon>;
using static GodotStrict.Types.Coroutine.AdventureExtensions;
using GodotUtilities;
using GodotStrict.Traits;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/script.png")]
public partial class EntityUnit : CharacterBody2D, LInfo2D
{
	public override void _Ready()
	{
		__PerformDependencyInjection();
	}
	
	public Node2D Entity => this;
}
