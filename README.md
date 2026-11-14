# Heritage VR — Virtual Museum Project

**Unity Version:** `2022.3.62f3`  
**Target Platforms:** Meta Quest 2 / Quest 3 / Quest 3S (Android OpenXR) & macOS / Windows Desktop Preview  
**Primary Scene:** `Assets/Scenes/UnifiedMuseum.unity`

---

## Project Overview

This project is a comprehensive Virtual Reality Museum built for academic and cultural heritage exploration. It presents a modeled museum gallery featuring **8 cultural artifacts** on dedicated display tables, complete exhibit information panels, 3 back-wall framed original paintings, and interactive inspection mechanics supporting both desktop and VR headset controls.

The project merges the assets, cultural models, and exhibits from the reference repository (`https://github.com/jalagam-saineha/VR-Virtual-Museum`) into a unified, high-performance architecture configured and validated for **Unity 2022.3.62f3 LTS**.

---

## Curated Museum Exhibits & 8 Artifacts

All eight display tables in the gallery are populated with authentic 3D models and curated historical descriptions:

| Table # | Exhibit Title | Historical Period / Provenance | Material & Cultural Significance |
|:---:|:---|:---|:---|
| **01** | **Mask of Tutankhamun** | New Kingdom, 18th Dynasty (c. 1323 BCE); Thebes / Valley of the Kings (KV62) | Solid gold inlaid with lapis lazuli, turquoise, carnelian, and quartz. Features the royal *nemes* headdress, divine false beard, and the protective uraeus cobra (*Wadjet*) and vulture (*Nekhbet*). |
| **02** | **Royal Egyptian Pharaoh Bust** | Ancient Egypt, New Kingdom (c. 14th Century BCE) | Polychrome limestone and plaster regal portraiture capturing idealized divine pharaonic features, ceremonial striped headcloth, and sacred beard. |
| **03** | **Statue of King Khasekhemwy** | Early Dynastic Period, 2nd Dynasty (c. 2686 BCE); Hierakonpolis / Ashmolean Museum Oxford | Fine indurated limestone carving depicting King Khasekhemwy enthroned in the ceremonial Sed festival jubilee robe and the White Crown of Upper Egypt (*Hedjet*). |
| **04** | **Classical Marble Resting Lion** | Classical / Hellenistic Antiquity (c. 4th–2nd Century BCE) | Crystalline Pentelic marble sculpture of a resting feline sentinel, traditionally erected at monumental tomb precincts and temple entrances as symbols of vigilance, valor, and sacred guardianship. |
| **05** | **Terracotta Storage Amphora (Pot I)** | Classical Mediterranean (c. 5th Century BCE) | Wheel-thrown terracotta with authentic slip patina, featuring an elongated body and double vertical strap handles used for aging and bulk maritime transport of olive oil, vintages, and grain. |
| **06** | **Ancient Ceremonial Hydria (Pot II)** | Ancient Mediterranean (c. 4th Century BCE) | Bulbous earthenware water vessel featuring flared pouring lip and dual carrying handles complemented by a central pouring handle, utilized in temple purification and domestic civic rituals. |
| **07** | **Classical Symposium Krater (Pot III)** | Classical Greece (c. 6th–5th Century BCE) | Large wide-mouthed earthenware vessel with fluted pedestal base, central to ancient Greek symposia (philosophical banquets) for blending vintage wine with spring water and herbs before serving guests. |
| **08** | **Allosaurus Fragilis Fossil Skeleton** | Late Jurassic Period (approx. 155–145 Million BCE); Morrison Formation | Complete mounted articulated theropod fossil skeleton digitized from the Osaka Museum of Natural History, featuring lightweight kinetic cranial structure, recurved serrated teeth, and three-clawed raptorial forelimbs. |

### Back-Wall Painting Collection
Three original framed artworks in brass frames accent the gallery's rear wall:
1. **River of Memory** — Atmospheric temple landscape at dawn along the Nile.
2. **Vessels & Arches** — Lyrical composition of terracotta vessels, geometry, and arches.
3. **Lotus at Dusk** — Serene lotus pond with twilight hills.

---

## Interaction & Controls

### Desktop Preview (Keyboard & Mouse)
- **Start Experience:** Press `Enter` on the entrance screen.
- **Locomotion:** `W` / `A` / `S` / `D` keys to walk through the gallery.
- **Look Around:** Hold **Right Mouse Button** and drag.
- **Select Exhibit:** Aim the pointer ray at any display table, artifact, or front plaque and **Left Click**.
- **Inspect Mode:** With an exhibit selected, press `I` (or click `INSPECT`) to teleport to the exhibit's close-up viewing station.
- **Rotate Artifact:** In Inspect mode, press `A` or `D` to rotate the artifact on its turntable.
- **Return / Back:** Press `Escape` (or click `BACK`) to return from inspection or return to the main gallery.

### Meta Quest VR Headset (Touch / Touch Plus Controllers)
- **Select & Interact:** Aim the right controller ray at buttons, exhibits, or tables and pull the **Right Trigger**.
- **Smooth / Snap Turn:** Flick the **Right Thumbstick** left or right (30° snap rotation).
- **Collision-Aware Step:** Push the **Left Thumbstick** forward and release for comfortable step movement.
- **Inspect Mode:** Select an exhibit and choose Inspect. Move the **Right Thumbstick** left/right to spin the artifact 360° in world space.
- **Return:** Press the **B Button** on the right controller to return from inspection or back to entrance.

---

## Project Structure & Merged Content

```
Museum-VR-main/
├── Assets/
│   ├── AK Studio Art/              # Egyptian Pharaoh Bust models, prefabs, materials, and textures
│   ├── Ancient Pots/               # Ancient Pots (pot_1 through pot_4) meshes, materials, and prefabs
│   ├── ThreeDragons/               # Tutankhamun Mask model, prefab, textures, and materials
│   ├── Models/                     # High-fidelity 3D models (Allosaurus, Marble Lion, Khasekhemwy)
│   ├── Images/                     # Museum reference images
│   ├── Materials/                  # Core gallery architectural materials
│   ├── HeritageVR/                 # Museum runtime scripts, shaders, fonts, and triptych paintings
│   │   ├── Scripts/                # MuseumExperience, MuseumExhibit, MuseumAction, MuseumText
│   │   ├── Paintings/              # HeritageTriptych texture atlas and custom materials
│   │   ├── Materials/              # URP materials (limestone, walnut, brass, porcelain)
│   │   └── Shaders/                # Headset vignette and museum text shaders
│   ├── Editor/                     # Editor tooling:
│   │   ├── MuseumArtifactSetup.cs  # Automates setup & verification of all 8 exhibits
│   │   ├── QuestMuseumSetup.cs     # OpenXR configuration and Quest APK build pipeline
│   │   └── MuseumPlaceholders.cs   # Placeholder utility
│   └── Scenes/
│       ├── UnifiedMuseum.unity     # [PRIMARY] Unified museum with 8 artifacts & entrance UI
│       ├── MuseumScene.unity       # Merged original gallery scene from reference repository
│       ├── BasicScene.unity        # Merged basic VR scene
│       └── SampleScene.unity       # Merged XR interaction sample scene
├── Packages/
│   └── manifest.json               # Configured for Unity 2022.3.62f3 LTS (URP 14.0.11, OpenXR 1.17.1, glTFast)
└── ProjectSettings/                # Project settings validated for Quest 2/3/3S Android OpenXR & Desktop
```

---

## Unity Editor Setup & Running the Project

1. Open **Unity Hub**.
2. Click **Add** > **Add project from disk** and select `/Users/tanishkadhaka/Downloads/Museum-VR-main`.
3. Select Unity Editor version **`2022.3.62f3`**.
4. Once the project opens in Unity:
   - In the Project window, navigate to `Assets/Scenes/UnifiedMuseum.unity` and double-click to open it.
   - Click the **Play** button at the top of the Unity Editor to explore the museum in Desktop mode.
   - You can also use the custom menu item **Museum > 12 Populate 8 Artifacts** to re-run the exhibit population script at any time.

---

## Standalone Meta Quest Build

The project includes build tooling for Meta Quest 2, Quest 3, and Quest 3S:
- **Build Target:** Android ARM64, IL2CPP, OpenGL ES 3, OpenXR runtime.
- **OpenXR Profiles:** Meta Quest Touch / Touch Plus controller profiles enabled.
- **One-Click Build:** In the Unity menu, select **`Museum > 09 Build Quest APK`**.
- **Installation via ADB:**
  ```bash
  adb install -r HeritageVR-Quest.apk
  adb shell monkey -p com.heritagevr.virtualmuseum -c android.intent.category.LAUNCHER 1
  ```
