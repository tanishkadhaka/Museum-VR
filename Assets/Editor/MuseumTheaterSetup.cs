using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Video;
using TMPro;
using System.IO;
using HeritageVR;

public static class MuseumTheaterSetup {
    const string ScenePath = "Assets/Scenes/UnifiedMuseum.unity";
    const string MaterialsPath = "Assets/HeritageVR/Materials/";
    const string TexturesPath = "Assets/HeritageVR/Textures/";

    static Material GetOrCreateMaterial(string name, Color color, Texture tex = null, bool unlit = false) {
        string path = MaterialsPath + name + ".mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (!mat) {
            string shaderName = unlit ? "Universal Render Pipeline/Unlit" : "Universal Render Pipeline/Lit";
            var shader = Shader.Find(shaderName) ?? Shader.Find("Universal Render Pipeline/Unlit");
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.SetColor("_BaseColor", color);
        if (tex) mat.SetTexture("_BaseMap", tex);
        EditorUtility.SetDirty(mat);
        return mat;
    }

    static GameObject CreateBox(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent, bool addCollider = true) {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        if (mat) go.GetComponent<Renderer>().sharedMaterial = mat;
        if (!addCollider) {
            var col = go.GetComponent<Collider>();
            if (col) Object.DestroyImmediate(col);
        }
        return go;
    }

    static GameObject CreateCylinder(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent) {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        if (mat) go.GetComponent<Renderer>().sharedMaterial = mat;
        return go;
    }

    [MenuItem("Museum/15 Add Egyptian History Projection Hall")]
    public static void BuildTheater() {
        if (EditorApplication.isPlaying) throw new System.Exception("Stop playback before building theater.");
        if (EditorSceneManager.GetActiveScene().path != ScenePath) {
            EditorSceneManager.OpenScene(ScenePath);
        }

        var experience = Object.FindObjectOfType<MuseumExperience>();
        if (!experience) throw new System.Exception("MuseumExperience not found in scene.");

        // 1. Ensure Camera solid midnight background
        if (experience.view) {
            experience.view.clearFlags = CameraClearFlags.SolidColor;
            experience.view.backgroundColor = new Color(0.012f, 0.012f, 0.016f, 1.0f);
            EditorUtility.SetDirty(experience.view);
        }

        // 2. Total Dark Outer Enclosure Shell
        // Enclosing X: -35 to +35 (width 70), Y: -1 to +15 (height 16, center 7), Z: -25 to +65 (depth 90, center 20)
        var oldEnclosure = GameObject.Find("TOTAL ENCLOSURE • Midnight Museum Dome");
        if (oldEnclosure) Object.DestroyImmediate(oldEnclosure);

        var enclosureMat = GetOrCreateMaterial("TotalDarkEnclosure", new Color(0.012f, 0.012f, 0.016f, 1.0f), null, true);
        var enclosureRoot = new GameObject("TOTAL ENCLOSURE • Midnight Museum Dome");
        enclosureRoot.transform.position = Vector3.zero;

        // Build 6 inward facing sides of outer shell
        // Floor
        CreateBox("Enclosure Floor", new Vector3(0, -1.5f, 20f), new Vector3(80f, 1f, 100f), enclosureMat, enclosureRoot.transform, false);
        // Ceiling
        CreateBox("Enclosure Ceiling", new Vector3(0, 15.5f, 20f), new Vector3(80f, 1f, 100f), enclosureMat, enclosureRoot.transform, false);
        // West Wall
        CreateBox("Enclosure West", new Vector3(-35.5f, 7f, 20f), new Vector3(1f, 18f, 100f), enclosureMat, enclosureRoot.transform, false);
        // East Wall
        CreateBox("Enclosure East", new Vector3(35.5f, 7f, 20f), new Vector3(1f, 18f, 100f), enclosureMat, enclosureRoot.transform, false);
        // South Wall
        CreateBox("Enclosure South", new Vector3(0, 7f, -25.5f), new Vector3(80f, 18f, 1f), enclosureMat, enclosureRoot.transform, false);
        // North Wall
        CreateBox("Enclosure North", new Vector3(0, 7f, 65.5f), new Vector3(80f, 18f, 1f), enclosureMat, enclosureRoot.transform, false);

        // 3. Open Rear Gallery Wall if needed (Archway opening)
        var rearWall = experience.gallery.Find("Collection wall");
        if (rearWall) {
            // Replace full solid wall with archway or resize it so players can walk through X in [-3, +3]
            rearWall.gameObject.SetActive(false);
        }

        // 4. Theater Root
        var oldTheater = experience.gallery.Find("EGYPTIAN HISTORY • CINEMA PROJECTION HALL");
        if (oldTheater) Object.DestroyImmediate(oldTheater.gameObject);

        var theaterRoot = new GameObject("EGYPTIAN HISTORY • CINEMA PROJECTION HALL").transform;
        theaterRoot.SetParent(experience.gallery, false);

        // Materials
        var basaltMat = GetOrCreateMaterial("EgyptianDarkBasalt", new Color(0.08f, 0.08f, 0.09f));
        var sandstoneMat = GetOrCreateMaterial("EgyptianSandstone", new Color(0.72f, 0.58f, 0.42f));
        var goldTrimMat = GetOrCreateMaterial("EgyptianBrushedGold", new Color(0.85f, 0.68f, 0.25f));
        var limestoneMat = GetOrCreateMaterial("EgyptianViewingLimestone", new Color(0.68f, 0.65f, 0.58f));
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/HeritageVR/Fonts/HeritageSans SDF.asset");

        // 5. Entrance Portal & Pylons (Z = 22.8m - 23.0m)
        // Flanking left/right pylons and top lintel archway
        // Left Pylon
        CreateBox("Portal Pylon West", new Vector3(-6.5f, 3.25f, 22.9f), new Vector3(5.2f, 6.5f, 0.8f), sandstoneMat, theaterRoot, true);
        // Right Pylon
        CreateBox("Portal Pylon East", new Vector3(6.5f, 3.25f, 22.9f), new Vector3(5.2f, 6.5f, 0.8f), sandstoneMat, theaterRoot, true);
        // Top Archway Lintel
        CreateBox("Portal Lintel", new Vector3(0f, 5.75f, 22.9f), new Vector3(8.0f, 1.5f, 0.8f), sandstoneMat, theaterRoot, true);
        // Portal Signage
        var portalSignGo = new GameObject("Portal Signage", typeof(RectTransform));
        var portalSign = portalSignGo.AddComponent<TextMeshPro>();
        portalSignGo.transform.SetParent(theaterRoot, false);
        portalSignGo.transform.localPosition = new Vector3(0, 5.3f, 22.45f);
        portalSign.font = font;
        portalSign.fontSize = 1.0f;
        portalSign.text = "EGYPTIAN HISTORY • CINEMA PROJECTION HALL";
        portalSign.color = new Color(0.95f, 0.82f, 0.55f);
        portalSign.alignment = TextAlignmentOptions.Center;
        portalSign.rectTransform.sizeDelta = new Vector2(7f, 0.6f);

        // 6. Cinema Hall Structure:
        // Dimensions: 18.2m wide x 22m deep (Z: 23m to 45m, center Z = 34m), Height 6.5m (Y: 0 to 6.5m)
        // Floor
        CreateBox("Cinema Floor", new Vector3(0, -0.05f, 34f), new Vector3(18.2f, 0.1f, 22f), basaltMat, theaterRoot, true);
        // Ceiling
        CreateBox("Cinema Ceiling", new Vector3(0, 6.55f, 34f), new Vector3(18.2f, 0.1f, 22f), basaltMat, theaterRoot, true);
        // West Wall
        CreateBox("Cinema West Wall", new Vector3(-9.1f, 3.25f, 34f), new Vector3(0.2f, 6.5f, 22f), sandstoneMat, theaterRoot, true);
        // East Wall
        CreateBox("Cinema East Wall", new Vector3(9.1f, 3.25f, 34f), new Vector3(0.2f, 6.5f, 22f), sandstoneMat, theaterRoot, true);
        // Back Wall (Screen Wall at Z = 45.0m)
        CreateBox("Cinema Screen Wall", new Vector3(0, 3.25f, 45.05f), new Vector3(18.2f, 6.5f, 0.2f), basaltMat, theaterRoot, true);

        // Ceiling Beams (coffered)
        for (int b = 0; b < 5; b++) {
            float bz = 25f + b * 4.5f;
            CreateBox("Ceiling Beam " + b, new Vector3(0, 6.35f, bz), new Vector3(18.2f, 0.4f, 0.5f), sandstoneMat, theaterRoot, false);
        }

        // 7. Monumental Cylindrical Columns (6 columns: 3 West, 3 East, diameter 1.1m)
        float[] colZ = new float[] { 27.5f, 34f, 40.5f };
        for (int c = 0; c < colZ.Length; c++) {
            CreateCylinder("West Column " + (c + 1), new Vector3(-7.5f, 3.25f, colZ[c]), new Vector3(1.1f, 3.25f, 1.1f), sandstoneMat, theaterRoot);
            CreateCylinder("East Column " + (c + 1), new Vector3(7.5f, 3.25f, colZ[c]), new Vector3(1.1f, 3.25f, 1.1f), sandstoneMat, theaterRoot);
        }

        // 8. Tiered Seating (3 rows of limestone amphitheater benches, 11m wide, 0.45m high)
        // Row 1: Z = 30m, Y = 0.225m
        CreateBox("Tiered Seating Row 1", new Vector3(0, 0.225f, 30.0f), new Vector3(11.0f, 0.45f, 0.9f), limestoneMat, theaterRoot, true);
        // Row 2: Z = 32.2m, Y = 0.55m
        CreateBox("Tiered Seating Tier 2 Base", new Vector3(0, 0.225f, 32.2f), new Vector3(11.0f, 0.45f, 1.2f), basaltMat, theaterRoot, true);
        CreateBox("Tiered Seating Row 2", new Vector3(0, 0.675f, 32.2f), new Vector3(11.0f, 0.45f, 0.9f), limestoneMat, theaterRoot, true);
        // Row 3: Z = 34.6m, Y = 0.9m
        CreateBox("Tiered Seating Tier 3 Base", new Vector3(0, 0.45f, 34.6f), new Vector3(11.0f, 0.9f, 1.2f), basaltMat, theaterRoot, true);
        CreateBox("Tiered Seating Row 3", new Vector3(0, 1.125f, 34.6f), new Vector3(11.0f, 0.45f, 0.9f), limestoneMat, theaterRoot, true);

        // 9. Projection Screen & Video Player Architecture
        // 16:9 Screen at Z = 44.7m, Y = 3.4m (9.8m wide x 5.51m high)
        // Screen Matting & Frame
        CreateBox("Screen Dark Matting", new Vector3(0, 3.4f, 44.75f), new Vector3(10.6f, 6.1f, 0.08f), basaltMat, theaterRoot, false);
        CreateBox("Screen Gold Frame Top", new Vector3(0, 6.2f, 44.73f), new Vector3(10.2f, 0.15f, 0.12f), goldTrimMat, theaterRoot, false);
        CreateBox("Screen Gold Frame Bottom", new Vector3(0, 0.6f, 44.73f), new Vector3(10.2f, 0.15f, 0.12f), goldTrimMat, theaterRoot, false);
        CreateBox("Screen Gold Frame Left", new Vector3(-5.0f, 3.4f, 44.73f), new Vector3(0.15f, 5.75f, 0.12f), goldTrimMat, theaterRoot, false);
        CreateBox("Screen Gold Frame Right", new Vector3(5.0f, 3.4f, 44.73f), new Vector3(0.15f, 5.75f, 0.12f), goldTrimMat, theaterRoot, false);

        // RenderTexture: 1920x1080 24-bit sRGB
        string rtPath = "Assets/HeritageVR/EgyptianCinema_RenderTexture.renderTexture";
        var rt = AssetDatabase.LoadAssetAtPath<RenderTexture>(rtPath);
        if (!rt) {
            rt = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            rt.name = "EgyptianCinema_RenderTexture";
            AssetDatabase.CreateAsset(rt, rtPath);
        }

        // Screen Material
        string screenMatPath = MaterialsPath + "EgyptianScreenMaterial.mat";
        var screenMat = AssetDatabase.LoadAssetAtPath<Material>(screenMatPath);
        if (!screenMat) {
            var unlitShader = Shader.Find("Universal Render Pipeline/Unlit");
            screenMat = new Material(unlitShader);
            AssetDatabase.CreateAsset(screenMat, screenMatPath);
        }
        screenMat.SetTexture("_BaseMap", rt);
        screenMat.SetColor("_BaseColor", Color.white);
        EditorUtility.SetDirty(screenMat);

        // Screen Quad Mesh
        var screenQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        screenQuad.name = "Egyptian History Projection Screen";
        screenQuad.transform.SetParent(theaterRoot, false);
        screenQuad.transform.localPosition = new Vector3(0, 3.4f, 44.68f);
        screenQuad.transform.localScale = new Vector3(9.8f, 5.51f, 1.0f);
        screenQuad.GetComponent<Renderer>().sharedMaterial = screenMat;
        Object.DestroyImmediate(screenQuad.GetComponent<Collider>());

        // Attach VideoPlayer
        var videoPlayer = screenQuad.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = true;
        videoPlayer.isLooping = true;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = rt;
        videoPlayer.aspectRatio = VideoAspectRatio.FitHorizontally;
        // Local clip or fallback URL
        string videoClipPath = "Assets/HeritageVR/Videos/EgyptianHistory.mp4";
        var clip = AssetDatabase.LoadAssetAtPath<VideoClip>(videoClipPath);
        if (clip) {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = clip;
        } else {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
        }

        // 10. Interactive Control Podium
        // Position at (X = 0, Y = 0, Z = 26.5m)
        var podiumRoot = new GameObject("Theater Control Podium").transform;
        podiumRoot.SetParent(theaterRoot, false);
        podiumRoot.localPosition = new Vector3(0, 0, 26.5f);

        CreateBox("Podium Base", new Vector3(0, 0.45f, 0), new Vector3(1.6f, 0.9f, 0.7f), sandstoneMat, podiumRoot, true);
        CreateBox("Podium Slanted Top", new Vector3(0, 0.95f, 0), new Vector3(1.5f, 0.12f, 0.65f), basaltMat, podiumRoot, true);

        // Podium Header Text
        var podiumTitleGo = new GameObject("Podium Header", typeof(RectTransform));
        var podiumTitle = podiumTitleGo.AddComponent<TextMeshPro>();
        podiumTitleGo.transform.SetParent(podiumRoot, false);
        podiumTitleGo.transform.localPosition = new Vector3(0, 1.03f, 0.18f);
        podiumTitleGo.transform.localRotation = Quaternion.Euler(30, 0, 0);
        podiumTitle.font = font;
        podiumTitle.fontSize = 0.42f;
        podiumTitle.text = "EGYPTIAN HISTORY SHORT STORY\nCINEMA PROJECTION CONTROLS";
        podiumTitle.color = new Color(0.95f, 0.85f, 0.65f);
        podiumTitle.alignment = TextAlignmentOptions.Center;
        podiumTitle.rectTransform.sizeDelta = new Vector2(1.4f, 0.35f);

        // Play/Pause Button
        var playBtn = CreateBox("Button • Play Pause", new Vector3(-0.35f, 1.03f, -0.05f), new Vector3(0.48f, 0.14f, 0.04f), goldTrimMat, podiumRoot, true);
        playBtn.transform.localRotation = Quaternion.Euler(30, 0, 0);
        var playAction = playBtn.AddComponent<MuseumAction>();
        playAction.action = "videoPlay";
        var playTextGo = new GameObject("Play Label", typeof(RectTransform));
        var playText = playTextGo.AddComponent<TextMeshPro>();
        playTextGo.transform.SetParent(playBtn.transform, false);
        playTextGo.transform.localPosition = new Vector3(0, 0, -0.6f);
        playText.font = font;
        playText.fontSize = 0.75f;
        playText.text = "PLAY / PAUSE";
        playText.color = Color.black;
        playText.alignment = TextAlignmentOptions.Center;
        playText.rectTransform.sizeDelta = new Vector2(0.45f, 0.12f);
        playText.transform.localScale = new Vector3(1f / 0.48f, 1f / 0.14f, 1f / 0.04f);

        // Restart Button
        var restartBtn = CreateBox("Button • Restart", new Vector3(0.35f, 1.03f, -0.05f), new Vector3(0.48f, 0.14f, 0.04f), goldTrimMat, podiumRoot, true);
        restartBtn.transform.localRotation = Quaternion.Euler(30, 0, 0);
        var restartAction = restartBtn.AddComponent<MuseumAction>();
        restartAction.action = "videoRestart";
        var restartTextGo = new GameObject("Restart Label", typeof(RectTransform));
        var restartText = restartTextGo.AddComponent<TextMeshPro>();
        restartTextGo.transform.SetParent(restartBtn.transform, false);
        restartTextGo.transform.localPosition = new Vector3(0, 0, -0.6f);
        restartText.font = font;
        restartText.fontSize = 0.75f;
        restartText.text = "RESTART";
        restartText.color = Color.black;
        restartText.alignment = TextAlignmentOptions.Center;
        restartText.rectTransform.sizeDelta = new Vector2(0.45f, 0.12f);
        restartText.transform.localScale = new Vector3(1f / 0.48f, 1f / 0.14f, 1f / 0.04f);

        // 11. Dynamic Theatrical Lighting & Dimmer
        // Ambient Ceiling Spotlights (Warm Color(0.95, 0.78, 0.55), intensity 0.85)
        var light1Go = new GameObject("Cinema Spot 1");
        light1Go.transform.SetParent(theaterRoot, false);
        light1Go.transform.localPosition = new Vector3(-4.5f, 6.0f, 28.0f);
        var l1 = light1Go.AddComponent<Light>();
        l1.type = LightType.Spot;
        l1.color = new Color(0.95f, 0.78f, 0.55f);
        l1.intensity = 0.85f;
        l1.range = 14f;
        l1.spotAngle = 65f;
        light1Go.transform.localRotation = Quaternion.Euler(75, 0, 0);

        var light2Go = new GameObject("Cinema Spot 2");
        light2Go.transform.SetParent(theaterRoot, false);
        light2Go.transform.localPosition = new Vector3(4.5f, 6.0f, 28.0f);
        var l2 = light2Go.AddComponent<Light>();
        l2.type = LightType.Spot;
        l2.color = new Color(0.95f, 0.78f, 0.55f);
        l2.intensity = 0.85f;
        l2.range = 14f;
        l2.spotAngle = 65f;
        light2Go.transform.localRotation = Quaternion.Euler(75, 0, 0);

        var light3Go = new GameObject("Cinema Spot 3");
        light3Go.transform.SetParent(theaterRoot, false);
        light3Go.transform.localPosition = new Vector3(0, 6.0f, 36.0f);
        var l3 = light3Go.AddComponent<Light>();
        l3.type = LightType.Spot;
        l3.color = new Color(0.95f, 0.78f, 0.55f);
        l3.intensity = 0.85f;
        l3.range = 14f;
        l3.spotAngle = 70f;
        light3Go.transform.localRotation = Quaternion.Euler(80, 0, 0);

        // Soft Blue Screen Wash Luminaire
        var blueWashGo = new GameObject("Screen Blue Wash Luminaire");
        blueWashGo.transform.SetParent(theaterRoot, false);
        blueWashGo.transform.localPosition = new Vector3(0, 5.8f, 41.0f);
        var lb = blueWashGo.AddComponent<Light>();
        lb.type = LightType.Spot;
        lb.color = new Color(0.2f, 0.45f, 0.85f);
        lb.intensity = 0.4f;
        lb.range = 12f;
        lb.spotAngle = 60f;
        blueWashGo.transform.localRotation = Quaternion.Euler(65, 0, 0);

        // Attach Dimmer
        var dimmer = theaterRoot.gameObject.AddComponent<TheaterLightDimmer>();
        dimmer.videoPlayer = videoPlayer;
        dimmer.theaterLights = new Light[] { l1, l2, l3, lb };
        dimmer.brightIntensity = 0.85f;
        dimmer.dimIntensity = 0.20f;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();

        Debug.Log("EGYPTIAN HISTORY PROJECTION HALL INSTALLED SUCCESSFULLY!");
    }
}

