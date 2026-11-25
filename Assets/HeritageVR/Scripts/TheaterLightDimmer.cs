using UnityEngine;
using UnityEngine.Video;

namespace HeritageVR {
    public class TheaterLightDimmer : MonoBehaviour {
        public VideoPlayer videoPlayer;
        public Light[] theaterLights;
        public float brightIntensity = 0.85f;
        public float dimIntensity = 0.20f;
        public float smoothSpeed = 3f;

        void Start() {
            if (!videoPlayer) videoPlayer = GetComponent<VideoPlayer>();
            if (!videoPlayer) videoPlayer = FindObjectOfType<VideoPlayer>();
        }

        void Update() {
            if (theaterLights == null || theaterLights.Length == 0) return;
            bool isPlaying = videoPlayer && videoPlayer.isPlaying;
            float target = isPlaying ? dimIntensity : brightIntensity;
            for (int i = 0; i < theaterLights.Length; i++) {
                if (theaterLights[i]) {
                    theaterLights[i].intensity = Mathf.MoveTowards(theaterLights[i].intensity, target, Time.deltaTime * smoothSpeed);
                }
            }
        }
    }
}

