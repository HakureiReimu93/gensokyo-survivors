using Godot;
using GensokyoSurvivors.Source.Library.Common;
using GodotStrict.Helpers.Guard;

[GlobalClass]
[Icon("res://GodotEditor/Icons/buf.png")]
public abstract partial class UnitBuf : Resource, IUnitBuf
{
    public abstract BufStackType StackType { get; }
    public abstract StringName Identifier { get; }
    protected abstract void _Process(double delta);
    protected abstract void _VisualProcess(double delta);

    public void VisualProcess(double delta)
    {
        if (valid)
        {
            _VisualProcess(delta);
        }
    }

    public void Process(double delta)
    {
        if (valid)
        {
            _Process(delta);
        }
    }

    public virtual float DamageScale()
    {
        return 1;
    }

    public virtual float MovementScale()
    {
        return 1f;
    }

    public virtual void ParamInit(UnitModel unit, Godot.Collections.Array<UnitBuf> parent)
    {
        mParent = parent;
        SafeGuard.EnsureNotNull(unit);
        mUnit = unit;
    }

    public virtual void OnRequestedDestroy()
    {
    }

    protected void Invalidate()
    {
        valid = false;
        mParent.Remove(this);
    }

    protected UnitModel mUnit;
    private bool valid = true;
    protected Godot.Collections.Array<UnitBuf> mParent;
}
