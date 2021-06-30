//Create a new Dropdown GameObject by going to the Hierarchy and clicking Create>UI>Dropdown. Attach this script to the Dropdown GameObject.
//Set your own Text in the Inspector window

using UnityEngine;
using UnityEngine.UI;

public class dropdownscript : MonoBehaviour
{
    //Attach this script to a Dropdown GameObject
    Dropdown m_Dropdown;
    int m_DropdownIndex;
    int m_DropdownValue;

    void Start()
    {
        m_Dropdown = GetComponent<Dropdown>();
    }

    void Update()
    {
        m_DropdownIndex = m_Dropdown.value;
    }
    public int selectedoption()
    {
        int.TryParse(m_Dropdown.options[m_DropdownIndex].text, out m_DropdownValue);
        return m_DropdownValue;
    }
}