using Godot;
using GodotUtilities;
using GensokyoSurvivors.Src.Library;
using GodotStrict.Helpers.Guard;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/hitbox.png")]
public partial class HitBox : Area2D
{
	[Autowired]
	CollisionShape2D mShape;

	public override void _Ready()
	{
		依赖注入();
		SafeGuard.Ensure(MyFaction != Faction.None, "pick a faction for the hitbox to target");
		SafeGuard.Ensure(MyDamage > 0);

		CollisionLayer = MyFaction switch
		{
			Faction.None => 0b0000_0000,
			Faction.Ally => 0b0000_0100,
			Faction.Enemy => 0b0000_1000,
			Faction.Both => 0b0000_0100 | 0b0000_1000,
			_ => throw new System.NotImplementedException(),
		};
		CollisionMask = 0;

		// HitBoxes don't monitor - HurtBoxes monitor hitboxes.
		Monitoring = false;
	}

	public void ConsiderHurtOtherHurtBox(HurtBox pVictim, out UnitBuf[] bufs)
	{
		bufs = new UnitBuf[MyApplyOnHitBufs.Length];
		for (int i = 0; i < bufs.Length; i++)
		{
			bufs[i] = (UnitBuf)MyApplyOnHitBufs[i].Duplicate();
		}
	}

	[Export]
	public Faction MyFaction { get; set; }

	[Export]
	public UnitBuf[] MyApplyOnHitBufs { get; set; }

	[Export]
	public float MyDamage { get; set; }
}
