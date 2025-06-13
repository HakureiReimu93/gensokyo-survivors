using Godot;
using System;

public partial class UpgradeMetaData : Resource
{
    [Export]
    StringName MyID { get; set; }

    [Export]
    string DisplayName { get; set; }

    [Export(PropertyHint.MultilineText)]
    string Description{ get; set; }
}
