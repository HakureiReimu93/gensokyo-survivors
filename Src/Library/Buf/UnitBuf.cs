using Godot;
using GensokyoSurvivors.Source.Library.Common;
using GodotStrict.Helpers.Guard;

[GlobalClass]
[Icon("res://GodotEditor/Icons/buf.png")]
public abstract partial class UnitBuf : Resource, IUnitBuf
{
    public abstract BufStackType StackType { get; }
    public abstract StringName Identifier { get; }
    public bool IsValid { get ; set; }

    protected abstract void _Process(double delta);
    protected abstract void _VisualProcess(double delta);

    public void VisualProcess(double delta)
    {
        if (IsValid)
        {
            _VisualProcess(delta);
        }
    }

    public void Process(double delta)
    {
        if (IsValid)
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

    public virtual void ParamInit(UnitModel unit)
    {
        SafeGuard.EnsureNotNull(unit);
        IsValid = true;
        mUnit = unit;
    }

    public virtual void OnRequestedDestroy()
    {
    }

    protected UnitModel mUnit;
}
