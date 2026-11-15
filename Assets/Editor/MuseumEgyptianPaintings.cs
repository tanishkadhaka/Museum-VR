using UnityEngine;using UnityEditor;using UnityEditor.SceneManagement;using HeritageVR;using TMPro;using System.IO;using System.Linq;
[InitializeOnLoad]
public static class MuseumEgyptianPaintings{
 const string Trigger="/Users/aasthathakur/Documents/Codex/2026-10-28/rea/work/add-egyptian-paintings.trigger";
 const string Output="/Users/aasthathakur/Documents/Codex/2026-10-28/rea/outputs/";
 static MuseumEgyptianPaintings(){EditorApplication.update+=Poll;}
 static void Poll(){if(!File.Exists(Trigger)||EditorApplication.isCompiling||EditorApplication.isUpdating)return;if(EditorApplication.isPlaying){EditorApplication.isPlaying=false;return;}if(EditorApplication.isPlayingOrWillChangePlaymode)return;File.Delete(Trigger);try{AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);Install();QuestMuseumSetup.BuildAPK();File.WriteAllText(Output+"Egyptian-Paintings-Complete.txt","Six Egyptian-inspired side-wall paintings installed, three on each wall. Scene saved. Quest APK rebuilt. Physical headset testing pending.\n");}catch(System.Exception e){File.WriteAllText(Output+"Egyptian-Paintings-Complete.txt",e.ToString());Debug.LogException(e);}}
 static Material Mat(string name,Color color,Texture tex=null){string path="Assets/HeritageVR/Paintings/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Unlit"));AssetDatabase.CreateAsset(m,path);}m.SetColor("_BaseColor",color);if(tex)m.SetTexture("_BaseMap",tex);EditorUtility.SetDirty(m);return m;}
 static void Box(Transform p,string name,Vector3 pos,Vector3 size,Material m){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(p,false);g.transform.localPosition=pos;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=m;Object.DestroyImmediate(g.GetComponent<Collider>());}
 [MenuItem("Museum/14 Add Six Egyptian Side Wall Paintings")]
 public static void Install(){
 if(EditorApplication.isPlaying)throw new System.Exception("Stop playback first.");
 if(EditorSceneManager.GetActiveScene().path!="Assets/Scenes/UnifiedMuseum.unity")EditorSceneManager.OpenScene("Assets/Scenes/UnifiedMuseum.unity");
 var e=Object.FindObjectOfType<MuseumExperience>();var old=e.gallery.Find("SIDE WALLS • Egyptian art collection");if(old)Object.DestroyImmediate(old.gameObject);
 var root=new GameObject("SIDE WALLS • Egyptian art collection").transform;root.SetParent(e.gallery,false);
 string[] names={"NILE AT DAWN","PYRAMIDS IN GOLD","ROYAL PROCESSION","LOTUS & IBIS","TEMPLE OF LIGHT","WINGED SUN"};
 var frame=Mat("Egyptian brass frame",new Color(.61f,.43f,.2f));var backing=Mat("Egyptian dark backing",new Color(.025f,.035f,.05f));var mount=Mat("Egyptian ivory mount",new Color(.76f,.7f,.58f));var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/HeritageVR/Fonts/HeritageSans SDF.asset");
 for(int i=0;i<6;i++){
 string path="Assets/HeritageVR/Paintings/Egyptian-"+(i+1)+".png";var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);if(!texture)throw new System.Exception("Missing painting "+path);var imp=(TextureImporter)AssetImporter.GetAtPath(path);imp.mipmapEnabled=true;imp.wrapMode=TextureWrapMode.Clamp;imp.anisoLevel=4;imp.maxTextureSize=2048;var android=imp.GetPlatformTextureSettings("Android");android.overridden=true;android.maxTextureSize=1024;android.format=TextureImporterFormat.ASTC_6x6;imp.SetPlatformTextureSettings(android);imp.SaveAndReimport();
 var p=new GameObject((i<3?"WEST":"EAST")+" • "+names[i]).transform;p.SetParent(root,false);p.localPosition=new Vector3(i<3?-11.72f:11.72f,3.1f,3+(i%3)*7);p.localRotation=Quaternion.Euler(0,i<3?-90:90,0);
 Box(p,"Brass frame",Vector3.zero,new Vector3(3.55f,3.55f,.08f),frame);Box(p,"Archival mount",new Vector3(0,0,-.05f),new Vector3(3.4f,3.4f,.02f),mount);
 var art=GameObject.CreatePrimitive(PrimitiveType.Quad);art.name="Egyptian-inspired original artwork";art.transform.SetParent(p,false);art.transform.localPosition=new Vector3(0,0,-.07f);art.transform.localScale=new Vector3(3.12f,3.12f,1);art.GetComponent<Renderer>().sharedMaterial=Mat("Egyptian artwork "+(i+1),Color.white,texture);Object.DestroyImmediate(art.GetComponent<Collider>());
 var label=new GameObject("Painting title",typeof(RectTransform)).AddComponent<TextMeshPro>();label.transform.SetParent(p,false);label.transform.localPosition=new Vector3(0,-1.99f,-.07f);label.font=font;label.text=names[i];label.fontSize=1.25f;label.enableAutoSizing=true;label.fontSizeMin=1;label.fontSizeMax=1.25f;label.alignment=TextAlignmentOptions.Center;label.rectTransform.sizeDelta=new Vector2(3.5f,.25f);label.color=new Color(.9f,.8f,.6f);
 }
 EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
 var pos=e.rig.position;var rot=e.rig.rotation;var hp=e.head.localPosition;var hr=e.head.localRotation;bool menu=e.menu.gameObject.activeSelf;e.menu.gameObject.SetActive(false);e.info.gameObject.SetActive(false);e.comfort.gameObject.SetActive(false);e.head.localPosition=new Vector3(0,1.65f,0);e.head.localRotation=Quaternion.Euler(-8,0,0);
 for(int side=0;side<2;side++){e.rig.position=new Vector3(0,0,10);e.rig.rotation=Quaternion.Euler(0,side==0?-90:90,0);MuseumAlignmentAudit.Render(e.view,side==0?"Egyptian-West-Wall":"Egyptian-East-Wall");}
 e.rig.position=pos;e.rig.rotation=rot;e.head.localPosition=hp;e.head.localRotation=hr;e.menu.gameObject.SetActive(menu);
 if(root.childCount!=6)throw new System.Exception("Expected six paintings");Debug.Log("SIX EGYPTIAN SIDE WALL PAINTINGS SAVED");
 }
}
