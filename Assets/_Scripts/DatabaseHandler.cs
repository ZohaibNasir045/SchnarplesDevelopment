using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;

public static class DatabaseHandler
{
    private const string projectId = "CardGameWEB2"; // You can find this in your Firebase project settings
    private static readonly string databaseURL = $"https://cardgameweb2-default-rtdb.firebaseio.com/cardgame/";
    
    private static fsSerializer serializer = new fsSerializer();

    public delegate void PostUserCallback();
    public delegate void MakeUserOnlineCallback();
    public delegate void MakeUserOnlineFallback();
    public delegate void GetUserCallback(User user);
    public delegate void GetUsersCallback(Dictionary<string, UserOnline> users);


    /// <summary>
    /// Adds a user to the Firebase Database
    /// </summary>
    /// <param name="user"> User object that will be uploaded </param>
    /// <param name="userId"> Id of the user that will be uploaded </param>
    /// <param name="callback"> What to do after the user is uploaded successfully </param>
    /// <param name="idToken"> Token which authenticates the request </param>
    public static void PostUser(User user, string userId, PostUserCallback callback, string idToken)
    {
        RestClient.Put<User>($"{databaseURL}users/{userId}.json?auth={idToken}", user).Then(response => { callback(); });
    }
    /// <summary>
    /// Adds a user to the Firebase Database
    /// </summary>
    /// <param name="user"> User object that will be uploaded </param>
    /// <param name="userId"> Id of the user that will be uploaded </param>
    /// <param name="callback"> What to do after the user is uploaded successfully </param>
    /// <param name="idToken"> Token which authenticates the request </param>
    public static void AddUserToRoom(User user, string userId,string roomname, PostUserCallback callback, string idToken)
    {
        RestClient.Put<User>($"{databaseURL}rooms/{roomname}/PlayersJoined/{userId}.json?auth={AuthHandler.idToken}", new User( PlayerPrefs.GetString("User_Name"), PlayerPrefs.GetString("User_Name"), int.Parse(PlayerPrefs.GetString("User_Age")))).Then(response => { callback(); });
    }
    /// <summary>
    /// Adds a user to the Firebase Database
    /// </summary>
    /// <param name="userId"> Id of the user that will be uploaded </param>
    /// <param name="callback"> What to do after the user is uploaded successfully </param>
    /// <param name="fallback"> What to do after the user is not uploaded successfully </param>
    /// <param name="idToken"> Token which authenticates the request </param>
    public static void MakeUserOnline(UserOnline userOnline, string userId, string UserName, MakeUserOnlineCallback callback, MakeUserOnlineFallback fallback, string idToken)
    {
        //Make user status online
        //Database->Online User->UserId
        RestClient.Put<UserOnline>($"{databaseURL}UserOnline/{userId}.json?auth={idToken}", new UserOnline("online", UserName)).Then(response => { callback(); });
    }
    /// <summary>
    /// Make a custom room
    /// </summary>
    /// <param name="userId"> Id of the user that will be uploaded </param>
    /// <param name="roomname"> room name </param>
    /// <param name="maxplayer"> max players </param>aw
    /// <param name="callback"> What to do after the user is uploaded successfully </param>
    /// <param name="fallback"> What to do after the user is not uploaded successfully </param>
    public static void CreateCustomRoom(string roomname, int maxplayer, string userId, MakeUserOnlineCallback callback, MakeUserOnlineFallback fallback)
    {
        //Make a custom room
        //Database->Room->Info
        RestClient.Put<RoomInfo2>($"{databaseURL}rooms/{roomname}/info.json?auth={AuthHandler.idToken}", new RoomInfo2(maxplayer)).Then(response => { callback(); });
    }
    /// <summary>
    /// Retrieves a user from the Firebase Database, given their id
    /// </summary>
    /// <param name="userId"> Id of the user that we are looking for </param>
    /// <param name="callback"> What to do after the user is downloaded successfully </param>
    public static void GetUser(string userId, GetUserCallback callback)
    {
        RestClient.Get<User>($"{databaseURL}users/{userId}.json?auth={AuthHandler.idToken}").Then(userdata => { callback(userdata); });
    }

    /// <summary>
    /// Gets all users from the Firebase Database
    /// </summary>
    /// <param name="callback"> What to do after all users are downloaded successfully </param>
    public static void GetUsers(string Link,GetUsersCallback callback)
    {
        RestClient.Get($"{databaseURL}UserOnline.json?auth={AuthHandler.idToken}").Then(response =>
        {
            var responseJson = response.Text;

            // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
            // to serialize more complex types (a Dictionary, in this case)
            var data = fsJsonParser.Parse(responseJson);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, UserOnline>), ref deserialized);

            var users = deserialized as Dictionary<string, UserOnline>;
            
            callback(users);
        });
    }
}
