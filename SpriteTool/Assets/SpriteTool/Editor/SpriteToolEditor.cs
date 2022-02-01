using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpriteTool
{
    public class SpriteToolEditor : EditorWindow
    {
        Texture2D texture;
        
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
            
            if (GUILayout.Button("Process"))
            {
                SpriteToolBehaviour.SetSecondaryTextures(texture);
            }
        }
    }
}