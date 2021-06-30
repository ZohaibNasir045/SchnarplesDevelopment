using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputReset : MonoBehaviour
{
    public InputField mainInputField;
    public Text TextField;

    public void Start()
    {
        //Adds a listener to the main input field and invokes a method when the value changes.
        mainInputField.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        TextField.text = "";
    }

    // Invoked when the value of the text field changes.
    public void ValueChangeCheck()
    {
        TextField.text = "";
    }
}
