namespace Levels;

/// <summary>
/// An output of a level's entity, an event it fires and the input it sends another entity, as
/// <see cref="LevelEntity.Outputs"/> reads it from a keyvalue.
/// </summary>
/// <param name="Name">The event, the keyvalue's key, as <c>OnOpen</c>.</param>
/// <param name="Target">The target name of the entities it sends to, which
/// <see cref="Level.Targeted"/> finds.</param>
/// <param name="Input">The input it sends, as <c>TurnOn</c>.</param>
/// <param name="Parameter">What it sends with the input, or an empty string.</param>
/// <param name="Delay">How many seconds after the event it is sent.</param>
/// <param name="Times">How many times it is sent, -1 for every time the event fires.</param>
public sealed record LevelOutput(string Name, string Target, string Input, string Parameter, double Delay, int Times);
