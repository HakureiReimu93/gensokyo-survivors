using System;
using GensokyoSurvivors.Core.Utility;

namespace GensokyoSurvivors.Core.Interface;

public interface IFactionMember
{
    public FactionEnum MyFaction { get; set; }
}