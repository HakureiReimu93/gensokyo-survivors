using Godot;
using GodotStrict.Traits;
using GodotStrict.Types.Traits;
using System.Diagnostics.CodeAnalysis;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class EffectLayer : Node2D, LMother
{

	public override void _Ready()
	{
		ChildEnteredTree += OnChildEnteredTree;
	}

	private void OnChildEnteredTree([NotNull] Node pWhat)
	{
		// sometimes effects are hidden until needed.
		if (pWhat is CanvasItem pWhatAsCanvasItem)
		{
			pWhatAsCanvasItem.Show();
		}

		if (pWhat is CpuParticles2D)
		{
			OnAdoptNewParticleEffect(pWhat as CpuParticles2D);
		}
	}

	private void OnAdoptNewParticleEffect([NotNull] CpuParticles2D particles)
	{
		particles.SetEmitting(true);
	}

	Node ILens<Node>.Entity => this;
}
