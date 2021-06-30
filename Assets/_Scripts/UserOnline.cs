using System;

/// <summary>
/// The user class, which gets uploaded to the Firebase Database
/// </summary>

[Serializable] // This makes the class able to be serialized into a JSON
public class UserOnline
{
    public string OnlineStatus;
    public string UserTag;

    public UserOnline(string status, string NameTag)
    {
        this.OnlineStatus = status;
        this.UserTag = NameTag;
    }
}
