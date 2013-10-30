using UnityEngine;
using UnityEditor;

using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
//using System.Net;

public class WooglieEditor : EditorWindow
{
    const int API_VERSION = 1;
    const string URL_LOSTPW = "http://www.wooglie.com/user.php?sub=lostpw";
    const string URL_REGISTER = "http://www.wooglie.com/user.php?sub=register&developer=1";
    const string URL_ACCOUNT = "http://www.wooglie.com/gamedevelopers.php";
    const string URL_HELP = "http://www.wooglie.com/gamedevelopers.php?subPage=apiHelp";

    static bool isUnityPRO = false;
    static bool checkedPRO = false;
    static WTextSettings settings;

    //static WebClient wcUploader;


    static WooglieEditor()
    {        
        settings = new WTextSettings("Assets/WooglieAPI/Settings/settings.txt");
    }


    [MenuItem("Window/Wooglie.com API")]
    static void Init()
    {       
        EditorWindow.GetWindow(typeof(WooglieEditor), false, "Wooglie API");
    }

    void Awake()
    {        
        ResetData();
        WWWForm form = new WWWForm();
        form.AddField("action", "checkLogin");
        Contact(form);
        wooglieLogo = (Texture2D)Resources.Load("WooglieLogo", typeof(Texture2D));

        SetupData();

    }

    bool startPublish = false;
    string uploadIndieGame = "";
    void WooglieUpdate()
    {     
        // ACTIONS

        if (startPublish)
        {
            startPublish = false;
            Publish();
        }
        if (uploadIndieGame != "")
        {
            UploadWebplayer(uploadIndieGame);
            uploadIndieGame = "";
        }
    }

    void SetupData()
    {
        //SETUP
        EditorApplication.update += WooglieUpdate;

        /*if (wcUploader == null)
        {
         wcUploader = new WebClient();
            wcUploader.UploadFileCompleted += UploadFileCompletedCallback;
            wcUploader.UploadProgressChanged += UploadProgressCallback;
        }         */

        //Test unity pro
        Assembly a = System.Reflection.Assembly.Load("UnityEditor");
        if (a == null) return;
        Type type = a.GetType("UnityEditorInternal.InternalEditorUtility");
        if (type == null) return;
        MethodInfo inf2 = type.GetMethod("HasPro");
        if (inf2 == null) return;

        isUnityPRO = (bool)inf2.Invoke(null, null);
        checkedPRO = true;
    }
    
    Texture2D wooglieLogo;
    Vector2 scrollPos = Vector2.zero;

    void OnGUI()
    {
        if (!checkedPRO)
            SetupData();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(wooglieLogo, new GUIStyle()))
            Application.OpenURL("http://www.Wooglie.com");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        bool hasLoginData = (EditorPrefs.GetString("WooglieAPIKey", "") != "");
        if (!hasLoginData)
        {
            NeedLoginGUI();
        }
        else
        {
            LoggedInGUI();
        }

        EditorGUILayout.EndScrollView();
    }


    public enum WooglieCategories { Action, Arcade, Adventure, Puzzle, Racing, Shooters, Sports, Strategy }
    public enum WooglieGameState { Completed, InDevelopment, Cancelled }

    class GameInfo
    {
        //LOADED VIA MAIN GAME LIST
        public int ID;
        public bool inDev;
        public string status = "";
        public bool requireEditor;
        public string title = "";
        public WooglieCategories gameCategory;
        public WooglieGameState gameState;
        public bool streamWebplayer;

        public bool isLive
        {
            get { return status.Contains("is live"); }
        }
        //LOADED WHEN EDITING ONLY
        public string changeNotes = "";
        public string gameControls = "";
        public string gameWords = "";
        public string gameLong = "";
        public string gameShort = "";
        public string gameTags = "";
        public int gameHeight;
        public int gameWidth;

        public bool uploadedAllImages = false;

        public GameInfo()
        { }
        //CLONE: USED FOR EDITING ONLY
        protected GameInfo(GameInfo other)
        {
            ID = other.ID;
            inDev = other.inDev;
            title = other.title;
            gameCategory = other.gameCategory;
            gameState = other.gameState;
            streamWebplayer = other.streamWebplayer;
            gameControls = other.gameControls;
            gameWords = other.gameWords;
            gameTags = other.gameTags;
            gameLong = other.gameLong;
            gameShort = other.gameShort;
            gameHeight = other.gameHeight;
            gameWidth = other.gameWidth;
            uploadedAllImages = other.uploadedAllImages;
            changeNotes = other.changeNotes;
        }

        public GameInfo Clone()
        {
            return new GameInfo(this);
        }
    }

    List<GameInfo> gameList = new List<GameInfo>();
    bool loadedGameList = false;
    int loadedGames = 0;

    void ResetData()
    {
        loadedGameList = false;
        gameList = new List<GameInfo>();
        editGameID = 0;
    }


    WooglieGameState StringToGameState(string state)
    {
        foreach (WooglieGameState suit in Enum.GetValues(typeof(WooglieGameState)))
        {
            if (suit + "" == state)
                return suit;
        }
        return WooglieGameState.InDevelopment;
    }
    WooglieCategories StringToCategory(string cat)
    {
        foreach (WooglieCategories suit in Enum.GetValues(typeof(WooglieCategories)))
        {
            if (suit + "" == cat)
                return suit;
        }
        return WooglieCategories.Action;
    }


    void LoadAccountData()
    {
        if (wooglieLogo == null) wooglieLogo = (Texture2D)Resources.Load("WooglieLogo", typeof(Texture2D));

        loadedGameList = true;

        WWWForm form = new WWWForm();
        form.AddField("action", "gameList");
        string output = Contact(form);
        string result = output;
        int statusCode = 0;
        if (result.Length >= 1)
        {
            statusCode = int.Parse(output.Substring(0, 1) + "");
            result = output.Substring(1);
        }
        if (statusCode != 1)
        {   //ERROR
            EditorUtility.DisplayDialog("Error", "Couldn't fetch game list, please relogin. Details:" + result, "OK");
        }
        else
        {
            string[] lines = result.Split('\n');
            gameList = new List<GameInfo>();
            foreach (string line in lines)
            {
                string[] items = line.Split('#');
                if (items.Length < 5) { continue; }
                GameInfo gi = new GameInfo();
                gi.ID = int.Parse(items[0]);
                gi.gameCategory = StringToCategory(items[1]);
                gi.status = items[2];
                gi.requireEditor = (items[3] == "1");
                gi.streamWebplayer = (items[4] == "1");
                gi.title = items[5];
                gi.gameState = StringToGameState(items[6]);
                gameList.Add(gi);
            }
            loadedGames = gameList.Count;
        }
    }


    #region LoginArea

    string loginUsername = "";
    string loginPassword = "";

    void NeedLoginGUI()
    {
        GUILayout.Space(10); loginUsername = EditorGUILayout.TextField("Username", loginUsername);
        loginPassword = EditorGUILayout.PasswordField("Password", loginPassword);
        GUILayout.Space(10);
        if (GUILayout.Button("Login"))
        {
            DoLogin();
        }


        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Register [URL]"))
        {
            Application.OpenURL(URL_REGISTER);
        }
        if (GUILayout.Button("Forgot your details? [URL]"))
        {
            Application.OpenURL(URL_LOSTPW);
        }
        GUILayout.EndHorizontal();
    }

    void DoLogin()
    {
        ResetData();
        WWWForm form = new WWWForm();
        form.AddField("action", "login");
        form.AddField("loginUser", loginUsername);
        form.AddField("loginPW", loginPassword);
        string result = Contact(form);
        int statusCode = 0;
        if (result.Length >= 1)
        {
            statusCode = int.Parse(result.Substring(0, 1) + "");
            result = result.Substring(1);

        }

        if (statusCode != 1)
        {   //ERROR
            bool status = EditorUtility.DisplayDialog("Error", "Login error: " + result, "OK", "Forgot PW?");
            if (!status)
                Application.OpenURL(URL_LOSTPW);
        }
        else
        {
            string[] output = result.Split('#');
            EditorPrefs.SetString("WooglieAPIKey", output[0]);
            EditorPrefs.SetString("WooglieDevID", output[1]);


        }
    }

    private enum MenuState { selectgame, editgame }
    MenuState menuState = MenuState.selectgame;

    void LoggedInGUI()
    {
        if (!loadedGameList || loadedGames != gameList.Count)
            LoadAccountData();

        switch (menuState)
        {
            case MenuState.selectgame:
                SelectGameGUI();
                break;
            case MenuState.editgame:
                EditGameGUI();
                break;
        }

    }

    #endregion

    void SelectGameGUI()
    {
        if (GUILayout.Button("Documentation [URL]"))
            Application.OpenURL(URL_HELP);
        if (GUILayout.Button("Open account page [URL]"))
            Application.OpenURL(URL_ACCOUNT);
        if (GUILayout.Button("Logout"))
        {
            EditorPrefs.SetString("WooglieAPIKey", "");
        }




        EditorGUILayout.Separator();
        if (gameList.Count > 0)
        {

            GUILayout.BeginHorizontal();
            GUILayout.Label("You have " + gameList.Count + " games on Wooglie.");
            GUILayout.FlexibleSpace();
            AddGameButton();
            GUILayout.EndHorizontal();


            foreach (GameInfo game in gameList)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Edit", GUILayout.Width(50)))
                {
                    editGameID = game.ID; menuState = MenuState.editgame;
                }
                GUILayout.Label(game.title, GUILayout.Width(120));
                GUILayout.Label("|    " + game.gameCategory, GUILayout.Width(100));
                GUILayout.Label("|  ", GUILayout.Width(20));
                string thisStatus = "";
                if (game.isLive)
                {
                    GUI.color = Color.green;
                    thisStatus = "LIVE";
                }
                else if (game.inDev)
                {
                    GUI.color = Color.yellow;
                    thisStatus = "DEV";
                }
                else
                {
                    if (game.requireEditor || game.status.Contains("Awaiting staff"))
                    {
                        GUI.color = Color.yellow;
                        thisStatus = "Awaiting staff review";
                    }
                    else
                    {
                        GUI.color = Color.red;
                        thisStatus = "Please upload!";
                    }
                }

                GUILayout.Label(thisStatus, GUILayout.Width(150));


                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("You have not yet added any games to Wooglie.");
            GUILayout.FlexibleSpace();
            AddGameButton();
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.Separator();
    }

    void AddGameButton()
    {
        if (GUILayout.Button("Add a new game"))
        {
            WWWForm form = new WWWForm();
            form.AddField("action", "addGame");
            string output = Contact(form);
            string result = output;
            int statusCode = 0;
            if (result.Length >= 1)
            {
                statusCode = int.Parse(output.Substring(0, 1) + "");
                result = output.Substring(1);
            }
            if (statusCode != 1)
            {   //ERROR
                EditorUtility.DisplayDialog("Error", "Couldn't add game. Details:" + result, "OK");
            }
            else
            {
                editGameID = int.Parse(result); menuState = MenuState.editgame;
                //Reload game list
                LoadAccountData();
            }
        }
    }


    private int editGameID = 0;
    private GameInfo editGameInfo = null;
    private int lastMetaDownload = 0;
    private bool metaIsUploaded = false;

    string IsMetaDataOK(GameInfo info)
    {
        if (info.title.Length <= 2) return "Please correct the title.";
        if (info.gameControls.Length <= 2) return "Please enter a controls description.";
        //if (info.gameWords.Length <= 2) return "Please enter a 5 word description.";
        if (info.gameShort.Length <= 2) return "Please enter a short description.";
        if (info.gameLong.Length <= 2) return "Please enter a long description.";
        return "";
    }

    bool showStepOne = true;
    bool showStepTwo = true;
    bool showStepThree = true;

    string CapString(string input, int len)
    {
        if (input.Length > len)
            return input.Substring(0, len);
        return input;
    }

    void EditGameGUI()
    {
        if (GUILayout.Button("Back"))
        {
            menuState = MenuState.selectgame;
            editGameInfo = null;
        }

        if (editGameInfo == null || editGameID != editGameInfo.ID)
        {
            editGameInfo = null; lastMetaDownload = 0; metaIsUploaded = false;
            foreach (GameInfo info in gameList)
            {
                if (info.ID == editGameID)
                {
                    editGameInfo = info.Clone();
                    break;
                }
            }
        }
        if (editGameInfo == null) return;
        if (lastMetaDownload == 0 || lastMetaDownload != editGameInfo.ID)
        {
            lastMetaDownload = editGameInfo.ID;
            DownloadEditMetaData();
            metaIsUploaded = IsMetaDataOK(editGameInfo) == "";
        }
        EditorGUILayout.Separator();

        GUILayout.Label("How to upload your game?", "boldLabel");
        GUILayout.Label("1) Submit the required metadata.", "miniLabel");
        GUILayout.Label("2) Select and upload the required promotional images.", "miniLabel");
        GUILayout.Label("3) Press publish!", "miniLabel");
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();


        GUILayout.Label("Add your game:", "boldLabel");
        showStepOne = EditorGUILayout.Foldout(showStepOne, "1) Meta data:");
        if (showStepOne)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            GUILayout.BeginVertical();

            GUILayout.Label("The game description etc. After completing this information you'll be able to upload your game.", "miniLabel");
            EditorGUILayout.Separator();


            editGameInfo.title = EditorGUILayout.TextField("Title:", editGameInfo.title);
            editGameInfo.gameCategory = (WooglieCategories)EditorGUILayout.EnumPopup("Category:", editGameInfo.gameCategory);
            editGameInfo.gameState = (WooglieGameState)EditorGUILayout.EnumPopup("State:", editGameInfo.gameState);
            editGameInfo.streamWebplayer = EditorGUILayout.Toggle("Stream webplayer:", editGameInfo.streamWebplayer);

            editGameInfo.gameWidth = EditorGUILayout.IntField("Width", editGameInfo.gameWidth);
            editGameInfo.gameHeight = EditorGUILayout.IntField("Height", editGameInfo.gameHeight);

            editGameInfo.gameTags = EditorGUILayout.TextField("Tags:", editGameInfo.gameTags);
            //editGameInfo.gameWords = EditorGUILayout.TextField("+-5 words description:", editGameInfo.gameWords);

            editGameInfo.gameShort = EditorGUILayout.TextField("Short description:", editGameInfo.gameShort);
            GUILayout.Label("Controls:");
            editGameInfo.gameControls = EditorGUILayout.TextArea(editGameInfo.gameControls, GUILayout.Height(60));
            GUILayout.Label("Long description:");
            editGameInfo.gameLong = EditorGUILayout.TextArea(editGameInfo.gameLong, GUILayout.Height(100));

            //LIMIT
            editGameInfo.gameWidth = Mathf.Clamp(editGameInfo.gameWidth, 100, 925);
            editGameInfo.gameHeight = Mathf.Clamp(editGameInfo.gameHeight, 100, 600);
            editGameInfo.title = CapString(editGameInfo.title, 100);
            editGameInfo.gameWords = CapString(editGameInfo.gameWords, 35);

            string err = IsMetaDataOK(editGameInfo);
            if (err != "")
            {
                GUI.color = Color.red;
                GUILayout.Label("Missing information: " + IsMetaDataOK(editGameInfo), "miniLabel");
                GUI.color = Color.white;

            }
            else
            {
                if (GUILayout.Button("Update metadata"))
                {
                    //SAVE META
                    SaveMetaData(editGameInfo);
                    metaIsUploaded = IsMetaDataOK(editGameInfo) == "";
                    if (metaIsUploaded)
                    {
                        showStepOne = false;
                        showStepTwo = true;
                    }
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }


        showStepTwo = EditorGUILayout.Foldout(showStepTwo, "2) Upload images");
        if (showStepTwo)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            GUILayout.BeginVertical();

            if (!metaIsUploaded)
            {
                GUI.color = Color.red;
                GUILayout.Label("You can not yet upload the webplayer and images:\n- Upload your metadata first.", "miniLabel");
                GUI.color = Color.white;
            }
            else
            {
                GUILayout.Label("When you press publish, the game will be build and the images and game will be uploaded to Wooglie.com.", "miniLabel");

                if (textureIcon == null && settings.GetString("promoImagesIcon", "") != "")
                    textureIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(settings.GetString("promoImagesIcon", ""), typeof(Texture2D));
                if (textureFeature == null && settings.GetString("promoImagesFeatured", "") != "")
                    textureFeature = (Texture2D)AssetDatabase.LoadAssetAtPath(settings.GetString("promoImagesFeatured", ""), typeof(Texture2D));


                GUILayout.BeginHorizontal();
                textureIcon = (Texture2D)EditorGUILayout.ObjectField("Icon:", textureIcon, typeof(Texture2D), false, GUILayout.Width(200));
                GUILayout.Label("Main icon\n100x100 ", "miniLabel");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();



                GUILayout.BeginHorizontal();
                textureFeature = (Texture2D)EditorGUILayout.ObjectField("*Feature:", textureFeature, typeof(Texture2D), false, GUILayout.Width(200));
                GUILayout.Label("Used when featured\n600x280\n*This image is optional", "miniLabel");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (GUI.changed)
                {
                    if (textureIcon != null){
                        string imgPath = AssetDatabase.GetAssetPath(textureIcon);
                        if(imgPath==""){
                            //string str = "(width:" + textureIcon.width + " height:" + textureIcon.height + ")";
                            textureIcon = null;
                            //EditorUtility.DisplayDialog("Error", "The image you selected does not match the required size (width: 100 height: 100), you select an image with the following sizes: "+str, "OK");        
                        }else{
                            settings.SetString("promoImagesIcon", imgPath);
                        }
                    }
                    if (textureFeature != null)
                    {
                        string imgPath = AssetDatabase.GetAssetPath(textureFeature);
                        if (imgPath == "") textureIcon = null;
                        else settings.SetString("promoImagesFeatured", imgPath);
                    }
                }

                if (GUILayout.Button("Upload promo images"))
                {
                    //SAVE META
                    if (UploadFiles())
                    {
                        if (editGameInfo.uploadedAllImages)
                        {
                            showStepTwo = false;
                            showStepThree = true;
                        }
                    }
                }

            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        showStepThree = EditorGUILayout.Foldout(showStepThree, "3) Upload game");
        if (showStepThree)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            GUILayout.BeginVertical();

            if (editGameInfo.uploadedAllImages)
            {
                if (isUnityPRO)
                {
                    GUILayout.Label("Because you are using Unity PRO, you can simply use the button below to complete\n" +
                        "your Wooglie submission. The tool will automatically build the game using your current\n" +
                        "build scenes. Furthermore a script will be added to your first screen to ensure no\n" +
                        "other websites can copy your game.", "miniLabel");

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    if (GUILayout.Button("Upload game!", GUILayout.Width(100)))
                    {
                        startPublish = true;   
                    }

                    GUILayout.EndHorizontal();
                }
                else
                {

                    GUILayout.Label("Because you're using Unity FREE(Indie), you cant upload in one click;\nYou'll need to manually build a .unity3d webplayer and upload it using the form below.\nNote that the build NEEDS to include the WoogliePiracyProtection script, use the button below to add it to your project(or to test if it is in).", "miniLabel");

                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    if (GUILayout.Button("Add/Test Wooglie script", GUILayout.Width(200)))
                    {   
                        testedScriptPresence = AddWooglieScript(true);
                    }

                    GUILayout.EndHorizontal();

                    if (testedScriptPresence)
                    {
                        GUILayout.Label("Upload webplayer:", "boldLabel");

                        if (!File.Exists(uploadFilePath)) uploadFilePath = "";
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        GUILayout.Label("Path: " + uploadFilePath, GUILayout.Width(200));
                        if (GUILayout.Button("Select a .unity3d webplayer", GUILayout.Width(200)))
                        {
                            uploadFilePath = EditorUtility.OpenFilePanel("Select a unity3d webplayer", "", "unity3d");
                        }
                        GUILayout.EndHorizontal();

                        if (uploadFilePath != "")
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            if (GUILayout.Button("Upload game!", GUILayout.Width(100)))
                            {
                                uploadIndieGame = "file://" + uploadFilePath; 
                            }
                            GUILayout.EndHorizontal();

                        }
                    }
                }
                GUILayout.Space(10);
                GUILayout.Label("Privacy notice: Please note that with uploading your game you will also submit your\nUnity version and the build size log of the webplayer (if present).", "miniLabel");

            }
            else
            {
                GUI.color = Color.red;
                GUILayout.Label("You can not yet upload the webplayer:\n- Upload your images first.", "miniLabel");
                GUI.color = Color.white;
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();


    }

    string uploadFilePath = "";
    bool testedScriptPresence = false;

    Texture2D textureIcon = null;
    Texture2D textureFeature = null;

    /*bool VerifyImage(Texture2D text, int width, int height)
    {
               if (text.width != width || text.height != height)
        {
            return false;
        }
        return true;
    }*/


    byte[] GetImageBytes(Texture2D text)
    {
        string path = AssetDatabase.GetAssetPath(text);
        byte[] bits = System.IO.File.ReadAllBytes(path);
        /*
        TextureImporter tI = (TextureImporter)TextureImporter.GetAtPath(path);
        tI.isReadable = true;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        byte[] bits = textureIcon.EncodeToPNG();
        tI.isReadable = false;
         */
        return bits;
    }

    bool UploadFiles()
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "uploadGame");
        form.AddField("gameID", editGameInfo.ID);
        if (textureIcon != null) form.AddBinaryData("uploadFileIcon", GetImageBytes(textureIcon));
        if (textureFeature != null) form.AddBinaryData("uploadFileFeatured", GetImageBytes(textureFeature));

        string result = Contact(form);
        int statusCode = 0;
        if (result.Length >= 1)
        {
            statusCode = int.Parse(result.Substring(0, 1) + "");
            result = result.Substring(1);
        }
        if (statusCode != 1)
        {   //ERROR
            EditorUtility.DisplayDialog("Error", "Couldn't upload images. Details:" + result, "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Succes", "Your image(s) have been uploaded!", "OK");
            LoadAccountData();
            editGameInfo.uploadedAllImages = true;
            return true;
        }
        return false;
    }


    void DownloadEditMetaData()
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "gameInfo");
        form.AddField("gameID", editGameInfo.ID);
        string result = Contact(form);
        int statusCode = 0;
        if (result.Length >= 1)
        {
            statusCode = int.Parse(result.Substring(0, 1) + "");
            result = result.Substring(1);
        }
        if (statusCode != 1)
        {   //ERROR
            EditorUtility.DisplayDialog("Error", "Couldn't download game metadata. Details:" + result, "OK");
        }
        else
        {
            string[] items = result.Split('\n');
            editGameInfo.title = items[0];
            editGameInfo.gameWidth = int.Parse(items[1]);
            editGameInfo.gameHeight = int.Parse(items[2]);
            editGameInfo.gameControls = BRtoN(items[3]);
            editGameInfo.gameWords = items[4];
            editGameInfo.gameShort = BRtoN(items[5]);
            editGameInfo.gameLong = BRtoN(items[6]);
            editGameInfo.changeNotes = BRtoN(items[7]);
            editGameInfo.uploadedAllImages = items[8] == "1";
            editGameInfo.gameTags = items[9];

        }
    }

    string BRtoN(string input)
    {
        return input.Replace("<br />", "\n");
    }

    void SaveMetaData(GameInfo gI)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "uploadMeta");
        form.AddField("gameID", gI.ID);
        form.AddField("gameName", gI.title);
        form.AddField("gameControls", gI.gameControls);
        form.AddField("gameCategory", "" + gI.gameCategory);
        form.AddField("gameState", "" + gI.gameState);
        form.AddField("gameTags", gI.gameTags);
        form.AddField("gameWords", gI.gameWords);
        form.AddField("gameShort", gI.gameShort);
        form.AddField("gameLong", gI.gameLong);
        form.AddField("gameName", gI.title);
        form.AddField("streamWebplayer", gI.streamWebplayer ? "1" : "0");
        form.AddField("gameWidth", gI.gameWidth);
        form.AddField("gameHeight", gI.gameHeight);

        string result = Contact(form);
        int statusCode = 0;
        if (result.Length >= 1)
        {
            statusCode = int.Parse(result.Substring(0, 1) + "");
            result = result.Substring(1);
        }
        if (statusCode != 1)
        {   //ERROR
            EditorUtility.DisplayDialog("Error", "Couldn't save game: " + result, "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Saved!", "Saved metadata!", "OK");
            LoadAccountData();
        }
    }




    #region BUILD

    public static string TMPFolder()
    {
        // string pat = Path.GetTempPath() + "WooglieUpload/";
        string pat = Application.dataPath.Replace("/Assets", "/") + "Builds/WooglieUpload/";
        EnsureFolders(pat);
        return pat;
    }
    public static void EnsureFolders(string path)
    {
        path = path.Replace('\\', '/');
        string[] folders = path.Split('/');
        for (int i = 0; i < folders.Length - 1; i++)
        {
            string currentpath = folders[i];
            if (i > 0)
            {
                for (int j = i - 1; j >= 0; j--)
                    currentpath = folders[j] + '/' + currentpath;
            }
            if (currentpath == "") continue;
            if (!Directory.Exists(currentpath))
            {
                Directory.CreateDirectory(currentpath);
            }
        }
    }

    static string[] GetBuildScenes()
    {
        List<string> scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                scenes.Add(scene.path);
        }
        return scenes.ToArray();
    }


    bool AddWooglieScript(bool warn)
    {
        //Add Wooglie security

        EditorApplication.SaveCurrentSceneIfUserWantsTo();

        string[] scenes =GetBuildScenes();
        if (scenes.Length <= 0)
        {
            EditorUtility.DisplayDialog("Wooglie security", "The script could not be added to your first scene as this project doesnt have any buildsettings. Please open the correct project.", "OK");
            return false;
        }

        if(EditorApplication.currentScene != scenes[0]){
            EditorApplication.OpenScene(scenes[0]);
        }

        UnityEngine.Object obj = GameObject.FindObjectOfType(typeof(WoogliePiracyProtection));
        if (obj == null)
        {
            if (warn) EditorUtility.DisplayDialog("Wooglie security", "Added script to the first scene. You can now make a build.", "OK");
            new GameObject("Wooglie", typeof(WoogliePiracyProtection));
        }
        else
        {
            if (warn) EditorUtility.DisplayDialog("Wooglie security", "The script has already been added to the first scene. You can now make a build.", "OK");
        }
        EditorApplication.SaveScene(scenes[0]);
        return true;
    }

    void Publish()
    {
        AddWooglieScript(false);

        string buildResult = StartBuild();
        if (buildResult != "")
        {
            if (buildResult.Contains("requires Unity PRO"))
            {
                isUnityPRO = false;
            }
            return;
        }

        UploadWebplayer("");
       
       //Remove wooglie protection
       WoogliePiracyProtection obj2 = (WoogliePiracyProtection)GameObject.FindObjectOfType(typeof(WoogliePiracyProtection));
       if (obj2 != null)
       {
           DestroyImmediate(obj2.gameObject);
       }

       try
       {
             Directory.Delete(TMPFolder(), true);
       }
       catch (Exception ex) { Debug.Log(ex); }
       
    }
    string StartBuild()
    {
        string outputFolder = TMPFolder();
        BuildTarget target = BuildTarget.WebPlayerStreamed;
        if (!editGameInfo.streamWebplayer)
            target = BuildTarget.WebPlayer;

        if (GetBuildScenes().Length <= 0)
        {
           string err = "Couldn't attempt an automatic build: No scenes are active in this project.";
           EditorUtility.DisplayDialog("Build error", err, "OK");
           return err;
        }

        string errorR = BuildPipeline.BuildPlayer(GetBuildScenes(), outputFolder, target, BuildOptions.None);
        if (errorR.Contains("requires Unity PRO"))
        {
            return errorR;
        }
        else if (errorR != "")
        {
            EditorUtility.DisplayDialog("Build error", errorR, "OK");
            return errorR;
        }
        return "";
    }


    void UploadWebplayer(string file)
    {
        if (file == "")
        {
            file = ("file://" + TMPFolder() + ".unity3d").Replace("\\", "/");
        }

        WWW localFile = new WWW(file);


        int mb = localFile.bytes.Length / (1024 * 1024);
        if (mb >= 23)
        {
            EditorUtility.DisplayDialog("Error", "The game is " + mb + "Mb. THe max. upload size is 20MB. Please optimize your game. ", "OK");
            return;
        }else if(localFile.bytes.Length/1024 <= 1 ){
            EditorUtility.DisplayDialog("Error", "The API tried to upload a file with filesize <=1kb (" + (localFile.bytes.Length/1024) + " kb). Please contact hello@Wooglie.com so that we can help you upload your game.", "OK");
            return;
        }

        //new System.Threading.Thread(() => wcUploader.UploadFileAsync(new Uri("http://devs.wooglie.com/editorAPI.php"), "POST", file)).Start();
                
        // Create a Web Form
        WWWForm form = new WWWForm();
        form.AddField("action", "uploadGame");
        form.AddField("gameID", editGameID);
        form.AddBinaryData("newUnityFile", localFile.bytes, "gameData");
        form.AddField("buildReport", GetBuildSizeReport());
        form.AddField("unityEditorVersion", Application.unityVersion);

        string wwwData = Contact(form);
        EditorUtility.ClearProgressBar();
        string result = wwwData;
        int statusCode = 0;
        if (result.Length >= 1)
        {
            statusCode = int.Parse(wwwData.Substring(0, 1) + "");
            result = wwwData.Substring(1);
        }

        if (statusCode != 1)
        {   //ERROR
            EditorUtility.DisplayDialog("Error", "Upload error: " + result, "OK");
        }
        else
        {            
            EditorUtility.DisplayDialog("Succes!", "Your game has been submitted to Wooglie!\nYou will receive an email once we review the game, usually within 3 business days." + result, "OK");
            LoadAccountData();
        }        
    }



    /// <summary>
    /// Gets the editor log. But -only- grab the last Player size statistics
    /// </summary>
    /// <returns></returns>
    static string GetBuildSizeReport()
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/Unity/Editor/Editor.log";

        string editorLogContents = "";
        try
        {
            using (FileStream fileStream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    editorLogContents = streamReader.ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Could not open editor log: " + ex);
        }

        if (editorLogContents != "")
        {
            int lastIndex = editorLogContents.LastIndexOf("***Player size statistics***");
            if (lastIndex <= 0) return ""; // No log
            int indexComplete = editorLogContents.LastIndexOf("*** Completed");
            int len = indexComplete - lastIndex;
            if (len < 0) len = editorLogContents.Length - lastIndex - 1;
            string buildSize = editorLogContents.Substring(lastIndex, len);

            indexComplete = buildSize.IndexOf("Unloading ");
            if (indexComplete > 0)
                buildSize = buildSize.Substring(0, indexComplete);

            return buildSize;
        }
        return "";
    }

    #endregion





    string Contact(WWWForm form)
    {
        form.AddField("editorAPIVersion", API_VERSION);

        string apiKey = EditorPrefs.GetString("WooglieAPIKey", "");
        string devID = EditorPrefs.GetString("WooglieDevID", "");
        if (apiKey != "")
        {
            form.AddField("apiKey", apiKey);
            form.AddField("devID", devID);
        }
        WWW www = new WWW("http://devs.wooglie.com/editorAPI.php", form);
        while (!www.isDone)
        {
            EditorUtility.DisplayProgressBar("Uploading", "Progress: " + ((int)(www.uploadProgress*100))+"%", www.uploadProgress);
                //HANG 
        }
        EditorUtility.ClearProgressBar();

        if (www.error != null)
        {
            EditorUtility.DisplayDialog("Wooglie.com connection error", "Details: " + www.error, "OK");
            return "";
        }


        if (www.text.Length >= 1)
        {
            if (www.text[0] == '0')
            {
                if (www.text.Contains("Invalid login"))
                {//Clear the invalid login data
                    EditorUtility.DisplayDialog("Invalid Wooglie API login", "Please relogin.", "OK");
                    EditorPrefs.SetString("WooglieAPIKey", "");
                    EditorPrefs.SetString("WooglieDevID", "");
                    ResetData();
                }
                else if (www.text.Contains("Outdated editor") || www.text.Contains("update the Wooglie API"))
                {
                    EditorUtility.DisplayDialog("Oudated Wooglie API", www.text, "OK");
                }
            }
        }

        //All good..but calls need to check for 1 or 0
        return www.text;
    }


    /*
    private void UploadProgressCallback(object sender, UploadProgressChangedEventArgs e)
    {
        Debug.Log((string)e.UserState + "\n\n"
                                        + "Uploaded " + e.BytesSent + "/" + e.TotalBytesToSend
                                        + "b (" + e.ProgressPercentage + "%)");
    }
    private void UploadFileCompletedCallback(object sender, UploadFileCompletedEventArgs e)
    {
        Debug.Log("Upload complete my liege!");
    }*/

}