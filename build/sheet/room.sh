#!/usr/bin/env bash
# Makes the example room's sheet from the module alone into a folder given: the room's meshes,
# lights and entities dumped as the module gives them, drawn from above, from an oblique eye and
# as the lux its lights give the floor, and laid out by Tools' sheet.py under the sheet's title.
#
#   build/sheet/room.sh <folder> <sheet.png>
set -euo pipefail
cd "$(dirname "$0")/../.."
folder="$1"
mkdir -p "$folder"
dotnet run -c Release --project build/sheet -- TrenchBroom/examples/room.map "$folder/room.json"
python3 -I build/sheet/draw.py "$folder/room.json" "$folder/above.png" "$folder/oblique.png" "$folder/lux.png"
cat > "$folder/layout.json" <<'LAYOUT'
{
  "title": "The example room, as the module gives it",
  "subtitle": "TrenchBroom/examples/room.map, its brushes' meshes shaded flat by one sun, its lights and entities read by the module's readers",
  "font": 16,
  "size": [480, 480],
  "columns": ["from above, north up", "from an oblique eye to the south-east", "lux its lights give the floor"],
  "rows": [
    {
      "panels": [
        {"image": "above.png"},
        {"image": "oblique.png"},
        {"image": "lux.png"}
      ]
    }
  ]
}
LAYOUT
python3 -I modules/Tools/sheets/sheet.py "$folder/layout.json" "$2"
