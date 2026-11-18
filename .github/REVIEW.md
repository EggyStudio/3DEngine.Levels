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