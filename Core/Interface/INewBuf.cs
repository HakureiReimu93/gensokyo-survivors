using System;

namespace GensokyoSurvivors.Core.Interface;

public interface INewBufDispatcher
{
    public void SendNewUpgrade(UnitBuf mBuf);
}

public interface INewBufSubject
{
    public void ConsiderNewBuf(UnitBuf mBuf);
}