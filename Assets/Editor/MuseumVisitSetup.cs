using UnityEngine;using UnityEditor;using UnityEditor.SceneManagement;using HeritageVR;using System.Linq;
public static class MuseumVisitSetup {
 public static void Apply(){
 if(EditorSceneManager.GetActiveScene().path!="Assets/Scenes/UnifiedMuseum.unity")EditorSceneManager.OpenScene("Assets/Scenes/UnifiedMuseum.unity");var e=Object.FindObjectOfType<MuseumExperience>();int count=0;
 foreach(string name in new[]{"SIDE WALLS • Egyptian art collection","BACK WALL • Painting collection"}){
 var root=e.gallery.Find(name);if(!root)throw new System.Exception("Missing artwork group "+name);
 foreach(var filter in root.GetComponentsInChildren<MeshFilter>(true)){
  if(!filter.sharedMesh||filter.sharedMesh.name!="Quad")continue;
  var art=filter.transform;var painting=art.GetComponent<MuseumPainting>();if(!painting)painting=art.gameObject.AddComponent<MuseumPainting>();
  string title=art.parent.name;if(title.Contains(" • "))title=title.Split(new[]{" • "},System.StringSplitOptions.None).Last();if(art.name.Contains("Original painting"))title=art.name.Split(new[]{" • "},System.StringSplitOptions.None)[0];
  painting.title=title;
  painting.viewingPosition=art.position+art.TransformDirection(Vector3.back)*3.8f;painting.viewingPosition=new Vector3(painting.viewingPosition.x,0,painting.viewingPosition.z);
  var box=art.GetComponent<BoxCollider>();if(!box)box=art.gameObject.AddComponent<BoxCollider>();box.center=Vector3.zero;box.size=new Vector3(1,1,.04f);box.enabled=true;EditorUtility.SetDirty(painting);count++;
 }
 }
 if(count!=9)throw new System.Exception("Expected nine paintings; found "+count);
 EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();Debug.Log("NINE PAINTINGS NOW SUPPORT TRAVEL ONLY");
 }
 public static void Build(){Apply();QuestMuseumSetup.BuildAPK();}
}
