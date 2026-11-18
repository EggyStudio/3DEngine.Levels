The format reading again after a brush's polygons are built with no list or closure a face, the same
made level at 100, 10,000 and 100,000 brushes. Each time is the best of up to five runs within five
seconds on a machine under a load of 14 at 18:42, from `dotnet run -c Release --project
build/bench`.

| Brushes | Step | ms | MiB allocated |
|---|---|---|---|
| 100 | Level.Write | 1.3 | 0.8 |
| 100 | Level.Parse | 2.2 | 0.1 |
| 100 | the level Level.Parse keeps | | 0.1 |
| 100 | Level.Save | 1.3 | 0.8 |
| 100 | Level.Load | 2.2 | 0.4 |
| 100 | LevelBrush.Polygons, every brush | 1.3 | 0.2 |
| 100 | LevelGeometry.Meshes, every entity | 1.5 | 0.5 |
| 100 | LevelGeometry.Hulls, every entity | 1.4 | 0.2 |
| 100 | LevelGeometry.Triangles, the world | 0.1 | 0.2 |
| 100 | Placement, Light and Tree, every entity | 0.0 | 0.0 |
| 100 | KeysText and ParseKeys, every entity | 0.0 | 0.0 |
| 100 | the level | 5 entities, 567 faces, 1,056 triangles in the world, 0.06 million characters, 0.1 MiB on disk | round trip equal |
| 10,000 | Level.Write | 61.4 | 83.0 |
| 10,000 | Level.Parse | 60.9 | 12.3 |
| 10,000 | the level Level.Parse keeps | | 11.4 |
| 10,000 | Level.Save | 63.2 | 83.0 |
| 10,000 | Level.Load | 79.8 | 35.6 |
| 10,000 | LevelBrush.Polygons, every brush | 17.1 | 15.6 |
| 10,000 | LevelGeometry.Meshes, every entity | 27.8 | 47.0 |
| 10,000 | LevelGeometry.Hulls, every entity | 25.2 | 19.5 |
| 10,000 | LevelGeometry.Triangles, the world | 5.4 | 15.6 |
| 10,000 | Placement, Light and Tree, every entity | 0.9 | 0.4 |
| 10,000 | KeysText and ParseKeys, every entity | 0.4 | 0.8 |
| 10,000 | the level | 401 entities, 56,667 faces, 105,600 triangles in the world, 6.10 million characters, 5.8 MiB on disk | round trip equal |
| 100,000 | Level.Write | 736.7 | 841.6 |
| 100,000 | Level.Parse | 808.1 | 122.5 |
| 100,000 | the level Level.Parse keeps | | 115.9 |
| 100,000 | Level.Save | 694.1 | 841.6 |
| 100,000 | Level.Load | 707.9 | 364.1 |
| 100,000 | LevelBrush.Polygons, every brush | 134.4 | 156.1 |
| 100,000 | LevelGeometry.Meshes, every entity | 210.9 | 443.2 |
| 100,000 | LevelGeometry.Hulls, every entity | 224.9 | 194.9 |
| 100,000 | LevelGeometry.Triangles, the world | 27.0 | 148.3 |
| 100,000 | Placement, Light and Tree, every entity | 6.2 | 3.8 |
| 100,000 | KeysText and ParseKeys, every entity | 2.6 | 7.9 |
| 100,000 | the level | 4,001 entities, 566,667 faces, 1,056,000 triangles in the world, 63.17 million characters, 60.2 MiB on disk | round trip equal |

At 100,000 brushes every brush's Polygons allocates 156.1 MiB in 134 ms, where it allocated 1,681.8
MiB in 544 to 562 ms, some 1.6 KiB a brush; Meshes 443.2 MiB in 211 ms, where it took 1,984.1 MiB,
some 4.5 KiB a brush; and Hulls 194.9 MiB in 225 ms, where it took 1,854.3 MiB, some 2.0 KiB a
brush. The process's peak working set is 1,253 MiB, where it was 1,258, so the level and its text
hold the peak, not the building of its shapes.
