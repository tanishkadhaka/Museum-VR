using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using HeritageVR;
using TMPro;
using System.Linq;
using System.IO;
using System.Text;
public static class MuseumAlignmentAudit {
 public const string Output="/Users/aasthathakur/Documents/Codex/2026-10-28/rea/outputs/Alignment/";
 public static void Render(Camera camera,string name){Directory.CreateDirectory(Output);var rt=new RenderTexture(1280,900,24);var old=camera.targetTexture;camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;var t=new Texture2D(1280,900,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,1280,900),0,0);t.Apply();File.WriteAllBytes(Output+name+".png",t.EncodeToPNG());camera.targetTexture=old;RenderTexture.active=null;Object.DestroyImmediate(t);Object.DestroyImmediate(rt);}
 public static void Audit(){
 EditorSceneManager.OpenScene("Assets/Scenes/UnifiedMuseum.unity");var e=Object.FindObjectOfType<MuseumExperience>();var s=new StringBuilder();
 foreach(var t in Object.FindObjectsOfType<TMP_Text>(true))s.AppendLine(t.name+" | "+t.text.Replace("\n"," / ")+" | local "+t.transform.localPosition+" scale "+t.transform.lossyScale+" rect "+t.rectTransform.sizeDelta+" font "+t.fontSize+" parent "+(t.transform.parent?t.transform.parent.name:"ROOT"));
 int i=0;e.menu.gameObject.SetActive(false);e.info.gameObject.SetActive(false);e.comfort.gameObject.SetActive(false);
 foreach(var x in e.gallery.GetComponentsInChildren<MuseumExhibit>(true).OrderBy(x=>x.name)){
 s.AppendLine("EXHIBIT "+x.name+" pos "+x.transform.position+" slot "+x.artifact.position+" view "+x.viewingPosition);
 foreach(var r in x.artifact.GetComponentsInChildren<Renderer>())s.AppendLine(" MESH "+r.name+" bounds "+r.bounds+" rot "+r.transform.eulerAngles);
 e.rig.position=x.viewingPosition;e.rig.rotation=Quaternion.identity;e.head.localPosition=new Vector3(0,1.65f,0);e.head.localRotation=Quaternion.Euler(6,0,0);e.view.stereoTargetEye=StereoTargetEyeMask.None;Render(e.view,"Before-"+(++i));
 }
 Directory.CreateDirectory(Output);File.WriteAllText(Output+"Before-Audit.txt",s.ToString());
 }
}
