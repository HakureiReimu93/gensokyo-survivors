using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;

namespace GensokyoSurvivors.Source.Library;

public static class FactionUtility
{
    private static TriggerFlag mConfigured;

    public static void Configure(
        IEnumerable<(uint, Faction)> pFactionLayerBits,
        IEnumerable<(uint, Faction)> pFactionMaskBits
        )
    {
        SafeGuard.Ensure(mConfigured.Never());

        FactionLayerBits = FactionLayerBits.ToFrozenDictionary(
            (x) => x.Key, (x) => x.Value
        );
        FactionMaskBits = FactionMaskBits.ToFrozenDictionary(
            (x) => x.Key, (x) => x.Value
        );
    }

    /// <summary>
    /// Re-assigns the faction (if inherited) to equal the faction of the most immediate parent with a set faction.
    /// </summary>
    /// <param name="pMember"></param>
    /// <param name="pNode"></param>
    public static void ResolveFaction(IFactionMember pMember, in Node pNode)
    {
        SafeGuard.Ensure(mConfigured);
        if (pMember.MyFaction is Faction.Inherit)
        {
            Node parent = pNode.GetParent();
            SafeGuard.EnsureNotNull(parent);

            while (parent is not IFactionMember fm || fm.MyFaction == Faction.Inherit)
            {
                parent = parent.GetParent();
                if (parent is null)
                {
                    SafeGuard.Fail($"No parents of {pNode.Name} have a set faction (all are inherit)");
                }
            }
            pMember.MyFaction = (pNode.Owner as IFactionMember).MyFaction;
        }
    }

    public static void MakeMonitor(Faction pFaction, CollisionObject2D pCollObject)
    {
        SafeGuard.Ensure(mConfigured);
        SafeGuard.Ensure(pFaction != Faction.Inherit, "call ResolveFaction() first.");

        pCollObject.CollisionMask = FactionMaskBits[pFaction];
    }

    public static void MakeMonitorableBy(Faction pFaction, CollisionObject2D pCollObject)
    {
        SafeGuard.Ensure(mConfigured);
        SafeGuard.Ensure(pFaction != Faction.Inherit, "call ResolveFaction() first.");

        pCollObject.CollisionLayer = FactionLayerBits[pFaction];
    }

    public static Faction Opposing(Faction pFaction)
    {
        SafeGuard.Ensure(pFaction != Faction.Inherit);
        if (pFaction == Faction.Ally) return Faction.Enemy;
        if (pFaction == Faction.Enemy) return Faction.Ally;
        if (pFaction == Faction.Both) return Faction.Both;
        throw new NotImplementedException("Unrecognized faction: " + pFaction);
    }

    public static FrozenDictionary<Faction, uint> FactionLayerBits { get; private set; }
    public static FrozenDictionary<Faction, uint> FactionMaskBits { get; private set; }
}
