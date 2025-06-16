using System;
using Godot;
using GodotStrict.Traits;

namespace GensokyoSurvivors.Core.Interface.Lens;

public interface LExperienceInfo : ILens<Node>
{
    public void ReceiveNewUpgrades_3(
        Tuple<UpgradeMetaData, UpgradeMetaData, UpgradeMetaData> pUpgrades_3
    );
}
