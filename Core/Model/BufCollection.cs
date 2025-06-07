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
    private bool mDead = false;

    public void Add(UnitBuf pBuf)
    {
        SafeGuard.EnsureFalse(mDead);
        mData.Add(pBuf);
    }

    public bool TryAddUnique(UnitBuf pBuf)
    {
        SafeGuard.EnsureFalse(mDead);

        if (mData.Any(buf => buf.GetType() == pBuf.GetType())) return false;

        mData.Add(pBuf);

        return true;
    }

    public bool RemoveUnitBuf<T>()
    {
        SafeGuard.EnsureFalse(mDead);

        return mData.RemoveAll(
            buf => buf is T
        ) > 0;
    }

    public void ProcessAll(double pDelta)
    {
        SafeGuard.EnsureFalse(mDead);

        RemoveExpired();

        foreach (var buf in mData)
        {
            buf.OnUnitProcess(pDelta);
        }
    }

    public float ProductAll(Func<UnitBuf, float> selectorFunction)
    {
        SafeGuard.EnsureFalse(mDead);

        RemoveExpired();

        return mData.Aggregate(1f,
            (curScale, curBuf) => curScale * selectorFunction(curBuf)
        );
    }

    public Color ColorMultiplyAll()
    {
        SafeGuard.EnsureFalse(mDead);

        RemoveExpired();

        return mData.Aggregate(Colors.White,
            (curColor, curBuf) => curColor * curBuf.GetModulateColor()
        );
    }

    public void OnDieAll()
    {
        SafeGuard.EnsureFalse(mDead);
        
        foreach (var buf in mData)
        {
            buf.OnUnitDied();
        }

        // remove all unit bufs without calling their remove effects
        // to avoid hacky design decisions

        mData.Clear();
        mDead = true;
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
        SafeGuard.EnsureFalse(mDead);

        RemoveExpired();

        return mData.Aggregate(1f,
                (curScale, curBuf) => curScale + selectorFunction(curBuf)
            );
    }

    public bool RemoveSpecificUnitBuf(UnitBuf pBuf)
    {
        SafeGuard.EnsureFalse(mDead);

        return mData.Remove(pBuf);
    }
}
