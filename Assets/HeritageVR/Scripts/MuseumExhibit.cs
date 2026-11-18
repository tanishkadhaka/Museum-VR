using UnityEngine;
namespace HeritageVR {
public class MuseumExhibit:MonoBehaviour {
 public string title;
 [TextArea]public string description;
 public AudioClip narration;
 public AudioClip[] narrationPages;
 public Transform artifact;
 public Vector3 viewingPosition;
}
}
