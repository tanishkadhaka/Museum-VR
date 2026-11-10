using UnityEngine;
namespace HeritageVR {
[ExecuteAlways,RequireComponent(typeof(TextMesh))] public class MuseumText:MonoBehaviour {
 public Shader depthShader;
 Material material;
 void OnEnable(){Font.textureRebuilt+=Rebuild;Apply();}
 void Rebuild(Font font){if(font==GetComponent<TextMesh>().font)Apply();}
 void Apply(){var text=GetComponent<TextMesh>();if(!text.font)return;if(!depthShader)depthShader=Shader.Find("HeritageVR/Depth Text");if(!material){material=new Material(depthShader);material.hideFlags=HideFlags.HideAndDontSave;}material.mainTexture=text.font.material.mainTexture;GetComponent<Renderer>().sharedMaterial=material;}
 void OnDisable(){Font.textureRebuilt-=Rebuild;if(material){if(Application.isPlaying)Destroy(material);else DestroyImmediate(material);}}
}
}
