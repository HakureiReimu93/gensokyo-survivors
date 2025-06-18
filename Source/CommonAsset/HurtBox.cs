using Godot;
using GodotUtilities;
using GensokyoSurvivors.Source.Library;
using GodotStrict.Types;
using GodotStrict.Helpers.Guard;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/script.png")]
public partial class HurtBox : Area2D, IFactionMember
{
	[Signal]
	public delegate void MyHurtEventHandler(float pDamage);

	public override void _Ready()
	{
		依赖注入();

		SafeGuard.Ensure(MyCooldown > 0);

		SafeGuard.Ensure(Owner is not null);

		mUnMonitorCallable = Callable.From(__DangerouslyTriggerUnMonitor);

		AreaEntered += HandleAreaEnter;
	}

	private void HandleAreaEnter(Area2D other)
	{
		SafeGuard.EnsureCorrectType<HitBox>(other, out HitBox hb);
		SafeGuard.Ensure(hb.MyFaction == FactionUtility.Opposing(MyFaction));

		EmitSignal(SignalName.MyHurt, hb.MyDamage);

		hb.OnHitOther(this);

		mUnMonitorCallable.CallDeferred();
		mCooldownTimer.Reset();
	}

	public override void _Process(double delta)
	{
		if (mCooldownTimer.Tick(delta))
		{
			Monitoring = true;
		}
	}

	private void __DangerouslyTriggerUnMonitor()
	{
		Monitoring = false;
	}

	[Export(PropertyHint.Range, "0.1,8")]
	public float MyCooldown
	{
		get
		{
			return mCooldown;
		}
		set
		{
			mCooldown = value;
			mCooldownTimer.ResetWithCustomTime(value);
		}
	}
	private float mCooldown;

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
			FactionUtility.SetGrudgeListenerFor(this, FactionUtility.Opposing(mFaction));
		}
	}
	private Faction mFaction;

	private LiteTimer mCooldownTimer;

	private Callable mUnMonitorCallable;
}
