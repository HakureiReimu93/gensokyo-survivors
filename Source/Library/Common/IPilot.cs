using System;
using Godot;

namespace GensokyoSurvivors.Source.Library.Common;

/// <summary>
/// This pattern is used to solve the Possession problem
/// Suppose the player-- or enemy -- possesses another entity
/// Suppose the game possesses the player to have them move to a particular spot in a cutscene
/// The entity control cannot reside in the Unit class because it would be impossible to possess it.
/// Testing enemies is also easier because you can 'play' as an enemy for testing purposes if you like.
/// </summary>
public interface IPilot
{
    public EntityUnit Entity { get; }
    public Vector2 CalculateMoveDecision(double delta);
    public Faction MyFaction { get; }
}
