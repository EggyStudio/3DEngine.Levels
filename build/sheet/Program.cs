using System.Numerics;
using System.Text.Json;
using Levels;

// Reads a level as a binding does, through Flattened and ReadEntities, and writes what the module
// gives of it as JSON, each mesh's triangles in meters with Y up about the level's origin with its
// material, each light, each other entity's class, name and place, and the lux the lights give the
// floor.
//
//   dotnet run -c Release --project build/sheet -- <level.map> <out.json>
var level = Level.Load(args[0]).Flattened();
var units = level.UnitsPerMeter;
var meshes = new List<object>();
var lights = new List<object>();
var entities = new List<object>();
level.ReadEntities((index, entity) =>
{
    var placed = entity.Placement(units);
    foreach (var mesh in LevelGeometry.Meshes(entity, units, placed.Position))
    {
        var at = mesh.Positions.Select(p => p + placed.Position).ToArray();
        meshes.Add(new
        {
            material = mesh.Material,
            triangles = Enumerable.Range(0, mesh.Indices.Length / 3).Select(t => new[]
            {
                Point(at[mesh.Indices[3 * t]]), Point(at[mesh.Indices[3 * t + 1]]), Point(at[mesh.Indices[3 * t + 2]]),
            }).ToArray(),
            normals = Enumerable.Range(0, mesh.Indices.Length / 3).Select(t => Point(mesh.Normals[mesh.Indices[3 * t]])).ToArray(),
        });
    }

    if (entity.Light(units) is { } light)
        lights.Add(new { kind = light.Kind.ToString(), at = Point(light.Position), way = Point(light.Direction), range = light.Range, inner = light.InnerCone, outer = light.OuterCone, lumens = light.Intensity });
    else if (entity.Brushes.Count == 0)
        entities.Add(new { className = entity.ClassName, name = entity.Name(index), at = Point(placed.Position), facing = Point(Vector3.Transform(-Vector3.UnitZ, placed.Rotation)) });
});

// The lux the room's lights, the sun apart, give the floor's top at its height, every tenth of a
// meter over the level's extent, each light's IlluminanceAt at its distance and angle times the
// cosine of its way onto a surface facing up.
var corners = meshes.SelectMany(m => (float[][][])m.GetType().GetProperty("triangles")!.GetValue(m)!).SelectMany(t => t).ToList();
var (x0, x1) = (corners.Min(c => c[0]), corners.Max(c => c[0]));
var (z0, z1) = (corners.Min(c => c[2]), corners.Max(c => c[2]));
const float Step = 0.1f;
var (nx, nz) = ((int)MathF.Ceiling((x1 - x0) / Step), (int)MathF.Ceiling((z1 - z0) / Step));
var lux = new double[nz][];
var lit = new List<LevelLight>();
level.ReadEntities((_, entity) => { if (entity.Light(units) is { Kind: not LevelLightKind.Directional } light) lit.Add(light); });
for (var k = 0; k < nz; k++)
{
    lux[k] = new double[nx];
    for (var i = 0; i < nx; i++)
    {
        var at = new Vector3(x0 + (i + 0.5f) * Step, 0, z0 + (k + 0.5f) * Step);
        foreach (var light in lit)
        {
            var to = at - light.Position;
            var meters = to.Length();
            var degrees = Math.Acos(Math.Clamp(Vector3.Dot(Vector3.Normalize(to), light.Direction), -1, 1)) * 180 / Math.PI;
            var onto = Math.Max(0, -to.Y / meters);
            lux[k][i] += light.IlluminanceAt(meters, degrees) * onto;
        }
    }
}

var floor = new { x0, z0, step = Step, nx, nz, lux };

// Each prototype's tone in sRGB from 0 to 1, from the Materials module, by the material name the
// level writes for it.
var tones = Materials.Prototypes.Names.ToDictionary(name => "proto/" + name, name => Point(Materials.Prototypes.SrgbColor(name)));
File.WriteAllText(args[1], JsonSerializer.Serialize(new { meshes, lights, entities, floor, tones }));
Console.WriteLine($"{meshes.Count} meshes, {meshes.Sum(m => ((Array)m.GetType().GetProperty("triangles")!.GetValue(m)!).Length)} triangles, {lights.Count} lights, {entities.Count} entities");

static float[] Point(Vector3 v) => [v.X, v.Y, v.Z];
