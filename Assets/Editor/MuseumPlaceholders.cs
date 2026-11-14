using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using HeritageVR;
using TMPro;
using System.Linq;
using System.IO;
public static class MuseumPlaceholders {
 [MenuItem("Museum/11 Set All Tables to Placeholders")]
 public static void Apply(){
  if(EditorApplication.isPlaying)throw new System.Exception("Stop playback before replacing artifacts.");
  const string scene="Assets/Scenes/UnifiedMuseum.unity";if(EditorSceneManager.GetActiveScene().path!=scene)EditorSceneManager.OpenScene(scene);
  var e=Object.FindObjectOfType<MuseumExperience>();var exhibits=e.gallery.GetComponentsInChildren<MuseumExhibit>(true).OrderBy(x=>x.name).ToArray();
  var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/HeritageVR/Fonts/HeritageSans SDF.asset");
  var material=AssetDatabase.LoadAssetAtPath<Material>("Assets/HeritageVR/Materials/Unified brushed brass.mat");
  for(int i=0;i<exhibits.Length;i++){
   var exhibit=exhibits[i];foreach(var child in exhibit.artifact.Cast<Transform>().ToArray())Object.DestroyImmediate(child.gameObject);
   exhibit.artifact.localRotation=Quaternion.identity;exhibit.artifact.localScale=Vector3.one;
   exhibit.title="ARTIFACT SLOT "+(i+1).ToString("00");exhibit.description="Reserved for your artifact.\n\nA placeholder is displayed on this table.\nThe final model and information will be added later.";exhibit.name="DISPLAY "+(i+1).ToString("00")+" • "+exhibit.title;
   foreach(var label in exhibit.GetComponentsInChildren<TMP_Text>(true)){label.text=(i+1).ToString("00")+" / RESERVED ARTIFACT";label.name="Table slot label";}
   var shape=GameObject.CreatePrimitive(PrimitiveType.Cylinder);shape.name="PLACEHOLDER • replace with your artifact";shape.transform.SetParent(exhibit.artifact,false);shape.transform.localPosition=new Vector3(0,.22f,0);shape.transform.localScale=new Vector3(.48f,.22f,.48f);shape.GetComponent<Renderer>().sharedMaterial=material;Object.DestroyImmediate(shape.GetComponent<Collider>());
   var go=new GameObject("Placeholder label",typeof(RectTransform));var text=go.AddComponent<TextMeshPro>();go.transform.SetParent(exhibit.artifact,false);go.transform.localPosition=new Vector3(0,.22f,-.25f);text.font=font;text.text="ARTIFACT\nPLACEHOLDER";text.fontSize=.65f;text.color=Color.white;text.alignment=TextAlignmentOptions.Center;text.rectTransform.sizeDelta=new Vector2(1,.5f);
   EditorUtility.SetDirty(exhibit);
  }
  e.selected=exhibits[0];e.statue=exhibits[0].artifact;e.ceramicModel=exhibits[0].artifact;e.exhibitTitle.text=exhibits[0].title;e.exhibitDescription.text=exhibits[0].description;e.info.gameObject.SetActive(false);e.exhibitTitle.name="Exhibit title";e.exhibitDescription.name="Exhibit description";
  EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  if(exhibits.Length!=8)throw new System.Exception("Expected eight tables; found "+exhibits.Length);
  foreach(var exhibit in exhibits)if(exhibit.artifact.childCount!=2)throw new System.Exception("Unexpected artifact child remains.");
  File.WriteAllText("/Users/aasthathakur/Documents/Codex/2026-10-28/rea/outputs/Placeholder-Update.txt","Eight tables contain only a generic placeholder and label. Original displayed models, source models, and old exhibit metadata removed from active project. Paintings retained.\n");Debug.Log("ALL EIGHT TABLES NOW USE PLACEHOLDERS");
 }
 public static void ApplyAndBuild(){
  Apply();
  var dependencies=AssetDatabase.GetDependencies("Assets/Scenes/UnifiedMuseum.unity",true);
  foreach(string folder in new[]{"Assets/ThreeDragons","Assets/Ancient Pots","Assets/AK Studio Art","Assets/Models","Assets/Prefab","Assets/Textures","Assets/Images","Assets/Materials","Assets/HeritageVR/Meshes"}){
   foreach(string guid in AssetDatabase.FindAssets("",new[]{folder}).Reverse()){
    string path=AssetDatabase.GUIDToAssetPath(guid);
    if(!AssetDatabase.IsValidFolder(path)&&!dependencies.Contains(path))AssetDatabase.DeleteAsset(path);
   }
  }
  AssetDatabase.DeleteAsset("Assets/Editor/SourceScenes");
  QuestMuseumSetup.PreviewPaintings();
  QuestMuseumSetup.BuildAPK();
 }
}
