namespace GensokyoSurvivors.Core.Utility;

/// <summary>
/// Simply used for 'Try'-type methods that can fail in a few different ways.
/// Tries to provide common reasons for failure.
/// </summary>
public enum Outcome
{
    Fail = -1,
    Succeed = 0,
    BadArgument = 100,
    EmptyQuery,
    Busy,
    NoHandler
}
