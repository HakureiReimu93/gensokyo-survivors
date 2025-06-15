using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Interface.Lens;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/bundle.png")]
public partial class UpgradeSelectUnitUI : PanelContainer
{
	public BaseButton ConfirmButton => MyButton;

	[Export]
	Label MyNameLabel { get; set; }

	[Export]
	Label MyDescriptionLabel { get; set; }

	[Export]
	Button MyButton { get; set; }

	public void ConnectToOnButtonClick(Action continuation)
	{
		MyButton.Pressed += () => continuation();
	}

	public void Hydrate(UpgradeMetaData md)
	{
		MyNameLabel.Text = md.MyDisplayName;
		MyDescriptionLabel.Text = md.MyDescription;
	}
}
