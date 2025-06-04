using System;
using System.Collections.Generic;
using System.Linq;
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

    public float ProductAll(Func<UnitBuf, float> selectorFunction)
    {
        return mData.Aggregate(1f,
            (curScale, curBuf) => curScale * selectorFunction(curBuf)
        );
    }

    public float SumAll(Func<UnitBuf, float> selectorFunction)
    {
        return mData.Aggregate(1f,
            (curScale, curBuf) => curScale + selectorFunction(curBuf)
        );
    }

    public bool RemoveSpecificUnitBuf(UnitBuf pBuf)
    {
        return mData.Remove(pBuf);
    }
}
