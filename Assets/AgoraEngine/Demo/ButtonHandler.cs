using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{

    /// <summary>
    ///   React to a button click event.  Used in the UI Button action definition.
    /// </summary>
    /// <param name="button"></param>
    public void Start()
    {
        Debug.Log("Agora Buttom Clicked");
        // which GameObject?
        GameObject go = GameObject.Find("AgoraController");
        if (go != null)
        {
            TestHome gameController = go.GetComponent<TestHome>();
            if (gameController == null)
            {
                Debug.LogError("Missing game controller...");
                return;
            }
            Debug.Log("Agora Join Buttom Clicked");
            gameController.onJoinButtonClicked();
            /*else if (button.name == "LeaveButton")
            {
                gameController.onLeaveButtonClicked();
            }*/
        }
    }
}
