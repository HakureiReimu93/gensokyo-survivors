using Godot;
using GensokyoSurvivors.Source.Library;
using GodotStrict.Helpers.Guard;

[GlobalClass]
// [UseAutowiring]
[Icon("res://GodotEditor/Icons/hitbox.png")]
public partial class HitBox : Area2D, IFactionMember
{

	public override void _Ready()
	{
		// 依赖注入();
		SafeGuard.Ensure(MyDamage != 0);
	}

	public virtual void OnHitOther(HurtBox pHurtbox) {}

	[Export]
	public float MyDamage { get; set; }

	[Export]
	public DamageTimes MyDamageTimes { get; set; }

	public Faction MyFaction
	{
		get
		{
			return mFaction;
		}

		set
		{
			mFaction = value;
			FactionUtility.ClearIdentityAndListener(this);
			FactionUtility.ResolveFaction(this, ref mFaction);
			FactionUtility.SetGrudgeIdentityFor(this, mFaction);
		}
	}
	private Faction mFaction;

	public enum DamageTimes
	{
		Unlimit,
		OnlyOne
	}
}
