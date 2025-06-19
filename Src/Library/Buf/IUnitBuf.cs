using System;
using Godot;

namespace GensokyoSurvivors.Source.Library.Common;

/// <summary>
/// A buf is similar to a middleware but is always introduced by another force.
/// It has free reign over the entity it is attached to.
/// However, it should not rely on an entity's base values for processing because this will create feedback.
/// 
/// Bufs should be reversible: no multiply or dividing attributes - only add + subtract.
/// If multiplying/dividing is necessary, implement a Scale() method.
/// In all, Bufs exist to permanently alter their subjects.
/// </summary>
public interface IUnitBuf
{
    public BufStackType StackType { get; }
    public StringName Identifier { get; }
    /// <summary>
    /// Initialize the buf with a reference to the UnitModel it affects, and an action that causes the collection to remove the buf.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="pInvalidateAction"></param>
    public void ParamInit(UnitModel unit, Godot.Collections.Array<UnitBuf> parent);
    public float MovementScale();
    public float DamageScale();
    public void OnRequestedDestroy();
    public void Process(double delta);
}
