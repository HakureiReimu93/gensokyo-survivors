using System;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Helpers.Logging;
using GodotStrict.Types;
using GodotUtilities;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/health.png")]
public partial class Health : Node
{
	[Signal]
	public delegate void MyHealthChangedEventHandler(float pOld, float pNew);

	public override void _Ready()
	{
		依赖注入();
		SafeGuard.Ensure(MyMaxHealth > 0);
	}

	[Export(PropertyHint.Range, "0,100")]
	public int MyMaxHealth
	{
		get
		{
			return myMaxHealth;
		}

		set
		{
			myMaxHealth = value;
			if (value > mHealth)
			{
				MyHealth = value;
			}
		}
	}

	public float MyHealth
	{
		get
		{
			return mHealth;
		}
		set
		{
			if (value != mHealth)
			{
				SafeGuard.Ensure(mHealth != 0, "Tried to set HP, but I am dead!");
				var oldValue = mHealth;
				mHealth = Mathf.Max(0, value);
				EmitSignal(SignalName.MyHealthChanged, oldValue, mHealth);
			}
		}
	}

	public bool IsDead => mHealth <= 0;
	private int myMaxHealth;
	private float mHealth = 1;
}
