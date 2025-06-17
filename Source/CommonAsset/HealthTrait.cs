using Godot;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using static GodotStrict.Types.Coroutine.AdventureExtensions;
using Adventure = System.Collections.Generic.IEnumerator<GodotStrict.Types.Coroutine.BaseSoon>;
using GodotStrict.Traits;
using GodotUtilities;
using GodotStrict.Helpers.Guard;

[GlobalClass]
// [UseAutowiring]
[Icon("res://GodotEditor/Icons/output.png")]
public partial class HealthTrait : Node
{
	[Signal]
	public delegate void MyHealthChangedEventHandler(float old, float newValue, float maxHealth);

	[Signal]
	public delegate void MyDiedEventHandler();

	public override void _Ready()
	{
		SafeGuard.Ensure(mMaximum > 0);
		mHealth = mMaximum;
		// 依赖注入();
	}

	[Export]
	public int MyMaximumHealth
	{
		get
		{
			return mMaximum;
		}

		set
		{
			SafeGuard.Ensure(mHealth != 0, "This health trait is dead");
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
