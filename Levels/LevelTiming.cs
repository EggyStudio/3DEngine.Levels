namespace Levels;

/// <summary>
/// How fast a door or a button moves and how long it waits before it returns, as
/// <see cref="LevelEntity.Timing"/> reads them.
/// </summary>
/// <param name="MetersPerSecond">How fast it moves along its travel, in meters a second.</param>
/// <param name="Wait">How many seconds it waits open before it returns, below 0 to stay.</param>
public readonly record struct LevelTiming(double MetersPerSecond, double Wait)
{
    /// <summary>Whether it stays where it moved to and never returns.</summary>
    public bool Stays => Wait < 0;
}
