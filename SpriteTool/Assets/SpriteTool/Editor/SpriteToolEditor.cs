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
            Texture2D LogoTex = (Texture2D)Resources.Load("spriteTool"); //don't put png
            GUILayout.Label(LogoTex);

            texture = (Texture2D)EditorGUI.ObjectField(new Rect(6, 106, 200, 200),
                "Add a Texture:",
                texture,
                typeof(Texture2D));

            GUILayout.Space(60);

            if(texture == null) return;

            importSettingsPreset =
                (ImportSettingsPreset) EditorGUILayout.EnumPopup("Import settings", importSettingsPreset);
            slice = EditorGUILayout.Toggle("Slice", slice);
            if (slice)
            {
                sliceWidth = EditorGUILayout.IntField("Width", sliceWidth);
                sliceHeight = EditorGUILayout.IntField("Height", sliceHeight);
            }
            
            if (GUILayout.Button("Process"))
            {
                SpriteToolBehaviour.SetSecondaryTextures(texture);

                if (importSettingsPreset == ImportSettingsPreset.PixelArt)
                {
                    SpriteToolBehaviour.SetToPixelArt(texture);
                }

                if (slice)
                {
                    SpriteToolBehaviour.SliceWithSecondaryTextures(texture, sliceWidth, sliceHeight);
                }
            }
        }
    }
}