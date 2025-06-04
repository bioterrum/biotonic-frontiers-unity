// Assets/Editor/BiotonicSceneGenerators.cs
// Rich scene generator covering *all* gameplay scenes.
// Menu:  Tools ▸ Biotonic ▸ Generate <Scene> Scene
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

//──────────────────────────────────────────────────────────────────────────────
public static class BiotonicSceneGenerators
{
    /* ───────── helpers (shared by every scene) ───────── */

    // Adds main camera, canvas (with scaler), and *exactly one* EventSystem.
    private static (GameObject cam, GameObject canvas) BootstrapScene(string sceneName)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = sceneName;
        SceneManager.SetActiveScene(scene);

        // camera
        var camGO = new GameObject("Main Camera");
        var cam   = camGO.AddComponent<Camera>();
        cam.clearFlags      = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        camGO.tag           = "MainCamera";

        // canvas
        var canvasGO = new GameObject("Canvas",
                                      typeof(Canvas),
                                      typeof(CanvasScaler),
                                      typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        EnsureSingleEventSystem();

        return (camGO, canvasGO);
    }

    // guarantee exactly one EventSystem exists (and attach helper)
    private static void EnsureSingleEventSystem()
    {
        var esList = Object.FindObjectsOfType<EventSystem>();
        EventSystem es = esList.Length > 0
            ? esList[0]
            : new GameObject("EventSystem",
                             typeof(EventSystem),
                             typeof(StandaloneInputModule)).GetComponent<EventSystem>();

        // destroy extras if any
        for (int i = 1; i < esList.Length; ++i) Object.DestroyImmediate(esList[i].gameObject);

        // add helper once
        if (!es.TryGetComponent(out EnsureSingleEventSystem _))
            es.gameObject.AddComponent<EnsureSingleEventSystem>();
    }

    // small anchored-RectTransform factory
    private static RectTransform RT(string name, Vector2 pos, Vector2 size, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta        = size;
        rt.anchorMin        = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        return rt;
    }

    // save + register in BuildSettings if missing
    private static void SaveScene(string path)
    {
        var scene = SceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(scene, path, true);

        var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        if (!list.Exists(s => s.path == path))
            list.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = list.ToArray();

        Debug.Log($"✔ {scene.name} scene generated/updated.");
    }

    /* ───────── MENU ITEMS ───────── */

    [MenuItem("Tools/Biotonic/Generate Main Menu Scene")]
    public static void GenerateMainMenu() => MainMenu();

    [MenuItem("Tools/Biotonic/Generate Login Scene")]
    public static void GenerateLogin()    => Login();

    [MenuItem("Tools/Biotonic/Generate Lobby Scene")]
    public static void GenerateLobby()    => Lobby();

    [MenuItem("Tools/Biotonic/Generate Duel Scene")]
    public static void GenerateDuel()     => Duel();

    [MenuItem("Tools/Biotonic/Generate Inventory Scene")]
    public static void GenerateInventory()=> Inventory();

    [MenuItem("Tools/Biotonic/Generate Shop Scene")]
    public static void GenerateShop()     => Shop();

    [MenuItem("Tools/Biotonic/Generate Faction Scene")]
    public static void GenerateFaction()  => Faction();

    [MenuItem("Tools/Biotonic/Generate Trade Scene")]
    public static void GenerateTrade()    => Trade();

    [MenuItem("Tools/Biotonic/Generate Land Map Scene")]
    public static void GenerateLandMap()  => LandMap();

    [MenuItem("Tools/Biotonic/Generate ALL Scenes %#g")] // Ctrl/Cmd+Shift+G
    public static void GenerateAllScenes()
    {
        GenerateMainMenu();
        GenerateLogin();
        GenerateLobby();
        GenerateDuel();
        GenerateInventory();
        GenerateShop();
        GenerateFaction();
        GenerateTrade();
        GenerateLandMap();
        Debug.Log("✔ All scenes regenerated.");
    }

    /* ───────── individual scene builders ───────── */

    // ——— MAIN MENU ——————————————————————————————————————————————
    private static void MainMenu()
    {
        var (_, canvasGO) = BootstrapScene("MainMenu");
        var parent = canvasGO.transform;

        var titleRT = RT("Title", new Vector2(0, 350), new Vector2(800, 100), parent);
        var title   = titleRT.gameObject.AddComponent<TextMeshProUGUI>();
        title.text      = "Biotonic Frontiers";
        title.alignment = TextAlignmentOptions.Center;
        title.fontSize  = 72;

        var menuRT = RT("Menu", new Vector2(0, -50), new Vector2(400, 500), parent);
        var layout = menuRT.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment         = TextAnchor.MiddleCenter;
        layout.spacing                = 10;
        layout.childForceExpandWidth  = false;
        layout.childForceExpandHeight = false;

        string[] labels = { "Play", "Inventory", "Shop", "Factions", "Trade", "Land Map", "Quit" };
        string[] scenes = { "Lobby", "Inventory", "Shop", "Faction", "Trade", "LandMap", null };

        for (int i = 0; i < labels.Length; ++i)
        {
            var btnRT = RT($"{labels[i]}Btn", Vector2.zero, new Vector2(300, 60), menuRT);
            var img   = btnRT.gameObject.AddComponent<Image>();
            img.color = new Color(1, 1, 1, 0.1f);
            img.raycastTarget = true;

            var btn = btnRT.gameObject.AddComponent<Button>();
            var lbl = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI))
                          .GetComponent<TextMeshProUGUI>();
            lbl.text      = labels[i];
            lbl.alignment = TextAlignmentOptions.Center;
            lbl.rectTransform.SetParent(btnRT, false);
            lbl.rectTransform.sizeDelta = btnRT.sizeDelta;

            string targetScene = scenes[i];
            if (targetScene != null) btn.onClick.AddListener(() => SceneManager.LoadScene(targetScene));
            else                     btn.onClick.AddListener(Application.Quit);
        }

        SaveScene("Assets/Scenes/MainMenu.unity");
    }

    // ——— LOGIN ————————————————————————————————————————————————
    private static void Login()
    {
        var (_, canvasGO) = BootstrapScene("Login");
        var parent = canvasGO.transform;

        // inputs & buttons
        var emailRT   = RT("EmailInput",  new Vector2(0, 120), new Vector2(400, 40), parent);
        var sendRT    = RT("SendLinkBtn", new Vector2(0,  60), new Vector2(200, 40), parent);
        var tokenRT   = RT("TokenInput",  new Vector2(0, -20), new Vector2(400, 40), parent);
        var verifyRT  = RT("VerifyBtn",   new Vector2(0, -80), new Vector2(200, 40), parent);
        var statusRT  = RT("StatusLabel", new Vector2(0,-140), new Vector2(600, 40), parent);

        var emailInput = emailRT.gameObject.AddComponent<TMP_InputField>();
        var emailText  = new GameObject("Text", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        emailText.transform.SetParent(emailRT, false);
        emailInput.textComponent = emailText; emailInput.placeholder = emailText;

        var tokenInput = tokenRT.gameObject.AddComponent<TMP_InputField>();
        var tokenText  = new GameObject("Text", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        tokenText.transform.SetParent(tokenRT, false);
        tokenInput.textComponent = tokenText; tokenInput.placeholder = tokenText;

        var sendBtn = BuildButton(sendRT, "Send Link");
        var verifyBtn = BuildButton(verifyRT, "Verify");

        var statusLbl = statusRT.gameObject.AddComponent<TextMeshProUGUI>();
        statusLbl.alignment = TextAlignmentOptions.Center;

        var authGO = new GameObject("AuthController");
        var am     = authGO.AddComponent<BiotonicFrontiers.UI.AuthManager>();
        am.emailInput     = emailInput;
        am.sendLinkButton = sendBtn;
        am.tokenInput     = tokenInput;
        am.verifyButton   = verifyBtn;
        am.statusLabel    = statusLbl;

        sendBtn.onClick.AddListener(am.SendMagicLink);
        verifyBtn.onClick.AddListener(am.VerifyToken);

        SaveScene("Assets/Scenes/Login.unity");
    }

    // ——— LOBBY ————————————————————————————————————————————————
    private static void Lobby()
    {
        var (_, canvasGO) = BootstrapScene("Lobby");
        var parent = canvasGO.transform;

        var nameRT   = RT("PlayerNameText", new Vector2(0, 200), new Vector2(600, 50), parent);
        var nameText = nameRT.gameObject.AddComponent<TextMeshProUGUI>();
        nameText.alignment = TextAlignmentOptions.Center;

        var factionRT   = RT("FactionText", new Vector2(0, 140), new Vector2(600, 50), parent);
        var factionText = factionRT.gameObject.AddComponent<TextMeshProUGUI>();
        factionText.alignment = TextAlignmentOptions.Center;

        var findRT  = RT("FindMatchButton", new Vector2(0, 0), new Vector2(200, 50), parent);
        var findBtn = BuildButton(findRT, "Find Match");

        var statusRT = RT("StatusText", new Vector2(0, -60), new Vector2(400, 40), parent);
        var status   = statusRT.gameObject.AddComponent<TextMeshProUGUI>();
        status.alignment = TextAlignmentOptions.Center;

        var waitRT = RT("WaitingIndicator", new Vector2(0, -120), new Vector2(40, 40), parent);
        var waitLbl = waitRT.gameObject.AddComponent<TextMeshProUGUI>();
        waitLbl.alignment = TextAlignmentOptions.Center;
        waitLbl.text = "⌛";

        // Attach LobbyUI controller
        var lobbyGO = new GameObject("LobbyUI", typeof(BiotonicFrontiers.UI.LobbyUI));
        lobbyGO.transform.SetParent(canvasGO.transform, false);
        var lobbyUI = lobbyGO.GetComponent<BiotonicFrontiers.UI.LobbyUI>();

        BindPrivate(lobbyUI, "playerNameText",  nameText);
        BindPrivate(lobbyUI, "factionText",     factionText);
        BindPrivate(lobbyUI, "findMatchButton", findBtn);
        BindPrivate(lobbyUI, "statusText",      status);
        BindPrivate(lobbyUI, "waitingIndicator", waitRT.gameObject);

        SaveScene("Assets/Scenes/Lobby.unity");
    }

    // ——— DUEL ————————————————————————————————————————————————
    private static void Duel()
    {
        var (_, canvasGO) = BootstrapScene("Duel");
        var parent = canvasGO.transform;

        var unitsRT = RT("UnitsContainer", new Vector2(0, 200), new Vector2(1600, 600), parent);

        var btnRT  = RT("EndTurnButton", new Vector2(0, -200), new Vector2(300, 60), parent);
        var btn    = BuildButton(btnRT, "End Turn");

        // Attach DuelUI controller (NO LobbyUI clones anymore)
        var duelGO = new GameObject("DuelUI", typeof(BiotonicFrontiers.UI.DuelUI));
        duelGO.transform.SetParent(canvasGO.transform, false);
        var duelUI = duelGO.GetComponent<BiotonicFrontiers.UI.DuelUI>();

        BindPrivate(duelUI, "endTurnButton",  btn);
        BindPrivate(duelUI, "unitsContainer", unitsRT);

        SaveScene("Assets/Scenes/Duel.unity");
    }

    // ——— INVENTORY ————————————————————————————————————————————
    private static void Inventory()
    {
        var (_, canvasGO) = BootstrapScene("Inventory");
        var parent = canvasGO.transform;

        var gridRT = RT("InventoryGrid", Vector2.zero, new Vector2(1600, 800), parent);
        var grid   = gridRT.gameObject.AddComponent<GridLayoutGroup>();
        grid.cellSize        = new Vector2(100, 100);
        grid.spacing         = new Vector2(10, 10);
        grid.constraint      = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 8;

        var invGO = new GameObject("InventoryUI", typeof(BiotonicFrontiers.UI.InventoryUI));
        invGO.transform.SetParent(canvasGO.transform, false);
        var invUI = invGO.GetComponent<BiotonicFrontiers.UI.InventoryUI>();

        BindPrivate(invUI, "gridRoot", gridRT);

        var prefabGO = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/InventoryItemWidget.prefab");
        if (prefabGO != null)
            BindPrivate(invUI, "itemWidgetPrefab", prefabGO.GetComponent<BiotonicFrontiers.UI.InventoryItemWidget>());

        SaveScene("Assets/Scenes/Inventory.unity");
    }

    // ——— SHOP ———————————————————————————————————————————————
    private static void Shop()
    {
        var (_, canvasGO) = BootstrapScene("Shop");
        var parent = canvasGO.transform;

        var gridRT = RT("GridRoot", Vector2.zero, new Vector2(1600, 800), parent);
        var grid   = gridRT.gameObject.AddComponent<GridLayoutGroup>();
        grid.cellSize        = new Vector2(200, 200);
        grid.spacing         = new Vector2(20, 20);
        grid.constraint      = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 4;

        var backRT = RT("BackButton", new Vector2(-850, 400), new Vector2(200, 50), parent);
        var backBtn = BuildButton(backRT, "Back");
        backBtn.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));

        var shopGO = new GameObject("ShopUI", typeof(BiotonicFrontiers.UI.ShopUI));
        shopGO.transform.SetParent(canvasGO.transform, false);
        var shopUI = shopGO.GetComponent<BiotonicFrontiers.UI.ShopUI>();

        BindPrivate(shopUI, "gridRoot", gridRT);

        var prefabGO = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ShopItemTile.prefab");
        if (prefabGO != null)
            BindPrivate(shopUI, "tilePrefab", prefabGO.GetComponent<BiotonicFrontiers.UI.ShopItemTile>());

        SaveScene("Assets/Scenes/Shop.unity");
    }

    // ——— FACTION ——————————————————————————————————————————————
    private static void Faction()
    {
        var (_, canvasGO) = BootstrapScene("Faction");
        var parent = canvasGO.transform;

        var cfRT = RT("CurrentFactionText", new Vector2(0, 350), new Vector2(600, 40), parent);
        var cfText = cfRT.gameObject.AddComponent<TextMeshProUGUI>();
        cfText.alignment = TextAlignmentOptions.Center;

        var leaveBtn   = BuildButton(RT("LeaveButton", new Vector2(0, 300), new Vector2(200, 40), parent), "Leave");
        var listRT     = RT("ListRoot", new Vector2(-200, 50), new Vector2(400, 300), parent);
        listRT.gameObject.AddComponent<VerticalLayoutGroup>();
        var refreshBtn = BuildButton(RT("RefreshButton", new Vector2(-200, -200), new Vector2(200, 40), parent), "Refresh");

        var nameInputRT = RT("CreateNameInput", new Vector2(200, 100), new Vector2(300, 40), parent);
        var nameInput   = nameInputRT.gameObject.AddComponent<TMP_InputField>();
        nameInput.textViewport = nameInputRT;

        var createBtn   = BuildButton(RT("CreateFactionBtn", new Vector2(200, 40), new Vector2(200, 40), parent), "Create");
        var statusLbl   = RT("CreateStatus", new Vector2(200, -20), new Vector2(300, 40), parent)
                          .gameObject.AddComponent<TextMeshProUGUI>();
        statusLbl.alignment = TextAlignmentOptions.Center;

        var fmGO = new GameObject("FactionUI", typeof(BiotonicFrontiers.UI.FactionManagementUI));
        fmGO.transform.SetParent(canvasGO.transform, false);
        var fmUI = fmGO.GetComponent<BiotonicFrontiers.UI.FactionManagementUI>();

        BindPrivate(fmUI, "currentFactionText", cfText);
        BindPrivate(fmUI, "leaveButton", leaveBtn);
        BindPrivate(fmUI, "listRoot", listRT);
        BindPrivate(fmUI, "refreshButton", refreshBtn);
        BindPrivate(fmUI, "createNameInput", nameInput);
        BindPrivate(fmUI, "createButton", createBtn);
        BindPrivate(fmUI, "createStatus", statusLbl);

        var prefabGO = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FactionRow.prefab");
        if (prefabGO != null)
            BindPrivate(fmUI, "rowPrefab", prefabGO.GetComponent<BiotonicFrontiers.UI.FactionRow>());

        SaveScene("Assets/Scenes/Faction.unity");
    }

    // ——— TRADE ——————————————————————————————————————————————
    private static void Trade()
    {
        var (_, canvasGO) = BootstrapScene("Trade");
        var parent = canvasGO.transform;

        var listRT = RT("TradeList", new Vector2(-200, 100), new Vector2(400, 400), parent);
        listRT.gameObject.AddComponent<VerticalLayoutGroup>();

        // inputs
        RectTransform offerItemRT   = MakeInput("OfferItemInput",   new Vector2(200, 200), "Offered Item");
        RectTransform offerQtyRT    = MakeInput("OfferQtyInput",    new Vector2(200, 140), "Quantity");
        RectTransform requestItemRT = MakeInput("RequestItemInput", new Vector2(200,  80), "Requested Item");
        RectTransform requestQtyRT  = MakeInput("RequestQtyInput",  new Vector2(200,  20), "Quantity");

        var createBtn = BuildButton(RT("CreateTradeBtn", new Vector2(200, -60), new Vector2(200, 50), parent), "Create Trade");
        var statusLbl = RT("StatusText", new Vector2(200, -140), new Vector2(300, 40), parent)
                        .gameObject.AddComponent<TextMeshProUGUI>();
        statusLbl.alignment = TextAlignmentOptions.Center;

        var tradeGO = new GameObject("TradeUI", typeof(BiotonicFrontiers.UI.TradeUI));
        tradeGO.transform.SetParent(canvasGO.transform, false);
        var tradeUI = tradeGO.GetComponent<BiotonicFrontiers.UI.TradeUI>();

        BindPrivate(tradeUI, "listRoot", listRT);

        var prefabGO = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/TradeRow.prefab");
        if (prefabGO != null)
            BindPrivate(tradeUI, "rowPrefab", prefabGO.GetComponent<BiotonicFrontiers.UI.TradeRow>());

        BindPrivate(tradeUI, "offerItemInput",   offerItemRT.GetComponent<TMP_InputField>());
        BindPrivate(tradeUI, "offerQtyInput",    offerQtyRT.GetComponent<TMP_InputField>());
        BindPrivate(tradeUI, "requestItemInput", requestItemRT.GetComponent<TMP_InputField>());
        BindPrivate(tradeUI, "requestQtyInput",  requestQtyRT.GetComponent<TMP_InputField>());

        BindPrivate(tradeUI, "createTradeButton", createBtn);
        BindPrivate(tradeUI, "statusText",        statusLbl);

        SaveScene("Assets/Scenes/Trade.unity");

        // local helpers
        RectTransform MakeInput(string name, Vector2 pos, string placeholder)
        {
            var rt  = RT(name, pos, new Vector2(200, 40), parent);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = new Color(1,1,1,0.1f); img.raycastTarget = true;

            var input = rt.gameObject.AddComponent<TMP_InputField>();
            input.targetGraphic = img;

            var text = new GameObject("Text", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            text.transform.SetParent(rt, false);
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = new Vector2(8,6);
            text.rectTransform.offsetMax = new Vector2(-8,-7);
            input.textComponent = text;

            var ph = new GameObject("Placeholder", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            ph.transform.SetParent(rt, false);
            ph.rectTransform.anchorMin = text.rectTransform.anchorMin;
            ph.rectTransform.anchorMax = text.rectTransform.anchorMax;
            ph.rectTransform.offsetMin = text.rectTransform.offsetMin;
            ph.rectTransform.offsetMax = text.rectTransform.offsetMax;
            ph.text = placeholder;
            ph.fontStyle = FontStyles.Italic;
            ph.color = new Color(.5f,.5f,.5f,.75f);
            input.placeholder = ph;

            return rt;
        }
    }

    // ——— LAND MAP ————————————————————————————————————————————
    private static void LandMap()
    {
        var (_, canvasGO) = BootstrapScene("LandMap");
        var parent = canvasGO.transform;

        var gridRT = RT("GridRoot", Vector2.zero, new Vector2(1024, 1024), parent);
        var grid   = gridRT.gameObject.AddComponent<GridLayoutGroup>();
        grid.cellSize        = new Vector2(16, 16);
        grid.constraint      = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 64;

        var mapGO = new GameObject("LandMapUI", typeof(BiotonicFrontiers.UI.Map.LandMapUI));
        mapGO.transform.SetParent(canvasGO.transform, false);
        var mapUI = mapGO.GetComponent<BiotonicFrontiers.UI.Map.LandMapUI>();

        BindPrivate(mapUI, "gridRoot", gridRT);

        var prefabGO = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/LandTile.prefab");
        if (prefabGO != null)
            BindPrivate(mapUI, "tilePrefab", prefabGO.GetComponent<BiotonicFrontiers.UI.Map.LandTile>());

        SaveScene("Assets/Scenes/LandMap.unity");
    }

    /* ───────── utility helpers ───────── */

    private static Button BuildButton(RectTransform rt, string label)
    {
        var img = rt.gameObject.AddComponent<Image>();
        img.color = new Color(1,1,1,0.1f); img.raycastTarget = true;

        var btn = rt.gameObject.AddComponent<Button>();
        var lbl = new GameObject("Label", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        lbl.text = label;
        lbl.alignment = TextAlignmentOptions.Center;
        lbl.rectTransform.SetParent(rt, false);
        lbl.rectTransform.sizeDelta = rt.sizeDelta;
        return btn;
    }

    // reflection helper for private-field wiring
    private static void BindPrivate(object target, string fieldName, object value)
    {
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        target.GetType().GetField(fieldName, flags).SetValue(target, value);
    }
}
#endif // UNITY_EDITOR
