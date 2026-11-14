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
    Vector3 previousPosition;
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
    Vector3 statueScale;
    bool stepHeld;
    GameObject desktopMask;
    TMPro.TMP_Text inputHint;
    Vector2 DesktopMove => Keyboard.current==null?Vector2.zero:new Vector2((Keyboard.current.dKey.isPressed?1:0)-(Keyboard.current.aKey.isPressed?1:0),(Keyboard.current.wKey.isPressed?1:0)-(Keyboard.current.sKey.isPressed?1:0));
    void OnEnable(){Application.onBeforeRender+=TrackHead;}
    void OnDisable(){Application.onBeforeRender-=TrackHead;}
    void TrackHead(){if(!UsingXR)return;var h=InputDevices.GetDeviceAtXRNode(XRNode.Head);if(h.TryGetFeatureValue(CommonUsages.devicePosition,out Vector3 p))head.localPosition=p;if(h.TryGetFeatureValue(CommonUsages.deviceRotation,out Quaternion q))head.localRotation=q;}

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
        if(exhibitPrompt)exhibitPrompt.SetActive(true);room=destination; inspecting=false; head.localRotation=Quaternion.identity;
        rig.position=destination==0?new Vector3(-60,0,-11):destination==1?new Vector3(0,0,-2):new Vector3(0,0,10);
        rig.rotation=Quaternion.identity; yaw=pitch=0;
        menu.gameObject.SetActive(destination==0); info.gameObject.SetActive(false); comfort.gameObject.SetActive(false);
        view.backgroundColor=destination==0?new Color(.025f,.045f,.085f):new Color(.025f,.026f,.035f);
    }
    public void Act(string action) {
        Debug.Log("HERITAGE ACTION: "+action);
        switch(action) {
            case "start": Go(1); break;
            case "tour": Go(1);if(selected)ShowExhibit(selected);break;
            case "comfort": comfort.gameObject.SetActive(!comfort.gameObject.activeSelf); break;
            case "examine": if(selected)ShowExhibit(selected);break;
            case "inspect":
                if(!selected)break;
                previousPosition=rig.position;inspecting=true;info.gameObject.SetActive(false);
                rig.position=selected.viewingPosition;rig.rotation=Quaternion.identity;yaw=pitch=0;head.localRotation=Quaternion.identity;break;
            case "back": pitch=0;head.localRotation=Quaternion.identity;if(inspecting)rig.position=previousPosition;inspecting=false;info.gameObject.SetActive(false);break;
            case "ceramics": Go(2); break;
            case "gallery": Go(1); break;
            case "home": Go(0); break;
            case "teleport": rig.position=new Vector3(0,0,10);break;
        }
    }
    public void ShowExhibit(MuseumExhibit exhibit){
        selected=exhibit;statue=exhibit.artifact;inspecting=false;
        exhibitTitle.text=exhibit.title;exhibitTitle.fontSize=Mathf.Min(2.7f,32f/Mathf.Max(1,exhibit.title.Length));exhibitDescription.text=exhibit.description;
        var forward=Vector3.ProjectOnPlane(head.forward,Vector3.up).normalized;if(forward.sqrMagnitude<.1f)forward=rig.forward;var rotation=Quaternion.LookRotation(forward,Vector3.up);info.position=head.position+forward*2.6f+rotation*Vector3.right*.9f;info.rotation=rotation;
        info.gameObject.SetActive(true);
    }
    public bool SelectRay(Ray pointer) {
        Physics.SyncTransforms();
        if(!Physics.Raycast(pointer,out RaycastHit hit,20))return false;
        var exhibit=hit.collider.GetComponentInParent<MuseumExhibit>();if(exhibit){ShowExhibit(exhibit);return true;}
        var action=hit.collider.GetComponentInParent<MuseumAction>();
        if(!action)return false;
        Act(action.action);return true;
    }
    void Update() {
        bool xr=UsingXR;
        if(desktopMask&&desktopMask.activeSelf==xr)desktopMask.SetActive(!xr);
        if(inputHint){string hint=xr?"TRIGGER  Select   •   RIGHT STICK  Turn / rotate   •   LEFT STICK  Step   •   B  Back":"WASD  Move   •   Right-drag  Look   •   Click  Select   •   ESC  Back";if(inputHint.text!=hint)inputHint.text=hint;}
        view.stereoTargetEye=xr?StereoTargetEyeMask.Both:StereoTargetEyeMask.None;
        if(xr) {
            if(!trackingConfigured){SubsystemManager.GetInstances(inputSubsystems);foreach(var subsystem in inputSubsystems)subsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);trackingConfigured=inputSubsystems.Count>0;}
            right=InputDevices.GetDeviceAtXRNode(XRNode.RightHand); left=InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            var h=InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if(h.TryGetFeatureValue(CommonUsages.devicePosition,out Vector3 hp)) head.localPosition=hp;
            if(h.TryGetFeatureValue(CommonUsages.deviceRotation,out Quaternion hr)) head.localRotation=hr;
        } else {
            head.localPosition=new Vector3(0,1.65f,0);
            if((Mouse.current!=null&&Mouse.current.rightButton.isPressed)) { yaw+=(Mouse.current.delta.x.ReadValue()*.05f)*2; pitch=Mathf.Clamp(pitch-(Mouse.current.delta.y.ReadValue()*.05f)*2,-65,65); rig.rotation=Quaternion.Euler(0,yaw,0); head.localRotation=Quaternion.Euler(pitch,0,0); }
            var move=new Vector3(DesktopMove.x,0,DesktopMove.y);
            if(room!=0 && !inspecting){var movement=rig.TransformDirection(Vector3.ClampMagnitude(move,1))*Time.deltaTime*2;var body=rig.GetComponent<CharacterController>();if(body)body.Move(movement);else rig.position+=movement;}
        }
        bool trigger=xr && right.TryGetFeatureValue(CommonUsages.triggerButton,out bool t)&&t;
        bool secondary=xr && right.TryGetFeatureValue(CommonUsages.secondaryButton,out bool s)&&s;
        if((secondary&&!lastSecondary)||(Keyboard.current!=null&&Keyboard.current.escapeKey.wasPressedThisFrame)) { if(inspecting||info.gameObject.activeSelf)Act("back");else Go(0); }
        if((Keyboard.current!=null&&Keyboard.current.enterKey.wasPressedThisFrame)) { if(room==0)Go(1);else Act("examine"); }
        if((Keyboard.current!=null&&Keyboard.current.iKey.wasPressedThisFrame)&&room==1) Act("inspect");
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
            if(step&&!stepHeld&&room>0&&!inspecting){var f=Vector3.ProjectOnPlane(head.forward,Vector3.up).normalized;var body=rig.GetComponent<CharacterController>();if(body)body.Move(f*1.25f);else rig.position+=f*1.25f;}stepHeld=step;
        }
        if(room>0){var p=rig.position;p.x=Mathf.Clamp(p.x,-10.8f,10.8f);p.z=Mathf.Clamp(p.z,-2.4f,21.5f);rig.position=p;}
        Ray pointer=view.ViewportPointToRay(new Vector3(.5f,.5f));
        if(!xr)pointer=view.ScreenPointToRay((Mouse.current!=null?(Vector3)Mouse.current.position.ReadValue():new Vector3(Screen.width*.5f,Screen.height*.5f)));
        if(xr&&right.TryGetFeatureValue(CommonUsages.devicePosition,out Vector3 rp)&&right.TryGetFeatureValue(CommonUsages.deviceRotation,out Quaternion rr)) pointer=new Ray(rig.TransformPoint(rp),rig.rotation*rr*Vector3.forward);
        bool hit=Physics.Raycast(pointer,out RaycastHit target,20);
        Vector3 end=hit?target.point:pointer.GetPoint(8);
        ray.gameObject.SetActive(xr); ray.SetPosition(0,pointer.origin);ray.SetPosition(1,end);
        reticle.gameObject.SetActive(hit&&(target.collider.GetComponentInParent<MuseumAction>()!=null||target.collider.GetComponentInParent<MuseumExhibit>()!=null));
        reticle.position=end-pointer.direction*.015f;reticle.forward=pointer.direction;
        if((trigger&&!lastTrigger)||(Mouse.current!=null&&Mouse.current.leftButton.wasPressedThisFrame))SelectRay(pointer);
        lastTrigger=trigger;lastSecondary=secondary;
    }
}
}
