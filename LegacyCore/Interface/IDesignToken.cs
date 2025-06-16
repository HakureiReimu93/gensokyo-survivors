using System;
using Godot;

namespace GensokyoSurvivors.Core.Interface;

public interface IDesignToken<out T>
where T : Node
{
    /// <summary>
    /// Activates the instance, meaning it runs its intended game behaviors.
    /// </summary>
    public void Activate();
    /// <summary>
    /// Duplicates the instance and returns an inactive copy of it.
    /// </summary>
    /// <returns></returns>
    public T Duplicate();
}
