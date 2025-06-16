using Godot;
using System;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/bundle.png")]
public partial class UpgradeMetaData : Resource
{
    [Export]
    public StringName MyID { get; set; }

    [Export]
    public string MyDisplayName { get; set; }

    [Export(PropertyHint.MultilineText)]
    public string MyDescription { get; set; }

    [Export]
    public SkillStackType MyStackType { get; set; }

    [Export]
    public PackedScene MyRewardScene { get; set; }
}

public enum SkillStackType
{
    Unlimit,
    OnlyOne
}
