A made level through every reader and writer of the module at 100, 10,000 and 100,000 brushes,
a third each boxes, ramps and boxes turned by 30 degrees, every hundredth a door's, with a light,
a spot and a tree for each hundred, read back equal at each count. Each time is the best of up to
five runs within five seconds on a machine under a load of 10 to 12 at 18:10, from
`dotnet run -c Release --project build/bench`.

| Brushes | Step | ms | MiB allocated |
|---|---|---|---|
| 100 | Level.Write | 1.7 | 0.8 |
| 100 | Level.Parse | 2.9 | 1.4 |
| 100 | Level.Save | 1.7 | 0.8 |
| 100 | Level.Load | 2.8 | 1.6 |
| 100 | LevelBrush.Polygons, every brush | 3.9 | 1.7 |
| 100 | LevelGeometry.Meshes, every entity | 4.8 | 2.0 |
| 100 | LevelGeometry.Hulls, every entity | 5.1 | 1.9 |
| 100 | LevelGeometry.Triangles, the world | 0.1 | 0.2 |
| 100 | Placement, Light and Tree, every entity | 0.1 | 0.0 |
| 100 | KeysText and ParseKeys, every entity | 0.0 | 0.0 |
| 100 | the level | 5 entities, 567 faces, 1,056 triangles in the world, 0.06 million characters, 0.1 MiB on disk | round trip equal |
| 10,000 | Level.Write | 66.2 | 83.0 |
| 10,000 | Level.Parse | 124.4 | 140.0 |
| 10,000 | Level.Save | 91.5 | 83.0 |
| 10,000 | Level.Load | 181.7 | 163.4 |
| 10,000 | LevelBrush.Polygons, every brush | 106.0 | 168.2 |
| 10,000 | LevelGeometry.Meshes, every entity | 118.2 | 201.0 |
| 10,000 | LevelGeometry.Hulls, every entity | 80.3 | 185.5 |
| 10,000 | LevelGeometry.Triangles, the world | 3.4 | 15.6 |
| 10,000 | Placement, Light and Tree, every entity | 1.4 | 1.9 |
| 10,000 | KeysText and ParseKeys, every entity | 0.5 | 1.2 |
| 10,000 | the level | 401 entities, 56,667 faces, 105,600 triangles in the world, 6.10 million characters, 5.8 MiB on disk | round trip equal |
| 100,000 | Level.Write | 1024.4 | 841.6 |
| 100,000 | Level.Parse | 1369.3 | 1408.5 |
| 100,000 | Level.Save | 743.6 | 841.6 |
| 100,000 | Level.Load | 1217.7 | 1650.1 |
| 100,000 | LevelBrush.Polygons, every brush | 561.9 | 1681.8 |
| 100,000 | LevelGeometry.Meshes, every entity | 649.3 | 1984.1 |
| 100,000 | LevelGeometry.Hulls, every entity | 682.0 | 1854.3 |
| 100,000 | LevelGeometry.Triangles, the world | 21.6 | 148.3 |
| 100,000 | Placement, Light and Tree, every entity | 11.2 | 19.0 |
| 100,000 | KeysText and ParseKeys, every entity | 3.1 | 11.9 |
| 100,000 | the level | 4,001 entities, 566,667 faces, 1,056,000 triangles in the world, 63.17 million characters, 60.2 MiB on disk | round trip equal |

One brush of many faces, a prism of n sides and its two ends, its polygons alone: 18 faces in 0.0
ms; 34 faces in 0.1 ms; 66 faces in 0.7 ms; 130 faces in 6.0 ms. The process's peak working set is
1,337 MiB.

At 100,000 brushes Level.Parse is the first step slow enough to see, 1,369 ms and 1,409 MiB
allocated for 63.17 million characters, some 23 bytes a character, a token's record and its text a
token, and Level.Load, which reads the file and parses it, takes 1,218 ms and 1,650 MiB. Every
brush's Polygons comes next in memory, 1,682 MiB, some 17 KiB a brush.
