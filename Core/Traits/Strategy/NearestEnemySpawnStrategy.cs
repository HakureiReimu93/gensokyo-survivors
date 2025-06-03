using Godot;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Interface;
using System.Collections.Generic;
using GodotStrict.Types;
using System.Linq;
using System.Collections;
using System;
using GodotStrict.Helpers;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class NearestEnemySpawnStrategy : Node, IAbilitySpawnStrategy
{
	#region exports
	[Export(PropertyHint.Range, "0,500")]
	public float MyMaxRange { get; set; }
	#endregion

	private Option<Vector2> mPrimaryOrigin = default;
	private IEnumerable<Vector2> mEnemyOrigins = [];

	public Option<Vector2> ComputeSpawnOrigin()
	{
		if (mPrimaryOrigin.Available(out Vector2 playerOrigin) is false) return default;
		
		var sortedEnemies = mEnemyOrigins
			.ToList()
			.Where(enemyPos => enemyPos.DistanceSquaredTo(playerOrigin) <= MyMaxRange * MyMaxRange)
			.OrderBy(enemyPos => enemyPos.DistanceSquaredTo(playerOrigin));

		if (sortedEnemies.Any())
		{
			return Option<Vector2>.Ok(sortedEnemies.First());
		}
		else
		{
			return Option<Vector2>.None;
		}
	}

	public IAbilitySpawnStrategy WithAntagonistPositions(IEnumerable<Vector2> origins)
	{
		mEnemyOrigins = origins;
		return this;
	}

	public IAbilitySpawnStrategy WithProtagonistPosition(Vector2 origin)
	{
		mPrimaryOrigin = origin;
		return this;
	}

	public override void _Ready()
	{
		SafeGuard.Ensure(MyMaxRange != 0, "Set the max range of this spawn strategy.");
		SafeGuard.EnsureIsConstType<IAbilitySpawner>(GetParent());
	}
}
