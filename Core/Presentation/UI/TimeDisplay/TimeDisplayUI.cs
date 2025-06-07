using GensokyoSurvivors.Core.Interface.Lens;
using GensokyoSurvivors.Core.Model;
using GensokyoSurvivors.Core.Presentation.UI.TimeDisplay;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Traits;
using GodotStrict.Types.PrevCurrent;
using System;

namespace GensokyoSurvivors.Core.Presentation;

public partial class TimeDisplayUI : CanvasLayer, ILensProvider<LTimeLeftChannel>
{
	public override void _Ready()
	{
		SafeGuard.EnsureNotNull(MyTimeValueLabel);

		if (SessionSignalBus.SingletonInstance.Available(out var ssb))
		{
			ssb.SessionTimeExpired += HandleSessionTimeExpired;
		}

		MyTimeValueLabel.Text = "...";
	}

	protected void HandleSessionTimeExpired()
	{
	}

	protected TimeDisplayUrgency CalculateUrgency(TimeBundle pTime)
	{
		var timeLeft = pTime.PercentTimeLeft();
		// 5, 4, 3, 2, 1
		if (pTime.TimeLeft <= 6f)
		{
			return TimeDisplayUrgency.Critical;
		}
		if (timeLeft < 0.2f)
		{
			return TimeDisplayUrgency.Mild;
		}

		return TimeDisplayUrgency.Normal;
	}

	protected void UpdateTime(TimeBundle pInput)
	{
		int minutes = Convert.ToInt32(pInput.TimeLeft / 60);
		int seconds = Convert.ToInt32(pInput.TimeLeft % 60);

		mSeconds.UpdateValue(seconds);

		var urgency = CalculateUrgency(pInput);

		if (mSeconds.CurrentNotEqualsPrevious() && urgency is not TimeDisplayUrgency.Normal)
		{
			// animate time down.
			MyTimeValueLabel.SelfModulate = urgency switch
			{
				TimeDisplayUrgency.Normal => Colors.White,
				TimeDisplayUrgency.Mild => Colors.Yellow,
				TimeDisplayUrgency.Critical => Colors.Red,
				_ => throw new NotImplementedException(),
			};

		}

		MyTimeValueLabel.Text = $"{minutes:D1}:{seconds:D2}";
	}

	[Export]
	Label MyTimeValueLabel;

	PrevCurrentValue<int> mSeconds;

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
