using System.Collections.Generic;
using System.Linq;
using GensokyoSurvivors.Source.Library.Common;
using GensokyoSurvivors.Src.Library;
using GensokyoSurvivors.Src.Library.Buf;
using GensokyoSurvivors.Src.Library.Midware;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Helpers.Logging;
using GodotStrict.Traits;
using GodotStrict.Types;
using GodotUtilities;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/unit.png")]
public partial class UnitModel : CharacterBody2D, LInfo2D
{
	[Autowired]
	Option<HurtBox> mHurtBox;

	[Autowired]
	Option<Health> mHealth;

	[Autowired]
	Option<IUnitModelController> mController;

	public override void _Ready()
	{
		依赖注入();

		SafeGuard.Ensure(MyMovementSpeed > 0);
		MyBufs ??= [];
		MyMiddleware ??= [];

		foreach (var buf in MyBufs)
		{
			buf.ParamInit(this);
		}

		if (mHurtBox) mHurtBox.Value.MyWasHurt += HandleDamageTaken;
	}

	public void Die()
	{
		SafeGuard.Ensure(mDied.Never());

		QueueFree();
	}

	private void HandleDamageTaken(float pRawDamage, UnitBuf[] unitBufsToAdd)
	{
		if (mController) mController.Value.ConsiderDamageInfo(ref pRawDamage, unitBufsToAdd);

		if (mHealth.Available(out var hp))
		{
			hp.MyHealth -= pRawDamage;
			this.LogAny($"Hp lost {pRawDamage} hp. now health is at: {hp.MyHealth}");
		}
		else
		{
			Die();
		}

		foreach (var buf in unitBufsToAdd)
		{
			TryAddUnitBuf(buf);
		}
	}

	public override void _Process(double delta)
	{
		if (mController.IsSome)
		{
			MyPlannedMoveDirection = mController.Value.MyPlannedMovement;
		}
		else
		{
			MyPlannedMoveDirection = Vector2.Zero;
		}

		MyModulateTemp = Colors.White;
		MyTempMovementSpeed = MyMovementSpeed;

		HashSet<UnitBuf> toRemove = new();
		foreach (var buf in MyBufs)
		{
			if (buf.IsValid is false)
			{
				toRemove.Add(buf);
			}
			else
			{
				buf.Process(delta);
				buf.VisualProcess(delta);
			}
		}
		foreach (var buf in toRemove)
		{
			MyBufs.Remove(buf);
		}

		foreach (var mware in MyMiddleware)
		{
			mware.Process(this, delta);
		}
		Velocity = MyPlannedMoveDirection * MyTempMovementSpeed;
		Modulate = MyModulateTemp;
		MoveAndSlide();
	}

	/// <summary>
	/// Returns whether 
	/// </summary>
	/// <param name="pBuf"></param>
	/// <returns>CommonResult.OK if succeeded, CommonResult.Duplicate if attempted to add a 'OnlyOne' buf for which there is already one running.</returns>
	public StringName TryAddUnitBuf(UnitBuf pBuf)
	{
		if (pBuf.StackType == BufStackType.OnlyOne &&
			MyBufs.Any((x) => x.Identifier == pBuf.Identifier))
		{
			return CommonResult.Duplicate;
		}

		pBuf.ParamInit(this);

		if (pBuf.StackType == BufStackType.ReplaceExisting &&
			MyBufs.Any((x) => x.Identifier == pBuf.Identifier))
		{
			HashSet<UnitBuf> unitBufsToRemove = new();
			foreach (var activeBuf in MyBufs)
			{
				if (activeBuf.Identifier == pBuf.Identifier)
				{
					unitBufsToRemove.Add(activeBuf);
				}
			}
			foreach (var bufToRemove in unitBufsToRemove)
			{
				MyBufs.Remove(bufToRemove);
			}
		}

		MyBufs.Add(pBuf);

		return CommonResult.Ok;
	}

	[Export(PropertyHint.Range, "0,900")]
	public float MyMovementSpeed { get; set; }
	public float MyTempMovementSpeed { get; set; }

	[Export(PropertyHint.ArrayType)]
	Godot.Collections.Array<UnitBuf> MyBufs { get; set; }

	[Export(PropertyHint.ArrayType)]
	Godot.Collections.Array<UnitMidWare> MyMiddleware { get; set; }

	public Vector2 MyPlannedMoveDirection { get; set; }
	public Color MyModulateTemp { get; set; }

	TriggerFlag mDied;

	public Node2D Entity => this;
}

