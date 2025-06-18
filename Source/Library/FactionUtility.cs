using System;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Generic;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;

namespace GensokyoSurvivors.Source.Library;

public static class FactionUtility
{
    public static void Configure(IEnumerable<KeyValuePair<string, uint>> pLayerBitValues)
    {
        SafeGuard.Ensure(mConfigured.Never());

        foreach (var layer in pLayerBitValues)
        {
            Dictionary<Faction, uint> targetDic;
            Faction targetFaction;

            if (layer.Key.EndsWith("player")) targetFaction = Faction.Ally;
            else if (layer.Key.EndsWith("enemy")) targetFaction = Faction.Enemy;
            else continue;

            if (layer.Key.StartsWith("hard")) targetDic = factionHardCollisionBits;
            else if (layer.Key.StartsWith("grudge")) targetDic = factionGrudgeBits;
            else if (layer.Key.StartsWith("item")) targetDic = factionItemInteractBits;
            else continue;

            targetDic.Add(targetFaction, layer.Value);
        }
    }

    /// <summary>
    /// Re-assigns the faction (if inherited) to equal the faction of the most immediate parent with a set faction.
    /// </summary>
    /// <param name="pMember"></param>
    /// <param name="pNode"></param>
    public static void ResolveFaction(in Node pNode, ref Faction current)
    {
        SafeGuard.Ensure(mConfigured);

        if (current is Faction.Inherit)
        {
            Node parent = pNode.GetParent();
            if (parent == null)
            {
                SafeGuard.Fail($"parent of node {pNode.Name} is null");
                return;
            }

            while (parent is not IFactionUnit fm || fm.MyFaction == Faction.Inherit)
            {
                parent = parent.GetParent();
                if (parent is null)
                {
                    SafeGuard.Fail($"No parents of {pNode.Name} have a set faction (all are inherit)");
                    return;
                }
            }
            current = (pNode.Owner as IFactionUnit).MyFaction;
        }
    }

    public static void SetHardListenerFor(CollisionObject2D pObject, Faction pFaction)
    {
        SafeGuard.Ensure(mConfigured);
        SafeGuard.Ensure(pFaction != Faction.Inherit, "make sure to resolve the faction first.");

        if (pFaction == Faction.Both)
        {
            pObject.CollisionMask = factionHardCollisionBits[Faction.Enemy] | factionHardCollisionBits[Faction.Ally];
        }
        else
        {
            pObject.CollisionMask = factionHardCollisionBits[pFaction];
        }
    }

    public static void SetHardIdentityFor(CollisionObject2D pObject, Faction pFaction)
    {
        SafeGuard.Ensure(mConfigured);
        SafeGuard.Ensure(pFaction != Faction.Inherit, "make sure to resolve the faction first.");

        if (pFaction == Faction.Both)
        {
            pObject.CollisionLayer = factionHardCollisionBits[Faction.Enemy] | factionHardCollisionBits[Faction.Ally];
        }
        else
        {
            pObject.CollisionLayer = factionHardCollisionBits[pFaction];
        }
    }

    public static void SetGrudgeListenerFor(CollisionObject2D pObject, Faction pFaction)
    {
        SafeGuard.Ensure(mConfigured);
        SafeGuard.Ensure(pFaction != Faction.Inherit, "make sure to resolve the faction first.");

        if (pFaction == Faction.Both)
        {
            pObject.CollisionMask = factionGrudgeBits[Faction.Enemy] | factionGrudgeBits[Faction.Ally];
        }
        else
        {
            pObject.CollisionMask = factionGrudgeBits[pFaction];
        }
    }

    public static void ClearIdentityAndListener(CollisionObject2D pObject)
    {
        pObject.CollisionLayer = 0;
        pObject.CollisionMask = 0;
    }

    public static void SetGrudgeIdentityFor(CollisionObject2D pObject, Faction pFaction)
    {
        SafeGuard.Ensure(mConfigured);
        SafeGuard.Ensure(pFaction != Faction.Inherit, "make sure to resolve the faction first.");

        if (pFaction == Faction.Both)
        {
            pObject.CollisionLayer = factionGrudgeBits[Faction.Enemy] | factionGrudgeBits[Faction.Ally];
        }
        else
        {
            pObject.CollisionLayer = factionGrudgeBits[pFaction];
        }
    }

    public static void SetPickupListenerFor(CollisionObject2D pObject, Faction pFaction)
    {
        SafeGuard.Ensure(mConfigured);
        SafeGuard.Ensure(pFaction != Faction.Inherit, "make sure to resolve the faction first.");

        if (pFaction == Faction.Both)
        {
            pObject.CollisionMask = factionItemInteractBits[Faction.Enemy] | factionItemInteractBits[Faction.Ally];
        }
        else
        {
            pObject.CollisionMask = factionItemInteractBits[pFaction];
        }
    }

    public static void SetPickupIdentityFor(CollisionObject2D pObject, Faction pFaction)
    {
        SafeGuard.Ensure(mConfigured);
        SafeGuard.Ensure(pFaction != Faction.Inherit, "make sure to resolve the faction first.");

        if (pFaction == Faction.Both)
        {
            pObject.CollisionLayer = factionItemInteractBits[Faction.Enemy] | factionItemInteractBits[Faction.Ally];
        }
        else
        {
            pObject.CollisionLayer = factionItemInteractBits[pFaction];
        }
    }

    public static Faction Opposing(Faction pFaction)
    {
        SafeGuard.Ensure(pFaction != Faction.Inherit);
        if (pFaction == Faction.Ally) return Faction.Enemy;
        if (pFaction == Faction.Enemy) return Faction.Ally;
        if (pFaction == Faction.Both) return Faction.Both;
        throw new NotImplementedException("Unrecognized faction: " + pFaction);
    }

    private static TriggerFlag mConfigured;
    private static readonly Dictionary<Faction, uint> factionGrudgeBits = new();
    private static readonly Dictionary<Faction, uint> factionHardCollisionBits = new();
    private static readonly Dictionary<Faction, uint> factionItemInteractBits = new();
}
