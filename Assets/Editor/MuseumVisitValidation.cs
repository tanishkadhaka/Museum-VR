using UnityEngine;using UnityEditor;using HeritageVR;using System.Linq;using System.IO;
[InitializeOnLoad]
public static class MuseumVisitValidation {
 const string Key="MuseumVisitValidation";const string Trigger="/Users/aasthathakur/Documents/Codex/2026-10-28/rea/work/visit.trigger";
 static MuseumVisitValidation(){EditorApplication.update+=Tick;}
 public static void Run(){MuseumVisitSetup.Apply();SessionState.SetInt(Key,1);EditorApplication.isPlaying=true;}
 static void Check(bool value,string message){if(!value)throw new System.Exception(message);}
 static Ray Aim(MuseumExperience e,Vector3 target){Physics.SyncTransforms();return new Ray(e.head.position,(target-e.head.position).normalized);}
 static void Tick(){try{
 int stage=SessionState.GetInt(Key,0);
 if(stage==0&&File.Exists(Trigger)&&!EditorApplication.isCompiling&&!EditorApplication.isUpdating){if(EditorApplication.isPlaying){EditorApplication.isPlaying=false;return;}if(EditorApplication.isPlayingOrWillChangePlaymode)return;File.Delete(Trigger);Run();return;}
 if(stage==1&&EditorApplication.isPlaying&&Time.frameCount>10){
 var e=Object.FindObjectOfType<MuseumExperience>();int artifacts=0,paintings=0;
 foreach(var exhibit in e.gallery.GetComponentsInChildren<MuseumExhibit>().ToArray()){
 e.Go(1);e.rig.position=exhibit.viewingPosition-Vector3.forward*1.4f;var center=exhibit.artifact.GetComponent<BoxCollider>().bounds.center;
 Check(e.SelectRay(Aim(e,center)),"Artifact travel ray failed: "+exhibit.title);Check(e.AtViewingSpot(exhibit),"Travel missed viewing point");Check(!e.info.gameObject.activeSelf&&!e.narrationPlayer.isPlaying,"Travel prematurely opened panel/audio");
 Check(e.SelectRay(Aim(e,center))&&e.info.gameObject.activeSelf&&e.selected==exhibit,"Second artifact click failed");Check(e.narrationPlayer.isPlaying,"Confirmed artifact description should narrate");
 e.Act("back");e.rig.position=exhibit.viewingPosition;Check(e.SelectRay(Aim(e,center))&&e.info.gameObject.activeSelf,"Walked arrival did not open description");artifacts++;
 }
 foreach(var painting in e.gallery.GetComponentsInChildren<MuseumPainting>()){
 e.Go(1);var normal=Vector3.ProjectOnPlane(painting.transform.position-painting.viewingPosition,Vector3.up).normalized;e.rig.position=painting.viewingPosition-normal*1.4f;
 Check(e.SelectRay(Aim(e,painting.transform.position)),"Painting travel ray failed: "+painting.title);Check(!e.info.gameObject.activeSelf&&!e.narrationPlayer.isPlaying,"Painting travel prematurely opened description");
 Check(e.SelectRay(Aim(e,painting.transform.position))&&!e.info.gameObject.activeSelf&&!e.narrationPlayer.isPlaying,"Repeated painting click must only travel");Check(e.selected==null,"Painting must not create an exhibit record");paintings++;
 }
 e.Go(1);var before=e.rig.position;var body=e.rig.GetComponent<CharacterController>();body.Move(Vector3.forward*.2f);Check(Vector3.Distance(before,e.rig.position)>.1f,"Walking unavailable");Check(artifacts==8&&paintings==9,"Expected eight artifacts and nine paintings");
 File.WriteAllText("apk/Visit-Interaction-Validation.txt","PASS: eight artifact and nine painting ray interactions.\nFirst distant click travels to viewing spot with no panel or audio. Artifacts: second click opens the correct description. Paintings: repeated clicks only travel, never opening a description or audio. Walking to artifact spots opens description on first nearby click. CharacterController movement preserved.\nArtifact narration begins only when the description opens.\nPhysical Quest 3S testing remains pending.\n");SessionState.SetInt(Key,2);EditorApplication.isPlaying=false;
 }else if(stage==2&&!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetInt(Key,0);QuestMuseumSetup.BuildAPK();File.WriteAllText("apk/Visit-Update-Complete.txt","Scene updated and Quest APK built successfully.\n");if(Application.isBatchMode)EditorApplication.Exit(0);}
 }catch(System.Exception ex){SessionState.SetInt(Key,0);File.WriteAllText("apk/Visit-Interaction-Validation.txt","FAILED: "+ex);Debug.LogException(ex);if(Application.isBatchMode)EditorApplication.Exit(1);}}
}
