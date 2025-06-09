using System;
using System.Collections.Generic;
using System.Linq;
using GensokyoSurvivors.Core.Interface;
using GensokyoSurvivors.Core.Traits.Visuals.UnitVisual;
using GensokyoSurvivors.Core.Utility;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Helpers.Logging;
using GodotStrict.Types;
using GodotStrict.Types.Traits;
using GodotUtilities;

/// <summary>
/// Annotate a Visual for a Unit with lifetime.
/// Supports an enter and exit animation.
/// enter and exit animations cannot be interrupted.
/// </summary>
[GlobalClass]
[UseAutowiring]
[Icon("res://Assets/GodotEditor/Icons/visual.png")]
public partial class UnitMonoVisual : Node2D, IKillable
{
	[Autowired("UnitMonoAnim")]
	AnimationPlayer mAnim;

	[Autowired("ReleaseOnDie")]
	Option<Node2D> mOnDieVisual;

	[Autowired("id-effect-layer")]
	Scanner<LMother> mEffectLayerRef;

	public override void _Ready()
	{
		__PerformDependencyInjection();

		SafeGuard.EnsureNotNull(MyAnimNamespace);

		// access namespaced anims with generic names
		// mResolvedAnimName = mAnim.GetAnimationList()
		// 	.ToDictionary(
		// 		animName => animName.Replace($"{MyAnimNamespace}/", ""), // value
		// 		animName => new StringName(animName) // new key
		// 	);

		if (mOnDieVisual.Available(out var visual))
		{
			visual.Hide();
		}

		mAnim.AnimationFinished += HandleAnimationFinished;
	}

	/// <summary>
	/// Expensive, try not to call too many times!
	/// </summary>
	/// <param name="pWhichAnimations"></param>
	/// <returns></returns>
	public bool AnimLibraryHasAnimations(IEnumerable<string> pWhichAnimations)
	{
		var animList = mAnim.GetAnimationList();
		// In other words, no extra missing animations allowed.
		return animList
			.Select(str => str.Replace($"{MyAnimNamespace}/", ""))
			.Union(pWhichAnimations)
			.Count() == animList.Length;

	}

	public UnitMonoVisual DoRegisterFallbackAnim(string pAnimKey)
	{
		var trueAnimName = $"{MyAnimNamespace}/{pAnimKey}";
		var animData = new UnitAnimMetaData(trueAnimName, UnitAnimPlayType.LoopFallback);
		SafeGuard.Ensure(mAnim.HasAnimation(trueAnimName), $"reqiured animation {trueAnimName} missing");
		SafeGuard.EnsureFalse(mAnim.GetAnimation(trueAnimName)?.LoopMode == Animation.LoopModeEnum.None, $"animation '{pAnimKey}' does not loop");
		SafeGuard.Ensure(mFallbackAnimData.IsNone, "only one fallback animation can be specified");
		SafeGuard.Ensure(mRegisteredAnims.TryAdd(pAnimKey, animData), $"'{pAnimKey}' has already been registered with me!");

		mFallbackAnimData = animData;

		return this;
	}

	public UnitMonoVisual DoRegisterLoopingAnim(string pAnimKey)
	{
		var trueAnimName = $"{MyAnimNamespace}/{pAnimKey}";
		var animData = new UnitAnimMetaData(trueAnimName, UnitAnimPlayType.LoopCanCancel);
		SafeGuard.Ensure(mAnim.HasAnimation(trueAnimName), $"reqiured animation {trueAnimName} missing");
		SafeGuard.EnsureFalse(mAnim.GetAnimation(trueAnimName)?.LoopMode == Animation.LoopModeEnum.None, $"animation '{pAnimKey}' does not loop");
		SafeGuard.Ensure(mRegisteredAnims.TryAdd(pAnimKey, animData), $"'{pAnimKey}' has already been registered with me!");

		mFallbackAnimData = animData;
		return this;
	}

	public UnitMonoVisual DoRegisterOneShotLockedAnim(string pAnimKey)
	{
		var trueAnimName = $"{MyAnimNamespace}/{pAnimKey}";
		var animData = new UnitAnimMetaData(trueAnimName, UnitAnimPlayType.SoloCanNotCancel);
		SafeGuard.Ensure(mAnim.HasAnimation(trueAnimName), $"reqiured animation {trueAnimName} missing");
		SafeGuard.Ensure(mAnim.GetAnimation(trueAnimName)?.LoopMode == Animation.LoopModeEnum.None, $"animation '{pAnimKey}' should not loop");
		SafeGuard.Ensure(mRegisteredAnims.TryAdd(pAnimKey, animData), $"'{pAnimKey}' has already been registered with me!");

		mFallbackAnimData = animData;
		return this;
	}

	public UnitMonoVisual DoRegisterFinalAnim(string pAnimKey)
	{
		var trueAnimName = $"{MyAnimNamespace}/{pAnimKey}";
		var animData = new UnitAnimMetaData(trueAnimName, UnitAnimPlayType.SoloCanNotCancel);
		SafeGuard.Ensure(mAnim.HasAnimation(trueAnimName), $"reqiured animation {trueAnimName} missing");
		SafeGuard.Ensure(mAnim.GetAnimation(trueAnimName)?.LoopMode == Animation.LoopModeEnum.None, $"animation '{pAnimKey}' should not loop");
		SafeGuard.Ensure(mFinalAnimData.IsNone, "only one final animation can be specified");
		SafeGuard.Ensure(mRegisteredAnims.TryAdd(pAnimKey, animData), $"'{pAnimKey}' has already been registered with me!");

		mFinalAnimData = animData;
		return this;
	}

	public UnitMonoVisual DoRegisterOneShotAnim(string pAnimKey)
	{
		var trueAnimName = $"{MyAnimNamespace}/{pAnimKey}";
		var animData = new UnitAnimMetaData(trueAnimName, UnitAnimPlayType.OneShotCanCancel);
		SafeGuard.Ensure(mAnim.HasAnimation(trueAnimName), $"reqiured animation {trueAnimName} missing");
		SafeGuard.Ensure(mAnim.GetAnimation(trueAnimName)?.LoopMode == Animation.LoopModeEnum.None, $"animation '{pAnimKey}' should not loop");
		SafeGuard.Ensure(mRegisteredAnims.TryAdd(pAnimKey, animData), $"'{pAnimKey}' has already been registered with me!");

		mFallbackAnimData = animData;
		return this;
	}

	public Outcome TryPlayAnimation(string pRequestKey)
	{
		if (!mRegisteredAnims.ContainsKey(pRequestKey)) return Outcome.EmptyQuery;
		var requestedAnim = mRegisteredAnims[pRequestKey];

		if (mCurrentAnimData.Unavailable(out var currentAnim))
		{
			mCurrentAnimData = requestedAnim;
			mAnim.Play(requestedAnim.AnimName);
			return 0;
		}
		if (currentAnim != requestedAnim)
		{
			if (currentAnim.PlayType == UnitAnimPlayType.SoloCanNotCancel) return Outcome.Busy;
			mCurrentAnimData = requestedAnim;
			mAnim.Play(requestedAnim.AnimName);
		}

		return 0;
	}

	public Outcome TryPlayAnimationAndAwaitCompletion(string pRequestKey, out AnimSoon result)
	{
		result = default;

		if (!mRegisteredAnims.TryGetValue(pRequestKey, out var requestedAnim)) return Outcome.EmptyQuery;

		// Only non-looping animations can be completed (for now).
		SafeGuard.Ensure(requestedAnim.PlayType != UnitAnimPlayType.LoopFallback &&
						 requestedAnim.PlayType != UnitAnimPlayType.LoopCanCancel, "Can only watch for completion if the animation doesn't loop.");

		if (mCurrentAnimData.Unavailable(out var currentAnim))
		{
			mCurrentAnimData = requestedAnim;
			mAnim.Play(requestedAnim.AnimName);
		}
		else if (currentAnim != requestedAnim)
		{
			if (currentAnim.PlayType == UnitAnimPlayType.SoloCanNotCancel) return Outcome.Busy;
			mCurrentAnimData = requestedAnim;
			mAnim.Play(requestedAnim.AnimName);
		}

		result = new AnimSoon(mAnim, requestedAnim.AnimName);
		return 0;
	}

	public void PlayAnimationForcely(string pRequestKey)
	{
		mCurrentAnimData = mRegisteredAnims[pRequestKey];
		mAnim.Play(mCurrentAnimData.Value.AnimName);
	}

	private void HandleAnimationFinished(StringName animName)
	{
		SafeGuard.Ensure(mFallbackAnimData.IsSome, "No animation to fall back to");

		if (mFinalAnimData.Available(out var finalAnim) &&
			animName == finalAnim.AnimName)
		{
			_Die();
		}

		mAnim.Play(mFallbackAnimData.Value.AnimName);
	}

	public Option<StringName> GetCurrentAnimationName()
	{
		if (mCurrentAnimData.IsNone)
		{
			return Option<StringName>.None;
		}
		else
		{
			return mCurrentAnimData.Value.AnimName;
		}
	}

	private void _Die()
	{
		SafeGuard.Ensure(mDidFinalize.Never());

		// emit death visuals
		if (mEffectLayerRef.Available(out var effectLayer))
		{
			if (mOnDieVisual.Available(out var visual))
			{
				RemoveChild(visual);
				effectLayer.TryHost(visual);
			}
			else
			{
				this.LogWarn("No die visuals found.");
			}
		}
		else
		{
			this.LogWarn("No effect layer to host death visuals on");
		}
		QueueFree();
	}

	public void TriggerDie()
	{
		if (mFinalAnimData.IsSome)
		{
			mCurrentAnimData = mFinalAnimData;
			mAnim.Play(mCurrentAnimData.Value.AnimName);
		}
		else
		{
			_Die();
		}
	}

	public void TriggerDieForcely()
	{
		_Die();
	}

	[Export]
	StringName MyAnimNamespace { get; set; }
	Dictionary<string, UnitAnimMetaData> mRegisteredAnims = [];

	public bool IsFacingLeft
	{
		get
		{
			return mIsFacingLeft;
		}
		set
		{
			mIsFacingLeft = value;
			if (mIsFacingLeft)
			{
				Scale = new Vector2(-1, 1);
			}
			else
			{
				Scale = new Vector2(1, 1);
			}
		}
	}

	public bool IsDead => mDidFinalize || mCurrentAnimData == mFinalAnimData;

	private bool mIsFacingLeft;
	private TriggerFlag mDidFinalize;

	Option<UnitAnimMetaData> mFallbackAnimData;
	Option<UnitAnimMetaData> mFinalAnimData;
	Option<UnitAnimMetaData> mCurrentAnimData;
}
