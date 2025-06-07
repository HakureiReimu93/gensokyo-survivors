using GensokyoSurvivors.Core.Model;
using Godot;
using GodotStrict.Traits;

namespace GensokyoSurvivors.Core.Interface.Lens;

public interface LTimeLeftChannel : ILens<Node>
{
    public void ReceiveTime(TimeBundle pInput);
}
