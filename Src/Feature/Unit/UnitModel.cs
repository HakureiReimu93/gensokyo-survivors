using System.Collections.Generic;
using System.Linq;
using GensokyoSurvivors.Source.Library.Common;
using GensokyoSurvivors.Src.Library;
using GensokyoSurvivors.Src.Library.Midware;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Traits;
using GodotUtilities;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/unit.png")]
public partial class UnitModel : CharacterBody2D, LInfo2D
{

	public override void _Ready()
	{
		依赖注入();

		SafeGuard.Ensure(MyMovementSpeed > 0);
		MyBufs ??= [];
		MyMiddleware ??= [];

		foreach (var buf in MyBufs)
		{
			buf.ParamInit(this, MyBufs);
		}
	}

	public override void _Process(double delta)
	{
		MyModulateTemp = Colors.White;
		MyTempMovementSpeed = MyMovementSpeed;
		foreach (var buf in MyBufs)
		{
			buf.Process(delta);
			buf.VisualProcess(delta);
		}
		foreach (var mware in MyMiddleware)
		{
			mware.Process(this, delta);
		}
		Velocity = MyPlannedMoveDirection * MyTempMovementSpeed;
		Modulate = MyModulateTemp;
		MoveAndSlide();
	}

	public StringName TryAddUnitBuf(UnitBuf buf)
	{
		if (buf.StackType == BufStackType.OnlyOne &&
			MyBufs.Any((x) => x.Identifier == buf.Identifier))
		{
			return CommonResult.Duplicate;
		}
		MyBufs.Add(buf);

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

	public Node2D Entity => this;
}

