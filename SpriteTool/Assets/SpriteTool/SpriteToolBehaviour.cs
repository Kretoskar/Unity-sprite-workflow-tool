using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace SpriteTool
{
    public static class SpriteToolBehaviour
    {
        private static readonly string EMISSION_MAP_SUFFIX = "_e";
        private static readonly string EMISSION_SECONDARY_TEXTURE_NAME = "_Emission";
        private static readonly string NORMAL_MAP_SUFFIX = "_n";
        private static readonly string NORMAL_MAP_SECONDARY_TEXTURE_NAME= "_NormalMap";
        
        public static void SetSecondaryTextures(Texture2D inputTex)
        {
            string assetPath = AssetDatabase.GetAssetPath(inputTex);
            
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            
            SecondarySpriteTexture[] secondarySpriteTextures = new SecondarySpriteTexture[]
            {
                new SecondarySpriteTexture
                {
                    name = EMISSION_SECONDARY_TEXTURE_NAME,
                    texture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(GetGuid(assetPath, EMISSION_MAP_SUFFIX)))
                },
                new SecondarySpriteTexture
                {
                    name = NORMAL_MAP_SECONDARY_TEXTURE_NAME,
                    texture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(GetGuid(assetPath, NORMAL_MAP_SUFFIX)))
                }
            };

            importer.secondarySpriteTextures = secondarySpriteTextures;
            
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }

        private static string GetGuid(string assetPath, string suffix)
        {
            string textureName = Path.GetFileNameWithoutExtension(assetPath) + suffix;
            string textureType = " t:Texture2D";
            List<string> searchFolders = AssetDatabase.GetSubFolders(Path.GetDirectoryName(assetPath)).ToList();
            searchFolders.Add(Path.GetDirectoryName(assetPath));
            string[] GUIDs = AssetDatabase.FindAssets(textureName + textureType, searchFolders.ToArray());

            if (GUIDs == null || GUIDs.Length == 0)
            {
                Debug.LogWarning(textureName + " not found");
                return null;
            }

            return GUIDs[0];
        }
    }
}