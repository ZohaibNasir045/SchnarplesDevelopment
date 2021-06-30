using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
public class LobbyManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Text roomname;
    public int maxplayer;
    public Dropdown dropdown;

    public static string UserID;
   
    public GameObject OnlineRowPrefab;
    void Awake()
    {
        UserIsOnline();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UserIsOnline()
    {
        Debug.Log("Setting user online");
        UserID = PlayerPrefs.GetString("User_ID");
        
    }
    public void HostAndPlay()
    {
        PlayerPrefs.SetString("Host_is_Playing","true");
    }
    public void JustHost()
    {
        PlayerPrefs.SetString("Host_is_Playing", "false");
    }
    public void CreateRoom()
    {
        maxplayer = dropdown.GetComponent<dropdownscript>().selectedoption();
        Debug.Log("max player is: " + maxplayer);
        DatabaseHandler.CreateCustomRoom(roomname.text,maxplayer,UserID,()=> { },()=> { });
        //Insert myseld to the room I made
        DatabaseHandler.GetUser(UserID, userdata => {

        });
    }
    dropdownscript dropdownscript;
    /*public void ShowOnlineUsers()
    {
        DatabaseHandler.GetUsers("users", users =>
        {
            foreach (KeyValuePair<string, User> user in users)
            {
                if (user.Key == PlayerPrefs.GetString("User_ID"))
                    Debug.Log("Ofcourse i am online");
                else
                {
                    
                    //get user data
                    DatabaseHandler.GetUser(user.Key, userdata => {
                        //populate users
                        Debug.Log("User ID: " + user.Key + " User name: " + userdata.name);
                        GameObject go = Instantiate(OnlineRowPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                        go.transform.SetParent(GameObject.Find("Content").transform);
                        GameObject FriendName = go.transform.Find("FriendName").gameObject;

                        FriendName.GetComponent<Text>().text = userdata.name;
                    });
                    
                }
            }
        });
    }*/
}
