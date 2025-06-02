using Godot;
using GodotStrict.Traits.EmptyImpl;
using GodotStrict.Traits;

using GodotUtilities;
using GodotStrict.Types.Traits;

[GlobalClass]
public partial class SkillLayer : Node2D, ILensProvider<LMother>
{
	#region lens
	protected BaseImplMother<Node2D> impl;
	LMother ILensProvider<LMother>.Lens => impl;
	public SkillLayer()
	{
		impl = new(this);
	}
	#endregion


	public override void _Ready()
	{
		base._Ready();
	}
}
