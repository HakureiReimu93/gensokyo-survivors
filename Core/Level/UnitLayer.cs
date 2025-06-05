using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GodotStrict.Traits;
using GodotStrict.Types.Traits;
using GodotStrict.Traits.EmptyImpl;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class UnitLayer : Node2D, ILensProvider<LMother>
{
	public override void _Ready()
	{
		base._Ready();
	}

	public LMother Lens => lens;
	private readonly BaseImplMother<Node2D> lens;
	public UnitLayer()
	{
		lens = new(this);
	}
}
