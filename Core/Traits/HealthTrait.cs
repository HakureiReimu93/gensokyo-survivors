using Godot;
using System;

public partial class HealthTrait : Node
{
	[Signal]
	public delegate void LostAllHPEventHandler();

	[Export(PropertyHint.Range,"1,50")]
	int @MyHealth
	{
		get
		{
			return Convert.ToInt32(mMaxHealth);
		}
		set
		{
			mMaxHealth = value;
			mHealth = Mathf.Min(mMaxHealth,	mHealth);
		}
	}


	float mHealth;
	float mMaxHealth;
    private bool mDead;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		mHealth = mMaxHealth;
	}

	public void DoLoseHP(float loseHowMuchHP)
	{
		if (mDead)
		{
			return;
		}

		mHealth -= loseHowMuchHP;
		if (mHealth < 0)
		{
			mDead = true;
			EmitSignal(SignalName.LostAllHP);
		}
	}
}
