namespace Levels;

public sealed partial class LevelEntity
{
    // Every key the module's readers take that does not read, each with the refusal its reader
    // gives, in the order of the entity's keys, so a level's check names them all at once.
    internal IEnumerable<(string Key, string Message)> KeyProblems()
    {
        var light = ClassName is LevelLight.PointClass or LevelLight.SpotClass or LevelLight.DirectionalClass;
        var problems = new List<(int At, string Key, string Message)>();
        void Try(string key, Action read)
        {
            var at = Keys.FindIndex(pair => pair.Key == key);
            if (at < 0) return;
            try
            {
                read();
            }
            catch (FormatException fault)
            {
                problems.Add((at, key, fault.Message));
            }
        }

        Try("origin", () => Point("origin"));
        Try("angles", () => Point("angles"));
        Try("angle", () => Number("angle"));
        if (light)
        {
            Try("_color", () => Point("_color"));
            Try("pattern", () => Pattern());
            if (this["pattern"] is not { Length: > 0 }) Try("style", () => Pattern());
            foreach (var key in new[] { "intensity", "range", "_inner_cone", "_cone" })
                Try(key, () => AtLeastZero(key, 0));
            if (ClassName == LevelLight.SpotClass)
            {
                Try("_inner_cone", () =>
                {
                    double? inner, outer;
                    try
                    {
                        (inner, outer) = (Number("_inner_cone"), Number("_cone"));
                    }
                    catch (FormatException)
                    {
                        return;
                    }

                    var (i, o) = (inner ?? LevelLight.DefaultInnerCone, outer ?? LevelLight.DefaultOuterCone);
                    if (i > o) throw new FormatException($"The {ClassName} has the _inner_cone {i} wider than its _cone {o}.");
                });
            }
        }

        if (ClassName == LevelTree.Class) Try("seed", () => WholeNumber("seed"));
        if (ClassName == "worldspawn")
        {
            Try("_ambient", () => Point("_ambient"));
            Try("_ambient_lux", () => AtLeastZero("_ambient_lux", 0));
        }
        if (ClassName is "func_door" or "func_button")
        {
            Try("lip", () => Number("lip"));
            Try("speed", () => Timing(1));
            Try("wait", () => Number("wait"));
        }
        if (ClassName == "func_rotating")
        {
            Try("speed", () => Number("speed"));
            Try("spawnflags", () => WholeNumber("spawnflags"));
        }
        for (var at = 0; at < Keys.Count; at++)
        {
            var (key, value) = Keys[at];
            var fields = value.Split(value.Contains('\u001b') ? '\u001b' : ',');
            if (fields.Length != 5) continue;
            var single = new LevelEntity();
            single.Keys.Add(new("classname", ClassName));
            single.Keys.Add(Keys[at]);
            try
            {
                single.Outputs();
            }
            catch (FormatException fault)
            {
                problems.Add((at, key, fault.Message));
            }
        }

        // One problem a key, the first its readers give, in the keys' order.
        return problems.GroupBy(problem => problem.Key).Select(group => group.First())
            .OrderBy(problem => problem.At).Select(problem => (problem.Key, problem.Message));
    }
}
