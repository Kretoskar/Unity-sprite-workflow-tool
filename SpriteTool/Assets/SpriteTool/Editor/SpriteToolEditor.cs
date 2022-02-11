using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SpriteTool
{
    public class SpriteToolEditor : EditorWindow
    {
        private Texture2D texture;
        private ImportSettingsPreset importSettingsPreset;
        private bool slice;
        private int sliceWidth = 128;
        private int sliceHeight = 128;

        private FilterMode filterMode;
        private TextureImporterCompression compression;
        private string path;
        private SpriteToolMode mode;

        [MenuItem("Window/Sprite Tool")]
        public static void ShowWindow()
        {
            GetWindow<SpriteToolEditor>("Sprite tool");
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            Texture2D LogoTex = (Texture2D) Resources.Load("spriteTool");
            GUILayout.Label(LogoTex);

            mode = (SpriteToolMode) EditorGUILayout.EnumPopup("Mode: ", mode);

            if (mode == SpriteToolMode.Single)
            {
                texture = (Texture2D) EditorGUI.ObjectField(new Rect(2, 130, 200, 200),
                    "Texture:",
                    texture,
                    typeof(Texture2D));

                GUILayout.Space(60);
            }
            else
            {
                path = EditorGUILayout.TextField("Path: ", path);
            }
            
            GUILayout.Space(10);

            importSettingsPreset =
                (ImportSettingsPreset) EditorGUILayout.EnumPopup("Import settings", importSettingsPreset);

            if (importSettingsPreset == ImportSettingsPreset.Custom)
            {
                filterMode = (FilterMode) EditorGUILayout.EnumPopup("Filter mode", filterMode);
                compression = (TextureImporterCompression) EditorGUILayout.EnumPopup("Compression", compression);
            }
            
            GUILayout.Space(10);


            slice = EditorGUILayout.Toggle("Slice", slice);

            if (slice)
            {
                sliceWidth = EditorGUILayout.IntField("Width", sliceWidth);
                sliceHeight = EditorGUILayout.IntField("Height", sliceHeight);
            }

            GUILayout.Space(20);
            
            if (GUILayout.Button("Process"))
            {
                if (mode == SpriteToolMode.Single) ProcessSingleSprite();
                else ProcessMultipleSprites();
            }
        }

        private void ProcessSingleSprite()
        {
            SpriteToolBehaviour.SetSecondaryTextures(texture);

            if (importSettingsPreset == ImportSettingsPreset.PixelArt)
            {
                SpriteToolBehaviour.SetToPixelArt(texture);
            }
            else if (importSettingsPreset == ImportSettingsPreset.Custom)
            {
                SpriteToolBehaviour.SetImportSettings(texture, filterMode, compression);
            }

            if (slice)
            {
                SpriteToolBehaviour.SliceWithSecondaryTextures(texture, sliceWidth, sliceHeight);
            }
        }

        private void ProcessMultipleSprites()
        {
            foreach (var tex in GetAtPath<Texture2D>(path))
            {
                Debug.Log(tex.name);
                texture = tex;
                ProcessSingleSprite();
            }
        }

        private T[] GetAtPath<T>(string path)
        {
            ArrayList al = new ArrayList();
            string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

            foreach (string fileName in fileEntries)
            {
                string temp = fileName.Replace("\\", "/");
                int index = temp.LastIndexOf("/");
                string localPath = "Assets/" + path;

                if (index > 0)
                    localPath += temp.Substring(index);

                System.Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

                if (t != null)
                    al.Add(t);
            }

            T[] result = new T[al.Count];

            for (int i = 0; i < al.Count; i++)
                result[i] = (T) al[i];

            return result;
        }
    }

    public enum SpriteToolMode
    {
        Single,
        Multiple
    }
}