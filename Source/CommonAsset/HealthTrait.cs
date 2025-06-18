using Godot;
using GodotUtilities;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/input.png")]
public partial class HealthTrait : Node
{
	[Autowired("@")]
	Option<HurtBox> mFriendHurtBox;

	public override void _Ready()
	{
		依赖注入();
		SafeGuard.Ensure(mMaximum > 0);
		mHealth = mMaximum;

		mFriendHurtBox.IfSome(then: (val) => val.MyHurt += HandleTakeDamage);
	}

	private void HandleTakeDamage(float pDamage)
	{
		SafeGuard.Ensure(mHealth > 0);
		MyHealth -= pDamage;
	}

	[Signal]
	public delegate void MyHealthChangedEventHandler(float old, float newValue, float maxHealth);

	[Signal]
	public delegate void MyDiedEventHandler();

	[Export]
	public int MyMaximumHealth
	{
		get
		{
			return mMaximum;
		}
		set
		{
			SafeGuard.Ensure(value >= 0);

			if (value < mHealth)
			{
				MyHealth = value;
			}

			mMaximum = value;
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
			SafeGuard.Ensure(mHealth != 0, "Revival not supported");
			SafeGuard.Ensure(value >= 0);

			if (mHealth != value)
			{
				EmitSignal(SignalName.MyHealthChanged, mHealth, value, mMaximum);
				mHealth = value;
				if (mHealth <= 0)
				{
					mHealth = 0;
					EmitSignal(SignalName.MyDied);
				}
			}

		}
	}
	
	private float mHealth;
	private int mMaximum;
}
