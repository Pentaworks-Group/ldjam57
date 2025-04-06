using System;

using Assets.Scripts.Core.Model;

using GameFrame.Core.Extensions;

using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class GroundBehaviour : TileBehaviour, IClickable
    {
        private static readonly Lazy<System.Random> lazyZeroRandom = new Lazy<System.Random>(new System.Random(0));

        private const int textureResolution = 32;
        public float noiseScale = 10f;

        private Tile tile;

        private Boolean isGenerated = false;

        private Texture2D groundTexture;
        private Texture2D normalMap;
        private Texture2D metallicMap;

        public void Init(WorldBehaviour worldBehaviour, Tile tile)
        {
            base.Init(worldBehaviour);

            this.tile = tile;

            if (!isGenerated)
            {
                isGenerated = true;

                if (TryGetComponent<MeshRenderer>(out var meshRenderer))
                {
                    meshRenderer.material = GenerateMaterial(meshRenderer);
                }
            }
        }

        public bool UpdateProgress(MiningTool miningTool)
        {
            float progress = (float)(miningTool.SpeedFactor / tile.SpeedFactor);

            tile.DigingProgress += progress;

            var finished = IsMined();

            if (finished)
            {
                worldBehaviour.ReplaceTile(this);
            }

            return finished;
        }

        public bool IsMined()
        {
            return tile.DigingProgress >= 1;
        }

        public override Point2 GetPosition()
        {
            return tile.Position;
        }

        private Material GenerateMaterial(MeshRenderer meshRenderer)
        {
            var material = default(Material);

            if (meshRenderer.sharedMaterial == null || meshRenderer.sharedMaterial == meshRenderer.material)
            {
                material = new Material(meshRenderer.material)
                {
                    name = "GeneratedMaterial"
                };
            }
            else
            {
                material = meshRenderer.sharedMaterial;
            }

            if (this.groundTexture == default || groundTexture.width != textureResolution)
            {
                groundTexture = InitializeTexture();
            }

            if (this.normalMap == default || normalMap.width != textureResolution)
            {
                normalMap = InitializeTexture();
            }

            if (this.metallicMap == default || metallicMap.width != textureResolution)
            {
                metallicMap = InitializeTexture();
            }

            var mineralAmounts = this.tile.MineralAmounts;
            // Generate the texture
            var pixels = new Color32[textureResolution * textureResolution];
            var normalPixels = new Color32[textureResolution * textureResolution];
            var metallicPixels = new Color32[textureResolution * textureResolution];

            // Create noise fields for each material
            float[,] noiseFields = new float[mineralAmounts.Count, textureResolution * textureResolution];

            // Generate noise values for each material
            for (int m = 0; m < mineralAmounts.Count; m++)
            {
                float offsetX = lazyZeroRandom.Value.NextSingle() * 1000f;
                float offsetY = lazyZeroRandom.Value.NextSingle() * 1000f;

                for (int y = 0; y < textureResolution; y++)
                {
                    for (int x = 0; x < textureResolution; x++)
                    {
                        var xValue = (x + offsetX) / noiseScale;
                        var yValue = (y + offsetY) / noiseScale;

                        float noiseValue = Mathf.PerlinNoise(xValue, yValue);

                        // Add some variation based on material index
                        noiseValue = Mathf.Pow(noiseValue, 1 + m * 0.5f);

                        noiseFields[m, y * textureResolution + x] = noiseValue;
                    }
                }
            }

            // Calculate material distribution
            double[] thresholds = new double[mineralAmounts.Count];

            double sum = 0;

            for (int i = 0; i < mineralAmounts.Count; i++)
            {
                sum += mineralAmounts[i].Amount;
                thresholds[i] = sum;
            }

            // Assign materials to pixels based on noise and thresholds
            for (int y = 0; y < textureResolution; y++)
            {
                for (int x = 0; x < textureResolution; x++)
                {
                    int index = y * textureResolution + x;

                    // Determine which material to use
                    float materialSelector = lazyZeroRandom.Value.NextSingle();

                    int materialIndex = 0;

                    for (int i = 0; i < thresholds.Length; i++)
                    {
                        if (materialSelector <= thresholds[i])
                        {
                            materialIndex = i;
                            break;
                        }
                    }

                    var mineralAmount = mineralAmounts[materialIndex];

                    // Get base color for this material
                    Color pixelColor = mineralAmounts[materialIndex].Mineral.Color.ToUnity();

                    // Default metallic values (high for metals, low for non-metals)
                    float metallicValue = 0.0f;
                    float smoothnessValue = 0.2f;

                    // Set metallic values based on material type
                    if (mineralAmount.Mineral.IsMetallic)
                    {
                        metallicValue = 0.8f + lazyZeroRandom.Value.NextSingleRange(-0.1f, 0.1f);
                        smoothnessValue = 0.7f + lazyZeroRandom.Value.NextSingleRange(-0.1f, 0.1f);
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
                    float normalStrength = mineralAmount.Mineral.Reference == "Stone" ? 0.7f : 0.4f;

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
            groundTexture.SetPixels32(pixels);
            groundTexture.Apply();

            normalMap.SetPixels32(normalPixels);
            normalMap.Apply();

            metallicMap.SetPixels32(metallicPixels);
            metallicMap.Apply();

            // These property names work for both Standard shader and URP/HDRP lit shaders
            material.SetTexture("_MainTex", groundTexture);           // Standard shader
            material.SetTexture("_BaseMap", groundTexture);           // URP/HDRP shader

            material.SetTexture("_BumpMap", normalMap);               // Standard shader
            material.SetTexture("_NormalMap", normalMap);             // URP/HDRP shader

            material.SetTexture("_MetallicGlossMap", metallicMap);    // Standard shader
            material.SetTexture("_MaskMap", metallicMap);             // URP/HDRP shader

            // Enable keywords for Standard shader
            material.EnableKeyword("_METALLICGLOSSMAP");
            material.EnableKeyword("_NORMALMAP");

            return material;
        }

        private Texture2D InitializeTexture()
        {
            return new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Repeat
            };
        }

        //private Color GenerateColor(Renderer meshRenderer)
        //{
        //    if (textureResolution <= 0 || tile.MineralAmounts.Count == 0 || meshRenderer == null)
        //    {
        //        return Color.white;
        //    }

        //    // Create the base material instance if needed
        //    if (baseMaterial == null)
        //    {
        //        Debug.LogWarning("No base material assigned. Please assign a base material in the inspector.");
        //        return Color.white;
        //    }

        //    // Make sure we have an instance of the material
        //    if (meshRenderer.sharedMaterial == null || meshRenderer.sharedMaterial == baseMaterial)
        //    {
        //        meshRenderer.sharedMaterial = new Material(baseMaterial)
        //        {
        //            name = "ProceduralGroundMaterial"
        //        };
        //    }

        //    // Create or resize the textures
        //    if (groundTexture == null || groundTexture.width != textureResolution)
        //    {
        //        groundTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false)
        //        {
        //            filterMode = FilterMode.Bilinear,
        //            wrapMode = TextureWrapMode.Repeat
        //        };
        //    }

        //    if (normalMap == null || normalMap.width != textureResolution)
        //    {
        //        normalMap = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false)
        //        {
        //            filterMode = FilterMode.Bilinear,
        //            wrapMode = TextureWrapMode.Repeat
        //        };
        //    }

        //    if (metallicMap == null || metallicMap.width != textureResolution)
        //    {
        //        metallicMap = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false)
        //        {
        //            filterMode = FilterMode.Bilinear,
        //            wrapMode = TextureWrapMode.Repeat
        //        };
        //    }

        //    // Set random seed for consistent generation
        //    var random = new System.Random(0);

        //    var mineralAmounts = this.tile.MineralAmounts;

        //    // Generate the texture
        //    Color[] pixels = new Color[textureResolution * textureResolution];
        //    Color[] normalPixels = new Color[textureResolution * textureResolution];
        //    Color[] metallicPixels = new Color[textureResolution * textureResolution];

        //    // Create noise fields for each material
        //    float[,] noiseFields = new float[mineralAmounts.Count, textureResolution * textureResolution];

        //    // Generate noise values for each material
        //    for (int m = 0; m < mineralAmounts.Count; m++)
        //    {
        //        float offsetX = random.NextSingle() * 1000f;
        //        float offsetY = random.NextSingle() * 1000f;

        //        for (int y = 0; y < textureResolution; y++)
        //        {
        //            for (int x = 0; x < textureResolution; x++)
        //            {
        //                var xValue = (x + offsetX) / noiseScale;
        //                var yValue = (y + offsetY) / noiseScale;

        //                float noiseValue = Mathf.PerlinNoise(xValue, yValue);

        //                // Add some variation based on material index
        //                noiseValue = Mathf.Pow(noiseValue, 1 + m * 0.5f);

        //                noiseFields[m, y * textureResolution + x] = noiseValue;
        //            }
        //        }
        //    }

        //    // Calculate material distribution
        //    double[] thresholds = new double[mineralAmounts.Count];

        //    double sum = 0;

        //    for (int i = 0; i < mineralAmounts.Count; i++)
        //    {
        //        sum += mineralAmounts[i].Amount;
        //        thresholds[i] = sum;
        //    }

        //    // Assign materials to pixels based on noise and thresholds
        //    for (int y = 0; y < textureResolution; y++)
        //    {
        //        for (int x = 0; x < textureResolution; x++)
        //        {
        //            int index = y * textureResolution + x;

        //            // Determine which material to use
        //            float materialSelector = random.NextSingle();

        //            int materialIndex = 0;

        //            for (int i = 0; i < thresholds.Length; i++)
        //            {
        //                if (materialSelector <= thresholds[i])
        //                {
        //                    materialIndex = i;
        //                    break;
        //                }
        //            }

        //            var mineralAmount = mineralAmounts[materialIndex];

        //            // Get base color for this material
        //            Color pixelColor = mineralAmounts[materialIndex].Mineral.Color.ToUnity();

        //            // Default metallic values (high for metals, low for non-metals)
        //            float metallicValue = 0.0f;
        //            float smoothnessValue = 0.2f;

        //            // Set metallic values based on material type
        //            if (mineralAmount.Mineral.IsMetallic)
        //            {
        //                metallicValue = 0.8f + random.NextSingleRange(-0.1f, 0.1f);
        //                smoothnessValue = 0.7f + random.NextSingleRange(-0.1f, 0.1f);
        //            }

        //            // Add variation based on noise
        //            float noiseValue = noiseFields[materialIndex, index];
        //            pixelColor = Color.Lerp(
        //                pixelColor,
        //                pixelColor * (0.7f + 0.3f * noiseValue),
        //                0.6f
        //            );

        //            pixels[index] = pixelColor;

        //            // Generate normal map data
        //            float nX = Mathf.PerlinNoise((x + 32.14f) / (noiseScale * 0.5f), y / (noiseScale * 0.5f)) * 2 - 1;
        //            float nY = Mathf.PerlinNoise(x / (noiseScale * 0.5f), (y + 57.29f) / (noiseScale * 0.5f)) * 2 - 1;
        //            float nZ = 1;

        //            // Make normals stronger/weaker based on material type
        //            float normalStrength = mineralAmount.Mineral.Reference == "Stone" ? 0.7f : 0.4f;

        //            nX *= normalStrength;
        //            nY *= normalStrength;

        //            // Normalize the normal vector
        //            float normalLength = Mathf.Sqrt(nX * nX + nY * nY + nZ * nZ);
        //            nX /= normalLength;
        //            nY /= normalLength;
        //            nZ /= normalLength;

        //            // Convert normal to color (x is red, y is green, z is blue)
        //            Color normalColor = new Color((nX + 1) * 0.5f, (nY + 1) * 0.5f, (nZ + 1) * 0.5f, 1);
        //            normalPixels[index] = normalColor;

        //            // Set metallic/smoothness map pixel
        //            metallicPixels[index] = new Color(metallicValue, 0, 0, smoothnessValue); // R = Metallic, A = Smoothness
        //        }
        //    }

        //    // Apply pixels to textures
        //    groundTexture.SetPixels(pixels);
        //    groundTexture.Apply();

        //    normalMap.SetPixels(normalPixels);
        //    normalMap.Apply();

        //    metallicMap.SetPixels(metallicPixels);
        //    metallicMap.Apply();

        //    // Apply textures to material
        //    Material mat = meshRenderer.sharedMaterial;

        //    // These property names work for both Standard shader and URP/HDRP lit shaders
        //    mat.SetTexture("_MainTex", groundTexture);           // Standard shader
        //    mat.SetTexture("_BaseMap", groundTexture);           // URP/HDRP shader

        //    mat.SetTexture("_BumpMap", normalMap);               // Standard shader
        //    mat.SetTexture("_NormalMap", normalMap);             // URP/HDRP shader

        //    mat.SetTexture("_MetallicGlossMap", metallicMap);    // Standard shader
        //    mat.SetTexture("_MaskMap", metallicMap);             // URP/HDRP shader

        //    // Enable keywords for Standard shader
        //    mat.EnableKeyword("_METALLICGLOSSMAP");
        //    mat.EnableKeyword("_NORMALMAP");

        //    return default;
        //}

        public void OnClicked()
        {
            var builder = new System.Text.StringBuilder();

            builder.AppendLine(String.Format("Tile: {0};{1}", this.tile.Position.X, this.tile.Position.Y));

            foreach (var mineralAmount in this.tile.MineralAmounts)
            {
                builder.AppendLine(String.Format("{0}: {1}", mineralAmount.Mineral.Reference, mineralAmount.Amount));
            }

            UnityEngine.Debug.Log(builder.ToString());
        }
    }
}
