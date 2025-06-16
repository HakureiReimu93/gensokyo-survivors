using GensokyoSurvivors.Core.Model;
using GensokyoSurvivors.Core.Utility;
using Godot;
using GodotStrict.Helpers.Guard;

// 'Buf' is not a mispelling
// It represents a buff, debuff, status effect, marked for death, curse, blessing, etc.
// It is all-encompassing, short, snippy, and easy to read.

// Inspired by Lobotomy Corporation's codebase
// Where they mispell 'Buff' by accident
// Yet not only use it for buffs, but also all the above.

// Is a node, so that it can be treated as a design token
// TODO: Eventually fully treat Buf objects as nodes, syncing lifetime to presence in tree.
// Eventually making the BufCollection read its children.
 
[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/buf.png")]
public partial class UnitBuf : Node
{
	public override void _Ready()
	{
	}

	public virtual UnitBuf DoCloneMe()
	{
		var duplicated = this.Duplicate();
		SafeGuard.EnsureIsConstType<UnitBuf>(duplicated, "cloned UnitBuf does not have a script attached!");

		UnitBuf result = duplicated as UnitBuf;

		return result;
	}

	public virtual void OnUnitAddsMe(MobUnit pParent)
	{
		SafeGuard.Ensure(mHostUnit is null, "Cannot re-attach");
		mHostUnit = pParent;
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

	public virtual Color GetModulateColor()
	{
		// The tint that this buf gives.
		return Colors.White;
	}

	public virtual bool IsExpired()
	{
		return false;
	}

	public virtual float GetMovementSpeedScale()
	{
		return MyBaseMovementSpeedScale;
	}

	[Export(PropertyHint.Range, "0,2")]
	public float MyBaseMovementSpeedScale { get; set; } = 1f;

	protected virtual BufStackType MyBufStackType => BufStackType.OnlyOne;

	MobUnit mHostUnit;
}
