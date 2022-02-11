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

        public static void SliceWithSecondaryTextures(Texture2D inputTex, int sliceWidth, int sliceHeight)
        {
            if(inputTex == null) return;
            
            string path = AssetDatabase.GetAssetPath(inputTex);

            TextureImporter importer =
                    (TextureImporter) TextureImporter.GetAtPath(path);

            importer.isReadable = true;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = sliceWidth;

            List<SpriteMetaData> newData = new List<SpriteMetaData>();

            for (int i = 0; i < inputTex.width; i += sliceWidth)
            {
                for (int j = inputTex.height; j > 0; j -= sliceHeight)
                {
                    SpriteMetaData smd = new SpriteMetaData();
                    smd.pivot = new Vector2(0.5f, 0.5f);
                    smd.alignment = 9;
                    smd.name = (inputTex.height - j) / sliceHeight + " , " + i / sliceWidth;
                    smd.rect = new Rect(i, j - sliceHeight, sliceWidth, sliceHeight);
                    
                    newData.Add(smd);
                }
            }

            foreach (var secondarySpriteTexture in importer.secondarySpriteTextures)
            {
                Texture2D sec = secondarySpriteTexture.texture;
                SliceWithSecondaryTextures(sec, sliceWidth, sliceHeight);
            }

            
            importer.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        public static void SetToPixelArt(Texture2D inputTex)
        {
            SetImportSettings(inputTex, FilterMode.Point, TextureImporterCompression.Uncompressed);
        }

        public static void SetImportSettings(Texture2D inputTex, FilterMode filterMode, TextureImporterCompression compression)
        {
            if(inputTex == null) return;
            
            TextureImporter importer =
                (TextureImporter) TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(inputTex));

            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            int biggerBorder = Mathf.Max(inputTex.width, inputTex.height);
            int power = 1;
            while (power < biggerBorder)
            {
                power *= 2;
            }

            if (power > 16394)
                power = 16394;
            
            importer.maxTextureSize = power;

            foreach (var secondarySpriteTexture in importer.secondarySpriteTextures)
            {
                Texture2D sec = secondarySpriteTexture.texture;
                SetImportSettings(sec, filterMode, compression);
            }
            
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }
        
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
    
    public enum ImportSettingsPreset
    {
        PixelArt, 
        Custom
    }
}