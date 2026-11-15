using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using HeritageVR;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
public static class MuseumArtifactAlignment {
 const string Scene="Assets/Scenes/UnifiedMuseum.unity";
 public static Bounds Geometry(Transform root,Transform relative){
  bool first=true;var bounds=new Bounds();
  foreach(var mf in root.GetComponentsInChildren<MeshFilter>(true)){
   if(!mf.sharedMesh)continue;
   foreach(var vertex in mf.sharedMesh.vertices){var p=relative.InverseTransformPoint(mf.transform.TransformPoint(vertex));if(first){bounds=new Bounds(p,Vector3.zero);first=false;}else bounds.Encapsulate(p);}
  }
  if(first)throw new System.Exception("No mesh geometry in "+root.name);
  return bounds;
 }
 static void Text(TMP_Text t,Vector2 size,float min,float max,TextAlignmentOptions alignment){
  t.rectTransform.sizeDelta=size;t.enableAutoSizing=true;t.fontSizeMin=min;t.fontSizeMax=max;t.fontSize=max;t.alignment=alignment;t.enableWordWrapping=true;t.overflowMode=TextOverflowModes.Overflow;t.margin=Vector4.zero;t.characterSpacing=0;t.lineSpacing=0;t.paragraphSpacing=0;t.color=new Color(.94f,.93f,.88f);t.ForceMeshUpdate(true,true);
 }
 static void CardText(TMP_Text t,Vector3 position,Vector2 size,float min,float max,TextAlignmentOptions alignment){t.transform.localPosition=position;t.transform.localRotation=Quaternion.identity;t.transform.localScale=Vector3.one;Text(t,size,min,max,alignment);}
 static void Button(MuseumAction action,Vector3 position,Vector3 size){
  action.transform.localPosition=position;action.transform.localScale=size;action.transform.localRotation=Quaternion.identity;
  // Button labels use world dimensions, independent of the stretched cube.
  foreach(var t in action.GetComponentsInChildren<TMP_Text>(true)){
   t.transform.localPosition=new Vector3(0,0,-.52f);t.transform.localScale=new Vector3(1/size.x,1/size.y,1/size.z);
   Text(t,new Vector2(size.x-.12f,size.y-.04f),.75f,1.05f,TextAlignmentOptions.Center);if(action.action=="inspect")t.color=new Color(.08f,.06f,.03f);
  }
 }
 static void PanelPart(Transform parent,string name,Vector3 position,Vector3 scale,Color color){
  string path="Assets/HeritageVR/Materials/"+name+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);if(!mat){mat=new Material(Shader.Find("Universal Render Pipeline/Unlit"));AssetDatabase.CreateAsset(mat,path);}mat.SetColor("_BaseColor",color);EditorUtility.SetDirty(mat);var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=position;go.transform.localScale=scale;go.GetComponent<Renderer>().sharedMaterial=mat;Object.DestroyImmediate(go.GetComponent<Collider>());
 }
 public static void Layout(MuseumExperience e){
  e.info.localScale=Vector3.one;
  foreach(Transform child in e.info.Cast<Transform>().ToArray())if(child.GetComponent<Renderer>()&&!child.GetComponent<TMP_Text>()&&!child.GetComponent<MuseumAction>())Object.DestroyImmediate(child.gameObject);
  PanelPart(e.info,"Information background",Vector3.zero,new Vector3(2.05f,1.5f,.035f),new Color(.035f,.045f,.065f));
  PanelPart(e.info,"Top accent",new Vector3(0,.75f,-.025f),new Vector3(2.05f,.015f,.015f),new Color(.7f,.49f,.23f));
  CardText(e.exhibitTitle,new Vector3(0,.4f,-.065f),new Vector2(1.78f,.28f),1.05f,1.5f,TextAlignmentOptions.TopLeft);
  CardText(e.exhibitDescription,new Vector3(0,-.12f,-.065f),new Vector2(1.78f,.66f),.6f,.8f,TextAlignmentOptions.TopLeft);
  foreach(var t in e.info.GetComponentsInChildren<TMP_Text>(true))if(t!=e.exhibitTitle&&t!=e.exhibitDescription&&!t.GetComponentInParent<MuseumAction>(true)){
   t.name="Collection heading";t.text="HERITAGE VR  /  COLLECTION NOTES";CardText(t,new Vector3(0,.61f,-.065f),new Vector2(1.78f,.12f),.65f,.7f,TextAlignmentOptions.Left);
  }
  foreach(var a in e.info.GetComponentsInChildren<MuseumAction>(true)){foreach(var label in a.GetComponentsInChildren<TMP_Text>(true))label.text=a.action=="inspect"?"INSPECT IN 3D":"CLOSE";Button(a,new Vector3(a.action=="inspect"?-.48f:.48f,-.59f,-.075f),new Vector3(.9f,.23f,.035f));}
  foreach(var t in Object.FindObjectsOfType<TMP_Text>(true)){
   if(t.name=="Painting label")Text(t,new Vector2(4,.25f),.9f,1.1f,TextAlignmentOptions.Center);
   if(t.text=="THE COLLECTION"){t.transform.localPosition=new Vector3(0,4.3f,1);Text(t,new Vector2(5,.5f),2.3f,2.8f,TextAlignmentOptions.Center);}
   if(t.text=="H E R I T A G E")Text(t,new Vector2(6,.5f),2.6f,3f,TextAlignmentOptions.Center);
   if(t.text=="OBJECTS  /  STORIES  /  CONNECTIONS")Text(t,new Vector2(6,.25f),1.1f,1.3f,TextAlignmentOptions.Center);
  }
  foreach(var text in e.comfort.GetComponentsInChildren<TMP_Text>(true)){text.text="CONTROLS & COMFORT\n\nQUEST\nRight trigger: select an exhibit or button\nLeft stick forward: take one step\nRight stick: snap turn; rotate while inspecting\nB: close information or leave inspection\n\nDESKTOP\nWASD: move  /  Right-drag: look\nClick: select  /  I: inspect nearest exhibit\nA / D: rotate  /  Escape: back\n\nMove your head naturally to look around.";text.transform.localPosition=new Vector3(0,-.12f,-.08f);Text(text,new Vector2(2.8f,2.3f),.8f,1.05f,TextAlignmentOptions.TopLeft);}
  // Give foreground card glyphs their own depth-independent SDF material.
  var foreground=AssetDatabase.LoadAssetAtPath<Material>("Assets/HeritageVR/Materials/Information text overlay.mat");
  if(!foreground){foreground=new Material(e.exhibitTitle.fontSharedMaterial);AssetDatabase.CreateAsset(foreground,"Assets/HeritageVR/Materials/Information text overlay.mat");}
  foreground.SetFloat("_ZTestMode",8);foreground.renderQueue=3100;EditorUtility.SetDirty(foreground);
  foreach(var text in e.info.GetComponentsInChildren<TMP_Text>(true))text.fontSharedMaterial=foreground;
  // Bound every arrival-menu text rectangle to its panel or button.
  foreach(var t in e.menu.GetComponentsInChildren<TMP_Text>(true)){
   var action=t.GetComponentInParent<MuseumAction>(true);
   if(action){var scale=action.transform.localScale;t.transform.localPosition=new Vector3(0,0,-.52f);t.transform.localScale=new Vector3(1/scale.x,1/scale.y,1/scale.z);Text(t,new Vector2(scale.x-.15f,scale.y-.04f),.9f,1.25f,TextAlignmentOptions.Center);if(action.action=="start")t.color=new Color(.08f,.06f,.03f);}
   else {float y=t.transform.localPosition.y;Text(t,new Vector2(2.9f,y>.45f?.35f:.25f),y>.45f?1.2f:1f,t.text=="HERITAGE VR"?3.2f:y>.8f?.8f:1.1f,TextAlignmentOptions.Center);}
  }
  var hint=e.head.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault();if(hint){hint.name="Controller guide";hint.transform.localPosition=new Vector3(0,-.72f,1.4f);Text(hint,new Vector2(2.5f,.12f),.32f,.4f,TextAlignmentOptions.Center);var mat=AssetDatabase.LoadAssetAtPath<Material>("Assets/HeritageVR/Materials/Controller hint overlay.mat");if(!mat){mat=new Material(hint.fontSharedMaterial);AssetDatabase.CreateAsset(mat,"Assets/HeritageVR/Materials/Controller hint overlay.mat");}mat.SetFloat("_ZTestMode",8);mat.renderQueue=4000;EditorUtility.SetDirty(mat);hint.fontSharedMaterial=mat;}
 }
 [MenuItem("Museum/13 Fix Artifact Alignment and Text")]
 public static void Apply(){
  if(EditorApplication.isPlaying)throw new System.Exception("Stop Play mode first.");
  EditorSceneManager.OpenScene(Scene);MuseumArtifactSetup.Populate();var e=Object.FindObjectOfType<MuseumExperience>();
  var exhibits=e.gallery.GetComponentsInChildren<MuseumExhibit>(true).OrderBy(x=>x.name).ToArray();var report=new StringBuilder();
  for(int i=0;i<exhibits.Length;i++){
   var x=exhibits[i];var slot=x.artifact;slot.localRotation=Quaternion.identity;slot.localScale=Vector3.one;var model=slot.GetChild(0);model.localPosition=Vector3.zero;model.localScale=Vector3.one;
   model.localRotation=Quaternion.Euler(MuseumArtifactSetup.Artifacts[i].localEulerAngles);
   var b=Geometry(model,slot);float maxWidth=1.6f,maxDepth=1.1f,maxHeight=i==7?1.65f:1.25f;
   // Fit within a circular turntable footprint, including a full inspection rotation.
   float diameter=Mathf.Sqrt(b.size.x*b.size.x+b.size.z*b.size.z);
   float scale=Mathf.Min(maxHeight/b.size.y,Mathf.Min(maxWidth/b.size.x,maxDepth/b.size.z));scale=Mathf.Min(scale,1.6f/diameter);
   model.localScale*=scale;b=Geometry(model,slot);model.localPosition-=new Vector3(b.center.x,b.min.y,b.center.z);b=Geometry(model,slot);
   if(i==0)foreach(var renderer in model.GetComponentsInChildren<Renderer>(true)){
    var materials=renderer.sharedMaterials;for(int m=0;m<materials.Length;m++){var source=materials[m];if(!source)continue;string path="Assets/HeritageVR/Materials/Display "+source.name+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);if(!mat){mat=new Material(source);AssetDatabase.CreateAsset(mat,path);}if(mat.HasProperty("_Smoothness"))mat.SetFloat("_Smoothness",.35f);if(mat.HasProperty("_Metallic"))mat.SetFloat("_Metallic",.55f);if(mat.HasProperty("_EnvironmentReflections"))mat.SetFloat("_EnvironmentReflections",1);EditorUtility.SetDirty(mat);materials[m]=mat;}renderer.sharedMaterials=materials;
   }
   foreach(var c in model.GetComponentsInChildren<Collider>(true))Object.DestroyImmediate(c);
   var collider=slot.GetComponent<BoxCollider>();collider.center=b.center;collider.size=b.size+Vector3.one*.06f;collider.enabled=true;
   x.viewingPosition=x.transform.position+new Vector3(0,0,i>=6?-3.2f:-2.8f);
   foreach(var t in x.GetComponentsInChildren<TMP_Text>(true))if(t.name=="Table slot label"){
    t.transform.localPosition=new Vector3(0,.88f,-1.011f);t.transform.localRotation=Quaternion.identity;t.transform.localScale=Vector3.one;Text(t,new Vector2(2.3f,.23f),.7f,.9f,TextAlignmentOptions.Center);
   }
   report.AppendLine(x.title+" | local mesh bounds "+b.ToString("F4")+" | model Euler "+model.localEulerAngles+" | view "+x.viewingPosition);
   if(Mathf.Abs(b.min.y)>.001f||Mathf.Abs(b.center.x)>.001f||Mathf.Abs(b.center.z)>.001f)throw new System.Exception("Grounding/centering failed: "+x.title);
  }
  Layout(e);e.selected=exhibits[0];e.statue=exhibits[0].artifact;e.exhibitTitle.text=exhibits[0].title;e.exhibitDescription.text=exhibits[0].description;
  EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();Directory.CreateDirectory(MuseumAlignmentAudit.Output);File.WriteAllText(MuseumAlignmentAudit.Output+"Alignment-Validation.txt",report.ToString());
  Preview();
 }
 public static void Preview(){
  var e=Object.FindObjectOfType<MuseumExperience>();e.menu.gameObject.SetActive(false);e.info.gameObject.SetActive(false);e.comfort.gameObject.SetActive(false);int i=0;
  foreach(var x in e.gallery.GetComponentsInChildren<MuseumExhibit>(true).OrderBy(x=>x.name)){
   e.rig.position=x.viewingPosition;e.rig.rotation=Quaternion.identity;e.head.localPosition=new Vector3(0,1.65f,0);e.head.localRotation=Quaternion.Euler(6,0,0);e.view.stereoTargetEye=StereoTargetEyeMask.None;
   MuseumAlignmentAudit.Render(e.view,"After-"+(++i));
   e.ShowExhibit(x);MuseumAlignmentAudit.Render(e.view,"Info-"+i);e.info.gameObject.SetActive(false);
  }
 }
}
