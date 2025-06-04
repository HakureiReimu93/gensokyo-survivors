using Godot;
using System;
using GodotStrict.Helpers.Guard;

[GlobalClass]
[Icon("res://Assets//GodotEditor/Icons/script.png")]
public partial class HealthTrait : Node
{
	public override void _Ready()
	{
		SafeGuard.Ensure(MyHealth > 0, "Set max health");
		mHealth = mMaxHealth;
	}

	public void TriggerDamage(float pDamage)
	{
		SafeGuard.Ensure(pDamage > 0);
		SafeGuard.Ensure(mDead == false);

		var prevHp = mHealth;
		mHealth -= pDamage;
		if (mHealth < 0)
		{
			mDead = true;
			EmitSignal(SignalName.MyHpDepleted, 0 - mHealth);
		}
		EmitSignal(SignalName.MyHpChanged, prevHp, mHealth);
	}

	public void TriggerHeal(float pHealAmount)
	{
		SafeGuard.Ensure(pHealAmount > 0);
		SafeGuard.Ensure(mDead == false);

		mHealth = Mathf.Min(mHealth + pHealAmount, mMaxHealth);
	}

	public void TriggerHealAll()
	{
		SafeGuard.Ensure(mDead == false);

		EmitSignal(SignalName.MyHpChanged, mHealth, mMaxHealth);
		mHealth = mMaxHealth;
	}

	public void TriggerDamageAll()
	{
		SafeGuard.Ensure(mDead == false);

		EmitSignal(SignalName.MyHpChanged, mHealth, 0);
		EmitSignal(SignalName.MyHpDepleted, 0);
		mHealth = 0;
	}

	[Export(PropertyHint.Range, "1,200")]
	int @MyHealth
	{
		get
		{
			return Convert.ToInt32(mMaxHealth);
		}
		set
		{
			mMaxHealth = value;
			mHealth = Mathf.Min(mMaxHealth, mHealth);
		}
	}

	[Signal]
	public delegate void MyHpDepletedEventHandler(float pRawOverflow);
	[Signal]
	public delegate void MyHpChangedEventHandler(float pOldHp, float pNewHp);

	float mHealth;
	float mMaxHealth;
	bool mDead;
	bool mOwnerBufsDamage;

}