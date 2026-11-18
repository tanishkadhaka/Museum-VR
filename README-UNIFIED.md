# Heritage VR Museum

Open this project in Unity 2022.3.62f3 and load Assets/Scenes/UnifiedMuseum.unity. Press Play and select Start.

The museum includes the entrance menu, eight installed artifacts, framed rear-wall paintings, table plaques, exhibit information and centered 3D inspection. The imported models are fitted to their table surfaces with geometry-based centering and selection boxes. Exiting inspection restores the original presentation.

Desktop: WASD moves; right-drag looks; click selects; I inspects the nearest exhibit; A/D rotates during inspection; Escape returns. Quest: right trigger selects, left stick forward steps, right stick snap-turns or rotates during inspection, B returns.

The information cards use bounded, wrapped text. Descriptions describe the visible models; precise dates, provenance and archaeological identification remain unverified for the commercial/scan assets. Vessel labels reflect their actual silhouettes rather than unsupported hydria/krater identifications.

To reimport and align the eight exhibits, use Museum > 12 Reimport and Align 8 Artifacts, or Museum > 13 Fix Artifact Alignment and Text. Alignment is explicit: opening the editor no longer silently replaces models. The model paths, presentation orientation and text are in Assets/Editor/MuseumArtifactSetup.cs. Geometry fitting, label formatting and card layout are in MuseumArtifactAlignment.cs.

The Android APK targets Quest 2/3/3S standalone with ARM64, IL2CPP and OpenXR. Physical headset visual, controller and performance testing remains necessary.

## Exhibit audio guide
Opening an exhibit starts its narration. History, Look Closer and Why It Matters advance with the matching audio. Previous/Next switches text and audio together; Pause/Resume preserves the playback position. Closing or hiding the card stops narration. See apk/Stories-and-Audio.md.

## Visiting exhibits and paintings
A distant click travels to the viewing spot. A nearby click opens the description. Walking remains available. Artifact narration starts with the description, never on the initial travel click. All nine paintings support travel only, with no description panels. See apk/Viewing-and-Descriptions.md.
