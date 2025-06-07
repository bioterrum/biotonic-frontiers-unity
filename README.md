# Biotonic Frontiers – Unity Client

This is the **initial scaffold** 
Steps to run:

1. Install **Unity 2023 LTS** (or newer) and open this folder.
2. Ensure Text serialization + Visible meta files (see Project Settings → Editor).
3. Install the following packages via Unity’s Package Manager:
   * "Native WebSocket" (Git URL: `https://github.com/endel/NativeWebSocket.git`)
   * "Newtonsoft Json" (from Unity Registry)
   * "TextMeshPro" (if not already)
4. Open <kbd>Assets/Scenes/Login.unity</kbd>, press Play, and hook up the UI to
   `GameManager` + `NetworkManager`.
5. Configure backend URLs in **NetworkManager.cs** per environment.
