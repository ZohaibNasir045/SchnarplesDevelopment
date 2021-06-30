using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public static class AuthHandler
{
    private const string apiKey = "AIzaSyCfvpfy1wF1_tB5hZuGRFVoYqcp1O-FGIs"; // You can find this in your Firebase project settings

    private static fsSerializer serializer = new fsSerializer();

    public delegate void EmailVerificationSuccess();
    public delegate void EmailVerificationFail();
    public delegate void FoundOnlineUser();
    public delegate void NotFoundOnlineUser();

    public static string idToken; // Key that proves the request is authenticated and the identity of the user
    public static string userId="";
    public static string userName="";
    /// <summary>
    /// Sings up user with Firebase Auth using Email and Password method
    /// Uploads the user object to Firebase Database
    /// Sends verification email
    /// </summary>
    /// <param name="email"> User's email </param>
    /// <param name="password"> User's password </param>
    /// <param name="user"> User object, which gets uploaded to Firebase Database </param>
    public static void SignUp(string email, string password, User user)
    {
        var payLoad = $"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}";
        RestClient.Post($"https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key={apiKey}",
            payLoad).Then(
            response =>
            {
                Debug.Log("Created User");
                
                var responseJson = response.Text;

                // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
                // to serialize more complex types (a Dictionary, in this case)
                var data = fsJsonParser.Parse(responseJson);
                object deserialized = null;
                serializer.TryDeserialize(data, typeof(Dictionary<string, string>), ref deserialized);

                var authResponse = deserialized as Dictionary<string, string>;

                DatabaseHandler.PostUser(user, authResponse["localId"], () => { }, authResponse["idToken"]);

                SendEmailVerification(authResponse["idToken"]);
                PlayerPrefs.SetString("idToken", authResponse["idToken"]);
            }).Catch(err => {
                Debug.Log("User credentials are wrong");
            });
    }
    public static IEnumerator SendFriendInvite(string Name, FoundOnlineUser foundOnlineUser,NotFoundOnlineUser notFoundOnlineUser)
    {
        bool OnlineStat = false;
        DatabaseHandler.GetUsers("UserOnline", users =>
        {
            foreach(KeyValuePair<string, UserOnline> user in users)
            {
                if(user.Value.UserTag == Name)
                {
                    Debug.Log("Akram in online");
                    OnlineStat = true;
                    userId = user.Key;
                    foundOnlineUser();
                }
            }
            if (OnlineStat)
            {
                //Add to firebase so user can get invitation
            }
            else
            {
                notFoundOnlineUser();
            }
        });
        yield return null;
        
    }
    
    
    /// <summary>
    /// Sends verification email
    /// </summary>
    /// <param name="newIdToken"> User's token, retrieved from SignUp </param>
    private static void SendEmailVerification(string newIdToken)
    {
        var payLoad = $"{{\"requestType\":\"VERIFY_EMAIL\",\"idToken\":\"{newIdToken}\"}}";
        RestClient.Post(
            $"https://www.googleapis.com/identitytoolkit/v3/relyingparty/getOobConfirmationCode?key={apiKey}", payLoad);
    }
    
    /// <summary>
    /// Signs in the user with Firebase Auth using Email and Password method
    /// Checks if user accepted verification email
    /// If that's the case, logs the user's object (name, surname, age)  
    /// </summary>
    /// <param name="email"> User's email </param>
    /// <param name="password"> User's password </param>
    public static string SignIn(string email, string password)
    {
        var payLoad = $"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}";
        RestClient.Post($"https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key={apiKey}",
            payLoad).Then(
            response =>
            {
                var responseJson = response.Text;

                // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
                // to serialize more complex types (a Dictionary, in this case)
                var data = fsJsonParser.Parse(responseJson);
                object deserialized = null;
                serializer.TryDeserialize(data, typeof(Dictionary<string, string>), ref deserialized);

                var authResponse = deserialized as Dictionary<string, string>;

                Debug.Log(authResponse);
                 CheckEmailVerification(authResponse["idToken"], () =>
                {
                    Debug.Log("Email verified, getting user info of "+ userId);
                    
                    DatabaseHandler.GetUser(userId, user =>
                    {
                        Debug.Log($"{user.name}, {user.surname}, {user.age}");
                        Debug.Log("task Completed");
                        PlayerPrefs.SetString("User_ID",userId);
                        PlayerPrefs.SetString("User_Name",user.name);
                        PlayerPrefs.SetString("User_Age",user.age.ToString());

                        DatabaseHandler.MakeUserOnline(new UserOnline("true", user.name), userId, user.name, () => {
                            Debug.Log("User " + PlayerPrefs.GetString("User_Name") + " sis online now");
                            //load Lobby Scene
                            SceneManager.LoadScene("PhotonLobbyScene");
                        }, () => {
                            Debug.Log("User could not be logined");
                        }, idToken);

                    });
                }, () => { Debug.Log("Email not verified"); });
            });
        return "abc";
    }

    /// <summary>
    /// Checks if user accepted verification email
    /// </summary>
    /// <param name="newIdToken"> User's token, retrieved from SignIn </param>
    /// <param name="callback"> What to do after acknowledging the user has verified the email </param>
    /// <param name="fallback"> What to do after acknowledging the user hasn't verified the email </param>
    private static void CheckEmailVerification(string newIdToken, EmailVerificationSuccess callback,
        EmailVerificationFail fallback)
    {
        var payLoad = $"{{\"idToken\":\"{newIdToken}\"}}";
        RestClient.Post($"https://www.googleapis.com/identitytoolkit/v3/relyingparty/getAccountInfo?key={apiKey}",
            payLoad).Then(
            response =>
            {
                var responseJson = response.Text;

                // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
                // to serialize more complex types (UserData, in this case)
                var data = fsJsonParser.Parse(responseJson);
                object deserialized = null;
                serializer.TryDeserialize(data, typeof(UserData), ref deserialized);

                var authResponse = deserialized as UserData;

                if (authResponse.users[0].emailVerified)
                {
                    userId = authResponse.users[0].localId;
                    idToken = newIdToken;
                    callback();
                }
                else
                {
                    fallback();
                }
            });
    }
    public static void GetOnlineFriends()
    {
        DatabaseHandler.GetUsers("UserOnline", users =>
        { 
            var Users = users;
            Debug.Log("task Completed");
        });
    }
        public static void CreateCustomRoom(string roomname, int maxplayer, string userId)
        {
            
        }
    }