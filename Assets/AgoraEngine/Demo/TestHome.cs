using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif
using System.Collections;

/// <summary>
///    TestHome serves a game controller object for this application.
/// </summary>
public class TestHome : MonoBehaviour
{

    // Use this for initialization
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif
    static TestHelloUnityVideo app = null;

    private string HomeSceneName = "SceneHome";

    private string PlaySceneName = "SceneHelloVideo";

    readonly string getURL = "http://18.222.126.18:80";

    // PLEASE KEEP THIS App ID IN SAFE PLACE
    // Get your own App ID at https://dashboard.agora.io/
    [SerializeField]
    private string AppID = "7017ad3e380a41c1aa1dba2a279dd45a";

    [SerializeField]
    private string TOKEN = "";
    Text TokenText;
    void Awake()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
		permissionList.Add(Permission.Microphone);         
		permissionList.Add(Permission.Camera);               
#endif

        // keep this alive across scenes
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        CheckAppId();
        // Get token from Server
        StartCoroutine(SimpleGetRequest());
        
    }

    void Update()
    {
        CheckPermissions();
    }

    private void CheckAppId()
    {
        Debug.Assert(AppID.Length > 10, "Please fill in your AppId first on Game Controller object.");
        
    }

    /// <summary>
    ///   Checks for platform dependent permissions.
    /// </summary>
    private void CheckPermissions()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        foreach(string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {                 
				Permission.RequestUserPermission(permission);
			}
        }
#endif
    }

    public void onJoinButtonClicked()
    {
        // get parameters (channel name, channel profile, etc.)
        //GameObject go = GameObject.Find("ChannelName");
        //InputField field = go.GetComponent<InputField>();

        string ChannelName = "ChannelA";
        // create app if nonexistent
        if (ReferenceEquals(app, null))
        {
            app = new TestHelloUnityVideo(); // create app
            app.loadEngine(AppID); // load engine
        }
        
        // join channel and jump to next scene
        app.join(ChannelName,TOKEN);
        //SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        //SceneManager.LoadScene(PlaySceneName, LoadSceneMode.Single);
        if (!ReferenceEquals(app, null))
        {
            app.onSceneHelloVideoLoaded(); // call this after scene is loaded
        }
    }
    IEnumerator SimpleGetRequest()
    {
        UnityWebRequest www = UnityWebRequest.Get(getURL);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }

        else
        {
            TOKEN = www.downloadHandler.text;
        }
    }
    public void onLeaveButtonClicked()
    {
        if (!ReferenceEquals(app, null))
        {
            app.leave(); // leave channel
            app.unloadEngine(); // delete engine
            app = null; // delete app
            SceneManager.LoadScene(HomeSceneName, LoadSceneMode.Single);
        }
        Destroy(gameObject);
    }

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == PlaySceneName)
        {
            if (!ReferenceEquals(app, null))
            {
                app.onSceneHelloVideoLoaded(); // call this after scene is loaded
            }
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }
    }

    void OnApplicationPause(bool paused)
    {
        if (!ReferenceEquals(app, null))
        {
            app.EnableVideo(paused);
        }
    }

    void OnApplicationQuit()
    {
        if (!ReferenceEquals(app, null))
        {
            app.unloadEngine();
        }
    }
}
