using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using UnityEngine.XR.ARSubsystems;
//using UnityEditor.XR.ARSubsystems;

public class MakeImageLibrary
{
    /*
    [MenuItem("Assets/Create/AR Image Library")]
    public static void CreateMyAsset()
    {
        XRReferenceImageLibrary asset = ScriptableObject.CreateInstance<XRReferenceImageLibrary>();

        string name = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/ReferenceImageLibrary.asset");
        AssetDatabase.CreateAsset(asset, name);


        Texture2D[] textures = Resources.LoadAll<Texture2D>("Targets");
        int i = 0;
        float w = 0.1f;
        foreach (var t in textures)
        {
            float ratio = (w / (float)t.width);
            float h = t.height * ratio;

            XRReferenceImageLibraryExtensions.Add(asset);
            XRReferenceImageLibraryExtensions.SetName(asset, i, t.name);
            XRReferenceImageLibraryExtensions.SetSpecifySize(asset, i, true);
            XRReferenceImageLibraryExtensions.SetSize(asset, i, new Vector2(w, h));
            XRReferenceImageLibraryExtensions.SetTexture(asset, i, t, false);
            i++;
        }
        AssetDatabase.SaveAssets();
        Debug.Log(i + " images added");
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
    */
}
