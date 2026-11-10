# Heritage VR Museum

Unity 2022.3.62f3 project. Open Assets/Scenes/UnifiedMuseum.unity and press Play.

The scene has the entrance UI, a museum hall, eight tables with generic artifact placeholders, three framed rear-wall paintings, exhibit information panels, and inspection controls. Previous artifact models and descriptions have been removed from the active project. All eight slots await your models.

To add a model: expand the gallery, a DISPLAY object, and its ARTIFACT SLOT. Remove the two placeholder children and add your model under the slot. Keep the slot collider. Set local position and scale to fit the table; edit the DISPLAY object's MuseumExhibit title and description.

Desktop: Enter starts, WASD moves, right-drag looks, click selects, I inspects, A/D rotates, Escape returns. Quest: right trigger selects, left stick steps, right stick snap-turns or rotates during inspection, B returns.

Quest 2/3/3S standalone uses Android ARM64, IL2CPP and OpenXR. The APK is built for later testing; physical headset rendering, input and performance still require device verification.

Keep Assets, Packages and ProjectSettings together. Previous source-project backups outside this active project are retained for recovery.
