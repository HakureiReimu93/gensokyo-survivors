using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;

namespace GensokyoSurvivors.Core.Model;

public class BufCollection
{
    private readonly List<UnitBuf> mData = [];

    public void Add(UnitBuf pBuf)
    {
        mData.Add(pBuf);
    }

    public bool TryAddUnique(UnitBuf pBuf)
    {
        if (mData.Any(buf => buf.GetType() == pBuf.GetType())) return false;

        mData.Add(pBuf);

        return true;
    }

    public bool RemoveUnitBuf<T>()
    {
        return mData.RemoveAll(
            buf => buf is T
        ) > 0;
    }

    public void ProcessAll()
    {
        RemoveExpired();

        foreach (var buf in mData)
        {
            
        }
    }

    public float ProductAll(Func<UnitBuf, float> selectorFunction)
    {
        RemoveExpired();

        return mData.Aggregate(1f,
            (curScale, curBuf) => curScale * selectorFunction(curBuf)
        );
    }

    public Color ColorMultiplyAll(Func<UnitBuf, Color> selectorFunction)
    {
        RemoveExpired();

        return mData.Aggregate(Colors.White,
            (curScale, curBuf) => curScale * selectorFunction(curBuf)
        );
    }

    private void RemoveExpired()
    {
        foreach (var buf in mData)
        {
            if (buf.IsExpired())
            {
                buf.QueueFree();
            }
        }

        mData.RemoveAll((buf) => buf.IsExpired());
    }

    public float SumAll(Func<UnitBuf, float> selectorFunction)
    {
        RemoveExpired();

        return mData.Aggregate(1f,
                (curScale, curBuf) => curScale + selectorFunction(curBuf)
            );
    }

    public bool RemoveSpecificUnitBuf(UnitBuf pBuf)
    {
        return mData.Remove(pBuf);
    }
}
