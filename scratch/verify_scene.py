import re

with open("Assets/Scenes/UnifiedMuseum.unity") as f:
    text = f.read()

blocks = text.split("--- !u!")
gameobjects = {}
for b in blocks:
    hdr = b.splitlines()[0].split("&")
    if len(hdr) >= 2 and hdr[0].strip() == "1":
        fid = hdr[1].strip()
        for l in b.splitlines():
            if "m_Name:" in l:
                gameobjects[fid] = l.split(":", 1)[1].strip()
                break

guid = "17e3aa1c604894595bb5009af718f836"
exhibits = []
for b in blocks:
    if guid in b:
        goid = re.search(r"m_GameObject:\s*\{fileID:\s*(-?[0-9]+)\}", b).group(1)
        name = gameobjects.get(goid, "")
        title = re.search(r"title:\s*([^\n\r]+)", b).group(1)
        art = re.search(r"artifact:\s*\{fileID:\s*(-?[0-9]+)\}", b).group(1)
        exhibits.append((name, title, art))

print("=== 8 EXHIBITS IN UnifiedMuseum.unity ===")
for name, title, art in sorted(exhibits):
    print(f"Exhibit: {name}")
    print(f"  Title: {title}")
    print(f"  Slot TF: {art}")

print("\n=== PREFAB INSTANCES ===")
for b in blocks:
    if b.startswith("1001 &"):
        hdr = b.splitlines()[0]
        sp = re.search(r"guid:\s*([a-f0-9]+)", b)
        tp = re.search(r"m_TransformParent:\s*\{fileID:\s*(-?[0-9]+)\}", b)
        name = re.search(r"value:\s*([^\n\r]+)", b)
        n_str = name.group(1) if name else "unknown"
        tp_str = tp.group(1) if tp else "0"
        guid_str = sp.group(1) if sp else "unknown"
        print(f"PrefabInstance {hdr}: {n_str}, parent={tp_str}, guid={guid_str}")

print("\n=== SLOT LABELS ===")
for b in blocks:
    if "Table slot label" in b and "m_text:" in b:
        txt = re.search(r"m_text:\s*([^\n\r]+)", b)
        if txt:
            print("  Label:", txt.group(1))

