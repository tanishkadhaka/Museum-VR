using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using HeritageVR;
using TMPro;
using System.Linq;

public static class MuseumArtifactSetup {
    const string ScenePath = "Assets/Scenes/UnifiedMuseum.unity";

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
            description = "A digital study of Tutankhamun’s funerary mask. The striped headdress and royal emblems frame the face.\n\nLook closely at the gold-and-blue decoration and the symmetry of the portrait.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(0, 180, 0),
            localScale = Vector3.one
        },
        // Table 02
        new ArtifactDef {
            assetPath = "Assets/AK Studio Art/Egypt Pharoah Bust/Prefabs/Egypt Pharoah Bust.prefab",
            shortName = "ROYAL PHARAOH BUST",
            title = "Royal Egyptian Pharaoh Bust",
            description = "A digital study of an Egyptian royal portrait bust, with a striped headdress and ceremonial beard.\n\nExplore the face and headcloth in 3D. The identity and date of this model have not been confirmed.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(0, 180, 0),
            localScale = Vector3.one
        },
        // Table 03
        new ArtifactDef {
            assetPath = "Assets/Models/MuseumObjects/egyptian_pharoah_khasekhemwy_-_oxford_ashmoleon.glb",
            shortName = "KHASEKHEMWY STATUE",
            title = "Statue of King Khasekhemwy",
            description = "A digital study of the seated statue identified as King Khasekhemwy. The tall crown, compact throne and long robe define its silhouette.\n\nRotate the model to compare the front, sides and throne.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(-90, 150, 0),
            localScale = Vector3.one
        },
        // Table 04
        new ArtifactDef {
            assetPath = "Assets/Models/MuseumObjects/marble_statue_of_a_lion.glb",
            shortName = "MARBLE LION STATUE",
            title = "Marble Lion Sculpture",
            description = "A digital study of a lion sculpture.\n\nNotice the mane, open mouth, extended paws and weathered surface. Rotate the model to explore the crouching pose. The model’s exact provenance is unconfirmed.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(-90, 150, 0),
            localScale = Vector3.one
        },
        // Table 05
        new ArtifactDef {
            assetPath = "Assets/Ancient Pots/URP & Built-In/URP/Prefabs/pot_1.prefab",
            shortName = "STORAGE VESSEL",
            title = "Two-handled Storage Vessel",
            description = "A digital study of a two-handled pottery vessel with a narrow neck, rounded body and small foot.\n\nCompare its proportions with the other ceramics. The model’s exact origin and date are unconfirmed.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(0, 0, 0),
            localScale = Vector3.one
        },
        // Table 06
        new ArtifactDef {
            assetPath = "Assets/Ancient Pots/URP & Built-In/URP/Prefabs/pot_2.prefab",
            shortName = "RIBBED STORAGE JAR",
            title = "Ribbed Storage Jar",
            description = "A digital study of an oval storage jar with a ribbed surface and a supporting metal stand.\n\nExplore the bands, surface texture and profile. The model’s original function, origin and date are unconfirmed.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(0, 0, 0),
            localScale = Vector3.one
        },
        // Table 07
        new ArtifactDef {
            assetPath = "Assets/Ancient Pots/URP & Built-In/URP/Prefabs/pot_3.prefab",
            shortName = "TALL CERAMIC VESSEL",
            title = "Tall Ceramic Vessel",
            description = "A digital study of a tall, narrow ceramic vessel with a small mouth and bands around the body.\n\nCompare the shoulder, neck and base with the wider jars nearby. The model’s exact attribution is unconfirmed.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(0, 0, 0),
            localScale = Vector3.one
        },
        // Table 08
        new ArtifactDef {
            assetPath = "Assets/Models/MuseumObjects/900868a0aa0b47e58571f5f5ae20094c.glb",
            shortName = "ALLOSAURUS FOSSIL",
            title = "Allosaurus Skeleton",
            description = "A digital study of a mounted skeleton identified as Allosaurus.\n\nExplore the skull, rib cage, tail and hind limbs. The skeleton is shown at reduced scale to fit this tabletop display.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(-90, 90, 0),
            localScale = Vector3.one
        }
    };

    [MenuItem("Museum/12 Reimport and Align 8 Artifacts")]
    public static void PopulateAndAlign(){MuseumArtifactAlignment.Apply();}

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

        foreach(var def in Artifacts) if(!AssetDatabase.LoadAssetAtPath<GameObject>(def.assetPath)) throw new System.Exception("Missing artifact: "+def.assetPath);

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
