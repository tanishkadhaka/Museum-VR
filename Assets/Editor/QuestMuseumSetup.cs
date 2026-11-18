using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Management;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;
using UnityEditor.XR.OpenXR.Features;
using UnityEditor.XR.OpenXR;
using UnityEditor.Build.Reporting;
using TMPro;
using HeritageVR;
using System.Collections.Generic;
using System.Linq;
using System.IO;
public static class QuestMuseumSetup {
 const string Scene="Assets/Scenes/UnifiedMuseum.unity";
 const string Root="Assets/HeritageVR/";
 static string Output=>System.IO.Path.GetFullPath("apk")+"/";
 public static void Prepare(){Paintings();Configure();}
 public static void BuildFinal(){Paintings();BuildAPK();}
 static GameObject Box(string name,Vector3 pos,Vector3 scale,Material mat,Transform parent){var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=pos;go.transform.localScale=scale;go.GetComponent<Renderer>().sharedMaterial=mat;Object.DestroyImmediate(go.GetComponent<Collider>());return go;}
 [MenuItem("Museum/06 Add Back Wall Paintings")]
 public static void Paintings(){
  if(EditorApplication.isPlaying){Debug.LogError("Stop playback before editing.");return;}
  if(EditorSceneManager.GetActiveScene().path!=Scene)EditorSceneManager.OpenScene(Scene);
  var e=Object.FindObjectOfType<MuseumExperience>();var old=e.gallery.Find("BACK WALL • Painting collection");if(old)Object.DestroyImmediate(old.gameObject);
  var parent=new GameObject("BACK WALL • Painting collection").transform;parent.SetParent(e.gallery,false);
  string texturePath=Root+"Paintings/HeritageTriptych.png";var importer=(TextureImporter)AssetImporter.GetAtPath(texturePath);importer.maxTextureSize=4096;importer.mipmapEnabled=true;importer.wrapMode=TextureWrapMode.Clamp;importer.anisoLevel=4;var android=importer.GetPlatformTextureSettings("Android");android.overridden=true;android.maxTextureSize=2048;android.format=TextureImporterFormat.ASTC_6x6;importer.SetPlatformTextureSettings(android);importer.SaveAndReimport();
  var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);var brass=AssetDatabase.LoadAssetAtPath<Material>(Root+"Materials/Unified brushed brass.mat");var charcoal=AssetDatabase.LoadAssetAtPath<Material>(Root+"Materials/Unified deep charcoal.mat");var ivory=AssetDatabase.LoadAssetAtPath<Material>(Root+"Materials/Unified warm limestone.mat");var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Root+"Fonts/HeritageSans SDF.asset");
  string[] titles={"RIVER OF MEMORY","VESSELS & ARCHES","LOTUS AT DUSK"};
  for(int i=0;i<3;i++){
   float x=(i-1)*6;Box(titles[i]+" • walnut backing",new Vector3(x,2.8f,22.77f),new Vector3(3.95f,3.95f,.16f),charcoal,parent);Box("Brushed brass frame",new Vector3(x,2.8f,22.66f),new Vector3(3.86f,3.86f,.04f),brass,parent);Box("Archival mount",new Vector3(x,2.8f,22.62f),new Vector3(3.68f,3.68f,.025f),ivory,parent);
   string path=Root+"Paintings/"+titles[i]+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);if(!mat){mat=new Material(Shader.Find("Universal Render Pipeline/Unlit"));AssetDatabase.CreateAsset(mat,path);}mat.SetTexture("_BaseMap",texture);mat.SetTextureScale("_BaseMap",new Vector2(1f/3-.002f,1));mat.SetTextureOffset("_BaseMap",new Vector2(i/3f+.001f,0));mat.SetColor("_BaseColor",Color.white);EditorUtility.SetDirty(mat);
   var canvas=GameObject.CreatePrimitive(PrimitiveType.Quad);canvas.name=titles[i]+" • Original painting";canvas.transform.SetParent(parent,false);canvas.transform.localPosition=new Vector3(x,2.8f,22.59f);canvas.transform.localScale=new Vector3(3.45f,3.45f,1);canvas.GetComponent<Renderer>().sharedMaterial=mat;Object.DestroyImmediate(canvas.GetComponent<Collider>());
   var label=new GameObject("Painting label",typeof(RectTransform));var text=label.AddComponent<TextMeshPro>();label.transform.SetParent(parent,false);label.transform.localPosition=new Vector3(x,.58f,22.56f);text.font=font;text.fontSize=.9f;text.text=titles[i];text.color=new Color(.9f,.78f,.54f);text.alignment=TextAlignmentOptions.Center;text.rectTransform.sizeDelta=new Vector2(4,.3f);
  }
  foreach(var text in e.gallery.GetComponentsInChildren<TMP_Text>()){
   if(text.text=="H E R I T A G E"){text.transform.localPosition=new Vector3(0,5.4f,22.7f);text.fontSize=3f;}
   if(text.text=="OBJECTS  /  STORIES  /  CONNECTIONS"){text.transform.localPosition=new Vector3(0,4.98f,22.7f);text.fontSize=1.1f;}
  }
  EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();Debug.Log("BACK WALL PAINTINGS ADDED: three framed original artworks.");
 }
 [MenuItem("Museum/07 Configure Meta Quest Standalone")]
 public static void Configure(){
  PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,"com.heritagevr.virtualmuseum");PlayerSettings.productName="Heritage VR Museum";PlayerSettings.companyName="Heritage VR";
  PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android,ScriptingImplementation.IL2CPP);PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
  PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel29;PlayerSettings.Android.targetSdkVersion=(AndroidSdkVersions)34;
  PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;PlayerSettings.colorSpace=ColorSpace.Linear;PlayerSettings.gpuSkinning=true;
  PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android,false);PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,new[]{GraphicsDeviceType.OpenGLES3});PlayerSettings.Android.forceSDCardPermission=false;
  var player=new SerializedObject(Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings"));var input=player.FindProperty("activeInputHandler");if(input!=null){input.intValue=1;player.ApplyModifiedProperties();}
  XRGeneralSettingsPerBuildTarget targets;if(!EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey,out targets))throw new System.Exception("XR configuration missing.");
  if(!targets.HasSettingsForBuildTarget(BuildTargetGroup.Android))targets.CreateDefaultSettingsForBuildTarget(BuildTargetGroup.Android);var settings=targets.SettingsForBuildTarget(BuildTargetGroup.Android);if(!settings.Manager){var manager=ScriptableObject.CreateInstance<XRManagerSettings>();AssetDatabase.AddObjectToAsset(manager,targets);settings.Manager=manager;}settings.InitManagerOnStart=true;settings.Manager.automaticLoading=true;settings.Manager.automaticRunning=true;XRPackageMetadataStore.AssignLoader(settings.Manager,"UnityEngine.XR.OpenXR.OpenXRLoader",BuildTargetGroup.Android);EditorUtility.SetDirty(settings);EditorUtility.SetDirty(settings.Manager);EditorUtility.SetDirty(targets);
  var oxr=OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
  if(!oxr){AssetDatabase.SaveAssets();throw new System.Exception("Android OpenXR settings unavailable. Restart Unity after installing Android Build Support.");}
  FeatureHelpers.RefreshFeatures(BuildTargetGroup.Android);oxr.renderMode=OpenXRSettings.RenderMode.SinglePassInstanced;oxr.depthSubmissionMode=OpenXRSettings.DepthSubmissionMode.None;
  foreach(var feature in oxr.GetFeatures<OpenXRFeature>())if(feature)feature.enabled=false;
  var quest=oxr.GetFeature<MetaQuestFeature>();if(!quest)throw new System.Exception("Meta Quest OpenXR feature unavailable.");quest.enabled=true;oxr.latencyOptimization=OpenXRSettings.LatencyOptimization.PrioritizeInputPolling;
  var questData=new SerializedObject(quest);var discard=questData.FindProperty("m_optimizeBufferDiscards");if(discard!=null)discard.boolValue=false;var devices=questData.FindProperty("targetDevices");if(devices!=null)for(int i=0;i<devices.arraySize;i++){var device=devices.GetArrayElementAtIndex(i);var label=device.FindPropertyRelative("visibleName");var enabled=device.FindPropertyRelative("enabled");if(label!=null&&enabled!=null)enabled.boolValue=label.stringValue=="Quest 2"||label.stringValue=="Quest 3"||label.stringValue=="Quest 3S";}questData.ApplyModifiedProperties();
  var touch=oxr.GetFeature<OculusTouchControllerProfile>();if(touch)touch.enabled=true;foreach(var feature in oxr.GetFeatures<OpenXRFeature>())if(feature&&feature.GetType().Name=="MetaQuestTouchPlusControllerProfile")feature.enabled=true;
  EditorUtility.SetDirty(quest);EditorUtility.SetDirty(oxr);
  var pipe=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(Root+"MuseumURP.asset");pipe.renderScale=1;pipe.msaaSampleCount=4;pipe.supportsHDR=false;pipe.shadowDistance=18;var data=new SerializedObject(pipe);data.FindProperty("m_AdditionalLightShadowsSupported").boolValue=false;data.FindProperty("m_AdditionalLightsPerObjectLimit").intValue=4;data.ApplyModifiedProperties();EditorUtility.SetDirty(pipe);
  var renderer=AssetDatabase.LoadAssetAtPath<UniversalRendererData>(Root+"MuseumRenderer.asset");if(renderer){var r=new SerializedObject(renderer);var mode=r.FindProperty("m_RenderingMode");if(mode!=null)mode.intValue=0;r.ApplyModifiedProperties();EditorUtility.SetDirty(renderer);}
  if(EditorSceneManager.GetActiveScene().path!=Scene)EditorSceneManager.OpenScene(Scene);foreach(var light in Object.FindObjectsOfType<Light>())light.shadows=LightShadows.None;
  foreach(var camera in Object.FindObjectsOfType<Camera>()){camera.allowHDR=false;camera.allowMSAA=true;camera.allowDynamicResolution=false;}
  foreach(string path in AssetDatabase.GetDependencies(Scene,true)){
   var texture=AssetImporter.GetAtPath(path) as TextureImporter;if(!texture)continue;var platform=texture.GetPlatformTextureSettings("Android");platform.overridden=true;platform.maxTextureSize=2048;platform.format=TextureImporterFormat.ASTC_6x6;texture.SetPlatformTextureSettings(platform);texture.SaveAndReimport();
  }
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();ValidateQuest();Debug.Log("QUEST STANDALONE CONFIGURATION SAVED");
 }
 [MenuItem("Museum/08 Validate Quest Build Settings")]
 public static void ValidateQuest(){
  var issues=new List<OpenXRFeature.ValidationRule>();OpenXRProjectValidation.GetCurrentValidationIssues(issues,BuildTargetGroup.Android);
  string report="Quest 2 / Quest 3 / Quest 3S standalone target\nUnity 2022.3.62f3 • Android ARM64 • IL2CPP • OpenXR • OpenGLES3 • single-pass stereo • 4x MSAA • ASTC textures\n";
  foreach(var issue in issues)report+=(issue.error?"ERROR: ":"NOTE: ")+issue.message+"\n";
  var scene=EditorSceneManager.GetActiveScene();long triangles=0;foreach(var root in scene.GetRootGameObjects())foreach(var mf in root.GetComponentsInChildren<MeshFilter>(true))if(mf.sharedMesh){for(int i=0;i<mf.sharedMesh.subMeshCount;i++)triangles+=(long)mf.sharedMesh.GetIndexCount(i)/3;}
  report+="Total scene triangles including inactive objects: "+triangles+"\n";report+="OpenXR blocking issues: "+issues.Count(i=>i.error)+"\nPhysical headset testing is still required; a successful APK build is not a device performance test.\n";Directory.CreateDirectory(Output);File.WriteAllText(Output+"Quest-Compatibility.txt",report);Debug.Log(report);
 }
 [MenuItem("Museum/10 Preview Back Wall Paintings")]
 public static void PreviewPaintings(){
  if(EditorSceneManager.GetActiveScene().path!=Scene)EditorSceneManager.OpenScene(Scene);
  var e=Object.FindObjectOfType<MuseumExperience>();e.rig.position=new Vector3(0,0,12);e.rig.rotation=Quaternion.identity;e.head.localPosition=new Vector3(0,1.65f,0);e.head.localRotation=Quaternion.identity;e.menu.gameObject.SetActive(false);e.info.gameObject.SetActive(false);e.comfort.gameObject.SetActive(false);e.view.stereoTargetEye=StereoTargetEyeMask.None;
  var rt=new RenderTexture(1600,900,24);e.view.targetTexture=rt;e.view.Render();RenderTexture.active=rt;var texture=new Texture2D(1600,900,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,1600,900),0,0);texture.Apply();File.WriteAllBytes(Output+"Museum-Back-Wall.png",texture.EncodeToPNG());e.view.targetTexture=null;RenderTexture.active=null;Object.DestroyImmediate(texture);Object.DestroyImmediate(rt);
  if(EditorApplication.isPlaying)e.room=2;else EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
  var game=typeof(Editor).Assembly.GetType("UnityEditor.GameView");EditorWindow.GetWindow(game).Focus();
 }
 [MenuItem("Museum/09 Build Quest APK")]
 public static void BuildAPK(){
  if(EditorApplication.isPlaying)throw new System.Exception("Stop playback before building.");Configure();var issues=new List<OpenXRFeature.ValidationRule>();OpenXRProjectValidation.GetCurrentValidationIssues(issues,BuildTargetGroup.Android);if(issues.Any(i=>i.error))throw new System.Exception("Resolve OpenXR blocking issues before build.");
  Directory.CreateDirectory(Output);var options=new BuildPlayerOptions{scenes=new[]{Scene},target=BuildTarget.Android,locationPathName=Output+"HeritageVR-Quest.apk",options=BuildOptions.None};var result=BuildPipeline.BuildPlayer(options);File.WriteAllText(Output+"Quest-Build.txt",result.summary.result+"\nErrors: "+result.summary.totalErrors+"\nSize: "+result.summary.totalSize+" bytes\n");if(result.summary.result!=BuildResult.Succeeded)throw new System.Exception("Quest APK build failed.");Debug.Log("QUEST APK BUILT: "+options.locationPathName);
 }
}
