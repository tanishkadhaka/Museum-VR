using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.XR.Management;
using UnityEngine.InputSystem;
using InputDevice=UnityEngine.XR.InputDevice;
using CommonUsages=UnityEngine.XR.CommonUsages;

namespace HeritageVR {
public class MuseumExperience : MonoBehaviour {
    public Transform rig, head, statue, entrance, gallery, ceramics, menu, info, comfort, ceramicModel;
    public Camera view;
    public TMPro.TMP_Text exhibitTitle,exhibitDescription;
    public MuseumExhibit selected;
    public AudioSource narrationPlayer;
    public TMPro.TMP_Text storyStatus,listenLabel,audioStatus;
    public Transform audioProgress;
    bool narrationPaused,narrationSession;
    int storyPage;
    string[] storyPages;
    Vector3 previousPosition;
    Quaternion previousRigRotation, previousHeadRotation, artifactRestRotation;
    float previousYaw, previousPitch;
    public GameObject exhibitPrompt;
    public LineRenderer ray;
    public Transform reticle;
    public int room;
    public bool inspecting;
    bool lastTrigger, lastSecondary;
    float yaw, pitch, turnCooldown;
    InputDevice right, left;
    readonly List<XRInputSubsystem> inputSubsystems=new List<XRInputSubsystem>();
    bool trackingConfigured;
    Transform trackingSpace;
    Vector3 trackingOffset;
    bool floorPoseEstablished;
    TrackingOriginModeFlags originMode;
    float nextTrackingAttempt,nextTrackingLog;
    void ConfigureTracking(){
        if(Time.unscaledTime<nextTrackingAttempt)return;nextTrackingAttempt=Time.unscaledTime+1;
        SubsystemManager.GetInstances(inputSubsystems);trackingConfigured=false;
        foreach(var subsystem in inputSubsystems){
            if(!subsystem.running)continue;
            var supported=subsystem.GetSupportedTrackingOriginModes();
            var current=subsystem.GetTrackingOriginMode();
            if((supported&TrackingOriginModeFlags.Floor)!=0 && current!=TrackingOriginModeFlags.Floor)subsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);
            current=subsystem.GetTrackingOriginMode();
            if(current!=originMode){originMode=current;floorPoseEstablished=false;}
            trackingConfigured=current!=TrackingOriginModeFlags.Unknown;
            break;
        }
    }
    Vector3 statueScale;
    bool stepHeld;
    GameObject desktopMask;
    TMPro.TMP_Text inputHint;
    Vector2 DesktopMove => Keyboard.current==null?Vector2.zero:new Vector2((Keyboard.current.dKey.isPressed?1:0)-(Keyboard.current.aKey.isPressed?1:0),(Keyboard.current.wKey.isPressed?1:0)-(Keyboard.current.sKey.isPressed?1:0));
    void OnEnable(){Application.onBeforeRender+=TrackHead;}
    void OnDisable(){Application.onBeforeRender-=TrackHead;StopNarration();}
    void OnApplicationPause(bool paused){if(paused&&narrationPlayer&&narrationPlayer.isPlaying){narrationPlayer.Pause();narrationPaused=true;}}
    void TrackHead(){
        if(!UsingXR)return;
        if(!trackingSpace){trackingSpace=new GameObject("XR Camera Floor Offset").transform;trackingSpace.SetParent(rig,false);head.SetParent(trackingSpace,false);head.localPosition=Vector3.zero;trackingSpace.localPosition=Vector3.up*1.65f;}
        ConfigureTracking();
        var h=InputDevices.GetDeviceAtXRNode(XRNode.Head);
        if(!h.isValid)return;
        if(h.TryGetFeatureValue(CommonUsages.isTracked,out bool tracked)&&!tracked)return;
        if(h.TryGetFeatureValue(CommonUsages.devicePosition,out Vector3 p)){
            if(originMode==TrackingOriginModeFlags.Floor&&p.y>.35f)floorPoseEstablished=true;
            trackingOffset=originMode==TrackingOriginModeFlags.Floor&&floorPoseEstablished?Vector3.zero:Vector3.up*1.65f;
            trackingSpace.localPosition=trackingOffset;head.localPosition=p;
            if(Time.unscaledTime>=nextTrackingLog){nextTrackingLog=Time.unscaledTime+5;Debug.Log("MUSEUM HEIGHT mode="+originMode+" rawY="+p.y.ToString("F3")+" offset="+trackingOffset.y.ToString("F3")+" eyeY="+head.position.y.ToString("F3"));}
        }
        if(h.TryGetFeatureValue(CommonUsages.deviceRotation,out Quaternion q))head.localRotation=q;
    }

    IEnumerator Start() {
        statueScale=statue.localScale; Go(0);
        desktopMask=head.Find("Headset depth vignette")?.gameObject;
        inputHint=head.GetComponentInChildren<TMPro.TMP_Text>();
#if !UNITY_EDITOR && (UNITY_STANDALONE_WIN || UNITY_ANDROID)
        var manager=XRGeneralSettings.Instance?XRGeneralSettings.Instance.Manager:null;
        if(manager&&!manager.isInitializationComplete){yield return manager.InitializeLoader();if(manager.activeLoader)manager.StartSubsystems();}
#endif
        yield break;
    }
    public bool UsingXR {
        get {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            return false;
#else
            var device=InputDevices.GetDeviceAtXRNode(XRNode.Head);
            return XRSettings.isDeviceActive || (XRGeneralSettings.Instance!=null && XRGeneralSettings.Instance.Manager!=null && XRGeneralSettings.Instance.Manager.activeLoader!=null);
#endif
        }
    }
    public void Go(int destination) {
        StopNarration();EndInspection();if(exhibitPrompt)exhibitPrompt.SetActive(true);room=destination; inspecting=false; head.localRotation=Quaternion.identity;
        rig.position=destination==0?new Vector3(-60,0,-11):destination==1?new Vector3(0,0,-2):new Vector3(0,0,10);
        rig.rotation=Quaternion.identity; yaw=pitch=0;
        menu.gameObject.SetActive(destination==0); info.gameObject.SetActive(false); comfort.gameObject.SetActive(false);
        view.backgroundColor=destination==0?new Color(.025f,.045f,.085f):new Color(.025f,.026f,.035f);
    }
    public void Act(string action) {
        Debug.Log("HERITAGE ACTION: "+action);
        switch(action) {
            case "start": Go(1); break;
            case "tour": Go(1);if(selected)VisitExhibit(selected);break;
            case "comfort": bool show=!comfort.gameObject.activeSelf;comfort.gameObject.SetActive(show);menu.gameObject.SetActive(!show&&room==0);if(show){comfort.position=menu.position-menu.forward*.1f;comfort.rotation=menu.rotation;}break;
            case "examine": if(selected)VisitExhibit(selected);break;
            case "inspect":
                if(!selected||!AtViewingSpot(selected))break;
                if(inspecting)break;StopNarration();previousPosition=rig.position;previousRigRotation=rig.rotation;previousHeadRotation=head.localRotation;previousYaw=yaw;previousPitch=pitch;artifactRestRotation=statue.localRotation;inspecting=true;info.gameObject.SetActive(false);
                rig.position=selected.viewingPosition;rig.rotation=Quaternion.identity;yaw=pitch=0;head.localRotation=Quaternion.identity;break;
            case "storyPrevious": ChangeStory(-1);break;
            case "storyNext": ChangeStory(1);break;
            case "listen": ToggleNarration();break;
            case "back": StopNarration();EndInspection();info.gameObject.SetActive(false);break;
            case "ceramics": Go(2); break;
            case "gallery": Go(1); break;
            case "home": Go(0); break;
            case "teleport": rig.position=new Vector3(0,0,10);break;
        }
    }
    void EndInspection(){
        if(!inspecting)return;
        if(statue)statue.localRotation=artifactRestRotation;
        rig.position=previousPosition;rig.rotation=previousRigRotation;
        if(!UsingXR)head.localRotation=previousHeadRotation;
        yaw=previousYaw;pitch=previousPitch;inspecting=false;
    }
    public bool AtViewingSpot(MuseumExhibit exhibit){return Vector3.ProjectOnPlane(head.position-exhibit.viewingPosition,Vector3.up).sqrMagnitude<=.95f*.95f;}
    public void VisitExhibit(MuseumExhibit exhibit){
        if(AtViewingSpot(exhibit)){ShowExhibit(exhibit);return;}
        TravelTo(exhibit.viewingPosition,exhibit.artifact);selected=exhibit;statue=exhibit.artifact;
    }
    public void TravelTo(Vector3 viewingPosition,Transform artwork){
        StopNarration();EndInspection();info.gameObject.SetActive(false);
        var direction=Vector3.ProjectOnPlane(artwork.position-viewingPosition,Vector3.up).normalized;
        var rotation=Quaternion.LookRotation(direction.sqrMagnitude>.1f?direction:Vector3.forward,Vector3.up);
        var body=rig.GetComponent<CharacterController>();bool enabled=body&&body.enabled;if(body)body.enabled=false;
        var offset=UsingXR?rotation*Vector3.ProjectOnPlane(head.localPosition,Vector3.up):Vector3.zero;
        rig.SetPositionAndRotation(viewingPosition-offset,rotation);yaw=rotation.eulerAngles.y;pitch=0;
        if(!UsingXR){float distance=Vector3.ProjectOnPlane(artwork.position-viewingPosition,Vector3.up).magnitude;pitch=Mathf.Clamp(-Mathf.Atan2(artwork.position.y-1.65f,Mathf.Max(.1f,distance))*Mathf.Rad2Deg,-35,35);head.localRotation=Quaternion.Euler(pitch,0,0);}
        if(body)body.enabled=enabled;Physics.SyncTransforms();
    }
    public void ShowExhibit(MuseumExhibit exhibit){
        StopNarration();EndInspection();selected=exhibit;statue=exhibit.artifact;
        exhibitTitle.text=exhibit.title;storyPages=exhibit.description.Split(new[]{"\n\n"},System.StringSplitOptions.RemoveEmptyEntries);storyPage=0;UpdateStory();
        PositionInformation();info.gameObject.SetActive(true);if(inputHint)inputHint.gameObject.SetActive(false);exhibitTitle.ForceMeshUpdate(true,true);exhibitDescription.ForceMeshUpdate(true,true);if(Application.isPlaying)PlayStoryPage();
    }
    void UpdateStory(){
        if(storyPages==null||storyPages.Length==0)return;
        exhibitDescription.text=storyPages[storyPage];exhibitDescription.ForceMeshUpdate(true,true);
        string[] headings={"HISTORY","LOOK CLOSER","WHY IT MATTERS"};
        if(storyStatus)storyStatus.text=(storyPage<headings.Length?headings[storyPage]:"STORY")+"  /  "+(storyPage+1)+" OF "+storyPages.Length;
        if(info)foreach(var action in info.GetComponentsInChildren<MuseumAction>(true)){
            if(action.action=="listen") {var audioCollider=action.GetComponent<Collider>();if(audioCollider)audioCollider.enabled=StoryClip()!=null;}
            bool available=action.action!="storyPrevious"||storyPage>0;
            available&=action.action!="storyNext"||storyPage<storyPages.Length-1;
            var collider=action.GetComponent<Collider>();if(collider&&(action.action=="storyPrevious"||action.action=="storyNext"))collider.enabled=available;
            if(action.action=="storyPrevious"||action.action=="storyNext")foreach(var label in action.GetComponentsInChildren<TMPro.TMP_Text>(true))label.color=available?Color.white:new Color(.38f,.42f,.48f);
        }
        RefreshAudioUI();
    }
    AudioClip StoryClip(){return selected&&selected.narrationPages!=null&&storyPage<selected.narrationPages.Length?selected.narrationPages[storyPage]:null;}
    void PlayStoryPage(){
        StopNarration();var clip=StoryClip();if(!clip||!narrationPlayer)return;
        narrationPlayer.clip=clip;narrationSession=true;narrationPaused=false;narrationPlayer.Play();RefreshAudioUI();
    }
    void ChangeStory(int step){
        if(storyPages==null)return;int next=Mathf.Clamp(storyPage+step,0,storyPages.Length-1);if(next==storyPage)return;
        bool paused=narrationPaused;bool active=narrationSession;storyPage=next;UpdateStory();
        if(active){PlayStoryPage();if(paused){narrationPlayer.Pause();narrationPaused=true;}}
        else if(narrationPlayer){narrationPlayer.clip=StoryClip();narrationPlayer.time=0;}
        RefreshAudioUI();
    }
    void ToggleNarration(){
        if(!narrationPlayer||!StoryClip())return;
        if(narrationPlayer.isPlaying){narrationPlayer.Pause();narrationPaused=true;}
        else if(narrationPaused){narrationPlayer.UnPause();narrationPaused=false;}
        else PlayStoryPage();
        RefreshAudioUI();
    }
    void StopNarration(){if(narrationPlayer)narrationPlayer.Stop();narrationPaused=false;narrationSession=false;RefreshAudioUI();}
    static string Clock(float seconds){int n=Mathf.Max(0,Mathf.FloorToInt(seconds));return (n/60).ToString("00")+":"+(n%60).ToString("00");}
    void RefreshAudioUI(){
        var clip=StoryClip();bool playing=narrationPlayer&&narrationPlayer.isPlaying;
        if(listenLabel)listenLabel.text=!clip?"NO AUDIO":playing?"PAUSE":narrationPaused?"RESUME":"REPLAY";
        float current=narrationPlayer&&narrationSession?narrationPlayer.time:0;
        if(audioStatus)audioStatus.text=(playing?"NARRATING":narrationPaused?"PAUSED":"AUDIO GUIDE")+"   "+Clock(current)+" / "+Clock(clip?clip.length:0);
        if(audioProgress){audioProgress.gameObject.SetActive(true);var scale=audioProgress.localScale;scale.x=clip?Mathf.Max(.001f,1.78f*current/clip.length):.001f;audioProgress.localScale=scale;var pos=audioProgress.localPosition;pos.x=-.89f+scale.x*.5f;audioProgress.localPosition=pos;}
    }
    void UpdateNarration(){
        if(!info.gameObject.activeSelf){if(narrationSession)StopNarration();return;}
        if(narrationSession&&!narrationPaused&&narrationPlayer&&!narrationPlayer.isPlaying){
            if(storyPage+1<storyPages.Length){storyPage++;UpdateStory();PlayStoryPage();}
            else{narrationSession=false;RefreshAudioUI();}
        }
        RefreshAudioUI();
    }
    void PositionInformation(){
        var forward=head.forward.normalized;
        if(forward.sqrMagnitude<.1f)forward=rig.forward;
        var rightward=Vector3.Cross(Vector3.up,forward).normalized;
        float side=Vector3.Dot(-head.position,rightward)>=0?1:-1;
        Vector2[] candidates={new Vector2(0,1.65f),new Vector2(0,1.5f),new Vector2(side*.15f,1.4f)};
        Physics.SyncTransforms();
        foreach(var offset in candidates){
            var position=head.position+forward*offset.y+rightward*offset.x;
            position.y=Mathf.Max(1.15f,position.y);
            position.x=Mathf.Clamp(position.x,-9.5f,9.5f);position.z=Mathf.Clamp(position.z,-1.3f,20.6f);
            var facing=Quaternion.LookRotation(position-head.position,Vector3.up);
            bool blocked=false;
            foreach(var c in Physics.OverlapBox(position,new Vector3(.89f,.87f,.04f),facing,~0,QueryTriggerInteraction.Ignore)){
                if(c.transform.IsChildOf(info)||c.transform.IsChildOf(rig)||c.GetComponentInParent<MuseumAction>())continue;
                blocked=true;break;
            }
            info.SetPositionAndRotation(position,facing);
            if(!blocked)return;
        }
    }
    public bool SelectRay(Ray pointer) {
        Physics.SyncTransforms();
        if(!Physics.Raycast(pointer,out RaycastHit hit,20))return false;
        var painting=hit.collider.GetComponentInParent<MuseumPainting>();if(painting){TravelTo(painting.viewingPosition,painting.transform);selected=null;return true;}
        var exhibit=hit.collider.GetComponentInParent<MuseumExhibit>();if(exhibit){VisitExhibit(exhibit);return true;}
        var action=hit.collider.GetComponentInParent<MuseumAction>();
        if(!action)return false;
        Act(action.action);return true;
    }
    void Update() {
        bool xr=UsingXR;
        UpdateNarration();
        if(desktopMask&&desktopMask.activeSelf==xr)desktopMask.SetActive(!xr);
        if(inputHint){inputHint.gameObject.SetActive(!info.gameObject.activeSelf&&!comfort.gameObject.activeSelf);string hint=xr?"TRIGGER  Travel / open nearby   •   RIGHT STICK  Turn / rotate   •   LEFT STICK  Step   •   B  Back":"WASD  Move   •   Right-drag  Look   •   Click  Travel / open nearby   •   ESC  Back";if(inputHint.text!=hint)inputHint.text=hint;}
        view.stereoTargetEye=xr?StereoTargetEyeMask.Both:StereoTargetEyeMask.None;
        if(xr) {
            TrackHead();
            right=InputDevices.GetDeviceAtXRNode(XRNode.RightHand);left=InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        } else {
            head.localPosition=new Vector3(0,1.65f,0);
            if((Mouse.current!=null&&Mouse.current.rightButton.isPressed)) { yaw+=(Mouse.current.delta.x.ReadValue()*.05f)*2; pitch=Mathf.Clamp(pitch-(Mouse.current.delta.y.ReadValue()*.05f)*2,-65,65); rig.rotation=Quaternion.Euler(0,yaw,0); head.localRotation=Quaternion.Euler(pitch,0,0); }
            var move=new Vector3(DesktopMove.x,0,DesktopMove.y);
            if(room!=0 && !inspecting){if(move.sqrMagnitude>.01f&&info.gameObject.activeSelf){info.gameObject.SetActive(false);StopNarration();}var movement=rig.TransformDirection(Vector3.ClampMagnitude(move,1))*Time.deltaTime*2;var body=rig.GetComponent<CharacterController>();if(body)body.Move(movement);else rig.position+=movement;}
        }
        bool trigger=xr && right.TryGetFeatureValue(CommonUsages.triggerButton,out bool t)&&t;
        bool secondary=xr && right.TryGetFeatureValue(CommonUsages.secondaryButton,out bool s)&&s;
        if((secondary&&!lastSecondary)||(Keyboard.current!=null&&Keyboard.current.escapeKey.wasPressedThisFrame)) { if(inspecting||info.gameObject.activeSelf)Act("back");else Go(0); }
        if((Keyboard.current!=null&&Keyboard.current.enterKey.wasPressedThisFrame)) { if(room==0)Go(1);else Act("examine"); }
        if((Keyboard.current!=null&&Keyboard.current.iKey.wasPressedThisFrame)&&room>0){
            if(!info.gameObject.activeSelf&&!inspecting){MuseumExhibit nearest=null;float distance=float.MaxValue;foreach(var exhibit in gallery.GetComponentsInChildren<MuseumExhibit>()){float d=(exhibit.viewingPosition-rig.position).sqrMagnitude;if(d<distance){distance=d;nearest=exhibit;}}if(nearest){selected=nearest;statue=nearest.artifact;}}
            Act("inspect");
        }
        if((Keyboard.current!=null&&Keyboard.current.gKey.wasPressedThisFrame)) Go(room==2?1:2);
        if(inspecting) {
            float spin=DesktopMove.x;
            if(xr&&right.TryGetFeatureValue(CommonUsages.primary2DAxis,out Vector2 axis))spin=axis.x;
            statue.Rotate(Vector3.up,-spin*60*Time.deltaTime,Space.World);
        }
        if(xr) {
            turnCooldown-=Time.deltaTime;
            if(right.TryGetFeatureValue(CommonUsages.primary2DAxis,out Vector2 stick)&&Mathf.Abs(stick.x)>.75f&&turnCooldown<=0&&!inspecting) {rig.RotateAround(head.position,Vector3.up,Mathf.Sign(stick.x)*30);turnCooldown=.35f;}
            bool step=left.TryGetFeatureValue(CommonUsages.primary2DAxis,out Vector2 nav)&&nav.y>.75f;
            if(step&&!stepHeld&&room>0&&!inspecting){if(info.gameObject.activeSelf){info.gameObject.SetActive(false);StopNarration();}var f=Vector3.ProjectOnPlane(head.forward,Vector3.up).normalized;var body=rig.GetComponent<CharacterController>();if(body)body.Move(f*1.25f);else rig.position+=f*1.25f;}stepHeld=step;
        }
        if(room>0){var p=rig.position;p.x=Mathf.Clamp(p.x,-10.8f,10.8f);p.z=Mathf.Clamp(p.z,-2.4f,21.5f);rig.position=p;}
        Ray pointer=view.ViewportPointToRay(new Vector3(.5f,.5f));
        if(!xr)pointer=view.ScreenPointToRay((Mouse.current!=null?(Vector3)Mouse.current.position.ReadValue():new Vector3(Screen.width*.5f,Screen.height*.5f)));
        if(xr&&right.TryGetFeatureValue(CommonUsages.devicePosition,out Vector3 rp)&&right.TryGetFeatureValue(CommonUsages.deviceRotation,out Quaternion rr)) pointer=new Ray(rig.TransformPoint(rp+trackingOffset),rig.rotation*rr*Vector3.forward);
        bool hit=Physics.Raycast(pointer,out RaycastHit target,20);
        Vector3 end=hit?target.point:pointer.GetPoint(8);
        ray.gameObject.SetActive(xr); ray.SetPosition(0,pointer.origin);ray.SetPosition(1,end);
        reticle.gameObject.SetActive(hit&&(target.collider.GetComponentInParent<MuseumAction>()!=null||target.collider.GetComponentInParent<MuseumExhibit>()!=null||target.collider.GetComponentInParent<MuseumPainting>()!=null));
        reticle.position=end-pointer.direction*.015f;reticle.forward=pointer.direction;
        if((trigger&&!lastTrigger)||(Mouse.current!=null&&Mouse.current.leftButton.wasPressedThisFrame))SelectRay(pointer);
        lastTrigger=trigger;lastSecondary=secondary;
    }
}
}
