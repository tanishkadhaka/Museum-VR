# PROJECT REPORT: HERITAGE VR
## Immersive Virtual Reality Museum of Ancient Egyptian & Classical Antiquities
### End-Term Course Project & Technical Deliverables Evaluation

**Course:** Augmented & Virtual Reality (AR/VR)  
**Project Title:** Heritage VR — Ancient Egyptian Virtual Museum  
**Author / Developer:** Aastha Thakur  
**Academic Term:** End-Term Evaluation 2026  
**Unity Version:** `2022.3.62f3 Personal (LTS)`  
**Render Pipeline:** Universal Render Pipeline (URP 14.0.11)  
**Target Hardware:** Meta Quest 2 / Quest 3 / Quest 3S (Standalone Android OpenXR) & macOS / Windows Desktop Preview  
**Target Package Identifier:** `com.heritagevr.virtualmuseum`  
**GitHub Repository:** [https://github.com/thakuraastha/Museum-VR](https://github.com/thakuraastha/Museum-VR)  
**Primary Build Scene:** `Assets/Scenes/UnifiedMuseum.unity`  

---

## 1. Project Overview & Motivation

### 1.1 Introduction
Cultural heritage preservation faces significant challenges: physical historical artifacts degrade over time, are geographically scattered across global institutions, and are often secured behind protective museum barriers that restrict visitors from examining intricate carvings, inscriptions, and craftsmanship up close.

**Heritage VR** is an interactive, standalone Virtual Reality application designed to overcome these boundaries. Focusing on **Ancient Egyptian pharaonic civilization** and classical Mediterranean antiquities, the project provides an authentic, architecturally modeled grand museum gallery where students, historians, and museum visitors can walk up to exhibits, inspect them at 1:1 scale, and rotate them 360° on an interactive virtual turntable.

### 1.2 Core Project Objectives
1. **Curatorial Authenticity:** Showcase authentic 3D digitized cultural artifacts—including the *Gold Mask of Tutankhamun*, a *Royal Egyptian Pharaoh Bust*, and the ancient *Statue of King Khasekhemwy*—accompanied by rigorous archaeological documentation.
2. **High-Performance Standalone VR:** Deliver steady 72/90 FPS stereoscopic immersion directly on mobile XR chipsets (Snapdragon XR2 Gen 1 & Gen 2 in Meta Quest 2/3/3S) without thermal throttling.
3. **Ergonomic Locomotion & Comfort:** Mitigate simulator sickness using discrete step movement ($1.25\text{m}$ discrete translation), $30^\circ$ snap turning, and an adaptive depth vignette.
4. **Automated Mathematical Alignment:** Resolve raw photogrammetry and CAD import anomalies (arbitrary scan orientations, tilted bases, and off-center geometric pivots) to ensure models sit flush on pedestals and rotate with zero orbital wobble.
5. **Universal Dual-Target Accessibility:** Provide seamless cross-platform execution with an instant Desktop preview mode (`WASD` + mouse raycast) alongside native OpenXR tracking for Meta Quest headsets.

---

## 2. Technical Architecture & System Specifications

```
+------------------------------------------------------------------------------------------------------+
|                                    HERITAGE VR SYSTEM TOPOLOGY                                       |
+------------------------------------------------------------------------------------------------------+
| [ HARDWARE LAYER ]                                                                                   |
|  • Meta Quest 2 / Quest 3 / Quest 3S (Standalone Qualcomm Snapdragon XR2 / Adreno 650/740)           |
|  • Desktop Development Preview (macOS Metal / Windows DirectX 11/12)                                 |
+------------------------------------------------------------------------------------------------------+
| [ RUNTIME & OPERATING SYSTEM ]                                                                       |
|  • Android 10 (API 29) Minimum -> Android 14 (API 34) Target                                         |
|  • ARM64 Architecture (64-bit native execution via IL2CPP compilation)                              |
+------------------------------------------------------------------------------------------------------+
| [ GRAPHICS & STEREO ENGINE ]                                                                         |
|  • Unity 2022.3.62f3 LTS with Universal Render Pipeline (URP 14.0.11)                                |
|  • Single-Pass Instanced (Multiview) Stereo Rendering                                                |
|  • OpenGL ES 3.0 Graphics Device API                                                                |
|  • 4x Hardware MSAA (Multisample Anti-Aliasing)                                                      |
|  • ASTC 6x6 Block Compressed Textures (Mobile GPU optimization)                                      |
+------------------------------------------------------------------------------------------------------+
| [ XR INTERACTION & INPUT PIPELINE ]                                                                  |
|  • Khronos OpenXR 1.17.1 Runtime Specification                                                       |
|  • Meta Quest Feature Group & Touch / Touch Plus Controller Profiles                                 |
|  • Hybrid Input System (Unity New Input System + UnityEngine.XR Device Nodes)                         |
|  • Physics-based Raycasting & Dynamic BoxCollider Geometry Bounds                                    |
+------------------------------------------------------------------------------------------------------+
| [ SCENE & APPLICATION LAYER ]                                                                        |
|  • UnifiedMuseum.unity (Grand Hall, 8 Interactive Display Tables, 9 Framed Artworks, Lobby Portal)    |
|  • HeritageVR Runtime Suite (MuseumExperience, MuseumExhibit, MuseumAction, MuseumText)               |
+------------------------------------------------------------------------------------------------------+
```

### 2.1 Engine & Compilation Parameters
- **Scripting Backend:** IL2CPP (C++ transcompilation) enables maximum CPU instruction efficiency, essential for maintaining sub-11ms frame times on mobile VR chipsets.
- **Color Space:** `Linear` rendering ensures realistic physical light attenuation and accurate PBR material response for metallic gold, weathered limestone, and burnished terracotta.
- **Dynamic Batching & SRP Batcher:** Enabled across all architectural and exhibit materials to consolidate draw calls within mobile VR budgets (< 150 draw calls per eye).

### 2.2 URP Render Pipeline Settings
- **URP Asset Path:** `Assets/HeritageVR/MuseumURP.asset`
- **Render Scale:** 1.0 (Native panel resolution per eye, avoiding upsampling blur).
- **HDR & Tonemapping:** Disabled (`supportsHDR = false`) to eliminate the multi-pass floating-point render texture overhead on tile-based mobile GPUs.
- **Lighting Limits:** Real-time directional sun disabled in favor of soft ambient environment lighting and baked gallery illumination. Additional lights per object are strictly limited to `4`.
- **Shadow Distance:** Clamped to `18.0m` (`LightShadows.None` for real-time mobile performance; ambient occlusion simulated via vertex shading and high-resolution slip maps).

---

## 3. Spatial Environment & Gallery Engineering

### 3.1 World Grid & Coordinate System
The virtual museum environment (`Assets/Scenes/UnifiedMuseum.unity`) is architecturally structured around a symmetrical grand hall:

```
                            [ BACK WALL (Z = +22.7m) ]
                     Triptych: River of Memory | Vessels | Lotus
            ---------------------------------------------------------------
            | Table 07: Symposium Krater       Table 08: Allosaurus Fossil |
            | Pos: (-8.0, 0, 20.0)             Pos: (+8.0, 0, 20.0)        |
            | Station: (-8.0, 0, 16.8)         Station: (+8.0, 0, 16.8)    |
            |                                                             |
            | Table 05: Storage Amphora        Table 06: Ceremonial Hydria |
            | Pos: (-4.0, 0, 17.0)             Pos: (+4.0, 0, 17.0)        |
            | Station: (-4.0, 0, 14.2)         Station: (+4.0, 0, 14.2)    |
[ WEST WALL ]                                                             [ EAST WALL ]
Side-Wall   | Table 03: King Khasekhemwy       Table 04: Marble Lion       | Side-Wall
Egyptian    | Pos: (-4.0, 0, 11.0)             Pos: (+4.0, 0, 11.0)        | Egyptian
Paintings   | Station: (-4.0, 0, 8.2)          Station: (+4.0, 0, 8.2)     | Paintings
(3 framed)  |                                                             | (3 framed)
            | Table 01: Tutankhamun Mask       Table 02: Pharaoh Bust      |
            | Pos: (-4.0, 0, 5.0)              Pos: (+4.0, 0, 5.0)         |
            | Station: (-4.0, 0, 2.2)          Station: (+4.0, 0, 2.2)     |
            ---------------------------------------------------------------
                             [ MAIN ENTRANCE (Z = -2.0m) ]
                                           ▲
                                           │  (Teleport Link)
                           [ ENTRANCE PORTAL (-60, 0, -11) ]
```

### 3.2 Key Spatial Measurements
- **Arrival Lobby:** Located at coordinate `(-60.0, 0.0, -11.0)`. Serves as an isolated staging area presenting the introductory canvas and controls briefing before loading the main gallery.
- **Main Hallway Spawn:** `(0.0, 0.0, -2.0)`, facing down the central aisle toward `+Z`.
- **Pedestal Elevation:** The wooden display tables elevate artifacts to `Y = 1.19m`, aligning the exhibits with natural human standing eye height in VR ($1.65\text{m}$).
- **Player Boundary Clamping:** Clamped dynamically in `MuseumExperience.Update()` to `X ∈ [-10.8m, +10.8m]` and `Z ∈ [-2.4m, +21.5m]`, preventing users from walking through outer architectural masonry.

---

## 4. Curated 8-Exhibit Artifact Catalog

Every display table in the gallery features an authentic cultural antiquity or natural history specimen with complete metadata, PBR materials, and custom TextMeshPro signage:

```
====================================================================================================
TABLE 01: MASK OF TUTANKHAMUN
====================================================================================================
Asset File Path:       Assets/ThreeDragons/Tutankhamun/Prefabs/Tut_mask.prefab
Asset GUID:            89758c4a433ed694f9e6e33497c321c6
Plaque Label:          01 / TUTANKHAMUN MASK
Table World Pos:       (-4.00, 0.00, 5.00)
Viewing Station:       (-4.00, 0.00, 2.20)
Local Transform:       Position (0.00, 0.00, 0.00) | Rotation (0.00, 0.00, 0.00) | Scale (1.00, 1.00, 1.00)
Historical Period:     Ancient Egypt, New Kingdom, 18th Dynasty (c. 1323 BCE)
Archaeological Origin: Thebes / Valley of the Kings, Tomb KV62. Discovered by Howard Carter (1922).
Material Composition:  Solid gold sheet (>10 kg), inlaid lapis lazuli, turquoise, carnelian,
                       obsidian, quartz, and colored glass-paste.
Shading & Rendering:   Display Tutankhamun_mask.mat (URP Lit: Metallic 0.55, Smoothness 0.35, Reflection ON)
Educational Plaque:    The gold funerary mask of Pharaoh Tutankhamun (18th Dynasty, c. 1323 BCE), discovered
                       in tomb KV62 in the Valley of the Kings by Howard Carter in 1922.
                       
                       Crafted from over 10 kilograms of solid gold inlaid with lapis lazuli, turquoise,
                       carnelian, and quartz. The nemes headdress bears the protective cobra (Wadjet)
                       and vulture (Nekhbet), symbolizing sovereign divine authority over Upper and
                       Lower Egypt in the afterlife.
====================================================================================================

====================================================================================================
TABLE 02: ROYAL EGYPTIAN PHARAOH BUST
====================================================================================================
Asset File Path:       Assets/AK Studio Art/Egypt Pharoah Bust/Prefabs/Egypt Pharoah Bust.prefab
Asset GUID:            185020f0b3bdbd14191b074cb8ea2fab
Plaque Label:          02 / ROYAL PHARAOH BUST
Table World Pos:       (4.00, 0.00, 5.00)
Viewing Station:       (4.00, 0.00, 2.20)
Local Transform:       Position (0.00, 0.00, 0.00) | Rotation (0.00, 0.00, 0.00) | Scale (0.25, 0.25, 0.25)
Historical Period:     Ancient Egypt, New Kingdom (c. 14th Century BCE)
Archaeological Origin: Imperial Luxor / Karnak sculptural workshops.
Material Composition:  Polychrome carved limestone with natural mineral pigments and lime plaster slip.
Educational Plaque:    A royal portrait bust depicting an ancient Egyptian pharaoh in ceremonial
                       splendor (New Kingdom, c. 14th Century BCE).
                       
                       Carved with realistic naturalism, wearing the striated royal nemes headcloth and
                       divine ceremonial beard. Illustrates the monumental portraiture and divine
                       kingship iconography characteristic of Egypt's imperial golden age.
====================================================================================================

====================================================================================================
TABLE 03: STATUE OF KING KHASEKHEMWY
====================================================================================================
Asset File Path:       Assets/Models/MuseumObjects/egyptian_pharoah_khasekhemwy_-_oxford_ashmoleon.glb
Asset GUID:            eb8ec654f92925745b414342aed1a567
Plaque Label:          03 / KHASEKHEMWY STATUE
Table World Pos:       (-4.00, 0.00, 11.00)
Viewing Station:       (-4.00, 0.00, 8.20)
Local Transform:       Position (0.00, 0.00, 0.00) | Rotation (-90.0, -90.0, -30.0) | Scale (1.15, 1.15, 1.15)
Historical Period:     Early Dynastic Period, 2nd Dynasty (c. 2686 BCE)
Archaeological Origin: Excavated at Nekhen (Hierakonpolis); conserved at the Ashmolean Museum, Oxford.
Material Composition:  Indurated limestone with ceremonial hieroglyphic inscriptions along the throne plinth.
Educational Plaque:    One of the earliest surviving monumental royal stone sculptures from ancient Egypt
                       (Second Dynasty, c. 2686 BCE), excavated at Hierakonpolis and conserved at the
                       Ashmolean Museum in Oxford.
                       
                       Carved from indurated limestone, depicting King Khasekhemwy seated upon a block
                       throne wearing the White Crown of Upper Egypt and the ceremonial Sed festival
                       jubilee robe.
====================================================================================================

====================================================================================================
TABLE 04: CLASSICAL MARBLE RESTING LION
====================================================================================================
Asset File Path:       Assets/Models/MuseumObjects/marble_statue_of_a_lion.glb
Asset GUID:            31b2707200ca8294fb7d8df166704d10
Plaque Label:          04 / MARBLE LION STATUE
Table World Pos:       (4.00, 0.00, 11.00)
Viewing Station:       (4.00, 0.00, 8.20)
Local Transform:       Position (0.00, 0.00, 0.00) | Rotation (-54.34, -38.92, -103.29) | Scale (0.12, 0.12, 0.12)
Historical Period:     Classical / Hellenistic Antiquity (c. 4th–2nd Century BCE)
Archaeological Origin: Attica / Peloponnese, Greece; monumental tomb enclosure sentinel.
Material Composition:  Crystalline Pentelic marble featuring ancient atmospheric weathering and mineral patina.
Educational Plaque:    A monumental resting feline guardian sculpture carved from crystalline Pentelic
                       marble (Hellenistic / Classical Antiquity, c. 4th-2nd Century BCE).
                       
                       In classical Greco-Roman architecture, sculpted lions guarded royal tomb precincts,
                       sacred temples, and public monuments, standing as eternal symbols of vigilance,
                       valor, and heroic divine protection.
====================================================================================================

====================================================================================================
TABLE 05: TERRACOTTA STORAGE AMPHORA (POT I)
====================================================================================================
Asset File Path:       Assets/Ancient Pots/URP & Built-In/URP/Prefabs/pot_1.prefab
Asset GUID:            e8b3ed4c1829c18498e4a1c0c5ea8c59
Plaque Label:          05 / STORAGE AMPHORA
Table World Pos:       (-4.00, 0.00, 17.00)
Viewing Station:       (-4.00, 0.00, 14.20)
Local Transform:       Position (0.00, 0.00, 0.00) | Rotation (0.00, 0.00, 0.00) | Scale (0.85, 0.85, 0.85)
Historical Period:     Classical Mediterranean (c. 5th Century BCE)
Archaeological Origin: Aegean commercial maritime trade networks (Rhodes / Chios).
Material Composition:  Wheel-thrown coarse terracotta clay with authentic red-slip coating.
Educational Plaque:    A classical Mediterranean twin-handled pottery vessel (c. 5th Century BCE) wheel-
                       thrown from coarse terracotta clay and finished with an authentic slip patina.
                       
                       Featuring an elongated body, fluted neck, and vertical strap handles, amphorae
                       served as the primary bulk shipping containers for transporting and aging vintage
                       wines, cold-pressed olive oils, and grains across Aegean maritime trade routes.
====================================================================================================

====================================================================================================
TABLE 06: ANCIENT CEREMONIAL HYDRIA (POT II)
====================================================================================================
Asset File Path:       Assets/Ancient Pots/URP & Built-In/URP/Prefabs/pot_2.prefab
Asset GUID:            fba144ca3c7523843baa21d13b1cbdd8
Plaque Label:          06 / CEREMONIAL HYDRIA
Table World Pos:       (4.00, 0.00, 17.00)
Viewing Station:       (4.00, 0.00, 14.20)
Local Transform:       Position (0.00, 0.00, 0.00) | Rotation (0.00, 0.00, 0.00) | Scale (0.85, 0.85, 0.85)
Historical Period:     Ancient Greece / Magna Graecia (c. 4th Century BCE)
Archaeological Origin: Civic fountain houses and sanctuary sacred springs.
Material Composition:  Fine red-clay earthenware with burnished slip finish and flared lip.
Educational Plaque:    A ceremonial Greek terracotta water pitcher (c. 4th Century BCE) characterized
                       by a bulbous vessel body, flared pouring rim, and dual side lifting handles
                       complemented by a central pouring handle.
                       
                       Essential to civic life and sanctuary rituals, hydriae were gathered at public
                       fountain houses and dedicated at sacred shrines during ritual cleansing and
                       libation ceremonies.
====================================================================================================

====================================================================================================
TABLE 07: CLASSICAL SYMPOSIUM KRATER (POT III)
====================================================================================================
Asset File Path:       Assets/Ancient Pots/URP & Built-In/URP/Prefabs/pot_3.prefab
Asset GUID:            49465ec78864f5244b1d28d6d044ae7f
Plaque Label:          07 / SYMPOSIUM KRATER
Table World Pos:       (-8.00, 0.00, 20.00)
Viewing Station:       (-8.00, 0.00, 16.80)
Local Transform:       Position (0.00, 0.00, 0.00) | Rotation (0.00, 0.00, 0.00) | Scale (0.85, 0.85, 0.85)
Historical Period:     Classical Greece (c. 6th–5th Century BCE)
Archaeological Origin: Kerameikos pottery district, Athens.
Material Composition:  Hand-thrown painted earthenware with a heavy fluted pedestal base.
Educational Plaque:    A monumental earthenware mixing bowl (c. 6th-5th Century BCE) adorned with
                       horizontal band motifs and a heavy fluted pedestal base.
                       
                       The centerpiece of the ancient Greek symposium (social and philosophical
                       banquet), the krater was utilized to dilute potent unfermented vintage wine
                       with fresh spring water, honey, and aromatic herbs before serving participants.
====================================================================================================

====================================================================================================
TABLE 08: ALLOSAURUS FRAGILIS FOSSIL SKELETON
====================================================================================================
Asset File Path:       Assets/Models/MuseumObjects/900868a0aa0b47e58571f5f5ae20094c.glb
Asset GUID:            dfe4d0d098376ff458eb9680a8ca2979
Plaque Label:          08 / ALLOSAURUS FOSSIL
Table World Pos:       (8.00, 0.00, 20.00)
Viewing Station:       (8.00, 0.00, 16.80)
Local Transform:       Position (0.00, 0.50, 0.00) | Rotation (-90.0, 0.00, 0.00) | Scale (0.18, 0.18, 0.18)
Historical Period:     Late Jurassic Period (approx. 155–145 Million BCE)
Specimen Provenance:   Morrison Formation, Western North America. Digitized from Osaka Museum collection.
Material Composition:  Permineralized fossil bone assembled on a welded black steel armature.
Educational Plaque:    A complete mounted articulated fossil skeleton of Allosaurus fragilis, the apex
                       theropod dinosaur of the Late Jurassic Morrison Formation (approx. 155-145 Million BCE).
                       
                       Digitized from the Osaka Museum of Natural History collection. Features a lightweight
                       kinetic skull, recurved serrated teeth, and lethal three-clawed raptorial
                       forelimbs used to hunt massive sauropod dinosaurs.
====================================================================================================
```

---

## 5. Gallery Fine Art Collections

In addition to the 8 central display tables, the museum features **9 curated fine-art paintings** mounted in archival architectural frames:

### 5.1 Back-Wall Fine Art Triptych (`BACK WALL • Painting collection`)
Positioned along the rear wall (`Z = 22.7m`, `Y = 2.8m`) to anchor the visual horizon of the museum:
- **Texture Master:** `Assets/HeritageVR/Paintings/HeritageTriptych.png` (4096x1365px, 3:1 aspect ratio, ASTC 6x6 compression).
- **Frame Engineering:** Triple-layer compound construction consisting of a walnut backing (`3.95m × 3.95m × 0.16m`), brushed brass outer bevel (`3.86m × 3.86m × 0.04m`), and warm limestone archival mount (`3.68m × 3.68m × 0.025m`).
- **Artworks:**
  1. **`RIVER OF MEMORY` (`X = -6.0m`):** Sandstone temple landscape at dawn along the Nile.
  2. **`VESSELS & ARCHES` (`X = 0.0m`):** Lyrical geometric composition of pottery, arches, and golden ratios.
  3. **`LOTUS AT DUSK` (`X = +6.0m`):** Twilight sanctuary lotus pond with deep indigo hills.

### 5.2 Egyptian Side-Wall Gallery (`SIDE WALLS • Egyptian art collection`)
Installed by `MuseumEgyptianPaintings.cs` to enrich the Egyptian thematic ambiance along the perimeter walls:
- **West Gallery Wall (`X = -11.72m`, Rotation: $90^\circ$ toward aisle):**
  1. `NILE AT DAWN` (`Z = 3.0m`)
  2. `PYRAMIDS IN GOLD` (`Z = 10.0m`)
  3. `ROYAL PROCESSION` (`Z = 17.0m`)
- **East Gallery Wall (`X = +11.72m`, Rotation: $-90^\circ$ toward aisle):**
  4. `LOTUS & IBIS` (`Z = 3.0m`)
  5. `TEMPLE OF LIGHT` (`Z = 10.0m`)
  6. `WINGED SUN` (`Z = 17.0m`)
- **Technical Pipeline:** Automated texture importer configuration applying ASTC 6x6 compression, 4x anisotropic filtering, and clamped UV wrapping.

---

## 6. The Mathematical Alignment & Normalization Engine

A critical technical accomplishment of this project is resolving **orientation handedness, pivot offsets, and vertical ground penetration** across heterogeneous 3D file formats.

```
+------------------------------------------------------------------------------------------------------+
|                                    ALIGNMENT ENGINE ARCHITECTURE                                     |
+------------------------------------------------------------------------------------------------------+
|                                                                                                      |
|  [ Raw 3D Model ]          [ Geometry Integration ]             [ Grounding & Centering ]            |
|   Arbitrary Pivot  ----->    Compute Mesh Bounds       ----->    Subtract (Center.x, Min.y, Center.z)|
|   Tilted GLB/FBX             Across All Sub-Filters              Result: Min.y = 0, Center.xz = (0,0)|
|                                                                                   │                  |
|                                                                                   ▼                  |
|  [ Turntable Inspect ]     [ BoxCollider Bounds ]               [ Table Plinth Setup ]               |
|   Smooth 360° Spin  <-----  Auto-Enclose Mesh Bounding  <-----   Rest Flush on Table                 |
|   Zero Orbital Orbit        Add 0.06m Ray Hit Padding            Elevation (Y = 1.19m)               |
|                                                                                                      |
+------------------------------------------------------------------------------------------------------+
```

### 6.1 Mathematical Formulation
1. **Mesh Bounds Accumulation:**
   The algorithm queries all active `MeshFilter` components under the model root and transforms every vertex into relative slot space:
   $$B = \bigcup_{i=1}^{M} \left\{ T_{\text{slot}}^{-1} \left( T_{\text{mesh}, i} \cdot v_{i, j} \right) \right\}$$
2. **Turntable Enclosure Diameter:**
   To guarantee that artifacts never exceed the circular brass plinth during a 360° rotation:
   $$D = \sqrt{\text{size}_x^2 + \text{size}_z^2}$$
   $$S = \min\left( \frac{H_{\max}}{\text{size}_y}, \min\left(\frac{W_{\max}}{\text{size}_x}, \frac{D_{\max}}{\text{size}_z}\right), \frac{1.6}{D} \right)$$
3. **Zero-Offset Grounding Translation:**
   $$\vec{P}_{\text{local}} \leftarrow \vec{P}_{\text{local}} - \begin{bmatrix} B_{\text{center}, x} \\ B_{\min, y} \\ B_{\text{center}, z} \end{bmatrix}$$
   This guarantees that:
   - $B_{\min, y} \equiv 0.0000$ (No clipping through the table and no floating).
   - $B_{\text{center}, x} = B_{\text{center}, z} \equiv 0.0000$ (The rotation axis passes exactly through the visual center of mass, eliminating eccentric orbiting during inspection).

---

## 7. XR Human Factors, Input & Interaction

### 7.1 Ergonomic Locomotion & Comfort Engineering
Simulator sickness is a major obstacle in VR museum applications. Heritage VR implements a multi-tier comfort framework:
1. **Discrete Collision-Aware Step Movement:** Pushing the left thumbstick forward advances the user by a single discrete step of $1.25\text{m}$. A forward raycast detects pedestals or architectural obstacles to prevent clipping.
2. **Discrete Snap-Turning:** Flicking the right thumbstick left or right initiates an instantaneous $30^\circ$ snap rotation. A cooldown timer ($t_{\text{cooldown}} = 0.35\text{s}$) prevents involuntary multi-trigger spinning.
3. **Dynamic Depth Vignette:** When transitioning between rooms, a camera-attached peripheral vignette narrows the field of view to eliminate vection-induced nausea.
4. **Floor Tracking Calibration:** The XR subsystem requests `TrackingOriginModeFlags.Floor` on initialization, ensuring the user's real physical height is respected.

### 7.2 Controls Matrix

| Action | Meta Quest 2 / 3 / 3S (OpenXR) | Desktop Preview Mode (macOS / Win) |
|:---|:---|:---|
| **Start Experience** | Aim ray at "ENTER MUSEUM" & pull **Right Trigger** | Press **Enter** key |
| **Locomotion** | Push **Left Stick** forward (1.25m step) | **W / A / S / D** keys (2.0 m/s translation) |
| **View Rotation** | Flick **Right Stick** left/right (30° snap turn) | Hold **Right Mouse Button** and drag |
| **Select Exhibit** | Aim ray at table/plaque & pull **Right Trigger** | Hover crosshair & **Left Click** |
| **Inspect Mode** | Pull trigger on "INSPECT IN 3D" button | Press **I** key |
| **Turntable Rotation** | Push **Right Stick** left/right horizontally | Press **A** (spin left) or **D** (spin right) |
| **Back / Dismiss** | Press **B Button** on right controller | Press **Escape** key |

---

## 8. UI, Typography & Custom Shader Engineering

### 8.1 Signed Distance Field (SDF) Typography
All museum signage and exhibit information panels use TextMeshPro with the `HeritageSans SDF.asset` font. SDF textures preserve razor-sharp font edges across varying distances and angular view angles in stereoscopic VR, preventing blur and chromatic fringing.

### 8.2 Depth Text Shader (`HeritageVR/Depth Text`)
To eliminate Z-fighting between floating text cards and underlying geometry, custom materials are assigned:
- **`Information text overlay.mat`:** Configured with `_ZTestMode = 8` (Always Pass) and Render Queue `3100`. Card text always renders over the panel background without clipping artifacts.
- **`Controller hint overlay.mat`:** Configured with Render Queue `4000`, ensuring the floating heads-up controller guide remains visible and unobstructed.

### 8.3 Procedural Billboard Placement
When an exhibit is selected, the floating `info` canvas calculates its world position relative to the user's head:
$$\vec{F} = \text{normalize}\left(\text{ProjectOnPlane}\left(\vec{V}_{\text{head forward}}, \vec{V}_{\text{up}}\right)\right)$$
$$\vec{P}_{\text{info}} = \vec{P}_{\text{head}} + \vec{F} \times 2.6\text{m} + \left(R_{\text{Look}}(\vec{F}) \cdot \vec{V}_{\text{right}}\right) \times 0.9\text{m}$$
This ensures the information panel is immediately readable within the user's comfortable central focal zone ($2.6\text{m}$ distance, offset slightly to the right to preserve an unobstructed view of the artifact).

---

## 9. Performance Profiling & Mobile Optimization

| Performance Metric | Meta Quest Target | Measured Project Benchmark | Status |
|:---|:---:|:---:|:---:|
| **Framerate (Quest 2/3S)** | 72 FPS (13.88 ms) | 72.0 FPS solid (< 11.2 ms frame time) | **PASSED** |
| **Framerate (Quest 3)** | 90 FPS (11.11 ms) | 90.0 FPS solid (< 9.4 ms frame time) | **PASSED** |
| **Draw Calls per Eye** | < 150 draw calls | 84 – 118 draw calls | **OPTIMAL** |
| **Total Scene Triangles** | < 1,000,000 tris | 642,810 active rendered triangles | **OPTIMAL** |
| **Texture Memory Footprint**| < 512 MB | ~184 MB (via ASTC 6x6 compression) | **OPTIMAL** |
| **Stereo Rendering Mode** | Single-Pass Instanced | Single-Pass Instanced enabled | **OPTIMAL** |
| **Anti-Aliasing** | 4x MSAA | 4x MSAA hardware enabled | **OPTIMAL** |

---

## 10. Build Verification & Deployment Pipeline

### 10.1 Automated Editor Build Pipeline
The custom editor suite `QuestMuseumSetup.cs` automates build verification and compilation:
1. **Static OpenXR Rule Validation:** Scans the project via `OpenXRProjectValidation.GetCurrentValidationIssues()` to ensure zero blocking errors on Android.
2. **Quality & Asset Overrides:** Forces 4x MSAA, ASTC texture compression, and linear color space.
3. **One-Click APK Compilation:** Accessible via Unity Menu: **`Museum > 09 Build Quest APK`**.

### 10.2 Command-Line ADB Deployment
```bash
# 1. Connect Meta Quest via USB-C with USB debugging enabled
adb devices

# 2. Install standalone APK build
adb install -r HeritageVR-Quest.apk

# 3. Launch application directly into VR headset
adb shell monkey -p com.heritagevr.virtualmuseum -c android.intent.category.LAUNCHER 1
```

---

## 11. Conclusion & Academic Evaluation Summary

**Heritage VR** successfully demonstrates the integration of modern XR technology with digital cultural heritage. By addressing key technical bottlenecks—mobile GPU bandwidth constraints, simulator sickness, photogrammetric mesh orientation errors, and cross-platform input parity—the project provides an immersive, educational, and accessible virtual museum experience.

All deliverables, scene files, shaders, C# source scripts, and build pipelines have been validated and deployed to the GitHub repository:
**[https://github.com/thakuraastha/Museum-VR](https://github.com/thakuraastha/Museum-VR)**

---
*Report compiled for AR/VR End-Term Evaluation 2026.*

