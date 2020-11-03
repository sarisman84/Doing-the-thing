using System;
using UnityEngine;
using UnityEditor;

namespace Editor
{
    public class SpriteProcessor : AssetPostprocessor
    {
        private void OnPostprocessTexture(Texture2D texture)
        {
            string lowerCaseAssetPath = assetPath.ToLower();
            bool isInSpritesDirectory = !lowerCaseAssetPath.IndexOf("/ui/", StringComparison.Ordinal).Equals(-1);

            if (isInSpritesDirectory)
            {
                TextureImporter importer = (TextureImporter)assetImporter;
                importer.textureType = TextureImporterType.Sprite;
            }
        }
    }
}
