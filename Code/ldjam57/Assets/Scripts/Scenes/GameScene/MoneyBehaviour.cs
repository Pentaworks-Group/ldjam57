using Assets.Scripts.Base;
using Assets.Scripts.Core.Model;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MoneyBehaviour : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI cashLabel;

    [Header("Market Settings")]
//    public float updateInterval = 1.0f; // Time in seconds between price updates
//    public float marketVolatility = 1.0f; // Overall market volatility multiplier
    [Range(-1f, 1f)]
    public float marketTrend = 0.0f; // Overall market trend: -1 (bearish) to 1 (bullish)

//    [Header("Random Events")]
//    public bool enableRandomEvents = true;
//    [Range(0f, 1f)]
//    public float eventProbability = 0.05f; // Probability of random event per update
//    public float eventImpactMultiplier = 2.0f; // How strongly events affect prices

    // Internal variables
    private System.Random random;
    private float nextUpdateTime;
    private float marketMood = 0f; // Current market sentiment that fluctuates over time

    private bool isInited = false;

    private void Awake()
    {
        Core.Game.ExecuteAfterInstantation(() => {
            isInited = true;

            random = new System.Random();
            nextUpdateTime = Time.time + (float)Core.Game.State.Market.UpdateInterval;

            // Initialize current prices
            foreach (var material in Core.Game.State.Market.MineralValues)
            {
                material.CurrentPrice = material.Value;
            }

        });
    }

    private void Start()
    {
        StartCoroutine(UpdateMarketMood());
    }

    private void Update()
    {
        if (isInited)
        {
            updateCashLabel();

            Core.Game.State.Bank.Credits -= calcDiggingCosts(Time.deltaTime);

            if (Core.Game.State.Bank.Credits < 0 ) 
            {
                //TODO: do a game over event
            }

            if (Time.time >= nextUpdateTime)
            {
                UpdatePrices();
                nextUpdateTime = Time.time + (float)Core.Game.State.Market.UpdateInterval;
            }
        }
    }

    private void updateCashLabel()
    {
        if (cashLabel != null)
        {
            cashLabel.text = Core.Game.State.Bank.Credits.ToString("F2");
        }
    }

    private float calcDiggingCosts(float dt)
    {
        float totalOperatingCost = 0;
        foreach (var digger in Core.Game.State.ActiveDiggers)
        {
            if(digger.IsMining && digger.MiningTool!=null)
            {
                totalOperatingCost += (float) digger.MiningTool.OperatingCost * dt;
            }
        }

        foreach (var transporter in Core.Game.State.ActiveTransporters)
        {
            if(transporter.Transport!=null)
            {
                totalOperatingCost += (float)transporter.Transport.OperatingCost * dt;
            }
        }
        return totalOperatingCost;
    }

    private void UpdatePrices()
    {
        // Market-wide factor affecting all materials
        float marketFactor = UnityEngine.Random.Range(-0.1f, 0.1f) * (float)Core.Game.State.Market.Volatility + marketTrend * 0.01f;

        foreach (var material in Core.Game.State.Market.MineralValues)
        {
            // Random price movement based on volatility
            float randomFactor = (float)((random.NextDouble() * 2 - 1) * material.Volatility * 0.1f);

            // Combined price movement
            float priceChange = (float) (material.Value * (randomFactor + marketFactor * material.TrendStrength));

            // Apply random events
            if (Core.Game.State.Market.EnableRandomEvents && UnityEngine.Random.value < Core.Game.State.Market.EventProbability)
            {
                float eventImpact = UnityEngine.Random.Range(-0.2f, 0.2f) * (float)Core.Game.State.Market.EventImpactMultiplier;
                priceChange += (float)material.Value * eventImpact;

                // Log event for debugging
                Debug.Log($"Random event affected {material.Mineral.Name}: {eventImpact:P2} change");
            }

            // Update price with some influence from market mood
            material.CurrentPrice += priceChange + (marketMood * material.TrendStrength * 0.01f * material.Value);

            // Enforce min/max price limits if set
            if (material.MinPrice > 0 && material.CurrentPrice < material.MinPrice)
                material.CurrentPrice = material.MinPrice;
            if (material.MaxPrice > 0 && material.CurrentPrice > material.MaxPrice)
                material.CurrentPrice = material.MaxPrice;
        }
    }

    public float GetMaterialPrice(Mineral mineral)
    {
        foreach (var material in Core.Game.State.Market.MineralValues)
        {
            if (material.Mineral == mineral)
            {
                return (float) material.CurrentPrice;
            }
        }
        Debug.LogWarning($"Material '{mineral.Name}' not found!");
        return -1f;
    }

    // Market mood changes over time to simulate market cycles
    private IEnumerator UpdateMarketMood()
    {
        while (true)
        {
            // Gradually shift market mood
            marketMood = Mathf.Lerp(marketMood, UnityEngine.Random.Range(-1f, 1f), 0.1f);
            yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 15f));
        }
    }
}
