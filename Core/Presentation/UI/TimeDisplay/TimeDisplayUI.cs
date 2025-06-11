using GensokyoSurvivors.Core.Interface.Lens;
using GensokyoSurvivors.Core.Model;
using GensokyoSurvivors.Core.Presentation.UI.TimeDisplay;
using Godot;
using GodotStrict.AliasTypes;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GodotStrict.Traits;
using GodotStrict.Types;
using GodotStrict.Types.PrevCurrent;
using GodotUtilities;
using System;

namespace GensokyoSurvivors.Core.Presentation;

[UseAutowiring]
public partial class TimeDisplayUI : CanvasLayer, ILensProvider<LTimeLeftChannel>
{
	[Autowired]
	Option<AnimationPlayer> mAnimation;

	public override void _Ready()
	{
		__PerformDependencyInjection();


		SafeGuard.EnsureNotNull(MyTimeLabel);
		SafeGuard.EnsureNotNull(MyMillisLabel);

		if (SessionSignalBus.SingletonInstance.Available(out var ssb))
		{
			ssb.SessionTimeExpired += HandleSessionTimeExpired;
		}

		MyTimeLabel.Text = "...";
		MyMillisLabel.Text = "";

		if (mAnimation.Available(out var anim))
		{
			SafeGuard.Ensure(anim.HasAnimation("pop-in"));
			SafeGuard.Ensure(anim.HasAnimation("pop-out"));
		}
	}

	protected void HandleSessionTimeExpired()
	{
		if (mAnimation.Available(out var anim))
		{
			anim.Play("pop-out");
		}
	}

	protected static TimeDisplayUrgency CalculateUrgency(TimeBundle pTime)
	{
		// 5, 4, 3, 2, 1
		if (pTime.TimeLeft <= 6f)
		{
			return TimeDisplayUrgency.Critical;
		}
		if (pTime.PercentTimeLeft() < 0.2f)
		{
			return TimeDisplayUrgency.Mild;
		}

		return TimeDisplayUrgency.Normal;
	}

	protected void UpdateTime(TimeBundle pInput)
	{
		if (mAnimation && mEverUpdatedTime.Never())
		{
			mAnimation.Value.Play("pop-in");
		}

		int minutes = Convert.ToInt32(pInput.TimeLeft / 60);
		int seconds = Convert.ToInt32(pInput.TimeLeft % 60);
		int millis  = Convert.ToInt32((pInput.TimeLeft - seconds) * Math.Pow(10, 3));
		var secondsFraction = Calculate.Fract(pInput.TimeLeft);

		if (mCurrentSeconds.NewValueNotEqualsPrevious(seconds))
		{
			var urgency = CalculateUrgency(pInput);
			mTextColorBase = urgency switch
			{
				TimeDisplayUrgency.Normal => Colors.White,
				TimeDisplayUrgency.Mild => MyMildColor,
				TimeDisplayUrgency.Critical => MyCriticalColor,
				_ => throw new NotImplementedException(),
			};
			// animate time down.
			if (mAnimation.Available(out var anim) &&
				urgency != TimeDisplayUrgency.Normal)
			{
				anim.Play("pulse-value");
			}
		}

		// Ease in curve
		MyTimeLabel.Modulate = mTextColorBase.Lerp(
			Colors.White,
			Calculate.PhiEasing((normal)0.8f, secondsFraction));

		MyTimeLabel.Text = $"{minutes:D2}:{seconds:D2}";
		MyMillisLabel.Text = $"{millis:D3}";
	}

	[Export]
	Label MyTimeLabel;

	[Export]
	Label MyMillisLabel;

	[ExportCategory("Theme")]
	[Export]
	Color MyMildColor { get; set; } = Colors.Yellow;

	[Export]
	Color MyCriticalColor { get; set; } = Colors.Red;

	PrevCurrentValue<int> mCurrentSeconds;
	TriggerFlag mEverUpdatedTime;
	Color mTextColorBase = Colors.White;

	#region lens
	private class TimeLeftChannel(TimeDisplayUI en) : LTimeLeftChannel
	{
		Node ILens<Node>.Entity => en;
		public void ReceiveTime(TimeBundle pInput) => en.UpdateTime(pInput);
	}
	public TimeDisplayUI()
	{
		mLens = new(this);
	}
	private TimeLeftChannel mLens;
	public LTimeLeftChannel Lens => mLens;
	#endregion
}
