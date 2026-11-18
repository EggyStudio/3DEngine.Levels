The format reading again after Level.Load reads its file through a stream into a window of 64
kilo-characters, the same made level at 100, 10,000 and 100,000 brushes. Each time is the best of up
to five runs within five seconds on a machine under a load of 21 to 35 at 19:39, so the times are
read with care and the bytes as they stand, from `dotnet run -c Release --project build/bench`.

| Brushes | Step | ms | MiB allocated |
|---|---|---|---|
| 100 | Level.Write | 1.6 | 0.8 |
| 100 | Level.Parse | 3.4 | 0.1 |
| 100 | the level Level.Parse keeps | | 0.1 |
| 100 | Level.Save | 1.7 | 0.8 |
| 100 | Level.Load | 3.4 | 0.4 |
| 100 | LevelBrush.Polygons, every brush | 1.7 | 0.2 |
| 100 | LevelGeometry.Meshes, every entity | 2.1 | 0.5 |
| 100 | LevelGeometry.Hulls, every entity | 2.0 | 0.2 |
| 100 | LevelGeometry.Triangles, the world | 0.2 | 0.2 |
| 100 | Placement, Light and Tree, every entity | 0.1 | 0.0 |
| 100 | KeysText and ParseKeys, every entity | 0.0 | 0.0 |
| 100 | the level | 5 entities, 567 faces, 1,056 triangles in the world, 0.06 million characters, 0.1 MiB on disk | round trip equal |
| 10,000 | Level.Write | 58.8 | 83.0 |
| 10,000 | Level.Parse | 80.2 | 12.3 |
| 10,000 | the level Level.Parse keeps | | 11.4 |
| 10,000 | Level.Save | 95.4 | 83.0 |
| 10,000 | Level.Load | 90.9 | 12.6 |
| 10,000 | LevelBrush.Polygons, every brush | 27.8 | 15.6 |
| 10,000 | LevelGeometry.Meshes, every entity | 30.3 | 47.0 |
| 10,000 | LevelGeometry.Hulls, every entity | 24.1 | 19.5 |
| 10,000 | LevelGeometry.Triangles, the world | 10.5 | 15.6 |
| 10,000 | Placement, Light and Tree, every entity | 1.1 | 0.4 |
| 10,000 | KeysText and ParseKeys, every entity | 0.5 | 0.9 |
| 10,000 | the level | 401 entities, 56,667 faces, 105,600 triangles in the world, 6.10 million characters, 5.8 MiB on disk | round trip equal |
| 100,000 | Level.Write | 1640.6 | 841.6 |
| 100,000 | Level.Parse | 921.7 | 122.5 |
| 100,000 | the level Level.Parse keeps | | 115.9 |
| 100,000 | Level.Save | 937.7 | 841.6 |
| 100,000 | Level.Load | 729.3 | 122.8 |
| 100,000 | LevelBrush.Polygons, every brush | 131.0 | 156.1 |
| 100,000 | LevelGeometry.Meshes, every entity | 209.3 | 443.2 |
| 100,000 | LevelGeometry.Hulls, every entity | 187.1 | 194.9 |
| 100,000 | LevelGeometry.Triangles, the world | 27.7 | 148.3 |
| 100,000 | Placement, Light and Tree, every entity | 4.0 | 3.8 |
| 100,000 | KeysText and ParseKeys, every entity | 3.9 | 8.5 |
| 100,000 | the level | 4,001 entities, 566,667 faces, 1,056,000 triangles in the world, 63.17 million characters, 60.2 MiB on disk | round trip equal |

At 100,000 brushes Level.Load allocates 122.8 MiB, where it allocated 364.1, the file's 63.17
million characters no longer held as one string, and Level.Parse of the same text in hand allocates
122.5, the level 115.9 of both. The process's peak working set is 1,258 MiB, as it was before the
change.
