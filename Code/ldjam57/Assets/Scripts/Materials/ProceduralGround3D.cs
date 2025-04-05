using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ProceduralGround3D : MonoBehaviour
{
    [System.Serializable]
    public class MaterialLayer
    {
        public string name;
        [Range(0f, 1f)]
        public float amount = 0.2f;
        public Color color = Color.white;
        public Texture2D texture;
        [Range(0.1f, 10f)]
        public float textureScale = 1f;
    }

    [Header("Ground Settings")]
    public int textureResolution = 512;
    public float noiseScale = 10f;
    public int seed = 0;

    [Header("Material Settings")]
    public Material baseMaterial; // Reference to existing material

    [Header("Material Layers")]
    public List<MaterialLayer> materialLayers = new List<MaterialLayer>();

    [Header("References")]
    public MeshRenderer meshRenderer;

    private Texture2D groundTexture;
    private Texture2D normalMap;
    private Texture2D metallicMap;

    private void OnValidate()
    {
        // Auto-assign renderer if not set
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        if (materialLayers.Count == 0)
        {
            // Add default materials if none exist
            materialLayers.Add(new MaterialLayer { name = "Copper", amount = 0.03f, color = new Color(0.85f, 0.5f, 0.2f) });
            materialLayers.Add(new MaterialLayer { name = "Gold", amount = 0.07f, color = new Color(1f, 0.85f, 0.1f) });
            materialLayers.Add(new MaterialLayer { name = "Iron", amount = 0.1f, color = new Color(0.6f, 0.6f, 0.6f) });
            materialLayers.Add(new MaterialLayer { name = "Lithium", amount = 0.02f, color = new Color(1.0f, 0.58f, 0.88f) });
            materialLayers.Add(new MaterialLayer { name = "Crystals", amount = 0.03f, color = new Color(1.0f, 0.0f, 0.9f) });
            materialLayers.Add(new MaterialLayer { name = "Stone", amount = 0.3f, color = new Color(0.6f, 0.6f, 0.6f) });
            materialLayers.Add(new MaterialLayer { name = "Dirt", amount = 0.6f, color = new Color(0.5f, 0.3f, 0.1f) });
        }

        // Normalize amounts if they exceed 1.0 in total
        NormalizeAmounts();
    }

    private void NormalizeAmounts()
    {
        float total = 0f;
        foreach (var layer in materialLayers)
        {
            total += layer.amount;
        }

        if (total > 1.0f)
        {
            float scale = 1.0f / total;
            foreach (var layer in materialLayers)
            {
                layer.amount *= scale;
            }
        }
    }

    [ContextMenu("Generate Ground")]
    public void GenerateGround()
    {
        if (textureResolution <= 0 || materialLayers.Count == 0 || meshRenderer == null)
            return;

        // Create the base material instance if needed
        if (baseMaterial == null)
        {
            Debug.LogWarning("No base material assigned. Please assign a base material in the inspector.");
            return;
        }

        // Make sure we have an instance of the material
        if (meshRenderer.sharedMaterial == null || meshRenderer.sharedMaterial == baseMaterial)
        {
            meshRenderer.sharedMaterial = new Material(baseMaterial);
            meshRenderer.sharedMaterial.name = "ProceduralGroundMaterial";
        }

        // Create or resize the textures
        if (groundTexture == null || groundTexture.width != textureResolution)
        {
            groundTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);
            groundTexture.filterMode = FilterMode.Bilinear;
            groundTexture.wrapMode = TextureWrapMode.Repeat;
        }

        if (normalMap == null || normalMap.width != textureResolution)
        {
            normalMap = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);
            normalMap.filterMode = FilterMode.Bilinear;
            normalMap.wrapMode = TextureWrapMode.Repeat;
        }

        if (metallicMap == null || metallicMap.width != textureResolution)
        {
            metallicMap = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);
            metallicMap.filterMode = FilterMode.Bilinear;
            metallicMap.wrapMode = TextureWrapMode.Repeat;
        }

        // Set random seed for consistent generation
        Random.InitState(seed);

        // Generate the texture
        Color[] pixels = new Color[textureResolution * textureResolution];
        Color[] normalPixels = new Color[textureResolution * textureResolution];
        Color[] metallicPixels = new Color[textureResolution * textureResolution];

        // Create noise fields for each material
        float[,] noiseFields = new float[materialLayers.Count, textureResolution * textureResolution];

        // Generate noise values for each material
        for (int m = 0; m < materialLayers.Count; m++)
        {
            float offsetX = Random.Range(0f, 1000f);
            float offsetY = Random.Range(0f, 1000f);

            for (int y = 0; y < textureResolution; y++)
            {
                for (int x = 0; x < textureResolution; x++)
                {
                    float noiseValue = Mathf.PerlinNoise(
                        (x + offsetX) / noiseScale,
                        (y + offsetY) / noiseScale
                    );

                    // Add some variation based on material index
                    noiseValue = Mathf.Pow(noiseValue, 1 + m * 0.5f);

                    noiseFields[m, y * textureResolution + x] = noiseValue;
                }
            }
        }

        // Calculate material distribution
        float[] thresholds = new float[materialLayers.Count];
        float sum = 0;
        for (int i = 0; i < materialLayers.Count; i++)
        {
            sum += materialLayers[i].amount;
            thresholds[i] = sum;
        }

        // Assign materials to pixels based on noise and thresholds
        for (int y = 0; y < textureResolution; y++)
        {
            for (int x = 0; x < textureResolution; x++)
            {
                int index = y * textureResolution + x;

                // Determine which material to use
                float materialSelector = Random.value;
                int materialIndex = 0;

                for (int i = 0; i < thresholds.Length; i++)
                {
                    if (materialSelector <= thresholds[i])
                    {
                        materialIndex = i;
                        break;
                    }
                }

                // Get base color for this material
                Color pixelColor = materialLayers[materialIndex].color;

                // Default metallic values (high for metals, low for non-metals)
                float metallicValue = 0.0f;
                float smoothnessValue = 0.2f;

                // Set metallic values based on material type
                if (materialLayers[materialIndex].name.ToLower().Contains("gold") ||
                    materialLayers[materialIndex].name.ToLower().Contains("copper") ||
                    materialLayers[materialIndex].name.ToLower().Contains("metal"))
                {
                    metallicValue = 0.8f + Random.Range(-0.1f, 0.1f);
                    smoothnessValue = 0.7f + Random.Range(-0.1f, 0.1f);
                }

                // Apply texture if available
                if (materialLayers[materialIndex].texture != null)
                {
                    Texture2D materialTexture = materialLayers[materialIndex].texture;
                    float scale = materialLayers[materialIndex].textureScale;

                    int tx = Mathf.FloorToInt((x / scale) % materialTexture.width);
                    int ty = Mathf.FloorToInt((y / scale) % materialTexture.height);

                    Color textureColor = materialTexture.GetPixel(tx, ty);
                    pixelColor = Color.Lerp(pixelColor, textureColor, 0.7f);
                }

                // Add variation based on noise
                float noiseValue = noiseFields[materialIndex, index];
                pixelColor = Color.Lerp(
                    pixelColor,
                    pixelColor * (0.7f + 0.3f * noiseValue),
                    0.6f
                );

                pixels[index] = pixelColor;

                // Generate normal map data
                float nX = Mathf.PerlinNoise((x + 32.14f) / (noiseScale * 0.5f), y / (noiseScale * 0.5f)) * 2 - 1;
                float nY = Mathf.PerlinNoise(x / (noiseScale * 0.5f), (y + 57.29f) / (noiseScale * 0.5f)) * 2 - 1;
                float nZ = 1;

                // Make normals stronger/weaker based on material type
                float normalStrength = materialLayers[materialIndex].name.ToLower().Contains("stone") ? 0.7f : 0.4f;
                nX *= normalStrength;
                nY *= normalStrength;

                // Normalize the normal vector
                float normalLength = Mathf.Sqrt(nX * nX + nY * nY + nZ * nZ);
                nX /= normalLength;
                nY /= normalLength;
                nZ /= normalLength;

                // Convert normal to color (x is red, y is green, z is blue)
                Color normalColor = new Color((nX + 1) * 0.5f, (nY + 1) * 0.5f, (nZ + 1) * 0.5f, 1);
                normalPixels[index] = normalColor;

                // Set metallic/smoothness map pixel
                metallicPixels[index] = new Color(metallicValue, 0, 0, smoothnessValue); // R = Metallic, A = Smoothness
            }
        }

        // Apply pixels to textures
        groundTexture.SetPixels(pixels);
        groundTexture.Apply();

        normalMap.SetPixels(normalPixels);
        normalMap.Apply();

        metallicMap.SetPixels(metallicPixels);
        metallicMap.Apply();

        // Apply textures to material
        Material mat = meshRenderer.sharedMaterial;

        // These property names work for both Standard shader and URP/HDRP lit shaders
        mat.SetTexture("_MainTex", groundTexture);           // Standard shader
        mat.SetTexture("_BaseMap", groundTexture);           // URP/HDRP shader

        mat.SetTexture("_BumpMap", normalMap);               // Standard shader
        mat.SetTexture("_NormalMap", normalMap);             // URP/HDRP shader

        mat.SetTexture("_MetallicGlossMap", metallicMap);    // Standard shader
        mat.SetTexture("_MaskMap", metallicMap);             // URP/HDRP shader

        // Enable keywords for Standard shader
        mat.EnableKeyword("_METALLICGLOSSMAP");
        mat.EnableKeyword("_NORMALMAP");
    }
}