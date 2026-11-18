// The README's block from here
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
// The end marker.
