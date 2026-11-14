import re
import shutil

# Restore clean backup
shutil.copyfile("Assets/Scenes/UnifiedMuseum.unity.bak", "Assets/Scenes/UnifiedMuseum.unity")

with open("Assets/Scenes/UnifiedMuseum.unity", "r") as f:
    text = f.read()

# 8 artifacts specification
artifacts_data = [
    {
        "index": 1,
        "disp_goid": "2018301618",
        "slot_tfid": "2118659350",
        "slot_goid": "2118659349",
        "boxcol_id": "2118659351",
        "label_goid": "1014530650",
        "placeholder_cyl_goid": "509649221",
        "placeholder_lbl_goid": "693296044",
        "short_name": "TUTANKHAMUN MASK",
        "title": "Mask of Tutankhamun",
        "description": "The gold funerary mask of Pharaoh Tutankhamun (18th Dynasty, c. 1323 BCE), discovered in tomb KV62 in the Valley of the Kings by Howard Carter in 1922.\\n\\nCrafted from over 10 kilograms of solid gold inlaid with lapis lazuli, turquoise, carnelian, and quartz. The nemes headdress bears the protective cobra (Wadjet) and vulture (Nekhbet), symbolizing sovereign divine authority over Upper and Lower Egypt in the afterlife.",
        "prefab_guid": "89758c4a433ed694f9e6e33497c321c6",
        "source_prefab_fid": 100100000,
        "root_go_fid": 8971477287287058576,
        "root_tf_fid": 8631931142083881514,
        "pos": (0.0, 0.0, 0.0),
        "rot": (0.0, 0.0, 0.0, 1.0),
        "scale": (1.0, 1.0, 1.0),
        "euler": (0.0, 0.0, 0.0)
    },
    {
        "index": 2,
        "disp_goid": "1091040105",
        "slot_tfid": "2031548598",
        "slot_goid": "2031548597",
        "boxcol_id": "2031548599",
        "label_goid": "1057135368",
        "placeholder_cyl_goid": "1688953029",
        "placeholder_lbl_goid": "1271166517",
        "short_name": "ROYAL PHARAOH BUST",
        "title": "Royal Egyptian Pharaoh Bust",
        "description": "A royal portrait bust depicting an ancient Egyptian pharaoh in ceremonial splendor (New Kingdom, c. 14th Century BCE).\\n\\nCarved with realistic naturalism, wearing the striated royal nemes headcloth and divine ceremonial beard. Illustrates the monumental portraiture and divine kingship iconography characteristic of Egypt''s imperial golden age.",
        "prefab_guid": "185020f0b3bdbd14191b074cb8ea2fab",
        "source_prefab_fid": 100100000,
        "root_go_fid": 1575860890064231406,
        "root_tf_fid": 2203744068000137556,
        "pos": (0.0, 0.0, 0.0),
        "rot": (0.0, 0.0, 0.0, 1.0),
        "scale": (0.25, 0.25, 0.25),
        "euler": (0.0, 0.0, 0.0)
    },
    {
        "index": 3,
        "disp_goid": "943639282",
        "slot_tfid": "248765598",
        "slot_goid": "248765597",
        "boxcol_id": "248765599",
        "label_goid": "1692813932",
        "placeholder_cyl_goid": "311730729",
        "placeholder_lbl_goid": "384527343",
        "short_name": "KHASEKHEMWY STATUE",
        "title": "Statue of King Khasekhemwy",
        "description": "One of the earliest surviving monumental royal stone sculptures from ancient Egypt (Second Dynasty, c. 2686 BCE), excavated at Hierakonpolis and conserved at the Ashmolean Museum in Oxford.\\n\\nCarved from indurated limestone, depicting King Khasekhemwy seated upon a block throne wearing the White Crown of Upper Egypt and the ceremonial Sed festival jubilee robe.",
        "prefab_guid": "eb8ec654f92925745b414342aed1a567",
        "source_prefab_fid": -5645937942226959685,
        "root_go_fid": -6689036041284674117,
        "root_tf_fid": 2774220344278030320,
        "pos": (0.0, 0.0, 0.0),
        "rot": (-0.35355344, -0.6123724, -0.6123724, 0.35355344),
        "scale": (1.15, 1.15, 1.15),
        "euler": (-90.0, -90.0, -30.0)
    },
    {
        "index": 4,
        "disp_goid": "1430355068",
        "slot_tfid": "899826708",
        "slot_goid": "899826707",
        "boxcol_id": "899826709",
        "label_goid": "20007938",
        "placeholder_cyl_goid": "2096785239",
        "placeholder_lbl_goid": "1941264273",
        "short_name": "MARBLE LION STATUE",
        "title": "Classical Marble Resting Lion",
        "description": "A monumental resting feline guardian sculpture carved from crystalline Pentelic marble (Hellenistic / Classical Antiquity, c. 4th-2nd Century BCE).\\n\\nIn classical Greco-Roman architecture, sculpted lions guarded royal tomb precincts, sacred temples, and public monuments, standing as eternal symbols of vigilance, valor, and heroic divine protection.",
        "prefab_guid": "31b2707200ca8294fb7d8df166704d10",
        "source_prefab_fid": -5645937942226959685,
        "root_go_fid": -6689036041284674117,
        "root_tf_fid": 2774220344278030320,
        "pos": (0.0, 0.0, 0.0),
        "rot": (-0.03469948, -0.52155703, -0.7522114, 0.40118867),
        "scale": (0.12, 0.12, 0.12),
        "euler": (-54.339, -38.924, -103.294)
    },
    {
        "index": 5,
        "disp_goid": "699905736",
        "slot_tfid": "620111015",
        "slot_goid": "620111014",
        "boxcol_id": "620111016",
        "label_goid": "1394697470",
        "placeholder_cyl_goid": "1933502635",
        "placeholder_lbl_goid": "645496927",
        "short_name": "STORAGE AMPHORA",
        "title": "Terracotta Storage Amphora",
        "description": "A classical Mediterranean twin-handled pottery vessel (c. 5th Century BCE) wheel-thrown from coarse terracotta clay and finished with an authentic slip patina.\\n\\nFeaturing an elongated body, fluted neck, and vertical strap handles, amphorae served as the primary bulk shipping containers for transporting and aging vintage wines, cold-pressed olive oils, and grains across Aegean maritime trade routes.",
        "prefab_guid": "e8b3ed4c1829c18498e4a1c0c5ea8c59",
        "source_prefab_fid": 100100000,
        "root_go_fid": 5744401535643898786,
        "root_tf_fid": 60528537176808922,
        "pos": (0.0, 0.0, 0.0),
        "rot": (0.0, 0.0, 0.0, 1.0),
        "scale": (0.85, 0.85, 0.85),
        "euler": (0.0, 0.0, 0.0)
    },
    {
        "index": 6,
        "disp_goid": "1783466209",
        "slot_tfid": "967196758",
        "slot_goid": "967196757",
        "boxcol_id": "967196759",
        "label_goid": "1776284515",
        "placeholder_cyl_goid": "523378270",
        "placeholder_lbl_goid": "1142461047",
        "short_name": "CEREMONIAL HYDRIA",
        "title": "Ancient Ceremonial Hydria",
        "description": "A ceremonial Greek terracotta water pitcher (c. 4th Century BCE) characterized by a bulbous vessel body, flared pouring rim, and dual side lifting handles complemented by a central pouring handle.\\n\\nEssential to civic life and sanctuary rituals, hydriae were gathered at public fountain houses and dedicated at sacred shrines during ritual cleansing and libation ceremonies.",
        "prefab_guid": "fba144ca3c7523843baa21d13b1cbdd8",
        "source_prefab_fid": 100100000,
        "root_go_fid": 4367424571762002097,
        "root_tf_fid": 1381129405411579968,
        "pos": (0.0, 0.0, 0.0),
        "rot": (0.0, 0.0, 0.0, 1.0),
        "scale": (0.85, 0.85, 0.85),
        "euler": (0.0, 0.0, 0.0)
    },
    {
        "index": 7,
        "disp_goid": "1424585268",
        "slot_tfid": "797329793",
        "slot_goid": "797329792",
        "boxcol_id": "797329794",
        "label_goid": "1862496847",
        "placeholder_cyl_goid": "1394656853",
        "placeholder_lbl_goid": "1217277832",
        "short_name": "SYMPOSIUM KRATER",
        "title": "Classical Symposium Krater",
        "description": "A monumental earthenware mixing bowl (c. 6th-5th Century BCE) adorned with horizontal band motifs and a heavy fluted pedestal base.\\n\\nThe centerpiece of the ancient Greek symposium (social and philosophical banquet), the krater was utilized to dilute potent unfermented vintage wine with fresh spring water, honey, and aromatic herbs before serving participants.",
        "prefab_guid": "49465ec78864f5244b1d28d6d044ae7f",
        "source_prefab_fid": 100100000,
        "root_go_fid": 3149203889887882826,
        "root_tf_fid": 6169036615234257938,
        "pos": (0.0, 0.0, 0.0),
        "rot": (0.0, 0.0, 0.0, 1.0),
        "scale": (0.85, 0.85, 0.85),
        "euler": (0.0, 0.0, 0.0)
    },
    {
        "index": 8,
        "disp_goid": "552498212",
        "slot_tfid": "1817513578",
        "slot_goid": "1817513577",
        "boxcol_id": "1817513579",
        "label_goid": "2015268297",
        "placeholder_cyl_goid": "589241807",
        "placeholder_lbl_goid": "98716087",
        "short_name": "ALLOSAURUS FOSSIL",
        "title": "Allosaurus Fragilis Fossil Skeleton",
        "description": "A complete mounted articulated fossil skeleton of Allosaurus fragilis, the apex theropod dinosaur of the Late Jurassic Morrison Formation (approx. 155-145 Million BCE).\\n\\nDigitized from the Osaka Museum of Natural History collection. Features a lightweight kinetic skull, recurved serrated teeth, and lethal three-clawed raptorial forelimbs used to hunt massive sauropod dinosaurs.",
        "prefab_guid": "dfe4d0d098376ff458eb9680a8ca2979",
        "source_prefab_fid": -5645937942226959685,
        "root_go_fid": -6689036041284674117,
        "root_tf_fid": 2774220344278030320,
        "pos": (0.0, 0.5, 0.0),
        "rot": (-0.7071068, 0.0, 0.0, 0.7071068),
        "scale": (0.18, 0.18, 0.18),
        "euler": (-90.0, 0.0, 0.0)
    }
]

table_map = {item["disp_goid"]: item for item in artifacts_data}
label_go_map = {item["label_goid"]: item for item in artifacts_data}

# Placeholder objects to remove
placeholder_goids = set()
for a in artifacts_data:
    placeholder_goids.add(a["placeholder_cyl_goid"])
    placeholder_goids.add(a["placeholder_lbl_goid"])

# Read all blocks
blocks = text.split("--- !u!")
header_line = blocks[0]

# First pass: map which components belong to placeholder GOs so we can remove them
placeholder_fids = set(placeholder_goids)
for b in blocks[1:]:
    for p_go in placeholder_goids:
        if f"m_GameObject: {{fileID: {p_go}}}" in b:
            lines = b.splitlines()
            hdr = lines[0].split("&")
            if len(hdr) >= 2:
                placeholder_fids.add(hdr[1].strip())

print(f"Total placeholder components/objects to remove: {len(placeholder_fids)}")

new_blocks = [header_line]
exhibit_guid = "17e3aa1c604894595bb5009af718f836"

for b in blocks[1:]:
    lines = b.splitlines()
    hdr = lines[0]
    parts = hdr.split("&")
    tid = parts[0].strip()
    fid = parts[1].strip() if len(parts) > 1 else ""

    # Skip placeholder objects
    if fid in placeholder_fids:
        continue

    # 1. Update DISPLAY GameObject name
    if tid == "1" and fid in table_map:
        art = table_map[fid]
        new_lines = []
        for l in lines:
            if l.startswith("  m_Name:"):
                new_lines.append(f'  m_Name: "DISPLAY {art["index"]:02d} • {art["title"]}"')
            else:
                new_lines.append(l)
        new_blocks.append("\n".join(new_lines) + "\n")
        continue

    # 2. Update MuseumExhibit component
    if tid == "114" and exhibit_guid in b:
        m_go = re.search(r"m_GameObject:\s*\{fileID:\s*(-?[0-9]+)\}", b)
        if m_go and m_go.group(1) in table_map:
            art = table_map[m_go.group(1)]
            new_lines = []
            for l in lines:
                if l.startswith("  title:"):
                    new_lines.append(f'  title: {art["title"]}')
                elif l.startswith("  description:"):
                    new_lines.append(f"  description: '{art['description']}'")
                else:
                    new_lines.append(l)
            new_blocks.append("\n".join(new_lines) + "\n")
            continue

    # 3. Update Table slot label TextMeshPro text
    if tid == "114":
        m_go = re.search(r"m_GameObject:\s*\{fileID:\s*(-?[0-9]+)\}", b)
        if m_go and m_go.group(1) in label_go_map and "m_text:" in b:
            art = label_go_map[m_go.group(1)]
            new_lines = []
            for l in lines:
                if l.startswith("  m_text:"):
                    new_lines.append(f'  m_text: {art["index"]:02d} / {art["short_name"]}')
                else:
                    new_lines.append(l)
            new_blocks.append("\n".join(new_lines) + "\n")
            continue

    # 4. BoxCollider on ARTIFACT SLOT: size and center
    if tid == "65" and any(art["boxcol_id"] == fid for art in artifacts_data):
        new_lines = []
        for l in lines:
            if l.startswith("  m_Size:"):
                new_lines.append("  m_Size: {x: 1.9, y: 1.4, z: 1.3}")
            elif l.startswith("  m_Center:"):
                new_lines.append("  m_Center: {x: 0, y: 0.7, z: 0}")
            else:
                new_lines.append(l)
        new_blocks.append("\n".join(new_lines) + "\n")
        continue

    # 5. Slot Transform: clean m_Children of placeholder references
    if tid == "4" and any(art["slot_tfid"] == fid for art in artifacts_data):
        new_lines = []
        in_children = False
        for l in lines:
            if l.startswith("  m_Children:"):
                in_children = True
                new_lines.append("  m_Children: []")
            elif in_children and l.startswith("  - {fileID:"):
                # skip old placeholder child
                continue
            elif in_children and not l.startswith("  -"):
                in_children = False
                new_lines.append(l)
            else:
                new_lines.append(l)
        new_blocks.append("\n".join(new_lines) + "\n")
        continue

    # Default
    new_blocks.append(b)

# Generate PrefabInstances for all 8 artifacts
prefab_instances = []
base_instance_fid = 888100000

for i, art in enumerate(artifacts_data):
    inst_fid = base_instance_fid + i
    px, py, pz = art["pos"]
    rx, ry, rz, rw = art["rot"]
    sx, sy, sz = art["scale"]
    ex, ey, ez = art["euler"]

    pblock = f"""1001 &{inst_fid}
PrefabInstance:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Modification:
    serializedVersion: 3
    m_TransformParent: {{fileID: {art["slot_tfid"]}}}
    m_Modifications:
    - target: {{fileID: {art["root_go_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
      propertyPath: m_Name
      value: {art["short_name"]}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {art["root_tf_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
      propertyPath: m_LocalScale.x
      value: {sx}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {art["root_tf_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
      propertyPath: m_LocalScale.y
      value: {sy}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {art["root_tf_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
      propertyPath: m_LocalScale.z
      value: {sz}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {art["root_tf_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
      propertyPath: m_LocalPosition.x
      value: {px}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {art["root_tf_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
      propertyPath: m_LocalPosition.y
      value: {py}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {art["root_tf_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
      propertyPath: m_LocalPosition.z
      value: {pz}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {art["root_tf_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
      propertyPath: m_LocalRotation.w
      value: {rw}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {art["root_tf_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
      propertyPath: m_LocalRotation.x
      value: {rx}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {art["root_tf_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
      propertyPath: m_LocalRotation.y
      value: {ry}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {art["root_tf_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
      propertyPath: m_LocalRotation.z
      value: {rz}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {art["root_tf_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
      propertyPath: m_LocalEulerAnglesHint.x
      value: {ex}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {art["root_tf_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
      propertyPath: m_LocalEulerAnglesHint.y
      value: {ey}
      objectReference: {{fileID: 0}}
    - target: {{fileID: {art["root_tf_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
      propertyPath: m_LocalEulerAnglesHint.z
      value: {ez}
      objectReference: {{fileID: 0}}
    m_RemovedComponents: []
    m_RemovedGameObjects: []
    m_AddedGameObjects: []
    m_AddedComponents: []
  m_SourcePrefab: {{fileID: {art["source_prefab_fid"]}, guid: {art["prefab_guid"]}, type: 3}}
"""
    prefab_instances.append(pblock)

final_text = "--- !u!".join(new_blocks) + "\n" + "\n".join(f"--- !u!{p}" for p in prefab_instances)

with open("Assets/Scenes/UnifiedMuseum.unity", "w") as f:
    f.write(final_text)

print("UnifiedMuseum.unity has been fully populated and cleaned!")
