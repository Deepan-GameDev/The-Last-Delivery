using UnityEngine;
using UnityEditor;
using System.IO;

public class TerrainColorTextureCreator
{
    [MenuItem("Tools/Terrain/Create Color Textures")]
    public static void CreateColorTextures()
    {
        string folder = "Assets/TerrainColors";

        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder("Assets", "TerrainColors");
        }

        CreateTexture("Grass", new Color(0.18f, 0.45f, 0.08f), folder);
        CreateTexture("Dirt", new Color(0.35f, 0.20f, 0.08f), folder);
        CreateTexture("Rock", new Color(0.35f, 0.35f, 0.35f), folder);
        CreateTexture("Sand", new Color(0.75f, 0.60f, 0.32f), folder);

        AssetDatabase.Refresh();

        Debug.Log("Terrain textures created successfully!");
    }

    static void CreateTexture(string name, Color color, string folder)
    {
        int size = 256;

        Texture2D texture = new Texture2D(
            size,
            size,
            TextureFormat.RGBA32,
            true
        );

        Color[] pixels = new Color[size * size];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }

        texture.SetPixels(pixels);
        texture.Apply(true, false);

        byte[] pngData = texture.EncodeToPNG();

        string path = folder + "/" + name + ".png";

        File.WriteAllBytes(path, pngData);

        Object.DestroyImmediate(texture);
    }
}