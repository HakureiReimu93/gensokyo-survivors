using Godot;
using GodotStrict.Traits;
using GodotStrict.Types.Traits;
using GodotStrict.Traits.EmptyImpl;
using System.Diagnostics.CodeAnalysis;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class EffectLayer : Node2D, ILensProvider<LMother>
{
	public override void _Ready()
	{
		ChildEnteredTree += OnChildEnteredTree;
	}

	private void OnChildEnteredTree(Node pWhat)
	{
		if (pWhat is CpuParticles2D)
		{
			OnAdoptNewEffect(pWhat as CpuParticles2D);
		}
	}

	private void OnAdoptNewEffect([NotNull] CpuParticles2D particles)
	{
		particles.Emitting = true;
	}
	
	#region lens
	protected BaseImplMother<Node2D> lens;
	public LMother Lens => lens;
	public EffectLayer() { lens = new(this); }
	#endregion
	
}
