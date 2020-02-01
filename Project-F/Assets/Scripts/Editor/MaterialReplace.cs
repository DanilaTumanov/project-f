using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MaterialReplace : EditorWindow
{
    /*string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;*/

    private Transform _root;
    private Material _replaceMat;
    private Material _newMat;
    
    
    // Add menu item named "My Window" to the Window menu
    [MenuItem("Tools/MaterialReplace")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(MaterialReplace));
    }
    
    void OnGUI()
    {
        /*GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField ("Text Field", myString);
        
        groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle ("Toggle", myBool);
        myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup ();*/
        
        
        GUILayout.Label ("Replace material for all child objects of root (include root object)", EditorStyles.boldLabel);
        
        _root = EditorGUILayout.ObjectField("Root object for search", _root, typeof(Transform), true) as Transform;
        
        _replaceMat = EditorGUILayout.ObjectField("Material for replace", _root, typeof(Material), true) as Material;
        
        _newMat = EditorGUILayout.ObjectField("New material", _root, typeof(Material), true) as Material;

        if (GUILayout.Button("Replace"))
        {
            ReplaceMat(_root, _replaceMat, _newMat);
        }
    }


    private void ReplaceMat(Transform root, Material replaceMat, Material newMat)
    {
        var meshRenderer = root.GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.sharedMaterial == replaceMat)
        {
            meshRenderer.sharedMaterial = newMat;
        }

        foreach (Transform child in _root)
        {
            ReplaceMat(child, replaceMat, newMat);
        }
    }
}