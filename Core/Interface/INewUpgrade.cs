using System;
using System.Collections.ObjectModel;
using GensokyoSurvivors.Core.Utility;

namespace GensokyoSurvivors.Core.Interface;

public interface INewUpgradeDispatcher
{
    public void SendNewUpgrade(UpgradeMetaData pMeta, ReadOnlyDictionary<UpgradeMetaData, uint> allUpgrades);
}

public interface INewUpgradeSubject
{
    public void ConsiderNewUpgrade(UpgradeMetaData pMeta, ReadOnlyDictionary<UpgradeMetaData, uint> allUpgrades);
}
