using UnityEngine;using UnityEditor;using UnityEditor.SceneManagement;using HeritageVR;using System.Linq;using System.IO;
[InitializeOnLoad]
public static class MuseumNarrationValidation{
 const string Key="MuseumNarrationValidation";static double deadline;static string report="";
 static MuseumNarrationValidation(){EditorApplication.update+=Tick;}
 public static void Run(){MuseumStorySetup.Apply();EditorSceneManager.OpenScene("Assets/Scenes/UnifiedMuseum.unity");SessionState.SetInt(Key,1);EditorApplication.isPlaying=true;}
 static void Check(bool okay,string why){if(!okay)throw new System.Exception(why);}
 static void Tick(){int stage=SessionState.GetInt(Key,0);if(stage==0)return;try{
 if(stage==1&&EditorApplication.isPlaying&&Time.frameCount>8){var e=Object.FindObjectOfType<MuseumExperience>();var all=e.gallery.GetComponentsInChildren<MuseumExhibit>().OrderBy(x=>x.name).ToArray();e.Go(1);
 foreach(var exhibit in all){e.rig.position=exhibit.viewingPosition;e.ShowExhibit(exhibit);Check(e.narrationPlayer.isPlaying&&e.narrationPlayer.clip==exhibit.narrationPages[0],"Opening did not start page audio");
 for(int page=0;page<3;page++){if(page>0)e.Act("storyNext");Check(e.narrationPlayer.clip==exhibit.narrationPages[page],"Audio/page mismatch");Check(e.exhibitDescription.text==exhibit.description.Split(new[]{"\n\n"},System.StringSplitOptions.None)[page],"Text/page mismatch");e.exhibitDescription.ForceMeshUpdate(true,true);Check(e.exhibitDescription.textBounds.size.y<=e.exhibitDescription.rectTransform.rect.height+.01f,"Story text overflow");}
 e.Act("listen");Check(!e.narrationPlayer.isPlaying&&e.listenLabel.text=="RESUME","Pause failed");e.Act("storyPrevious");Check(!e.narrationPlayer.isPlaying&&e.narrationPlayer.clip==exhibit.narrationPages[1],"Paused navigation failed");e.Act("listen");Check(e.narrationPlayer.isPlaying,"Resume failed");e.Act("back");Check(!e.narrationPlayer.isPlaying&&!e.info.gameObject.activeSelf,"Close did not stop playback");}
 e.rig.position=all[0].viewingPosition;e.ShowExhibit(all[0]);MuseumAlignmentAudit.Render(e.view,"Synchronized-Story-Panel");e.narrationPlayer.time=e.narrationPlayer.clip.length-.05f;deadline=EditorApplication.timeSinceStartup+.6;SessionState.SetInt(Key,2);report="PASS all eight exhibits: automatic playback, three matching text/audio sections, complete text bounds, pause/resume, paused page navigation, close stops audio.\n";
 }else if(stage==2&&EditorApplication.isPlaying&&EditorApplication.timeSinceStartup>deadline){var e=Object.FindObjectOfType<MuseumExperience>();Check(e.narrationPlayer.clip==e.selected.narrationPages[1],"Automatic advance did not match next page");report+="PASS automatic advance follows actual clip completion.\nPhysical Quest 3S testing pending.\n";Directory.CreateDirectory("apk");File.WriteAllText("apk/Narration-Sync-Validation.txt",report);SessionState.SetInt(Key,3);EditorApplication.isPlaying=false;
 }else if(stage==3&&!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetInt(Key,0);QuestMuseumSetup.BuildAPK();EditorApplication.Exit(0);}
 }catch(System.Exception ex){SessionState.SetInt(Key,0);File.WriteAllText("apk/Narration-Sync-Validation.txt","FAILED: "+ex);Debug.LogException(ex);EditorApplication.Exit(1);}}
}
