# TrenchBroom

The game config and the entity classes that let TrenchBroom draw a level of the Valve 220 map format
the module reads. An engine copies this folder into TrenchBroom's games folder, which its
preferences name, under the engine's own name, adds a file of its own classes beside `Levels.fgd`
and names it in `GameConfig.cfg`'s `definitions`, and gives TrenchBroom a game path holding a
`textures` folder, the engine's pictures of its materials.

A new face takes a scale of a sixteenth, so a picture of 512 pixels spans 32 units, a meter at the
units a meter a level takes when its `worldspawn` names none (`_units_per_meter`), and the brush
tool's 8, 16 and 32 unit grid ends on a meter's quarters. A level is saved in the Valve map format,
which is the only one offered.

`Levels.fgd` names the classes the module reads, `worldspawn`, `info_player_start`, `func_door`, the
lights and `tree`, a tree of a species' name and a seed that an engine grows through the species it
has registered. A `light` is a point light, a `light_spot` a spot and a `light_environment` the sun,
their `_color` in sRGB from 0 to 255 a channel, their `intensity` in lumens for a point or a spot
and in lux for the sun, and a spot's `_inner_cone` and `_cone` in degrees from its axis. A spot and
the sun face along the arrow TrenchBroom draws for their `angles`.

`examples/room.map` is a room of every class `Levels.fgd` names, written by the module's tests,
which hold the file to what they write.

The classes Levels.fgd names are these, each with its keys and their defaults and the reader that
reads it, the table written from the fgd by the tests.

<!-- written by FgdTests from Levels.fgd -->

| Class | Keys, with their defaults | Read by |
|---|---|---|
| `worldspawn` | `_units_per_meter` 32, `_sky`, `_ambient` 255 255 255, `_ambient_lux` 0 | `Level.UnitsPerMeter`, `LevelEntity.Ambient` |
| `info_player_start` | `angle` 0 | `LevelEntity.Placement` |
| `light` | `_color` 255 255 255, `intensity` 800, `range` 10, `style` 0, `pattern` | `LevelEntity.Light` |
| `light_spot` | `angles` 0 0 0, `_color` 255 255 255, `intensity` 800, `range` 10, `_inner_cone` 25, `_cone` 30, `style` 0, `pattern` | `LevelEntity.Light` |
| `light_environment` | `angles` 0 0 0, `_color` 255 255 255, `intensity` 100000, `style` 0, `pattern` | `LevelEntity.Light` |
| `tree` | `species`, `seed` 0 | `LevelEntity.Tree` |
| `func_door` | `angle` 0, `lip` 8, `speed` 100, `wait` 3 | `LevelEntity.Travel`, `LevelEntity.Timing` |
| `func_button` | `angle` 0, `lip` 4, `speed` 40, `wait` 1 | `LevelEntity.Travel`, `LevelEntity.Timing` |
| `func_rotating` | `speed` 100, `spawnflags` 0 | `LevelEntity.Spin` |
