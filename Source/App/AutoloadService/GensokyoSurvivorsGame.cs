using GensokyoSurvivors.Source.Library;
using Godot;
using GodotStrict.Helpers;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class GensokyoSurvivorsGame : Node
{
    public override void _Ready()
    {
        Prelude._Initialize(GetTree());

        Dictionary<string, uint> dic = new();

        for (int i = 1; i <= 32; i++)
        {
            string layerName = ProjectSettings.GetSetting($"layer_names/2d_physics/layer_{i}").AsString();
            if (string.IsNullOrEmpty(layerName))
            {
                break;
            }
            dic.Add(layerName, 1u << (i - 1));
        }

        FactionUtility.Configure(dic);
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsKeyPressed(Key.Escape))
        {
            GetTree().Quit();
        }
    }

    public override void _Process(double delta)
    {
        Prelude._Process(delta);
    }
}
