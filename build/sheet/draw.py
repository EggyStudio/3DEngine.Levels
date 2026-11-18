# Draws a level as the module gives it, from build/sheet's JSON, on a white ground, from above, the
# file's north up, each face lighter the higher it stands, its lights, a spot's way, the sun's and
# its entities marked by class; from an oblique eye; each triangle shaded flat by the way it faces a
# fixed sun, the nearest kept at each pixel; and the lux the lights give the floor, as the module
# reads it, on a scale of its own. It needs Pillow.
#
#   python3 -I build/sheet/draw.py <level.json> <above.png> <oblique.png> <lux.png>
import json
import math
import sys

from PIL import Image, ImageDraw, ImageFont

level = json.load(open(sys.argv[1]))
SIZE, MARGIN = 720, 56
SUN = (0.35, 0.85, 0.4)
COLORS = {"proto/floor": (150, 150, 156), "proto/wall": (208, 206, 200), "proto/door": (106, 176, 106), "proto/stairs": (232, 142, 64)}
DARK = (24, 24, 24)


def unit(v):
    length = math.sqrt(sum(c * c for c in v)) or 1
    return tuple(c / length for c in v)


def dot(a, b):
    return sum(x * y for x, y in zip(a, b))


def font(size):
    try:
        return ImageFont.truetype("DejaVuSans.ttf", size)
    except OSError:
        return ImageFont.load_default(size=size)


def view(right, up, toward, name, rise=0.0):
    """Draws the level seen along toward, with right and up the picture's axes, and returns how a
    point in meters maps onto the picture."""
    points = [p for mesh in level["meshes"] for t in mesh["triangles"] for p in t]
    xs = [dot(p, right) for p in points]
    ys = [dot(p, up) for p in points]
    scale = (SIZE - 2 * MARGIN) / max(max(xs) - min(xs), max(ys) - min(ys))
    cx, cy = (max(xs) + min(xs)) / 2, (max(ys) + min(ys)) / 2

    def place(p):
        return (SIZE / 2 + (dot(p, right) - cx) * scale, SIZE / 2 - (dot(p, up) - cy) * scale)

    sun = unit(SUN)
    heights = [p[1] for p in points]
    low, high = min(heights), max(heights)
    faces = []
    for mesh in level["meshes"]:
        base = COLORS.get(mesh["material"], (120, 120, 210))
        for t, n in zip(mesh["triangles"], mesh["normals"]):
            if dot(n, toward) >= 0:
                continue
            light = 0.4 + 0.6 * max(0.0, dot(unit(n), sun))
            if rise:
                height = sum(p[1] for p in t) / 3
                light *= 1 - rise + rise * (height - low) / ((high - low) or 1)
            faces.append(([dot(p, toward) for p in t], [place(p) for p in t], tuple(int(c * light) for c in base)))

    # Each pixel keeps the nearest triangle that covers it, its depth read across the triangle
    # from its corners', so a large face never hides a small one before it.
    image = Image.new("RGB", (SIZE, SIZE), (255, 255, 255))
    pixels = image.load()
    nearest = [[math.inf] * SIZE for _ in range(SIZE)]
    for depths, corners, color in faces:
        (ax, ay), (bx, by), (cx2, cy2) = corners
        area = (bx - ax) * (cy2 - ay) - (cx2 - ax) * (by - ay)
        if abs(area) < 1e-9:
            continue
        for y in range(max(0, int(min(ay, by, cy2))), min(SIZE, int(max(ay, by, cy2)) + 1)):
            for x in range(max(0, int(min(ax, bx, cx2))), min(SIZE, int(max(ax, bx, cx2)) + 1)):
                px, py = x + 0.5, y + 0.5
                w0 = ((bx - px) * (cy2 - py) - (cx2 - px) * (by - py)) / area
                w1 = ((cx2 - px) * (ay - py) - (ax - px) * (cy2 - py)) / area
                w2 = 1 - w0 - w1
                if min(w0, w1, w2) < -1e-6:
                    continue
                depth = w0 * depths[0] + w1 * depths[1] + w2 * depths[2]
                if depth < nearest[y][x]:
                    nearest[y][x] = depth
                    pixels[x, y] = color
    draw = ImageDraw.Draw(image)
    return image, draw, place, scale


def label(draw, at, text, size=19):
    # Dark text on a white backing, so a label reads over a dark floor as over the ground.
    where = (at[0] + 22, at[1] - 10)
    box = draw.textbbox(where, text, font=font(size))
    if box[2] > SIZE - 4:
        where = (at[0] - 22 - (box[2] - box[0]), at[1] - 10)
        box = draw.textbbox(where, text, font=font(size))
    draw.rectangle([box[0] - 3, box[1] - 2, box[2] + 3, box[3] + 2], fill=(255, 255, 255))
    draw.text(where, text, fill=DARK, font=font(size))


# From above, looking down minus Y, X to the right and minus Z, the file's north, up.
above, draw, place, scale = view((1, 0, 0), (0, 0, -1), (0, -1, 0), "above", rise=0.35)
for light in level["lights"]:
    at = place(light["at"])
    if light["kind"] == "Directional":
        way = light["way"]
        start = (SIZE - 90, 70)
        end = (start[0] + way[0] * 50, start[1] - (-way[2]) * 50)
        draw.line([start, end], fill=DARK, width=3)
        draw.ellipse([end[0] - 4, end[1] - 4, end[0] + 4, end[1] + 4], fill=DARK)
        label(draw, (start[0] - 70, start[1] + 26), "sun's way")
        continue
    draw.ellipse([at[0] - 6, at[1] - 6, at[0] + 6, at[1] + 6], fill=(240, 190, 0), outline=DARK)
    if light["kind"] == "Spot":
        way = light["way"]
        draw.line([at, (at[0] + way[0] * 40, at[1] + way[2] * 40)], fill=DARK, width=2)
        label(draw, at, f"spot, {light['range']:g} m, {light['outer']:g} degrees")
    else:
        label(draw, at, f"light, {light['range']:g} m")
for entity in level["entities"]:
    at = place(entity["at"])
    if entity["className"] == "info_player_start":
        f = entity["facing"]
        tip = (at[0] + f[0] * 18, at[1] + f[2] * 18)
        side = (-f[2] * 7, f[0] * 7)
        draw.polygon([tip, (at[0] + side[0], at[1] + side[1]), (at[0] - side[0], at[1] - side[1])], fill=(40, 90, 200))
        label(draw, at, "player start")
    else:
        draw.ellipse([at[0] - 7, at[1] - 7, at[0] + 7, at[1] + 7], fill=(60, 140, 60), outline=DARK)
        label(draw, at, entity["className"])
above.save(sys.argv[2])
flat = place

# From an oblique eye to the south-east, 40 degrees above the ground, looking at the room.
yaw, pitch = math.radians(-35), math.radians(55)
toward = (math.sin(yaw) * math.cos(pitch), -math.sin(pitch), -math.cos(yaw) * math.cos(pitch))
right = unit((math.cos(yaw), 0, math.sin(yaw)))
up = unit((right[1] * toward[2] - right[2] * toward[1], right[2] * toward[0] - right[0] * toward[2], right[0] * toward[1] - right[1] * toward[0]))
up = up if up[1] > 0 else tuple(-c for c in up)
oblique, draw, place, _ = view(right, up, toward, "oblique")
oblique.save(sys.argv[3])

# The lux on the floor, each tenth of a meter a cell, from above as the first view is, on a scale
# of a pale yellow at none to a dark red at the most, with its legend.
floor = level["floor"]
most = max(max(row) for row in floor["lux"])
ramp = [(255, 247, 214), (253, 200, 110), (240, 120, 50), (180, 40, 30), (90, 10, 20)]


def color(value):
    t = min(1.0, value / most) * (len(ramp) - 1)
    a, b = ramp[int(t)], ramp[min(int(t) + 1, len(ramp) - 1)]
    f = t - int(t)
    return tuple(int(a[c] + (b[c] - a[c]) * f) for c in range(3))


lux = Image.new("RGB", (SIZE, SIZE), (255, 255, 255))
draw = ImageDraw.Draw(lux)
for k, row in enumerate(floor["lux"]):
    for i, value in enumerate(row):
        x, z = floor["x0"] + i * floor["step"], floor["z0"] + k * floor["step"]
        a, b = flat((x, 0, z)), flat((x + floor["step"], 0, z + floor["step"]))
        draw.rectangle([a[0], a[1], b[0], b[1]], fill=color(value))
for light in level["lights"]:
    if light["kind"] == "Directional":
        continue
    at = flat(light["at"])
    draw.ellipse([at[0] - 6, at[1] - 6, at[0] + 6, at[1] + 6], fill=(255, 255, 255), outline=DARK, width=2)
for n in range(5):
    value = most * n / 4
    top = SIZE - MARGIN + 8
    left = MARGIN + n * 130
    draw.rectangle([left, top, left + 24, top + 16], fill=color(value), outline=DARK)
    draw.text((left + 30, top), f"{value:,.0f} lux", fill=DARK, font=font(19))
lux.save(sys.argv[4])
print(f"{sum(len(m['triangles']) for m in level['meshes'])} triangles, {len(level['lights'])} lights, {len(level['entities'])} entities, the most lux on the floor {most:,.1f}")
