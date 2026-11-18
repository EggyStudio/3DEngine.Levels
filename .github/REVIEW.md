# Review

Reviewed up to `3ca2ac0`, item 2, pushed at 12:31 on 2026-10-31 with `751c272`, item 1, after the
owner's first commit, its run green at 12:35 on the three systems in about three minutes, the first
of this repository. After it `6b10049`, item 3's first part, pushed at 14:13, its run green at
14:15, the format and the geometry moved as they were under the namespace Levels, LevelPoint's
conversions on System.Numerics.Vector3, Level.Load and Save on plain file paths through the BCL with
an engine handing its own asset's text to Level.Parse (N 2.1), two cross-references to the engine's
spawner made plain words, InternalsVisibleTo for the tests; 17 of 17 tests, the strict build clean,
no engine named. After it `01c44b0`, the second part, pushed at 14:15, its run green at 14:18,
LevelGeometry.Meshes giving a LevelMesh a material in first-named order with Vector3 positions and
normals, Vector2 texture coordinates and uint indices in meters with Y up, a face's corners fanned
counterclockwise from outside, LevelGeometry.Hulls a LevelHull a solid brush, Texel and PictureSize
public, ILevelMaterials.PictureSize the engine's answer (N 2.1) and the level's units a meter
without one; 21 of 21 tests, the strict build clean. After it `a01b2df`, the third part, pushed at
14:18, its run green at 14:21, TrenchBroom's GameConfig.cfg with the Valve format alone and a scale
of a sixteenth, Levels.fgd with worldspawn's units a meter and info_player_start, a README saying an
engine copies the folder under its own name and lists its own fgd beside, TrenchBroomTests on the
config's format and scale and the fgd's key, AGENTS.md's TrenchBroom row; 23 of 23 tests, the strict
build clean. Item 3 is done in its three parts. Both commits keep the form, the three-mark subject,
one sentence, the owner's identity as author and committer, no trailer (N 7.2 and N 7.6 kept at the
first commit); the library references no engine (N 2.1). The documents are the engines' with the
engine's name replaced and the engine-only lines dropped, COMMITS.md the game's with N 7.1's pointer
rule in place of the legacy branch line, .gitignore with the dump folder, .claude and .ref;
AGENTS.md's devlog row carries no link until DEVLOG.md exists and its table of areas gains the rows
item 2 adds. The skeleton sets ImportDirectoryBuildTargets false, since a submodule under modules/
would inherit an engine's Directory.Build.targets, makes a missing summary an error, loads the
assembly by name in its one test, and build/version.sh prints 0.1.0; the strict build 0 warnings and
0 errors, the test passing. After it `a93b2b8`, the norm alone, the seven repositories named, pushed
at 14:51 on 2026-11-03, with no run since the build passes over the review's files; it keeps the
form. Before it `b40fa1d`, the owner's first commit alone.

## Now

The order the owner set on 2026-10-31 for the six modules, this one holding the Valve 220 map
format's reader, the brushes to meshes and collision, and the TrenchBroom config and fgd. The
documents first, then the rest as the other projects have it, then the engines taking the module.
Items 1 and 2 are the game's session's, being free while the game waits on the engine's package;
item 3 is the session's that owns the code today; item 4 is each engine's session's. Among the six
this module comes third, after Modeling, on Modeling.

1. **The documents, one commit.** AGENTS.md on the game's shape (one paragraph saying what the
    module is and that no engine is referenced, N 2.1; the documents table with STYLE, COMMITS,
    REVIEW, NORM and DEVLOG; building and testing; Where things are with a row a folder; the
    conventions, the commit form among them), README.md (what the module is, how an engine takes it
    as a submodule, `dotnet build` and `dotnet test`), `.github/STYLE.md`, `COMMITS.md` and
    `CODE_OF_CONDUCT.md` as the engines' with the engine's name replaced, `.gitignore` of the
    engines' shape with `/.github/dump/` in it, `.gitattributes`. The game's AGENTS.md and the
    engines' documents are the models; nothing is written that the module does not have yet. This
    commit is the first test of the commit form here, N 7.2 and N 7.6, read by the reviewing session
    before it is pushed. Done at `751c272`.

2. **The skeleton, one commit after item 1.** `Levels.slnx`, `Levels/Levels.csproj` as a class
    library on net10.0 with `Nullable` and `ImplicitUsings` in `Directory.Build.props`,
    `Levels.Tests/` on xunit with one test that the library loads, `build/version.txt` at `0.1` and
    `build/version.sh` as the engines', and `.github/workflows/build.yml` on a push, `test (os)` on
    ubuntu-24.04, windows-latest and macos-15 with `dotnet build Levels.slnx -warnaserror` and
    `dotnet test Levels.Tests --no-build`, a time limit on every job with its reason, `permissions`
    read and the engines' concurrency group. Green on the three systems before item 3. Done at
    `3ca2ac0`.

3. **The code's move.** The source is BevyCSharp's Levels, moved as it is, each engine binding the
    meshes and the collision to its own world; 3DEngine reads the format through this module for the
    first time. One commit a part, the tests coming with the code, the strict build and the suite
    green, a Shared line in the engines' REVIEW.md when a part lands. Items written by the reviewing
    session when item 2 is green and the owning session is free. The parts, proposed by BevyCSharp's
    session on 2026-10-31 at 14:10 and taken, one commit each with the tests beside the code, the
    namespace `Levels`. First the format and the geometry as they are, Level with its reading,
    writing and shapes, LevelEntity, LevelFace, LevelBrush and its shapes, LevelPoint, the two Vec3
    conversions becoming System.Numerics.Vector3, with LevelTests' parse, round trip, refusals,
    corners and meters and LevelShapesTests. Second the meshes and the collision as data without the
    edit mesh, so this module waits on nothing of Modeling's, Level.Meshes giving a LevelMesh a
    material with Vector3 positions and normals, Vector2 texture coordinates and uint indices in
    meters with Y up, the texel arithmetic and the PNG and JPEG size reader moving with it, the
    brushes giving a LevelHull a solid brush with its corners and planes in meters, convex, and a
    material's picture size coming through an interface the engine implements (N 2.1), with tests of
    the texels, the normals and the hull counts. Third TrenchBroom's GameConfig.cfg and a base fgd
    with worldspawn and info_player_start, an engine's own classes staying in its own fgd; the
    prototype pictures stay in BevyCSharp until Modeling's move, since the modeling paints them.
    Done at `a01b2df`, the parts at `6b10049`, `01c44b0` and `a01b2df`.

4. **The engines take the module.** Each engine adds this repository as a git submodule under
    `modules/` and references its project, its workflows checking submodules out, its own folder for
    this code becoming the binding of the module's interfaces (N 2.1) and the rest deleted; a change
    used by an engine is committed here first and pushed before the engine's commit that moves the
    pointer. One commit an engine, after item 3's first part. For BevyCSharp, its item 85, the
    binding keeps Levels.Load and Spawn renamed LevelSpawner (Decision 6), the entities, transforms,
    LevelKeys, the class registry, the materials by name, Render.CreateMesh from a LevelMesh and
    Bepu bodies from a LevelHull, a convex hull a brush in place of one mesh collider,
    WritePrototypePictures, the console commands, FeatureTestLevelTests and docs/levels.md pointing
    here. BevyCSharp took it at `f4e99658` on 2026-10-31, its item 85; 3DEngine's binding is open.

5. **The middle dot out.** The owner wrote on 2026-11-08 that the middle dot is not used anywhere;
    `Levels/LevelFace.cs`'s doc comment says the texel's coordinates with the word dot or in words
    in place of `p · UAxis` and `p · VAxis`, and STYLE.md's Punctuation gains the sentence that the
    middle dot is not used, a row of links separated by a space, a list by commas and a product said
    in words. With the next batch here.

## Verdicts

None yet.

## Decisions

1. **The repository's birth.** The owner made this repository on 2026-10-31 at 12:04 among six,
Modeling, Vegetation, Landscape, RadianceCascades, Tools beside it, after the ideas recorded as
3DEngine's Decision 78 and BevyCSharp's Decision 71, to hold the Valve 220 map format's reader, the
brushes to meshes and collision, and the TrenchBroom config and fgd as one library both engines use
rather than one implementation an engine. The owner set the order the same day, the documents first,
then the rest as the other projects have it, then the engines taking the module, and said the
modules may be tried at once beside the engine's global illumination work (3DEngine Decision 79).

2. **A git submodule, not a package.** The owner chose on 2026-10-31 that the engines take the
module as a git submodule built as a project of their own, since a package's upload, wait, test and
pack is slower to iterate than a commit. A change is committed here first; the reviewing session
pushes this repository before the engine's commit that moves its pointer; the workflows check
submodules out. The game keeps using the engine through the engine's package.

3. **The module knows no engine.** N 2.1. The module references no engine, package or bridge; what
it needs of a renderer, a world, a scheduler or a file system it takes through interfaces it
declares, which each engine implements where it binds the module. A type of an engine's in this
repository is a fault.

4. **The repository's terms are the owner's.** The repository was made by the owner with its terms
   file in place, which stands as the owner set it and is spoken of nowhere else.

5. **Who writes here.** The game's session writes items 1 and 2 for the six modules, being free
while the game waits on the engine's package; the session that owns the code today moves it (item
3), BevyCSharp's for Vegetation and Levels, 3DEngine's for Modeling and the cascades, the reviewing
session's scripts going into Tools; each engine's session binds the module (item 4). Every session
reads REVIEW.md's head, its items and the Replies before a batch, as N 7.7 says, and the reviewing
session pushes.

6. **The binding is named LevelSpawner.** With the format types in the namespace `Levels`,
    BevyCSharp's binding class `Bevy.Levels` would clash with it under a using, so it is renamed
    LevelSpawner, which says what it does, loads a level and spawns it; a public listing reshape as
    the owner's rule of no users until a release allows.

7. **BevyCSharp's session writes this module's commits.** It owns the code today, commits here with
    the owner's identity (N 7.6) and the form, and the reviewing session pushes, this repository
    before the engine's commit that moves the pointer.

8. **The module is worked by a session of its own.** Decided at 16:43 on 2026-11-18 for the seven
    modules at once. From its first commit the module's session alone writes the module's source,
    tests and documents, and the engines' sessions write here no longer; an engine's coder that
    needs something of the module asks the module's session in a message and takes the commit by
    moving its submodule's pointer. Every open item above that names an engine's or the game's
    session as its worker is the module's session's, apart from the binding inside an engine, which
    stays that engine's. The session's first Reply reads this file against the tree and says what is
    done, what is open and the order it proposes; the review answers, and that order stands. The
    rules are NORM.md's as before, among them no engine referenced (N 2.1), tests that need no
    window and no GPU, the commit's form, a failing test before a mend, a picture on a white ground
    where a thing is seen, and any GPU program through the lock of N 7.9 with a waiting file. The
    session reports each checked commit to the reviewing session, which reads it, pushes it and
    reads its run; it does not push.

9. **The module is agnostic enough that a stranger binds it in an engine of their own from its
    README alone (2026-11-18 17:28).** It asks a README section, Binding it in your engine, with
    every interface the module declares, the conventions it stands on and the smallest binding that
    works, a small host in the tests that implements those interfaces with no engine and that the
    section's steps are run against, a test that fails where the library references an engine, a
    window or a graphics API, the form in the Tools module's ReferenceTests.cs, and no engine's name
    in a public member or its summary. The two engines appear in the README as examples. A public
    shape that fits the two engines alone is named in a Reply before it changes.

10. **The package id and the assembly are 3DEngine.Levels and the namespace stays Levels (2026-11-18
    17:28).** The prefix gives a unique name. A package is published when the time is right and on
    the owner's word, for a reader who takes the module alone. The engines keep taking the module as
    a git submodule, compiled with them and carried inside each engine's one package. Folders and
    project file names stay.

## Replies

The module's session's first Reply, Decision 8, reading this file against the tree at `8e16b33`.
Items 1, 2 and 3 are in the tree as the head records them, the documents at `751c272`, the skeleton
at `3ca2ac0` and the code's move in its three parts at `6b10049`, `01c44b0` and `a01b2df`. The
strict build has no warning and the 23 tests pass (dotnet build Levels.slnx -warnaserror, dotnet
test Levels.Tests), and no engine, package or project reference is named in the library or its
props (grep -rniE 'bevy|3dengine|PackageReference|ProjectReference' Levels Directory.Build.props).

Item 4 stays the engines'. BevyCSharp took the module at `f4e99658`, and 3DEngine's .gitmodules
names Sky alone, so its binding is open and nothing of it waits here until its coder asks for a
change.

Item 5 in the commit carrying this line. LevelFace's remarks give a point p's texel as (p dot UAxis
/ ScaleU + UOffset, p dot VAxis / ScaleV + VOffset), and STYLE.md's Punctuation gains the engines'
sentence on the middle dot word for word, so the tree holds the mark only in that sentence and in
item 5's text. The commit carries DEVLOG.md regenerated by Tools' scripts/devlog.py, which gains
item 3's Done line, missing since `a01b2df` (N 4.8), and the review's files with N 7.9 and N 7.10.

Rule: comment lines are held to 100 columns, and no document here, nor the engines' STYLE.md or
AGENTS.md, says so. The library and its tests hold 66 comment lines past 100 columns in 16 files,
most of them a summary or a param on one line (awk 'length > 100 && /^[ \t]*\/\//' Levels/*.cs
Levels.Tests/*.cs | wc -l). A sentence in STYLE.md's Comments would let a review point at it, and a
test that reads the sources would keep it.

AGENTS.md's devlog row names `DEVLOG.md` with no link, though the file is in the tree since
`6b10049` and the head says the row waits on it; the link is the owner's to allow (N 7.4).

The order proposed, one commit each. First item 5, with the devlog and the review's files, in the
commit carrying this line. Second the comment lines held to 100 columns, a test that reads every
source file of the library and its tests and fails on a comment line past 100 columns, then the
lines rewrapped with no word changed. Third what 3DEngine's binding asks of the module, in the order
its coder asks.

N 4.11 in the commit carrying this line. SourceTests reads every source file of the library and its
tests and fails on a comment line past 100 columns, a line holding an address alone apart; it failed
on 66 lines before the rewrap and passes after it. The 16 files are rewrapped with no word changed,
a summary, a remark or a return that does not fit one line opening onto lines of its own. STYLE.md's
Comments names the width and the rule, and AGENTS.md's devlog row gains its link. The strict build
is clean and the 24 tests pass (dotnet test Levels.Tests).

What the module lacks for both engines to load one level alike, read from BevyCSharp's
LevelSpawner, 380 lines of which about half know no engine, and 3DEngine reads no map file yet. The
order proposed, one commit each with its tests.

1. An entity's place. LevelEntity.Placement of a level's units a meter gives a position in meters
   and a System.Numerics Quaternion, a point entity at its origin turned by its angle or angles so
   its minus Z faces where TrenchBroom draws its arrow, a brush entity unturned at its origin or
   its brushes' middle, and worldspawn at zero. Readers for a key as a number, three numbers or a
   whole number come with it, refusing a malformed value with the key and its text named.
2. The material names. The proto/ folder, the slot a name is drawn as, the prototype picture's 512
   texels and the pictures searched under textures/ as a PNG then a JPEG, with an ILevelMaterials
   of the module's over an interface for the files an engine reads, so a face repeats alike in both.
3. An entity's keyvalues as text and back, which a component of each engine carries, and an
   entity's meshes as one triangle shape for a static body.
4. BevyCSharp's binding reduced to its world, its renderer and its physics on these, asked of its
   session in a message once 1 to 3 are pushed, and 3DEngine's binding written on them as its own
   item.

The first is begun.

The first of these in the commit carrying this line. LevelEntity.Placement gives LevelPlacement, a
position in meters and a System.Numerics Quaternion, by BevyCSharp's Place and Turn as they are, and
LevelEntity reads a key as a Number, a WholeNumber or a Point, refusing a malformed value with its
class, key and text named. Its tests check the forward against the file's angles at seven turns, a
pitch above zero looking down, the roll, a brush entity at its origin or its brushes' middle, the
world at zero and the refusals. Level.cs and Level.Reading.cs say the module where they said this
engine. The strict build is clean and the tests pass, 43 where there were 24 (dotnet test
Levels.Tests).

The second part in the commit carrying this line. LevelPictures is the module's ILevelMaterials
over ILevelFiles, an interface of two members, Exists and ReadAllBytes, by relative paths with
forward slashes, naming no engine and no asset reader. It holds BevyCSharp's names as they are,
proto/ and a prototype's slot, the 512 texels of its picture and the search of textures/ for a PNG,
a JPG and a JPEG of a material's name, the first whose header reads, and Find gives the picture's
path and size for an engine to draw. The prototypes' pictures are not written here, since
Modeling's prototype colors paint them, so no bytes are pinned. Its tests check the names, the
order of the search, a header that does not read and a mesh's texels over the pictures found. The
strict build is clean and the tests pass, 50 where there were 43 (dotnet test Levels.Tests).

The third part in the commit carrying this line. LevelEntity.KeysText gives the keyvalues as the
level file writes them, a line each in their order, and the writer writes an entity's keys through
it, so the two lines never differ; LevelEntity.ParseKeys reads them back and refuses a text holding
a brush. LevelGeometry.Triangles gives an entity's meshes as one LevelTriangles, each mesh's corners
after the ones before and its indices moved past them, BevyCSharp's Collision as it is. Its tests
check the order, a key given twice, a value holding a quote, the empty text, the refusal and every
triangle's corners in the shape; no test of this part or the first two compares a value worked out
by a sine, a cosine or a root exactly, those being held to 1e-5. The strict build is clean and the
tests pass, 56 where there were 50 (dotnet test Levels.Tests).

Parts 1 to 3 are pushed, and both engines' coders have part 4 in a message. What comes next in the
module, weighed by what both engines must agree on for one level to look and play alike. Neither
the module nor BevyCSharp's binding reads a light, and the module's fgd names two classes,
worldspawn and info_player_start, beside BevyCSharp's tree. The order proposed, one commit each
with its tests.

1. The level's lights. Levels.fgd gains light, light_spot and light_environment, and
   LevelEntity.Light reads one into a LevelLight of the module's, its kind, its place and the way
   it faces from LevelEntity.Placement, its color from `_color` as TrenchBroom's color picker
   writes it, sRGB from 0 to 255, decoded to linear by sRGB's own curve, its strength in lumens for
   a point or a spot and in lux for the sun, as both engines take a physical light, a point's and a
   spot's range in meters, and a spot's inner and outer cone in degrees from its axis, as Source's
   `_inner_cone` and `_cone` name them. A key missing takes the default the fgd shows, a 60 watt
   bulb's 800 lumens and a clear noon's 100,000 lux among them.
2. The classes both engines read. info_player_start and the lights are the module's, read and
   documented here; tree moves from BevyCSharp's fgd into the module's with a LevelTree of its
   species and seed, since both engines grow it through Vegetation, and a game's classes stay in
   the game's fgd.
3. A test that Levels.fgd and the readers agree, every class the module reads named in the fgd
   with each key it reads and the same default, so the editor and the reader never drift.

The first is begun.

The first in the commit carrying this line. LevelEntity.Light reads a light, light_spot or
light_environment into a LevelLight, its place and its way from Placement, its `_color` decoded
from sRGB by the curve's own formula, its intensity in lumens over the whole sphere for a point and
over the outer cone for a spot, as 3DEngine's SetLightLuminousFlux takes it, and in lux for the
sun, a range in meters and a spot's two cones in degrees, a missing key taking LevelLight's default
and a negative one or an inner cone wider than the outer refused. The remarks give the conversion
for an engine whose spot takes the lumens of a point light of the same candelas. Levels.fgd gains
the three classes with those keys and defaults, and TrenchBroom's README names them. Its tests hold
every value worked out by a power, a sine or a cosine to 1e-5. The strict build is clean and the
tests pass, 67 where there were 56 (dotnet test Levels.Tests).

The second in the commit carrying this line. LevelEntity.Tree reads a tree entity into a LevelTree
of plain values, the species' name as text and the seed, referencing no module, an unknown name
read as it is for the engine to refuse. A missing or empty species is black-tupelo, the default
TrenchBroom shows and does not write, where BevyCSharp's binding read it as an empty name, and a
missing seed is zero. Levels.fgd gains tree with those keys, and TrenchBroom's README names it;
BevyCSharp's fgd drops its tree with its part 4. LevelLight's remarks say an engine whose sky places
the sun by its clock may leave light_environment's intensity unread. The strict build is clean and
the tests pass, 73 where there were 67 (dotnet test Levels.Tests).

The third in the commit carrying this line. FgdTests reads Levels.fgd's classes and keys and holds
them to the readers both ways, the six classes the module reads named in the fgd and no other, each
with the keys its reader reads and no other, and an entity of a class with no keys reading as one
given every default the fgd shows, its units a meter, its placement, its light and its tree. A
default changed in the fgd and a key taken out of it each turned the test red before the file was
restored. LevelLightTests' check of the three light classes' names goes, the new test holding more.
The strict build is clean and the tests pass, 79 where there were 73 (dotnet test Levels.Tests).

The module read as a stranger reads it, its first fault mended in the commit carrying this line. A
brush whose top is moved down onto its floor, two of its planes facing each other in one place,
passed Polygons as two faces back to back and gave Hulls four corners in one plane, which a physics
engine's convex hull cannot take. Polygons refuses it with an InvalidOperationException naming the
face whose plane holds every corner, its test failing before the mend. LevelInputTests comes with
it, the test room's text cut short at each of its characters and 3,000 copies of it with one to
three characters changed among braces, quotes, backslashes and digits, each read or refused by a
FormatException or an InvalidOperationException and none read without end, numbers of no end refused
at their line, three points on one line refused by the face's number, and a brush too far out for
its planes to meet refused. The strict build is clean and the tests pass, 89 where there were 79
(dotnet test Levels.Tests).

The second fault mended in the commit carrying this line. Level.Write wrote what Level.Parse could
not read back, a key or a value ending in a backslash, which the format reads as an escaped quote,
a material of a space, a quote, a bracket or a brace, an empty one or one beginning with two
slashes, which reads as a comment, a number not finite, and a game name of two lines. Write refuses
each with an InvalidOperationException naming the entity, the brush and the face, its ten cases
failing before the mend. A material with two slashes inside it, as wall//a, reads back and is
written. LevelWritingTests holds the round trip as well, a level of every shape, a box, a ramp, a
doorway and a turned box, with a quoted value, a backslash, a key given twice and a value of two
lines, read back with each key and face equal and written again to the byte, and 10,000 numbers
written as the shortest text reading back the same. The strict build is clean and the tests pass,
101 where there were 89 (dotnet test Levels.Tests).

The read as a stranger, its three things. One, a level's reading under bad input holds, the two
faults it found mended at `67b5dca` and `85d43c5`; no input read without end or refused by other
than a FormatException or an InvalidOperationException. Two, a made level of 10,000 brushes, a third
each boxes, ramps and boxes turned by 30 degrees in one world, 56,667 faces and 6.06 million
characters of text, read at its best of five on a machine under a load of 10 to 12. Level.Parse is
the worst, 119 to 206 ms and 139 MiB allocated over runs, a token's record and its text a token;
Level.Write takes 76 to 137 ms and 83 MiB, every brush's Polygons 46 to 65 ms and 142 MiB, about 14
KiB a brush, Meshes and Hulls some 60 ms each over Polygons, Triangles 4 ms, and the process's peak
working set 147 MiB. A brush's Polygons grows as its faces' count to the fourth, and one of 130
faces takes 4 ms, so no brush TrenchBroom draws is slow. None of it is a fault. Parse could read its
numbers from spans of the text with no string a token, unmeasured until it is tried, if a level ten
times larger is ever loaded at a game's start. The numbers come from a console program of the
module's API kept in the session's scratch space, which N 3.6 asks to be checked in; I propose it at
`build/bench/` as a project outside Levels.slnx, run as `dotnet run -c Release --project
build/bench`, with the numbers saved as a reading under `.github/assets/readings/`. Three, a level
written by Level.Write reads back equal, every key and face of a level of every shape, and writes
again to the byte, once Write refuses what map text cannot hold (`85d43c5`).

Beside the three, a stranger finds LevelPoint.Dot and Cross internal though LevelFace.Normal is
public, so an engine's binding writes its own dot product for a face; and the message for a brush
too far out for its planes to meet says its planes close no solid, true and not the reason. Both
are small and wait on the review's word.

LevelPoint.Dot, Cross and Length are public in the commit carrying this line, each with its
summary, Cross right-handed in the file's axes as LevelFace.Normal reads it, with LevelPointTests
on the three and on a face's normal as its points' cross product.

The bench is in `build/bench/` in the commit carrying this line, outside Levels.slnx, run with
`dotnet run -c Release --project build/bench`, its comments saying what it builds and no figure of
a run. Its level is the one above, 6.06 million characters, 56,667 faces and 106,668 triangles;
each time is the best of five, and the two runs differ by the machine's load, 12 at 17:12 for the
scratch copy of the same program and 29 at 17:16 for this one.

| Step | ms at load 12 | ms at load 29 | MiB allocated |
|---|---|---|---|
| Level.Write | 91.8 | 165.2 | 82.8 |
| Level.Parse | 118.9 | 366.9 | 139.2 |
| LevelBrush.Polygons, every brush | 48.2 | 107.9 | 141.8 |
| LevelGeometry.Meshes | 63.5 | 86.4 | 172.3 |
| LevelGeometry.Hulls | 57.2 | 76.1 | 159.1 |
| LevelGeometry.Triangles | 3.7 | 3.8 | 15.7 |
| Polygons of one brush of 130 faces | 4.0 | 4.2 | |

The peak working set is 147 MiB in both. AGENTS.md's row for `build` names the version alone, and
the bench beside it waits on the owner's word for the row's text (N 7.4).

The too-far brush's message in the commit carrying this line. Polygons refuses a face with a point
more than a billion units out along an axis, naming the face, the point's distance and the bound,
where it said the planes close no solid; a double resolves the hundred-thousandth of a unit its
corners are merged at out to 2 to the 35th, and the bound keeps a margin for the plane's products.
Its test failed on the old message, and a brush at 900 million units is read. The strict build is
clean and the tests pass, 105 where there were 104 (dotnet test Levels.Tests).

The agnostic direction's first ask in the commit carrying this line. The README gains Binding it in
your engine, the two interfaces, ILevelFiles and ILevelMaterials, with LevelPictures over them, the
conventions of length, axes, triangles, angles, color and light, what an engine of other axes or
winding turns, and the smallest binding, which the next commit makes the tests' host. The public
shapes that fit our two engines more than a stranger's, said before any is changed, are three.
LevelPictures' proto/ folder and its 512 texels are the prototype grid our engines paint; a
stranger's engine gets a slot name it may ignore, and the README says that one naming its pictures
otherwise implements ILevelMaterials itself. An entity facing minus Z with Y up is BevyCSharp's
frame and OpenGL's camera's, and an engine of Z up or left-handed axes turns it, as the README says.
LevelTree's default species, black-tupelo, is a name of the species both engines register through
Vegetation, and a stranger's engine with no such species refuses it as the README's binding leaves
to it; I propose keeping all three as they are.

The second ask in the commit carrying this line. Levels.Tests/Host/LevelBinding.cs is the README's
smallest binding, an IWorld standing for an engine's calls, FolderFiles as the ILevelFiles over a
folder and LevelBinding.Load, and LevelBindingTests runs it with no engine against a level saved
into a folder of its own with a PNG's header, checking the slots drawn, a picture's size and a
prototype's, the two bodies and the door's place, the spot's way and the entities handed on by
class. A second test holds the README's code block to the host's text between its two marks, and
it turned red when a comment was added to the README's copy alone. The README's section names the
host. The strict build is clean and the tests pass, 107 where there were 105 (dotnet test
Levels.Tests).
The check queued under the shared lock because I took N 7.9's suite and build for every dotnet
command of mine, and a run of tests that reads files and draws nothing needs no lock; this
module's tests and its bench run on the CPU alone, so its filtered runs read and run with no lock,
and its builds take the shared lock as N 7.9 says of a build.

The owner's names in the commit carrying this line. The library's AssemblyName and PackageId are
3DEngine.Levels, IsPackable staying false, and the tests' assembly is 3DEngine.Levels.Tests, its
InternalsVisibleTo following; folders, project files and the namespace Levels stay. LibraryTests
loads the library by its new name and reads that Level stands in Levels, failing before the
rename, and the README's binding section names the assembly. The workflow names folders alone and
is unchanged, and the bench builds against the renamed library. The strict build is clean and the
tests pass, 107 (dotnet test Levels.Tests).

AGENTS.md's table of areas gains `build/bench`'s row in the commit carrying this line, the new
path's row the review left to this session, nothing else in the file changing.

The third ask in the commit carrying this line. ReferenceTests is Tools' file at its head as it
is, its library 3DEngine.Levels, reading the assembly's references and the packages the tests'
dependency file records for it against the engines' names and the windows, graphics APIs and
bindings it lists. A reference from the library to a scratch project named Vortice.Fake turned it
red with that name in its message, and taking the reference out turned it green. The strict build
is clean and the tests pass, 119 where there were 107 (dotnet test Levels.Tests).

The fourth ask in the commit carrying this line. EngineNameTests reads the library's exported
types and their public members by reflection and its documentation file's members, its assembly's
own name apart, and fails on either engine's name or 3DEngine's API prefix. No name was found; a
summary given BevyCSharp's name turned it red naming the member, and taking the name out turned it
green. The engines stand in the README alone, as two examples. The strict build is clean and the
tests pass, 121 where there were 119 (dotnet test Levels.Tests).

The sweep's three findings in the commit carrying this line. SourceTests read the library's and the
tests' top folders alone and C#'s comments alone; it reads every folder deep, Host and build/bench
among them, and the comments of the project files, the solution, the workflow and the scripts, and
it turned red on Levels/Levels.csproj:13 and .github/workflows/build.yml:46 at 103 columns before
both were rewrapped. Level.Shapes.cs's banner says the doorway in its own terms, convex brushes one
each side of the opening and straight segments round its top, where it named another product's
prototyping. Two lines of the same kind may stand, LevelBrush.Shapes.cs's banner making the brushes
"as Hammer's and TrenchBroom's tools make them" and Arch's remark cutting the ring "as Hammer's
arch tool cuts one"; the arch's even segments are a rule taken from that tool, so they are left for
the review's word. The strict build is clean and the tests pass, 121 (dotnet test Levels.Tests).

The second item in the commit carrying this line. LevelBindingTests gains Sky's test of the public
surface, every exported type named whole in backticks in Binding it in your engine or in a row of a
README table, and it turned red naming 13 of the 17, LevelBrush, LevelEntity, LevelFace,
LevelGeometry, LevelHull, LevelLight, LevelLightKind, LevelMesh, LevelPicture, LevelPlacement,
LevelPoint, LevelTree and LevelTriangles. The section gains a table of the types a binding reads,
a row a type with the members it calls by, Level.Doorway among them. The tests pass, 122 where
there were 121 (dotnet test Levels.Tests).

The third item, a read of both engines' level code as it stands, BevyCSharp at `8e7fa0f5` and
3DEngine at `ee7ac712`, read and not changed. Each row is a thing an engine's level code does,
whether the module holds it at `3d6ae0c`, and where it belongs; the rows marked for the module are
the list the bindings of part 4 are judged against, and the rest stay the engine's or another
module's.

| Engine | File | What it does | The module | Belongs |
|---|---|---|---|---|
| BevyCSharp | `BevyCSharp/Levels/LevelSpawner.cs` Load | Reads a level's text through the engine's asset files, refusing a missing file | Parse alone; ILevelFiles reads pictures, not the level | Module, a Level.Load over ILevelFiles, so both engines read a level as they read its pictures |
| BevyCSharp | LevelSpawner.Spawn | A root under a given place, named for the level, every entity under it | No | Engine |
| BevyCSharp | LevelSpawner.Spawn | Names an entity by its targetname, else its class, else `entity` and its index | No | Module, an entity's name, so both engines and their consoles name one alike |
| BevyCSharp | LevelSpawner.Spawn | Wraps a fault as a FormatException naming the entity's index and class | Faults name the class and key, not the index | Module, a fault naming the entity's index, read where the level is read |
| BevyCSharp | LevelSpawner Place, Turn, Middle | An entity's place and turn | Placement (`b19895a`) | Module, held |
| BevyCSharp | LevelSpawner Number, Three, Point | A key read as numbers, refused by name | Number, WholeNumber, Point | Module, held |
| BevyCSharp | LevelSpawner KeysText, KeysOf | Every key kept as text and read back | KeysText, ParseKeys | Module, held |
| BevyCSharp | LevelSpawner Register, ClassNames | A registry of what each class spawns, on the engine's world | No | Engine, its signature is the world's; the classes both read are Levels.fgd's |
| BevyCSharp | LevelSpawner.Build, Drawn | A part a material under the entity, its mesh made from a LevelMesh, where a renderer runs | Meshes | Engine |
| BevyCSharp | LevelSpawner.Build, Collision | A static body of the meshes' triangles | Triangles | Module for the shape, held; engine for the body |
| BevyCSharp | LevelSpawner MaterialOf, Picture, SlotOf, Pictures, PrototypeFolder, PrototypePixels | A material by name, a prototype or a picture under textures/ | LevelPictures | Module, held; the drawing the engine's |
| BevyCSharp | LevelSpawner.WritePrototypePictures, `BevyCSharp/Levels/TrenchBroom/textures/proto` | The prototypes' pictures TrenchBroom shows, from the prototype colors | No | Modeling, which paints the prototypes; then this module's TrenchBroom folder ships the pictures |
| BevyCSharp | LevelSpawner.PlantTree | A tree of the species and seed keys, named by them | Tree reads the keys | Module for the reading, held; engine for the growing and its name |
| BevyCSharp | `BevyCSharp/Levels/LevelKeys.cs` | The component an entity's class, name and keys stand in | KeysText | Engine |
| BevyCSharp | `BevyCSharp/Levels/ConsoleLevelCommands.cs` | `level.load` and `level.textures` from the console | No | Engine |
| BevyCSharp | `BevyCSharp/Levels/TrenchBroom/GameConfig.cfg` | The module's config named BevyCSharp with its own fgd | Its own config | Engine, as TrenchBroom's README says |
| BevyCSharp | `BevyCSharp/Levels/TrenchBroom/BevyCSharp.fgd` | The tree class | tree in Levels.fgd (`2f3017a`) | Module; BevyCSharp drops it in part 4 |
| BevyCSharp | `docs/levels.md` | Registers `light` reading the key `light` in lumens | light reads `intensity` | Module's class stands; BevyCSharp's page and any level writing `light` follow it in part 4 |
| BevyCSharp | `BevyCSharp.Tests/Levels/FeatureTestLevelTests.cs` Slab | A prism from an outline across Y | Prism, internal | Module, Prism public |
| BevyCSharp | FeatureTestLevelTests Wall, Walls | A wall between two points with rounded openings, turned and moved into place | Doorway along X alone | Module, a wall between two points with openings |
| BevyCSharp | FeatureTestLevelTests Ramp | A ramp between two points, as wide and as high as given | Wedge along X alone | Module, a ramp between two points |
| BevyCSharp | FeatureTestLevelTests At, Block | Meters to the file's units, a box in meters | FromMeters, Box | Module, held |
| BevyCSharp | FeatureTestLevelTests Hit | How far along a ray it first meets a brush | No | Module, a ray against a brush's planes, which a test or a game reads alike in either engine |
| BevyCSharp | FeatureTestLevelTests Dot, Length | A point's arithmetic | Dot, Length (`4eb35e2`) | Module, held |
| BevyCSharp | `games/FeatureTest/Behaviors/Packs.cs`, `SceneFloor.cs` | info_scene and func_scene_floor, read from LevelKeys | No | Game, its own classes in its own fgd |
| BevyCSharp | `BevyCSharp.Tests/Levels/LevelTests.cs` | Spawning by class, the console, the shipped pictures held to the drawn ones | Their reading | Engine |
| BevyCSharp | `BevyCSharp.Editor/Framework/LevelCameras.cs` | The cameras of a scene file edited and saved | No | Engine, its scene format and not map text |
| BevyCSharp | `games/Courtyard/build-level.sh`, `courtyard.scene.json` | A level built in the editor and saved as a scene | No | Engine, its scene format |
| BevyCSharp, 3DEngine | `games/Blockout/*/levels/blockout.model` | A level cut from one block by a modeling recipe | No | Modeling |
| 3DEngine | none | Reads no map text and binds no level | Everything above marked the module's | Engine's binding of part 4 |
| 3DEngine | `3DEngine/Api/E3D.Prototypes.cs` | The prototypes' colors by the same seven names, over a grid of 1024 pixels a meter | proto/ and 512 texels | Engine; the texture coordinates are fractions of a picture, so a grid of 1024 a meter and TrenchBroom's 512 repeat once a meter alike |
| 3DEngine | `3DEngine/Api/E3D.Meshes.cs` UploadMesh | A mesh from vertices and indices | LevelMesh | Engine |
| 3DEngine | `3DEngine/Physics/Bepu/PhysicsWorld.Creation.cs` | A static mesh and a convex hull body, internal, with no public call | Triangles, Hulls | Engine, to make public for its binding |
| 3DEngine | `3DEngine/Api/E3D.Lights.cs` | A light's lumens and lux | LevelLight in lumens and lux | Engine; the units agree |
| 3DEngine | `games/FirstGame`, `games/Summit` level.json, `3DEngine.Examples/Scenes/ScenesLevel.cs`, `games/Manor/Level.cs` | Levels as the engine's scene files and as code | No | Engine and games, not map text |

So the module lacks six things both engines' bindings stand on, each proposed as a commit of its
own in this order: Level.Load over ILevelFiles; an entity's name; a fault naming the entity's
index; Prism public; a wall and a ramp between two points; and a ray against brushes. The
prototypes' pictures wait on Modeling.

The two lines the review left, in the commit carrying this line. LevelBrush.Shapes.cs's banner says
the brushes by their outline and their mapping alone, and Arch's summary says its rule, every
segment spanning an equal share of the arch's angle. LevelBrush.cs's remark that a brush is kept as
its planes "as Quake's tools, Hammer and TrenchBroom keep one" names where the format's rule comes
from and stands unless the review says otherwise. The tests pass, 122 (dotnet test Levels.Tests).

The trial of the modules' checks in the commit carrying this line. Tools is a submodule at
`modules/Tools`, pinned at `ebdd08e`, referenced from the tests' project alone with the target that
stops a build where it is not fetched, and ModuleTests holds the four facts of Tools' README, its
Library 3DEngine.Levels and its Host `Levels.Tests/Host/LevelBinding.cs`. ReferenceTests,
EngineNameTests, the public-surface test and the README's block test go, and the host stays, its
two marks become Tools' `// The README's block from here` and `// The end marker.`. Each fact
turned red once, a reference to a scratch project named Vortice.Fake, a summary given BevyCSharp's
name, LevelHull's row taken out of the README and a comment added to the README's copy of the
binding alone, and each turned green when undone. AGENTS.md gains the `modules` row N 1.5 asks
for, and the README's Building and testing says a clone fetches the submodule.

| Asked | Found |
|---|---|
| What the README left unsaid | The block's two marks, which only ModuleChecks.BindingBlock's remarks give, so a host with marks of its own fails with the sentence that names them; that the four facts in one test stop at the first, so a second fault shows only once the first is mended, as the engine's name showed only after the reference went; that each Assert.Empty prints a bare list with no rule named; that an engine taking the module needs no nested submodule, since the library builds without Tools; that a module's sweep walking its whole tree reads the submodule's files unless it skips `modules` |
| What the workflow needed | `submodules: true` on `actions/checkout`, and nothing more |
| The build's time | A full rebuild, `dotnet build Levels.slnx --no-incremental -warnaserror` three times under the shared lock, took 8.5 to 9.8 s before at a load of 10.7 and 12.9 to 13.4 s after at a load of 12.6, Tools' project some 3.5 to 4 s of it |
| A clone without the submodule | `Levels.Tests/Levels.Tests.csproj(21,9): error : The Tools submodule is not fetched. Run git submodule update --init modules/Tools.`, and `dotnet build Levels/Levels.csproj` alone builds clean, as an engine builds it |
| A clone after `git submodule update --init` | Tools fetched from GitHub at `ebdd08e`, the strict build clean and 107 tests passing |

What went badly, plainly. One test of four facts hides every fault after the first, and its
failures name no rule; one fact a test, or a message a fact, would serve the modules that follow.
The tests pass, 107 where there were 122, the sixteen replaced by one (dotnet test Levels.Tests).

The fourth item in the commit carrying this line. The bench takes one made level, a third each
boxes, ramps and turned boxes, every hundredth brush a door's and a light, a spot and a tree for
each hundred, through Write, Parse, Save, Load, every brush's Polygons, Meshes, Hulls and Triangles,
and every entity's Placement, Light, Tree, KeysText and ParseKeys, at 100, 10,000 and 100,000
brushes, holding the level read back and its text written again equal to the byte, and ending the
run where either differs. The sheet is `.github/assets/readings/2026-11-18-format-reading.md`,
which DEVLOG.md carries on its day. The round trip is equal at all three counts, and at 100,000
brushes Level.Parse is the first step slow enough to see, 1,369 ms and 1,409 MiB for 63.17 million
characters, some 23 bytes a character, Level.Load 1,218 ms and 1,650 MiB, and every brush's
Polygons next in memory at 1,682 MiB; the process peaks at 1,337 MiB. AGENTS.md's bench row says
what it runs.

The light key. The module's light reads `intensity`, and a map written for BevyCSharp's page, with
the key `light` holding lumens, reads as a light of LevelLight.DefaultLumens, 800, its `light` key
kept among its keys and read by nothing; such a map needs each `light` key renamed `intensity`, the
number unchanged, since both are lumens over the whole sphere. No map in BevyCSharp's games writes
the key.

The first of the six in the commit carrying this line. Level.Load of an ILevelFiles and a path
reads a level through the engine's files as LevelPictures reads its pictures, the bytes as UTF-8
with a byte order mark skipped, refusing a missing path with a FileNotFoundException naming it and
bad text with a FormatException at the path and line. The README's binding and its host read the
level through FolderFiles, and the README's rows say so. LevelLoadTests checks the read, the mark,
the missing path and the bad text. The tests pass, 111 where there were 107 (dotnet test
Levels.Tests).

The second of the six in the commit carrying this line. LevelEntity.TargetName gives the
`targetname` or an empty string, and LevelEntity.Name of its index gives the name BevyCSharp's
spawner gave, its target name, else its class, else `entity` and its index, so both engines and
their consoles name an entity alike; the README's row names it. LevelNameTests checks the three and
an empty target name. The tests pass, 113 where there were 111 (dotnet test Levels.Tests).

The third of the six in the commit carrying this line. Level.ReadEntities hands each entity to the
engine in order with its index, and gives a FormatException or an InvalidOperationException the
reading throws again as a FormatException naming the entity's index and name, the first fault inside
it, as BevyCSharp's spawner wrapped one; an engine's own exception goes on as it is. The README's
binding and its host read the entities through it, and the README's row names it. LevelReadTests
checks the order, a malformed origin, a brush that closes no solid and an engine's own fault. The
tests pass, 117 where there were 113 (dotnet test Levels.Tests).

Tools' revised checks in the commit carrying this line, the pin moved from `ebdd08e` to `f92df1c`
and ModuleTests rewritten on its README's block, four facts a test each, Library, Root and Host kept.
With all four faults planted at once, a reference to Vortice.Fake, a summary given BevyCSharp's
name, LevelHull's row taken out and a comment in the README's copy of the binding, all four tests
turned red together, and each sentence names what was found, the rule and what to do. Two things
read badly. xunit's Assert.Null shows a string cut at some fifty characters and an ellipsis, so the
runner prints "The library references Vortice.Fake, an engine, a " and the rule and the remedy are
never seen; `Assert.True(fault is null, fault)` would print the sentence whole. And the engine-name
sentence says "F:Levels.LevelTree.Class name an engine" for one member. Both go to Tools in a
message. The tests pass, 120 where there were 117 (dotnet test Levels.Tests).

The fourth of the six in the commit carrying this line. LevelBrush.Prism is public, a brush of any
convex outline across Y in either winding between two ends in either order, and refuses an outline
that bends in, one whose corners lie on one line, and two ends that are one or not a number, its
three refusals failing before the mend; the README's row names it. My table's row for the feature
test's Slab was wrong. Its slab is an outline on the ground stretched up the file's Z, which Prism,
standing across Y, does not make, so a slab of a ground outline goes with the wall and the ramp in
the next commit. The README's top row links DEVLOG.md, and AGENTS.md's devlog row names its command,
`python3 modules/Tools/scripts/devlog.py . Levels`, the rule the review gave today. The tests pass,
126 where there were 120 (dotnet test Levels.Tests).

The fifth of the six in the commit carrying this line. Prism's body is a builder of a convex
outline stretched along an axis, which Prism takes across Y and LevelBrush.Slab across Z, a ground
outline stretched up, as the feature test's slab is. LevelBrush.Along turns a brush made along X at
the origin to run between two points of the ground and moves it to the first, so a doorway or an
arch between two points is Level.Doorway or Arch with each brush put along the line, and
LevelBrush.Wall and LevelBrush.Ramp are a box and a wedge put so, centered on the line, standing up
from the first point's height. LevelPlacedShapesTests holds a slab to a box, a wall north to a box,
a diagonal wall's corners within its thickness of the line, a ramp each way, a doorway put along a
line and the refusals; the earlier shapes' tests pass unchanged on the builder. The README's row
names the four. The tests pass, 133 where there were 126 (dotnet test Levels.Tests).
