# Levels

[Devlog](DEVLOG.md)

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
and entities of it. Its assembly is `3DEngine.Levels`, and its types stand in the namespace
`Levels`. The module opens no window and no graphics device, reads no file but through an
interface, and declares two interfaces for what it needs of an engine.

| Interface | What the engine answers |
|---|---|
| `ILevelFiles` | Whether a file stands at a relative path with forward slashes, and its bytes, which a level and its pictures are read through |
| `ILevelMaterials` | How many texels a material's picture spans each way, or nothing where it has none |

`LevelPictures` is an `ILevelMaterials` over an `ILevelFiles`. It finds a material's picture as a
PNG or a JPEG of its name under `textures/`, and gives a name under `proto/` a prototype's picture
of 512 texels, which the engine draws by the slot of the rest of the name. An engine that names its
pictures another way implements `ILevelMaterials` itself.

The types a binding reads, each in the namespace `Levels`, are these.

| Type | What it is |
|---|---|
| `Level` | A level file, read by `Level.Parse`, or by `Level.Load` from a path or through an `ILevelFiles`, and written by `Level.Write` or `Level.Save`, with its entities, handed to the engine one by one by `Level.ReadEntities` with a fault naming the entity, and `Level.UnitsPerMeter`, and `Level.Doorway`, a wall's brushes round a rounded opening |
| `LevelEntity` | An entity, its keyvalues in order and its brushes, read by `Placement`, `Light`, `Tree`, `Number`, `WholeNumber`, `Point` and `KeysText`, and called by `Name` |
| `LevelBrush` | A brush, a convex solid of its faces' planes, its polygons from `Polygons` and its shapes from `Box`, `Wedge`, `Prism` of any convex outline, `Slab` of a ground outline, `Arch`, `Wall` and `Ramp` between two points, `Along`, `Turned` and `Moved`, and how far a ray meets it from `Hit` |
| `LevelFace` | A face, its plane through three points, its material and its mapping of two axes, offsets, a rotation and two scales |
| `LevelPoint` | A point or a way in the file's units with Z up, turned into meters by `Meters`, with `Dot`, `Cross` and `Length` |
| `LevelPlacement` | Where an entity stands in meters and how it is turned, a `Vector3` and a `Quaternion` |
| `LevelGeometry` | The brushes as meshes (`Meshes`), as convex hulls (`Hulls`) and as one shape of triangles (`Triangles`), how far a ray first meets any of a set of brushes (`Hit`), and a picture's size from its header (`PictureSize`) |
| `LevelMesh` | One material's triangles, their positions, normals, texture coordinates and indices |
| `LevelHull` | One brush's corners in meters, a convex hull for a body |
| `LevelTriangles` | An entity's triangles as one shape for a static body |
| `LevelPicture` | A material's picture as `LevelPictures` finds it, its path and its size in texels |
| `LevelLight` | A light in physical units, its kind, place, way, color, strength, range and cones |
| `LevelLightKind` | A light's kind, `Point`, `Spot` or `Directional` |
| `LevelTree` | A tree, the name of its species and its seed |

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

The smallest binding reads a level from a folder through its files, draws each brush entity's meshes and makes them
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
        var files = new FolderFiles(root);
        var level = Level.Load(files, path);
        var units = level.UnitsPerMeter;
        var pictures = new LevelPictures(files);
        level.ReadEntities((index, entity) =>
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
        });
    }
}
```

The tests' host, `Levels.Tests/Host/LevelBinding.cs`, is this code, and the tests run it against a
level and a folder of pictures with no engine, so the build and the suite hold the binding to the
module. BevyCSharp and 3DEngine take the module as two examples of an engine, and BevyCSharp's
binding is `BevyCSharp/Levels/LevelSpawner.cs` in its repository.

## Building and testing

The tests take the checks every module keeps from the Tools module, a git submodule under
`modules/Tools`, so a clone fetches it with `git clone --recurse-submodules`, or with
`git submodule update --init` in a clone made without it. With the .NET 10 SDK:

```bash
dotnet build Levels.slnx
dotnet test Levels.Tests
```

## License

The Mozilla Public License 2.0, in [LICENSE](LICENSE).
