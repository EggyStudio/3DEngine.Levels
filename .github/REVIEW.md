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
