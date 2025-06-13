using Godot;
using GensokyoSurvivors.Core.Utility;
using GensokyoSurvivors.Core.Interface;
using GensokyoSurvivors.Core.Utility.Adventure;
using System;
using GodotStrict.Types;
using GodotUtilities;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
[UseAutowiring]
public partial class EnterBox : Area2D, IFactionMember
{
	[Autowired]
	CollisionShape2D mCollisionShape;

	[Signal]
	public delegate void EnterBoxEnteredEventHandler(HitBox pHitBox);

	public override void _Ready()
	{
		__PerformDependencyInjection();

		CollisionMask = FactionUtil.PickUpMaskFromFaction(MyFaction);

		AreaEntered += HandleAreaEntered;
	}

	public void SetEnabled(bool pEnabled)
	{
		mCollisionShape.Disabled = !pEnabled;
	}

	private void HandleAreaEntered(Area2D other)
	{
		if (other is HitBox otherHitBox &&
			FactionUtil.DoFactionsOppose(otherHitBox.MyFaction, MyFaction))
		{
			EmitSignal(SignalName.EnterBoxEntered, otherHitBox);
		}
	}

	public void DoRespondToBoxEntered(Action<HitBox> with)
	{
		EnterBoxEntered += (hb) => with(hb);
	}

	[Export]
	public FactionEnum MyFaction { get; set; }

}