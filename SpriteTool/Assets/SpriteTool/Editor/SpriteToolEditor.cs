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
        private bool pixelArt;
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
            texture = (Texture2D)EditorGUI.ObjectField(new Rect(6, 6, 200, 200),
                "Add a Texture:",
                texture,
                typeof(Texture2D));

            GUILayout.Space(60);

            pixelArt = EditorGUILayout.Toggle("Pixel Art", pixelArt);
            slice = EditorGUILayout.Toggle("Slice", slice);
            if (slice)
            {
                sliceWidth = EditorGUILayout.IntField("Width", sliceWidth);
                sliceHeight = EditorGUILayout.IntField("Height", sliceHeight);
            }
            
            if (GUILayout.Button("Process"))
            {
                SpriteToolBehaviour.SetSecondaryTextures(texture);

                if (pixelArt)
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