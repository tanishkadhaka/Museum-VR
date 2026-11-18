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
            description = "Imagine a face made to last beyond a lifetime. Tutankhamun ruled during Egypt’s Eighteenth Dynasty. His burial mask covered the head of his mummy, joining the identity of the young king with Egyptian hopes for life after death.\n\nGold, coloured inlays and the striped royal headcloth turn a human portrait into an image of enduring kingship. The cobra and vulture on the brow connect the ruler with the protection of Upper and Lower Egypt.\n\nWhat makes the mask special is the meeting of extraordinary craftsmanship and a deeply personal purpose: protecting and transforming one person after death. Look at the symmetry of the face. This is a digital interpretation of the original mask.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(0, 180, 0),
            localScale = Vector3.one
        },
        // Table 02
        new ArtifactDef {
            assetPath = "Assets/AK Studio Art/Egypt Pharoah Bust/Prefabs/Egypt Pharoah Bust.prefab",
            shortName = "ROYAL PHARAOH BUST",
            title = "Royal Egyptian Pharaoh Bust",
            description = "A royal portrait was more than a likeness. In ancient Egypt, images of rulers helped present kingship as stable, sacred and enduring. Artists used recognisable symbols so viewers could read a king’s status without knowing his name.\n\nThe striped nemes headcloth and ceremonial beard belong to this language of authority. Rulers across different periods reused such symbols, connecting themselves with earlier kings and an established visual tradition.\n\nLook at the contrast between the individual face and the formal headdress. That combination makes royal portraiture special: a human being becomes a symbol of power. The identity, date and original material of this particular model remain unconfirmed.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(0, 180, 0),
            localScale = Vector3.one
        },
        // Table 03
        new ArtifactDef {
            assetPath = "Assets/Models/MuseumObjects/egyptian_pharoah_khasekhemwy_-_oxford_ashmoleon.glb",
            shortName = "KHASEKHEMWY STATUE",
            title = "Statue of King Khasekhemwy",
            description = "This seated ruler is identified as King Khasekhemwy, also known as Khasekhem. The Ashmolean’s limestone statue comes from Hierakonpolis and belongs to Egypt’s Second Dynasty. It takes us close to the beginnings of royal stone sculpture.\n\nThe tall White Crown, compact throne and enveloping robe create a controlled, enduring image. Instead of showing a moment of everyday activity, the sculptor presents kingship as something solid and lasting.\n\nIts importance lies in its great age and the early royal tradition it preserves. Compare the front with the side: the body and throne form a closely joined shape. This digital study represents the statue; it is not the ancient object itself.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(-90, 150, 0),
            localScale = Vector3.one
        },
        // Table 04
        new ArtifactDef {
            assetPath = "Assets/Models/MuseumObjects/marble_statue_of_a_lion.glb",
            shortName = "MARBLE LION STATUE",
            title = "Marble Lion Sculpture",
            description = "Imagine approaching a monument watched over by a powerful animal. In ancient Greek sculpture, marble lions could mark tombs or guard the ends of a tomb’s facade. The animal made protection visible in stone.\n\nA lion’s open mouth, mane and extended paws could suggest strength even when the sculpture stood still. Such features helped a sculptor turn an animal into a lasting presence beside a place of remembrance.\n\nWhat is special here is the suggestion of movement in a fixed form. Walk around the crouching body and notice how the silhouette changes. This explains a historical tradition; the exact origin and date of this model are unconfirmed.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(-90, 150, 0),
            localScale = Vector3.one
        },
        // Table 05
        new ArtifactDef {
            assetPath = "Assets/Ancient Pots/URP & Built-In/URP/Prefabs/pot_1.prefab",
            shortName = "STORAGE VESSEL",
            title = "Two-handled Storage Vessel",
            description = "Before modern packaging, pottery was part of the infrastructure of daily life. In the ancient Mediterranean, amphorae and related containers stored and carried products such as wine and oil. Ordinary vessels connected homes, workshops and trading routes.\n\nThis digital vessel has two handles, a narrow neck and a rounded body. Think about lifting, pouring and storing rather than decoration alone. A container’s shape can help us ask how people used it.\n\nIts story is special because everyday objects reveal lives often missing from royal monuments. We can explore how useful forms become beautiful. This is general historical context: this model’s precise type, origin and date have not been verified.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(0, 0, 0),
            localScale = Vector3.one
        },
        // Table 06
        new ArtifactDef {
            assetPath = "Assets/Ancient Pots/URP & Built-In/URP/Prefabs/pot_2.prefab",
            shortName = "RIBBED STORAGE JAR",
            title = "Ribbed Storage Jar",
            description = "A storage jar invites us to imagine the work of keeping a household supplied. Pottery containers belonged to the practical world of making, carrying and preserving goods. Their value was often measured through repeated use rather than precious materials.\n\nLook at this model’s repeated ribs and the stand that supports its rounded body. Compare the broad profile with the narrower vessel nearby. These visible details make its construction and display worth examining.\n\nIts importance in this gallery is the attention it brings to ordinary material culture. Useful objects can tell stories about skill, care and daily routines. The stand is part of the displayed model; its original use, date and origin remain unconfirmed.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(0, 0, 0),
            localScale = Vector3.one
        },
        // Table 07
        new ArtifactDef {
            assetPath = "Assets/Ancient Pots/URP & Built-In/URP/Prefabs/pot_3.prefab",
            shortName = "TALL CERAMIC VESSEL",
            title = "Tall Ceramic Vessel",
            description = "A tall vessel changes the way we look at pottery. Follow the outline from the small mouth, across the shoulder and down the narrow body. A simple silhouette records decisions made by a maker.\n\nAncient ceramic containers served many purposes, including storage and transport. Archaeologists compare shapes, clay, decoration and excavation context to understand them. Shape alone is not enough to give an object a secure historical identity.\n\nThis display is special as a lesson in looking carefully. Compare its bands and proportions with the two wider vessels. The model is presented as a tall ceramic vessel; a specific archaeological type, workshop and date have not been confirmed.",
            localPosition = Vector3.zero,
            localEulerAngles = new Vector3(0, 0, 0),
            localScale = Vector3.one
        },
        // Table 08
        new ArtifactDef {
            assetPath = "Assets/Models/MuseumObjects/900868a0aa0b47e58571f5f5ae20094c.glb",
            shortName = "ALLOSAURUS FOSSIL",
            title = "Allosaurus Skeleton",
            description = "Long before people built museums, Allosaurus lived in Jurassic landscapes. The Natural History Museum places it around 156 to 144 million years ago. This large meat-eating dinosaur walked on powerful hind legs and carried a long balancing tail.\n\nA skeleton tells its story through relationships between bones. Look at the skull, rib cage, short forelimbs and tail. These structures help scientists investigate how an extinct animal moved and lived, even though its behaviour cannot be watched directly.\n\nAllosaurus is special because it lets us explore a vanished ecosystem through physical evidence. This tabletop skeleton is greatly reduced in scale. The particular specimen and museum attribution of this digital model have not been verified.",
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
            exhibit.narration=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/HeritageVR/Narration/Exhibit-"+(i+1).ToString("00")+".wav");
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
