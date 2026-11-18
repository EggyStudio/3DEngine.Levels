# Levels

Levels is a library both engines, 3DEngine and BevyCSharp, take as a git submodule, for the reader
of the Valve 220 map format, the meshes and collision shapes made from its brushes, and the
TrenchBroom configuration and FGD file those maps are edited with. It references no engine, and each
engine implements the interfaces it declares where it binds the module.

## Taking it into an engine

An engine adds the repository as a git submodule under `modules/` and references its project, here
from a project one folder below the engine's root.

```bash
git submodule add https://github.com/EggyStudio/Levels.git modules/Levels
```

```xml
<ProjectReference Include="..\modules\Levels\Levels\Levels.csproj" />
```

A clone of the engine fetches the module with `git clone --recurse-submodules`, or with
`git submodule update --init` in a clone made without it, and a workflow checks it out with
`submodules: true` on `actions/checkout`. A change the engine uses is committed and pushed here
before the engine's commit that moves its pointer, so the engine's history never names a commit
this repository does not have.

## Binding it in your engine

The module reads a level and gives out its data, and an engine makes its own meshes, bodies, lights
and entities of it. The module opens no window and no graphics device, reads no file but through an
interface, and declares two interfaces for what it needs of an engine.

| Interface | What the engine answers |
|---|---|
| `ILevelFiles` | Whether a file stands at a relative path with forward slashes, and its bytes |
| `ILevelMaterials` | How many texels a material's picture spans each way, or nothing where it has none |

`LevelPictures` is an `ILevelMaterials` over an `ILevelFiles`. It finds a material's picture as a
PNG or a JPEG of its name under `textures/`, and gives a name under `proto/` a prototype's picture
of 512 texels, which the engine draws by the slot of the rest of the name. An engine that names its
pictures another way implements `ILevelMaterials` itself.

What the module gives out stands on these conventions.

| Quantity | Convention |
|---|---|
| Length | Meters, the file's units over `Level.UnitsPerMeter`, 32 where `worldspawn` names none |
| Axes | Y up and right-handed, the file's (x, y, z) at (x, z, -y), an entity facing minus Z |
| Triangles | Counterclockwise seen from outside, each face's corners its own |
| Angles | Degrees, a pitch above zero looking down |
| Color | Linear sRGB from 0 to 1, decoded from the file's sRGB of 0 to 255 |
| Light | Lumens for a point over the whole sphere and for a spot over its outer cone, lux for the sun, a range in meters, a spot's cones in degrees from its axis |

An engine with Z up or left-handed axes turns the positions, normals and turns it is given, and an
engine that takes clockwise triangles as their front takes each triangle's corners in the other
order.

The smallest binding reads a level from a folder, draws each brush entity's meshes and makes them
one static body, makes each light, and hands every other entity to the engine by its class.
`IWorld` stands for the engine's own calls.

```csharp
using System.Numerics;
using Levels;

// What an engine makes of a level, each method one of the engine's own calls.
public interface IWorld
{
    // A mesh drawn with its material, by the picture found for it or the slot of its name, its
    // positions about a place in meters.
    void Draw(LevelMesh mesh, string slot, LevelPicture? picture, Vector3 at);

    // A static body of the triangles, about a place in meters.
    void Collide(LevelTriangles triangles, Vector3 at);

    // A light in physical units.
    void Light(LevelLight light);

    // An entity placed, for the engine to make what its class is, a player's start or a tree.
    void Spawn(LevelEntity entity, LevelPlacement placement);
}

// The files a level's pictures are read from, a folder on the disk.
public sealed class FolderFiles(string root) : ILevelFiles
{
    public bool Exists(string path) => File.Exists(Path.Combine(root, path));

    public byte[] ReadAllBytes(string path) => File.ReadAllBytes(Path.Combine(root, path));
}

public static class LevelBinding
{
    // Reads a level from a folder of the engine's files and makes it in the world.
    public static void Load(string root, string path, IWorld world)
    {
        var level = Level.Load(Path.Combine(root, path));
        var units = level.UnitsPerMeter;
        var pictures = new LevelPictures(new FolderFiles(root));
        foreach (var entity in level.Entities)
        {
            var placed = entity.Placement(units);
            if (entity.Brushes.Count > 0)
            {
                var meshes = LevelGeometry.Meshes(entity, units, placed.Position, pictures);
                foreach (var mesh in meshes)
                {
                    var picture = pictures.Find(mesh.Material);
                    world.Draw(mesh, LevelPictures.Slot(mesh.Material), picture, placed.Position);
                }

                world.Collide(LevelGeometry.Triangles(meshes), placed.Position);
            }

            if (entity.Light(units) is { } light) world.Light(light);
            else world.Spawn(entity, placed);
        }
    }
}
```

BevyCSharp and 3DEngine take the module as two examples of an engine, and BevyCSharp's binding is
`BevyCSharp/Levels/LevelSpawner.cs` in its repository.

## Building and testing

With the .NET 10 SDK:

```bash
dotnet build Levels.slnx
dotnet test Levels.Tests
```

## License

The Mozilla Public License 2.0, in [LICENSE](LICENSE).
