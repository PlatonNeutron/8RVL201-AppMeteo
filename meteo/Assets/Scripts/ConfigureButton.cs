using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using TMPro;
using System.Diagnostics;

public class ConfigureButton : MonoBehaviour
{
    // Parameters of the button instance
    public string label;
    public Material buttonMaterial;
    public Material bottomButtonMaterial;

    // Start is called before the first frame update
    void Start()
    {
        // Setting every parameters of the button to the values specified in the inspector
        TextMeshProUGUI textMesh = GetComponentInChildren<TextMeshProUGUI>();
        textMesh.text = label;

        MeshRenderer buttonMeshRenderer = transform.Find("Button").GetComponent<MeshRenderer>();
        buttonMeshRenderer.material = buttonMaterial;

        MeshRenderer bottomButtonMeshRenderer = transform.Find("Bottom").GetComponent<MeshRenderer>();
        bottomButtonMeshRenderer.material = bottomButtonMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
