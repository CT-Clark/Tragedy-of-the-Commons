using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputFieldManager : MonoBehaviour
{
    public SimManager simScript;

    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<InputField>().onEndEdit.AddListener(ChangeValues);
    }

    private void ChangeValues(string textInField)
    {
    }
}
