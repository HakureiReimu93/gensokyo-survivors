using System;
using Godot;

namespace GensokyoSurvivors.Core.Utility.Adventure;

public interface ISoon : IAwaiter
{
    public bool IsRejected { get; }
    public bool IsInProgress => !IsCompleted && !IsRejected;
    public void OnRejected(Action failure);
    public void Finally(Action final);
}
