The format reading again after Level.Parse reads its tokens where they stand in the text, the same
made level at 100, 10,000 and 100,000 brushes, with a row for the level Level.Parse keeps, the
memory the level read holds once the collector has run. Each time is the best of up to five runs
within five seconds on a machine under a load of 13 to 15 at 18:37, from `dotnet run -c Release
--project build/bench`.

| Brushes | Step | ms | MiB allocated |
|---|---|---|---|
| 100 | Level.Write | 1.6 | 0.8 |
| 100 | Level.Parse | 2.8 | 0.1 |
| 100 | the level Level.Parse keeps | | 0.1 |
| 100 | Level.Save | 1.7 | 0.8 |
| 100 | Level.Load | 2.8 | 0.4 |
| 100 | LevelBrush.Polygons, every brush | 4.3 | 1.7 |
| 100 | LevelGeometry.Meshes, every entity | 4.8 | 2.0 |
| 100 | LevelGeometry.Hulls, every entity | 5.1 | 1.9 |
| 100 | LevelGeometry.Triangles, the world | 0.1 | 0.2 |
| 100 | Placement, Light and Tree, every entity | 0.1 | 0.0 |
| 100 | KeysText and ParseKeys, every entity | 0.0 | 0.0 |
| 100 | the level | 5 entities, 567 faces, 1,056 triangles in the world, 0.06 million characters, 0.1 MiB on disk | round trip equal |
| 10,000 | Level.Write | 65.3 | 83.0 |
| 10,000 | Level.Parse | 75.9 | 12.3 |
| 10,000 | the level Level.Parse keeps | | 11.4 |
| 10,000 | Level.Save | 85.4 | 83.0 |
| 10,000 | Level.Load | 93.7 | 35.6 |
| 10,000 | LevelBrush.Polygons, every brush | 65.0 | 168.2 |
| 10,000 | LevelGeometry.Meshes, every entity | 90.2 | 201.0 |
| 10,000 | LevelGeometry.Hulls, every entity | 126.7 | 185.5 |
| 10,000 | LevelGeometry.Triangles, the world | 5.1 | 15.6 |
| 10,000 | Placement, Light and Tree, every entity | 2.8 | 1.9 |
| 10,000 | KeysText and ParseKeys, every entity | 0.7 | 0.8 |
| 10,000 | the level | 401 entities, 56,667 faces, 105,600 triangles in the world, 6.10 million characters, 5.8 MiB on disk | round trip equal |
| 100,000 | Level.Write | 995.1 | 841.6 |
| 100,000 | Level.Parse | 744.0 | 122.5 |
| 100,000 | the level Level.Parse keeps | | 115.9 |
| 100,000 | Level.Save | 854.6 | 841.6 |
| 100,000 | Level.Load | 1197.2 | 364.1 |
| 100,000 | LevelBrush.Polygons, every brush | 543.8 | 1681.8 |
| 100,000 | LevelGeometry.Meshes, every entity | 1302.4 | 1984.1 |
| 100,000 | LevelGeometry.Hulls, every entity | 886.5 | 1854.3 |
| 100,000 | LevelGeometry.Triangles, the world | 52.0 | 148.3 |
| 100,000 | Placement, Light and Tree, every entity | 20.2 | 19.0 |
| 100,000 | KeysText and ParseKeys, every entity | 4.6 | 7.9 |
| 100,000 | the level | 4,001 entities, 566,667 faces, 1,056,000 triangles in the world, 63.17 million characters, 60.2 MiB on disk | round trip equal |

At 100,000 brushes Level.Parse takes 744 ms and allocates 122.5 MiB, where it took 1,369 ms and
1,409 MiB, and the level it reads keeps 115.9 MiB of them, so the reading's own allocations are some
7 MiB beside the level. Level.Load allocates 364.1 MiB, the file's text read whole into one string
of 63.17 million characters and the level. The process's peak working set is 1,258 MiB, where it was
1,337, and every brush's Polygons, Meshes and Hulls allocate the most of the steps, the next to
read.
