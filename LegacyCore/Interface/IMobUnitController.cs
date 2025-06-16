using System;
using Godot;

namespace GensokyoSurvivors.Core.Interface;

public interface IMobUnitController
{
    public Vector2 GetNormalMovement();
    public void OnControllerRequestDie(Action mEventHandler);
    public void OnUnitDie();
}
