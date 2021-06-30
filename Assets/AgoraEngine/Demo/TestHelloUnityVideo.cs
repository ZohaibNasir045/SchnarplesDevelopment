using UnityEngine;
using UnityEngine.UI;

using agora_gaming_rtc;
using agora_utilities;
using System.Collections.Generic;


// this is an example of using Agora Unity SDK
// It demonstrates:
// How to enable video
// How to join/leave channel
// 
public class TestHelloUnityVideo : MonoBehaviour
{
    
    // instance of agora engine
    private IRtcEngine mRtcEngine;
    private Text MessageText;
    static List<uint> remoteStreams = new List<uint>();

    // load agora engine
    public void loadEngine(string appId)
    {
        // start sdk
        Debug.Log("initializeEngine");

        if (mRtcEngine != null)
        {
            Debug.Log("Engine exists. Please unload it first!");
            return;
        }

        // init engine
        mRtcEngine = IRtcEngine.GetEngine(appId);

        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
        Debug.Log("initializeEngine finished");

        mRtcEngine.OnUserJoined += (uint uid, int elapsed) => {
        string userJoinedMessage = string.Format("onUserJoined with uid {0}", uid);
        Debug.Log(userJoinedMessage);
        remoteStreams.Add(uid); // add remote stream id to list of users
    };
    }

    public void join(string channel,string token)
    {
        Debug.Log("calling join (channel = " + channel + ")");

        if (mRtcEngine == null)
            return;

        // set callbacks (optional)
        mRtcEngine.OnJoinChannelSuccess = onJoinChannelSuccess;
        mRtcEngine.OnUserJoined = onUserJoined;
        mRtcEngine.OnUserOffline = onUserOffline;
        mRtcEngine.OnWarning = (int warn, string msg) =>
        {
            Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
        };
        mRtcEngine.OnError = HandleError;

        // enable video
        mRtcEngine.EnableVideo();
        // allow camera output callback
        mRtcEngine.EnableVideoObserver();

        // join channel
        mRtcEngine.JoinChannel(token,channel, null, 0);
    }

    public string getSdkVersion()
    {
        string ver = IRtcEngine.GetSdkVersion();
        return ver;
    }

    public void leave()
    {
        Debug.Log("calling leave");

        if (mRtcEngine == null)
            return;

        // leave channel
        mRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        mRtcEngine.DisableVideoObserver();
    }

    // unload agora engine
    public void unloadEngine()
    {
        Debug.Log("calling unloadEngine");

        // delete
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            mRtcEngine = null;
        }
    }

    public void EnableVideo(bool pauseVideo)
    {
        if (mRtcEngine != null)
        {
            if (!pauseVideo)
            {
                mRtcEngine.EnableVideo();
            }
            else
            {
                mRtcEngine.DisableVideo();
            }
        }
    }

    // accessing GameObject in Scnene1
    // set video transform delegate for statically created GameObject
    private int PlayerCount=0;
    public void onSceneHelloVideoLoaded()
    {
        // Attach the SDK Script VideoSurface for video rendering
        //GameObject quad = GameObject.Find("User_Cam");\

        //Check playeer count
        if (remoteStreams.Count == 0)
            Debug.Log("No user found");
        Debug.Log(remoteStreams);
        //foreach(uint User in remoteStreams)
        //{
        GameObject point = null;
        GameObject quad;

        quad = GameObject.Find("Player1Portrait").GetComponent<Transform>().gameObject;
        if (PlayerCount == 0)
        {
            quad = GameObject.Find("Player1Portrait").GetComponent<Transform>().gameObject;
            Debug.Log("Found First Player");
            PlayerCount++;
        }
        
        if (ReferenceEquals(quad, null))
        {
            Debug.Log("failed to find Quad");
            return;
        }
        else
        {
            quad.AddComponent<VideoSurface>();
        }  
        

        /*GameObject cube = GameObject.Find("Cube");
        if (ReferenceEquals(cube, null))
        {
            Debug.Log("failed to find Cube");
            return;
        }
        else
        {
            cube.AddComponent<VideoSurface>();
        }*/

        GameObject text = GameObject.Find("MessageText");
        if (!ReferenceEquals(text, null))
        {
            MessageText = text.GetComponent<Text>();
        }
    //}
    }

    // implement engine callbacks
    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    private void onUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
        // this is called in main thread
        GameObject quad = GameObject.Find("Player2Portrait").GetComponent<Transform>().gameObject; ;
        if (PlayerCount == 1)
        {
            quad = GameObject.Find("Player2Portrait").GetComponent<Transform>().gameObject;
            PlayerCount++;
        }
        else if (PlayerCount == 2)
        {
            quad = GameObject.Find("Player3Portrait").GetComponent<Transform>().gameObject;
            PlayerCount++;
        }
        else if (PlayerCount == 3)
        {
            quad = GameObject.Find("Player4Portrait").GetComponent<Transform>().gameObject;
            PlayerCount++;
        }

        quad.AddComponent<VideoSurface>();
        // find a game object to render video stream from 'uid'
        /*GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }*/

        // create a GameObject and assign to this new user
        //VideoSurface videoSurface = makeImageSurface(uid.ToString());
        if (!ReferenceEquals(quad.GetComponent<VideoSurface>(), null))
        {
            // configure videoSurface
            quad.GetComponent<VideoSurface>().SetForUser(uid);
            quad.GetComponent<VideoSurface>().SetEnable(true);
            quad.GetComponent<VideoSurface>().SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            quad.GetComponent<VideoSurface>().SetGameFps(30);
        }

    }

    public VideoSurface makePlaneSurface(string goName)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);

        if (go == null)
        {
            return null;
        }
        go.name = goName;
        // set up transform
        go.transform.Rotate(-90.0f, 0.0f, 0.0f);
        float yPos = Random.Range(3.0f, 5.0f);
        float xPos = Random.Range(-2.0f, 2.0f);
        go.transform.position = new Vector3(xPos, yPos, 0f);
        go.transform.localScale = new Vector3(0.25f, 0.5f, .5f);

        // configure videoSurface
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }

    private const float Offset = 100;
    public VideoSurface makeImageSurface(string goName)
    {
        GameObject go = new GameObject();

        if (go == null)
        {
            return null;
        }

        go.name = goName;

        // to be renderered onto
        go.AddComponent<RawImage>();

        // make the object draggable
        go.AddComponent<UIElementDragger>();
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            go.transform.parent = canvas.transform;
        }
        // set up transform
        go.transform.Rotate(0f, 0.0f, 180.0f);
        float xPos = Random.Range(Offset - Screen.width / 2f, Screen.width / 2f - Offset);
        float yPos = Random.Range(Offset, Screen.height / 2f - Offset);
        go.transform.localPosition = new Vector3(xPos, yPos, 0f);
        go.transform.localScale = new Vector3(3f, 4f, 1f);

        // configure videoSurface
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }
    // When remote user is offline, this delegate will be called. Typically
    // delete the GameObject for this user
    private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        Debug.Log("onUserOffline: uid = " + uid + " reason = " + reason);
        // this is called in main thread
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Object.Destroy(go);
        }
    }

    #region Error Handling
    private int LastError { get; set; }
    private void HandleError(int error, string msg)
    {
        if (error == LastError)
        {
            return;
        }

        msg = string.Format("Error code:{0} msg:{1}", error, IRtcEngine.GetErrorDescription(error));

        switch (error)
        {
            case 101:
                msg += "\nPlease make sure your AppId is valid and it does not require a certificate for this demo.";
                break;
        }

        Debug.LogError(msg);
        if (MessageText != null)
        {
            if (MessageText.text.Length > 0)
            {
                msg = "\n" + msg;
            }
            MessageText.text += msg;
        }

        LastError = error;
    }

    #endregion
}
