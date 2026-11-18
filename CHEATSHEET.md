# 3DEngine.Levels cheatsheet

Every public type of the library with its public members, each a line of its declaration and the
first sentence of its summary. The list is written by `Cheatsheet.Write` from the built library, and
the text around it by hand.

<!-- written by Cheatsheet.Write from the built library -->
```csharp
interface ILevelFiles                                    // The files an engine reads a level and its pictures from, by paths relative to its own root, which Load and LevelPictures read (N 2.1)
bool Exists(string path);                                // Whether the engine has a file at the path
byte[] ReadAllBytes(string path);                        // The file's bytes

interface ILevelMaterials                                // What an engine says of a level's materials, the size of the picture each is drawn with, which a face's texture coordinates are worked out over (N 2.1)
(double, double)? PictureSize(string material);          // How many texels the picture of a material spans each way, or nothing where the engine has none for it, which Meshes then takes as the level's units a meter, so a face at a scale of one repeats its material once a meter

class LevelPictures                                      // A level's materials by the module's names for them, a prototype under PrototypeFolder and any other material a picture of its name under PictureFolder, so a face repeats over the texels TrenchBroom draws it with
string PrototypeFolder = "proto/";                       // The folder of a level's materials a prototype stands in, so a face of proto/floor is drawn as the floor prototype
int PrototypePixels = 512;                               // How many texels a side a prototype's picture has, a meter's cell of the grid at a face scale of a sixteenth and 32 units a meter
string PictureFolder = "textures/";                      // The folder a material's picture is searched for in, relative to the engine's root
LevelPictures(ILevelFiles files);
IReadOnlyList<string> PictureExtensions { get; }         // The kinds of picture searched for, in the order they are tried
bool IsPrototype(string material);                       // Whether a material is a prototype, a name under PrototypeFolder
string Slot(string material);                            // The slot a material is drawn as, a prototype's name without PrototypeFolder and any other name as it is
LevelPicture? Find(string material);                     // The material's picture, a prototype's with no path, the first PNG or JPEG of its name whose header reads, or nothing
(double, double)? PictureSize(string material);          // How many texels the picture of a material spans each way, or nothing where the engine has none for it, which Meshes then takes as the level's units a meter, so a face at a scale of one repeats its material once a meter

class Level                                              // A level file, its brushes and entities, in the Valve 220 map format TrenchBroom reads and writes
double DefaultUnitsPerMeter = 32;                        // How many of a file's units make a meter where its worldspawn says none
string UnitsPerMeterKey = "_units_per_meter";            // The worldspawn key saying how many of the file's units make a meter
string Extension = ".map";                               // The extension a level file is named with
Level();
List<LevelEntity> Entities { get; }                      // The entities, worldspawn first where the file has one
LevelEntity? World { get; }                              // The worldspawn entity, which holds the level's own brushes, or nothing where there is none
double UnitsPerMeter { get; }                            // How many of the file's units make a meter, the worldspawn's UnitsPerMeterKey or DefaultUnitsPerMeter
string Game { get; set; }                                // The game TrenchBroom's first comment names, which a file written again keeps, or Generic where it names none
void ReadEntities(Action<int, LevelEntity> read);        // Hands each entity to the engine in the level's order with its index, a fault the level's text causes named by the entity it stands in
Level Load(string path);                                 // Reads a level from a file
Level Load(ILevelFiles files, string path);              // Reads a level from an engine's files, as its pictures are read
string Save(string path);                                // Writes the level to a file, which Load reads back the same
Level Parse(string text, string name = "the level");     // Reads a level from map text
IReadOnlyList<LevelBrush> Doorway(LevelPoint low, LevelPoint high, double width, double height, string material, string? arch = null, int segments = 8, double scale = 0.0625);  // A wall along X with a rounded opening through its middle, a brush each side of the opening and the wall above it cut into straight segments round the opening's top, as a level editor's arch tool cuts one
string Write();                                          // The level as map text in the Valve 220 format, as TrenchBroom lays it out

class LevelEntity                                        // An entity of a level file, its keyvalues in the order the file gives them and the brushes it is made of, if any
LevelEntity();
List<KeyValuePair<string, string>> Keys { get; }         // The keyvalues, in the file's order, a key more than once where the file gives it so
List<LevelBrush> Brushes { get; }                        // The brushes, none for a point entity
string ClassName { get; }                                // The value of its classname key, or an empty string where it has none
string TargetName { get; }                               // The value of its targetname key, by which another entity's output names it, or an empty string where it has none
string? this[string key] { get; set; }                   // The first value of a key, or nothing where the entity has no such key
string Name(int index);                                  // What the entity is called where an engine shows it, its TargetName, else its ClassName, else entity and its index in the level
IEnumerable<string> All(string key);                     // Every value of a key, in the file's order
string KeysText();                                       // The keyvalues as the level file writes them, a line each in their order, which an engine keeps on what it spawns for the entity and ParseKeys reads back
List<KeyValuePair<string, string>> ParseKeys(string text);  // The keyvalues of KeysText's text, in their order
LevelLight? Light(double unitsPerMeter);                 // The light the entity is, read from its keys, or nothing where its class is not a light's
double? Number(string key);                              // A key's value as a number, or nothing where the entity has no such key
uint? WholeNumber(string key);                           // A key's value as a whole number of zero or more, or nothing where the entity has no such key
LevelPoint? Point(string key);                           // A key's value as three numbers, a point in the file's units, or nothing where the entity has no such key
LevelPlacement Placement(double unitsPerMeter);          // Where the entity stands in meters with Y up and how it is turned, as an engine places what it spawns for it
LevelTree? Tree();                                       // The tree the entity places, read from its keys, or nothing where its class is not tree
void Place(LevelPlacement placement, double unitsPerMeter);  // Writes where the entity stands and how it is turned to its origin and its angle or angles, as Placement reads them
void SetLight(LevelLight light, double unitsPerMeter);   // Writes a light to the entity's keys, its class, place, way, color, strength, range and cones, as Light reads them
void SetTree(LevelTree tree);                            // Writes a tree to the entity's keys, its class, its species and its seed, as Tree reads them

class LevelBrush                                         // A brush of a level file, a convex solid, what lies behind every one of its faces' planes
double DefaultScale = 0.0625;                            // The scale a face made here takes, a sixteenth, which TrenchBroom's game config beside the engine's levels gives a new face, so a prototype's picture of 512 texels spans 32 units, a meter at the default units a meter
LevelBrush();
List<LevelFace> Faces { get; }                           // The faces
LevelPoint[]?[] Polygons();                              // Each face's polygon, its corners wound clockwise seen from outside as the file's points are, in the file's units, or nothing for a face whose plane only touches the solid
double? Hit(LevelPoint from, LevelPoint way, double reach);  // How far along a ray it first meets the brush's solid, or nothing where it meets none within its reach
LevelBrush Box(LevelPoint low, LevelPoint high, string material, double scale = 0.0625);  // A box between two corners
LevelBrush Wedge(LevelPoint low, LevelPoint high, string material, double scale = 0.0625);  // A ramp between two corners, its slope rising along X from the low corner's height at its low X to the high corner's at its high X
IReadOnlyList<LevelBrush> Arch(LevelPoint center, double radius, double thickness, double depth, string material, int segments = 8, double degrees = 180, double scale = 0.0625);  // An arch standing across X, a half ring or a part of one cut into straight segments, a brush each, every segment spanning an equal share of the arch's angle
LevelBrush Turned(double degrees, LevelPoint about);     // The brush turned about the vertical line through a point, its faces' mapping taken again from the world, so its material lies on it as on one drawn where it stands
LevelBrush Moved(LevelPoint by);                         // The brush moved, its faces' mapping taken again from the world, so its material stays where the world puts it rather than going with the brush, as a moved brush's does in TrenchBroom with its texture lock off
LevelBrush Prism(IReadOnlyList<(double, double)> outline, double from, double to, string material, double scale = 0.0625);  // A brush of a convex outline across Y, its points as (X, Z) in either winding, stretched from one Y to another, each face's three points wound as the file winds them and its axes the world's
LevelBrush Slab(IReadOnlyList<(double, double)> outline, double from, double to, string material, double scale = 0.0625);  // A brush of a convex outline on the ground, its points as (X, Y) in either winding, stretched up from one height to another, as a floor's, a ceiling's or a platform's slab
LevelBrush Wall(LevelPoint from, LevelPoint to, double height, double thickness, string material, double scale = 0.0625);  // A wall from one point of the ground to another, centered on the line between them, standing up from the first point's height
LevelBrush Ramp(LevelPoint from, LevelPoint to, double width, double rise, string material, double scale = 0.0625);  // A ramp from one point of the ground to another, as wide as given and centered on the line between them, rising from the first point's height by its rise at the second
LevelBrush Along(LevelPoint from, LevelPoint to);        // The brush, made along X from the file's origin, turned to run from one point of the ground toward another and moved to start at the first, its height raised by the first's

record LevelFace                                         // A face of a brush in a level file, the plane through three points and the material the face is drawn with, mapped onto it along two axes of the world, as the Valve 220 map format writes it
LevelFace(LevelPoint First, LevelPoint Second, LevelPoint Third, string Material, LevelPoint UAxis, double UOffset, LevelPoint VAxis, double VOffset, double Rotation, double ScaleU, double ScaleV);
LevelPoint First { get; init; }                          // The plane's first point
LevelPoint Second { get; init; }                         // Its second point
LevelPoint Third { get; init; }                          // Its third point
string Material { get; init; }                           // The material, a name with no spaces, which the face is drawn with as a material slot of that name
LevelPoint UAxis { get; init; }                          // The way across the world the material's width runs
double UOffset { get; init; }                            // How many texels the material is moved along its width
LevelPoint VAxis { get; init; }                          // The way its height runs
double VOffset { get; init; }                            // How many texels it is moved along its height
double Rotation { get; init; }                           // The turn in degrees the axes were given, which an editor shows and the axes hold turned
double ScaleU { get; init; }                             // How many of the file's units a texel spans along the width
double ScaleV { get; init; }                             // How many it spans along the height
LevelPoint Normal { get; }                               // The way out of the brush, a length of one, or zero where the three points lie on one line
double Distance { get; }                                 // How far the plane stands from the file's origin along its normal

record struct LevelPoint                                 // A point or a way of a level file, in the file's units with Z up, as the file writes it
LevelPoint(double X, double Y, double Z);
double X { get; init; }                                  // Along the file's X
double Y { get; init; }                                  // Along the file's Y
double Z { get; init; }                                  // Along the file's Z, which is up
double Length { get; }                                   // How long the way is, in the file's units
Vector3 Meters(double unitsPerMeter);                    // The point in meters with Y up, the file's (x, y, z) at (x, z, -y) and divided by its units a meter
LevelPoint FromMeters(Vector3 meters, double unitsPerMeter);  // The point a place in meters with Y up stands at in a file of the given units a meter
string ToString();                                       // The three numbers as a level file writes them, apart by spaces
LevelPoint operator Addition(LevelPoint a, LevelPoint b);  // The sum of two points or ways
LevelPoint operator Subtraction(LevelPoint a, LevelPoint b);  // The way from the second point to the first
LevelPoint operator Multiply(LevelPoint a, double s);    // A way scaled
double Dot(LevelPoint a, LevelPoint b);                  // The dot product of two ways, in the file's units squared
LevelPoint Cross(LevelPoint a, LevelPoint b);            // The cross product of two ways, right-handed in the file's axes, so X crossed with Y is Z

record struct LevelPlacement                             // Where a level's entity stands and how it is turned, in meters with Y up about the level's origin, as Placement gives it
LevelPlacement(Vector3 Position, Quaternion Rotation);
Vector3 Position { get; init; }                          // The place, in meters with Y up
Quaternion Rotation { get; init; }                       // The turn, which takes minus Z to the way the entity faces

class LevelGeometry                                      // An entity's brushes as the triangles an engine draws, a mesh a material, and as the convex hulls it collides with, in meters with Y up about a place, and the texel math and the pictures' sizes they are made with
IReadOnlyList<LevelMesh> Meshes(LevelEntity source, double unitsPerMeter, Vector3 origin, ILevelMaterials? materials = null);  // An entity's brushes as triangles, a mesh each material in the order the faces first name them
IReadOnlyList<LevelHull> Hulls(LevelEntity source, double unitsPerMeter, Vector3 origin);  // An entity's brushes as convex hulls, a hull each brush with a solid
double? Hit(IEnumerable<LevelBrush> brushes, LevelPoint from, LevelPoint way, double reach);  // How far along a ray it first meets any of the brushes, or nothing where it meets none within its reach, as Hit reads each
LevelTriangles Triangles(IReadOnlyList<LevelMesh> meshes);  // An entity's meshes as one shape of triangles, for a static body
Vector2 Texel(LevelFace face, LevelPoint point, double width, double height);  // Where a point of a face falls on its material, its texel along each axis, scaled and moved, over the texels its picture spans each way
(int, int)? PictureSize(ReadOnlySpan<byte> bytes);       // A PNG's or a JPEG's width and height from its header, or nothing for other bytes

record LevelMesh                                         // One material's share of an entity's brushes as triangles, in meters with Y up about a place, which an engine builds a mesh of to draw with the material of the same name
LevelMesh(string Material, Vector3[] Positions, Vector3[] Normals, Vector2[] Uvs, uint[] Indices);
string Material { get; init; }                           // The faces' material, as the level file names it
Vector3[] Positions { get; init; }                       // A corner of a face each, a face's corners its own
Vector3[] Normals { get; init; }                         // The way out of the face each corner belongs to
Vector2[] Uvs { get; init; }                             // Where each corner falls on the material's picture, its texels along the face's axes over the picture's size, the picture's top left at (0, 0) and V growing down it
uint[] Indices { get; init; }                            // Three corners a triangle, counter-clockwise seen from outside the brush

record LevelHull                                         // A brush's solid as its corners, in meters with Y up about a place, which an engine makes a convex collision shape of
LevelHull(Vector3[] Corners);
Vector3[] Corners { get; init; }                         // Each corner of the brush once, where three of its planes or more meet

record LevelTriangles                                    // An entity's triangles as one shape, in meters with Y up about the place its meshes were given about, which an engine makes a static body of, as Triangles gives them
LevelTriangles(Vector3[] Positions, uint[] Indices);
Vector3[] Positions { get; init; }                       // The corners, each mesh's after the ones before
uint[] Indices { get; init; }                            // Three corners a triangle, counter-clockwise seen from outside the brush

record struct LevelPicture                               // A material's picture as Find gives it
LevelPicture(string? Path, int Width, int Height);
string? Path { get; init; }                              // The picture's path among the engine's files, or nothing for a prototype's, which the engine draws itself
int Width { get; init; }                                 // Its width in texels
int Height { get; init; }                                // Its height in texels

record LevelLight                                        // A light of a level in physical units, as Light reads it from a light class's keys, for an engine to make a light of
string PointClass = "light";                             // The class of a point light
string SpotClass = "light_spot";                         // The class of a spot
string DirectionalClass = "light_environment";           // The class of the sun
double DefaultLumens = 800;                              // The lumens of a point or a spot with no intensity, a 60 watt bulb's
double DefaultLux = 100000;                              // The lux of a sun with no intensity, a clear noon's
double DefaultRange = 10;                                // The meters a point or a spot with no range reaches
double DefaultInnerCone = 25;                            // The inner cone of a spot with no _inner_cone, in degrees from its axis
double DefaultOuterCone = 30;                            // The outer cone of a spot with no _cone, in degrees from its axis
LevelLight(LevelLightKind Kind, Vector3 Position, Vector3 Direction, Vector3 Color, double Intensity, double Range, double InnerCone, double OuterCone);
LevelLightKind Kind { get; init; }                       // The kind, by the entity's class
Vector3 Position { get; init; }                          // The place, in meters with Y up
Vector3 Direction { get; init; }                         // The way the light faces, a length of one, where the entity's arrow points in TrenchBroom
Vector3 Color { get; init; }                             // The color in linear sRGB, each channel from 0 to 1
double Intensity { get; init; }                          // The strength, the lumens a point light sends over the whole sphere or a spot over its outer cone, or the lux the sun gives a surface facing it
double Range { get; init; }                              // How far in meters a point or a spot reaches, zero for the sun
double InnerCone { get; init; }                          // A spot's full strength out to this angle in degrees from its axis
double OuterCone { get; init; }                          // A spot's light out to this angle in degrees from its axis, fading from the inner one
double IlluminanceAt(double meters);                     // The lux the light gives a surface facing it at a distance, along a spot's axis, which says what the light's range means
Vector3 Linear(double red, double green, double blue);   // A color of 0 to 255 a channel in sRGB, as TrenchBroom's color picker writes it, in linear sRGB from 0 to 1, decoded by sRGB's own curve

enum LevelLightKind                                      // The kind of a level's light, by its class
Point = 0,                                               // A light, sending its light every way from its place
Spot = 1,                                                // A light_spot, sending its light in a cone about the way it faces
Directional = 2,                                         // A light_environment, the sun, its light falling the way it faces across the whole level, its place read by no engine

record struct LevelTree                                  // A tree a level places, by the name of its species and its seed, as Tree reads it from a tree entity's keys
string Class = "tree";                                   // The class of a tree
LevelTree(string? Species, uint Seed);
string? Species { get; init; }                           // The species' name as the level writes it, or nothing where it names none
uint Seed { get; init; }                                 // The seed, which picks its variant and turns and sizes it
```
