using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using HeritageVR;
using TMPro;
using System.Linq;

[InitializeOnLoad]
public static class MuseumArtifactSetup {
    const string ScenePath = "Assets/Scenes/UnifiedMuseum.unity";

    static MuseumArtifactSetup() {
        EditorApplication.delayCall += AutoPopulateIfPlaceholders;
    }

    public static void AutoPopulateIfPlaceholders() {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path == ScenePath) {
            var exhibits = Object.FindObjectsOfType<MuseumExhibit>();
            if (exhibits.Any(e => e.title != null && (e.title.StartsWith("ARTIFACT SLOT") || e.title.Contains("RESERVED")))) {
                Debug.Log("[MuseumArtifactSetup] Initializing and populating all 8 museum artifacts...");
                Populate();
            }
        }
    }

    public struct ArtifactDef {
        public string assetPath;
        public string shortName;
        public string title;
        public string description;
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
        public Vector3 localScale;
    }

    public static readonly ArtifactDef[] Artifacts = new ArtifactDef[] {
        // Table 01
        new ArtifactDef {
            assetPath = "Assets/ThreeDragons/Tutankhamun/Prefabs/Tut_mask.prefab",
            shortName = "TUTANKHAMUN MASK",
            title = "Mask of Tutankhamun",
            description = "The gold funerary mask of Pharaoh Tutankhamun (18th Dynasty, c. 1323 BCE), discovered in tomb KV62 in the Valley of the Kings by Howard Carter in 1922.\n\nCrafted from over 10 kilograms of solid gold inlaid with lapis lazuli, turquoise, carnelian, and quartz. The nemes headdress bears the protective cobra (Wadjet) and vulture (Nekhbet), symbolizing sovereign divine authority over Upper and Lower Egypt in the afterlife.",
            localPosition = new Vector3(0, 0f, 0),
            localEulerAngles = new Vector3(0, 0, 0),
            localScale = new Vector3(1f, 1f, 1f)
        },
        // Table 02
        new ArtifactDef {
            assetPath = "Assets/AK Studio Art/Egypt Pharoah Bust/Prefabs/Egypt Pharoah Bust.prefab",
            shortName = "ROYAL PHARAOH BUST",
            title = "Royal Egyptian Pharaoh Bust",
            description = "A royal portrait bust depicting an ancient Egyptian pharaoh in ceremonial splendor (New Kingdom, c. 14th Century BCE).\n\nCarved with realistic naturalism, wearing the striated royal nemes headcloth and divine ceremonial beard. Illustrates the monumental portraiture and divine kingship iconography characteristic of Egypt's imperial golden age.",
            localPosition = new Vector3(0, 0f, 0),
            localEulerAngles = new Vector3(0, 0, 0),
            localScale = new Vector3(0.25f, 0.25f, 0.25f)
        },
        // Table 03
        new ArtifactDef {
            assetPath = "Assets/Models/MuseumObjects/egyptian_pharoah_khasekhemwy_-_oxford_ashmoleon.glb",
            shortName = "KHASEKHEMWY STATUE",
            title = "Statue of King Khasekhemwy",
            description = "One of the earliest surviving monumental royal stone sculptures from ancient Egypt (Second Dynasty, c. 2686 BCE), excavated at Hierakonpolis and conserved at the Ashmolean Museum in Oxford.\n\nCarved from indurated limestone, depicting King Khasekhemwy seated upon a block throne wearing the White Crown of Upper Egypt and the ceremonial Sed festival jubilee robe.",
            localPosition = new Vector3(0, 0f, 0),
            localEulerAngles = new Vector3(-90, -90, -30),
            localScale = new Vector3(1.15f, 1.15f, 1.15f)
        },
        // Table 04
        new ArtifactDef {
            assetPath = "Assets/Models/MuseumObjects/marble_statue_of_a_lion.glb",
            shortName = "MARBLE LION STATUE",
            title = "Classical Marble Resting Lion",
            description = "A monumental resting feline guardian sculpture carved from crystalline Pentelic marble (Hellenistic / Classical Antiquity, c. 4th-2nd Century BCE).\n\nIn classical Greco-Roman architecture, sculpted lions guarded royal tomb precincts, sacred temples, and public monuments, standing as eternal symbols of vigilance, valor, and heroic divine protection.",
            localPosition = new Vector3(0, 0f, 0),
            localEulerAngles = new Vector3(-54.3f, -38.9f, -103.3f),
            localScale = new Vector3(0.12f, 0.12f, 0.12f)
        },
        // Table 05
        new ArtifactDef {
            assetPath = "Assets/Ancient Pots/URP & Built-In/URP/Prefabs/pot_1.prefab",
            shortName = "STORAGE AMPHORA",
            title = "Terracotta Storage Amphora",
            description = "A classical Mediterranean twin-handled pottery vessel (c. 5th Century BCE) wheel-thrown from coarse terracotta clay and finished with an authentic slip patina.\n\nFeaturing an elongated body, fluted neck, and vertical strap handles, amphorae served as the primary bulk shipping containers for transporting and aging vintage wines, cold-pressed olive oils, and grains across Aegean maritime trade routes.",
            localPosition = new Vector3(0, 0f, 0),
            localEulerAngles = new Vector3(0, 0, 0),
            localScale = new Vector3(0.85f, 0.85f, 0.85f)
        },
        // Table 06
        new ArtifactDef {
            assetPath = "Assets/Ancient Pots/URP & Built-In/URP/Prefabs/pot_2.prefab",
            shortName = "CEREMONIAL HYDRIA",
            title = "Ancient Ceremonial Hydria",
            description = "A ceremonial Greek terracotta water pitcher (c. 4th Century BCE) characterized by a bulbous vessel body, flared pouring rim, and dual side lifting handles complemented by a central pouring handle.\n\nEssential to civic life and sanctuary rituals, hydriae were gathered at public fountain houses and dedicated at sacred shrines during ritual cleansing and libation ceremonies.",
            localPosition = new Vector3(0, 0f, 0),
            localEulerAngles = new Vector3(0, 0, 0),
            localScale = new Vector3(0.85f, 0.85f, 0.85f)
        },
        // Table 07
        new ArtifactDef {
            assetPath = "Assets/Ancient Pots/URP & Built-In/URP/Prefabs/pot_3.prefab",
            shortName = "SYMPOSIUM KRATER",
            title = "Classical Symposium Krater",
            description = "A monumental earthenware mixing bowl (c. 6th-5th Century BCE) adorned with horizontal band motifs and a heavy fluted pedestal base.\n\nThe centerpiece of the ancient Greek symposium (social and philosophical banquet), the krater was utilized to dilute potent unfermented vintage wine with fresh spring water, honey, and aromatic herbs before serving participants.",
            localPosition = new Vector3(0, 0f, 0),
            localEulerAngles = new Vector3(0, 0, 0),
            localScale = new Vector3(0.85f, 0.85f, 0.85f)
        },
        // Table 08
        new ArtifactDef {
            assetPath = "Assets/Models/MuseumObjects/900868a0aa0b47e58571f5f5ae20094c.glb",
            shortName = "ALLOSAURUS FOSSIL",
            title = "Allosaurus Fragilis Fossil Skeleton",
            description = "A complete mounted articulated fossil skeleton of Allosaurus fragilis, the apex theropod dinosaur of the Late Jurassic Morrison Formation (approx. 155-145 Million BCE).\n\nDigitized from the Osaka Museum of Natural History collection. Features a lightweight kinetic skull, recurved serrated teeth, and lethal three-clawed raptorial forelimbs used to hunt massive sauropod dinosaurs.",
            localPosition = new Vector3(0, 0.5f, 0),
            localEulerAngles = new Vector3(-90, 0, 0),
            localScale = new Vector3(0.18f, 0.18f, 0.18f)
        }
    };

    [MenuItem("Museum/12 Populate 8 Artifacts")]
    public static void Populate() {
        if (EditorApplication.isPlaying) {
            Debug.LogError("Stop playback before configuring artifacts.");
            return;
        }

        if (EditorSceneManager.GetActiveScene().path != ScenePath) {
            EditorSceneManager.OpenScene(ScenePath);
        }

        var experience = Object.FindObjectOfType<MuseumExperience>();
        if (!experience) {
            Debug.LogError("MuseumExperience not found in scene!");
            return;
        }

        var exhibits = experience.gallery.GetComponentsInChildren<MuseumExhibit>(true)
            .OrderBy(x => x.name)
            .ToArray();

        if (exhibits.Length != 8) {
            Debug.LogError($"Expected 8 display tables, found {exhibits.Length}");
            return;
        }

        for (int i = 0; i < exhibits.Length; i++) {
            var exhibit = exhibits[i];
            var def = Artifacts[i];

            // 1. Destroy existing placeholder children under the artifact slot
            foreach (var child in exhibit.artifact.Cast<Transform>().ToArray()) {
                Object.DestroyImmediate(child.gameObject);
            }

            // 2. Load model/prefab asset
            var prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(def.assetPath);
            if (!prefabAsset) {
                Debug.LogError($"Could not load asset at path: {def.assetPath}");
                continue;
            }

            // 3. Instantiate model under exhibit.artifact
            GameObject instance = PrefabUtility.InstantiatePrefab(prefabAsset, exhibit.artifact) as GameObject;
            if (!instance) {
                instance = Object.Instantiate(prefabAsset, exhibit.artifact);
            }

            instance.name = def.shortName;
            instance.transform.localPosition = def.localPosition;
            instance.transform.localEulerAngles = def.localEulerAngles;
            instance.transform.localScale = def.localScale;

            // 4. Update exhibit metadata
            exhibit.title = def.title;
            exhibit.description = def.description;
            exhibit.name = $"DISPLAY {(i + 1):D2} • {def.title}";

            // 5. Update table slot label text
            foreach (var label in exhibit.GetComponentsInChildren<TMP_Text>(true)) {
                if (label.name == "Table slot label") {
                    label.text = $"{(i + 1):D2} / {def.shortName}";
                }
            }

            // 6. Ensure slot BoxCollider is active and correctly sized for raycast interaction
            var boxCol = exhibit.artifact.GetComponent<BoxCollider>();
            if (!boxCol) {
                boxCol = exhibit.artifact.gameObject.AddComponent<BoxCollider>();
            }
            boxCol.enabled = true;
            boxCol.size = new Vector3(1.9f, 1.4f, 1.3f);
            boxCol.center = new Vector3(0, 0.7f, 0);

            EditorUtility.SetDirty(exhibit);
        }

        // Configure initial selection in MuseumExperience
        experience.selected = exhibits[0];
        experience.statue = exhibits[0].artifact;
        experience.ceramicModel = exhibits[0].artifact;
        experience.exhibitTitle.text = exhibits[0].title;
        experience.exhibitDescription.text = exhibits[0].description;
        experience.info.gameObject.SetActive(false);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();

        Debug.Log("Successfully populated all 8 artifacts with authentic models, descriptions, and labels!");
    }
}
