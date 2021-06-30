using System;

/// <summary>
/// The user class, which gets uploaded to the Firebase Database
/// </summary>

[Serializable] // This makes the class able to be serialized into a JSON
public class Room2
{
    public string RoomName;
    public int MaxPlayers;


    public Room2(string roomname, int maxplayer)
    {
        this.RoomName = roomname;
        this.MaxPlayers = maxplayer;
    }
}

