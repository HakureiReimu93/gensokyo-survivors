using GensokyoSurvivors.Core.Model;
using Godot;
using GodotStrict.Helpers.Guard;


// 'Buf' is not a mispelling
// It represents a buff, debuff, status effect, marked for death, curse, blessing, etc.
// It is all-encompassing, short, snippy, and easy to read.

// Inspired by Lobotomy Corporation's codebase
// Where they mispell 'Buff' by accident
// Yet not only use it for buffs, but also all the above.
[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/buf.png")]
public partial class UnitBuf : Node
{
	public override void _Ready()
	{
		if (IsDesignToken is false)
		{
			SafeGuard.EnsureIsConstType<MobUnit>(Owner, "UnitBufs must go on top of a Unit. ()");
		}
	}

	public virtual UnitBuf DoCloneMe()
	{
		var duplicated = this.Duplicate();
		SafeGuard.EnsureIsConstType<UnitBuf>(duplicated, "cloned UnitBuf does not have a script attached!");

		UnitBuf result = duplicated as UnitBuf;
		result.IsDesignToken = false;

		return result;
	}

	public virtual void MyParamInit(MobUnit pWhat)
	{
		SafeGuard.Ensure(IsDesignToken is false);
		SafeGuard.Ensure(mHostUnit is null, "Cannot re-attach");
		mHostUnit = pWhat;
	}

	public virtual void OnUnitProcess(double deltaTime)
	{
		// run a routine every process frame.
	}

	public virtual void OnUnitDied()
	{
		// run when the unit dies.
	}

	public virtual void OnUnitRemovesMe()
	{
		// run when the unit removes this buf 
	}

	public virtual bool IsExpired()
	{
		return false;
	}

	[Export(PropertyHint.Range, "0,2")]
	public float MySpeedScale { get; set; } = 1f;

	[ExportCategory("Design")]
	[Export]
	public bool IsDesignToken { get; set; }

	MobUnit mHostUnit;
}
