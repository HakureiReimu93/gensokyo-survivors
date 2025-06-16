using System;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;
using GodotUtilities;

[UseAutowiring]
public partial class GensokyoSurvivorsSession : Node
{
	public override void _Ready()
	{
		__PerformDependencyInjection();

		// always have a blocking ui layer on hand.
		mBlockingUILayer = new Node();
		AddChild(mBlockingUILayer);

		// This layer will pause all nodes underneath it to run ui
		// But this effect should always run.
		mBlockingUILayer.ProcessMode = ProcessModeEnum.Always;

		GodotStrict.Helpers.Prelude._Initialize(GetTree());

		mSession = this;
	}

	public override void _Process(double delta)
	{
		GodotStrict.Helpers.Prelude._Process(delta);
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}
	}

	private void DecrementBlockCounter()
	{
		SafeGuard.EnsureNotNull(MyMainSceneRoot);
		SafeGuard.Ensure(mBlockCounter > 0);
		mBlockCounter = Math.Max(0, mBlockCounter - 1);
		if (mBlockCounter == 0)
		{
			MyMainSceneRoot.ProcessMode = ProcessModeEnum.Pausable;
		}
	}

	private void IncrementBlockCounter()
	{
		SafeGuard.EnsureNotNull(MyMainSceneRoot);
		mBlockCounter += 1;

		// ERROR: Disabling a CollisionObject node during a physics callback is not allowed and will cause undesired behavior. Disable with call_deferred() instead.
		Callable.From(() => MyMainSceneRoot.ProcessMode = ProcessModeEnum.Disabled).CallDeferred();
	}

	/// <summary>
	/// Pause the game and present a UI. 
	/// Make sure to unblock the game by calling pUnblock() ONCE.
	/// </summary>
	/// <param name="ui"></param>
	/// <param name="pUnblock">Call this ONCE when the UI queue frees itself</param>
	public T HostBlockingUIFromPacked<T>(PackedScene pUiPacked, out Action pUnblock)
	where T : CanvasLayer
	{
		var instantiated = pUiPacked.InstantiateOrNull<T>();
		SafeGuard.EnsureNotNull(instantiated);

		mBlockingUILayer.AddChild(instantiated);

		IncrementBlockCounter();
		pUnblock = DecrementBlockCounter;

		return instantiated;
	}

	/// <summary>
	/// Pause the game and present a UI. 
	/// Make sure to unblock the game by calling pUnblock() ONCE.
	/// </summary>
	/// <param name="ui"></param>
	/// <param name="pUnblock">Call this ONCE when the UI queue frees itself</param>
	public void HostBlockingUI<T>(CanvasLayer pUi, out Action pUnblock)
	{
		SafeGuard.EnsureNotNull(pUi);

		mBlockingUILayer.AddChild(pUi);

		IncrementBlockCounter();
		pUnblock = DecrementBlockCounter;
	}


	public Node2D MyMainSceneRoot { get; set; }
	public static GensokyoSurvivorsSession Instance
	{
		get
		{
			return mSession;
		}
	}
	private static GensokyoSurvivorsSession mSession;
	private int mBlockCounter = 0;
	private Node mBlockingUILayer;
}
