using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
 public class FirebaseManager : MonoBehaviour
{ 
    
        /// This class is just the implementation of all the functions shown in AuthHandler
        /// - It will sign up a user to Firebase Auth
        /// - It will sign in a user to Firebase Auth
        /// </summary>

        [Header("User Credencials")]
        public static string UserID;
        public static string UserName;
        public static string UserAge;

        [Header("ID Token")]
        public static string IDtoken;
        [Header("Loading")]
        public GameObject LoadingPopUp;

    [Header("Input Fields")]
        public InputField Email;
        public InputField Pass;
        static string UserCreds;

    private GameObject CANVAS;
        public void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            PlayerPrefs.SetString("User_ID", "");
            PlayerPrefs.SetString("User_Name", "");
            PlayerPrefs.SetString("User_Age", "");
        }
    public void Start()
    {
        CANVAS = GameObject.Find("Canvas"); ;
    }
    private void StartLoading()
    {
        GameObject Loading = Instantiate(LoadingPopUp, new Vector3(0, 0, 0), Quaternion.EulerRotation(0,0,0));
        
        Loading.transform.SetParent(CANVAS.transform);
    }
    public void OnLogin()
        {
        StartLoading();
            Debug.Log("Signing IN: Email = " + Email.text + " & Password = " + Pass.text);
            AuthHandler.SignIn(Email.text, Pass.text);
            Debug.Log("User ID is "+ PlayerPrefs.GetString("User_ID"));
        }
        public void OnSignUp()
        {
            Debug.Log("Signing UP: Email = " + Email.text + " & Password = " + Pass.text);
            AuthHandler.SignUp(Email.text, Pass.text, new User("zohaib nasir", "nasir", 0));
        }
        protected void FixedUpdate()
        {
            UserID = PlayerPrefs.GetString("User_ID");
            UserName = PlayerPrefs.GetString("User_Name");
            UserAge = PlayerPrefs.GetString("User_Age");
            IDtoken = PlayerPrefs.GetString("idToken"); 
        }
    }
