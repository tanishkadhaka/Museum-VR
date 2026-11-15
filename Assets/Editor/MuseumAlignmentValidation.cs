using UnityEngine;using UnityEditor;using UnityEditor.SceneManagement;using HeritageVR;using TMPro;using System.Linq;using System.IO;using System.Text;
[InitializeOnLoad]
public static class MuseumAlignmentValidation{
 const string Key="HeritageAlignmentValidation";
 static MuseumAlignmentValidation(){EditorApplication.update+=Tick;}
 public static void ValidateAndBuild(){MuseumArtifactAlignment.Apply();EditorSceneManager.OpenScene("Assets/Scenes/UnifiedMuseum.unity");SessionState.SetInt(Key,1);EditorApplication.isPlaying=true;}
 static void Check(bool condition,string message){if(!condition)throw new System.Exception(message);}
 static void Tick(){int phase=SessionState.GetInt(Key,0);if(phase==0)return;
 try{
  if(phase==1&&EditorApplication.isPlaying&&Time.frameCount>8){Verify();SessionState.SetInt(Key,2);EditorApplication.isPlaying=false;}
  else if(phase==2&&!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetInt(Key,0);QuestMuseumSetup.BuildAPK();EditorApplication.Exit(0);}
 }catch(System.Exception ex){SessionState.SetInt(Key,0);File.WriteAllText(MuseumAlignmentAudit.Output+"Play-Validation.txt","FAILED: "+ex);Debug.LogException(ex);EditorApplication.Exit(1);}
 }
 static void Verify(){
 var e=Object.FindObjectOfType<MuseumExperience>();var report=new StringBuilder("Unity 2022.3.62f3 desktop Play Mode validation\n");
 e.Go(0);MuseumAlignmentAudit.Render(e.view,"Entrance-Final");e.Act("comfort");MuseumAlignmentAudit.Render(e.view,"Controls-Final");e.Act("comfort");
 var start=e.menu.GetComponentsInChildren<MuseumAction>().First(a=>a.action=="start");Physics.SyncTransforms();Check(e.SelectRay(new Ray(e.head.position,(start.transform.position-e.head.position).normalized)),"Start button pointer failed");Check(e.room==1,"Start failed");
 var exhibits=e.gallery.GetComponentsInChildren<MuseumExhibit>().OrderBy(x=>x.name).ToArray();Check(exhibits.Length==8,"Eight exhibits required");int index=0;
 foreach(var x in exhibits){index++;e.Go(1);e.rig.position=x.viewingPosition;e.head.localRotation=Quaternion.identity;var b=MuseumArtifactAlignment.Geometry(x.artifact,x.artifact);
 Check(Mathf.Abs(b.min.y)<.002f&&Mathf.Abs(b.center.x)<.002f&&Mathf.Abs(b.center.z)<.002f,"Model is not centered/grounded: "+x.title);
 var box=x.artifact.GetComponent<BoxCollider>();Check(box&&box.enabled,"Selection collider missing");Check(box.size.y>=b.size.y&&box.size.x>=b.size.x&&box.size.z>=b.size.z,"Collider does not cover model");
 Physics.SyncTransforms();Check(e.SelectRay(new Ray(e.head.position,(x.artifact.TransformPoint(box.center)-e.head.position).normalized)),"Exhibit pointer failed: "+x.title);Check(e.selected==x&&e.info.gameObject.activeSelf,"Wrong selected exhibit");
 MuseumAlignmentAudit.Render(e.view,"Play-Info-"+index);
 foreach(var t in new[]{e.exhibitTitle,e.exhibitDescription}){t.ForceMeshUpdate(true,true);float preferred=t.textBounds.size.y;report.AppendLine(x.title+" / "+t.name+" text height "+preferred.ToString("F3")+" / available "+t.rectTransform.rect.height.ToString("F3")+" font "+t.fontSize);Check(preferred<=t.rectTransform.rect.height+.005f&&t.textBounds.size.x<=t.rectTransform.rect.width+.005f,"Text overflows: "+x.title+" "+t.name);}
 foreach(float vx in new[]{-1.025f,1.025f})foreach(float vy in new[]{-.75f,.75f}){var p=e.view.WorldToViewportPoint(e.info.TransformPoint(new Vector3(vx,vy,0)));Check(p.z>0&&p.x>0&&p.x<1&&p.y>0&&p.y<1,"Information panel outside view: "+x.title+" "+p);}
 var inspect=e.info.GetComponentsInChildren<MuseumAction>().First(a=>a.action=="inspect");Physics.SyncTransforms();Check(e.SelectRay(new Ray(e.head.position,(inspect.transform.position-e.head.position).normalized)),"Inspect button pointer failed");Check(e.inspecting,"Inspect action failed");
 var rest=x.artifact.localRotation;var position=e.rig.position;x.artifact.Rotate(Vector3.up,127,Space.World);var rotated=MuseumArtifactAlignment.Geometry(x.artifact,x.artifact);Check((rotated.center-b.center).magnitude<.002f,"Inspection orbit wobble");e.Act("back");Check(!e.inspecting&&Quaternion.Angle(rest,x.artifact.localRotation)<.01f,"Inspection did not restore orientation");Check((e.rig.position-position).magnitude<.01f,"Inspection return position changed");
 report.AppendLine("PASS "+index+": geometry fit, selection, complete card, inspect ray, centered rotation, Back restoration.");
 }
 int missing=0;foreach(var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())foreach(var t in root.GetComponentsInChildren<Transform>(true))missing+=GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);Check(missing==0,"Missing scripts");report.AppendLine("PASS: no missing scripts. Quest physical headset testing remains pending.");File.WriteAllText(MuseumAlignmentAudit.Output+"Play-Validation.txt",report.ToString());
 }
}
