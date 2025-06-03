using System;

namespace GensokyoSurvivors.Core.Utility;

public static class FactionUtil
{
  private const UInt32 AllyLayerID = 0b0000_1000;
  private const UInt32 EnemyLayerID = 0b0001_0000;

  public static UInt32 LayerFromFaction(FactionEnum factionValue)
  {
    return factionValue switch
    {
      FactionEnum.NONE => 0b0000_0000,
      FactionEnum.ENEMY => EnemyLayerID,
      FactionEnum.FRIEND => AllyLayerID,
      FactionEnum.BOTH => EnemyLayerID | AllyLayerID,
      _ => throw new NotImplementedException(),
    };
  }
    
    public static UInt32 MaskFromFaction(FactionEnum factionValue)
    {
        return factionValue switch
        {
          FactionEnum.NONE => 0b0000_0000,
          FactionEnum.ENEMY => AllyLayerID,
          FactionEnum.FRIEND => EnemyLayerID,
          FactionEnum.BOTH => EnemyLayerID | AllyLayerID,
          _ => throw new NotImplementedException(),
        };
    }
}
