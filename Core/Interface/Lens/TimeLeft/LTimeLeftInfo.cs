using GensokyoSurvivors.Core.Model;
using Godot;
using GodotStrict.Traits;
using GodotStrict.Types;

namespace GensokyoSurvivors.Core.Interface.Lens;

public interface LTimeLeftInfo: ILens<Node>
{
    public Option<TimeBundle> GetTime();
}
