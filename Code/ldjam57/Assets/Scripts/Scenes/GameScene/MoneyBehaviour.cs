using Assets.Scripts.Base;
using Assets.Scripts.Constants;
using Assets.Scripts.Core.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MoneyBehaviour : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI cashLabel;
    [SerializeField]
    NewsBehaviour newsBehaviour;

    [Header("Market Settings")]
    [Range(-1f, 1f)]
    public float marketTrend = 0.0f; // Overall market trend: -1 (bearish) to 1 (bullish)

    // Internal variables
    private System.Random random;
    private float nextUpdateTime;
    private float marketMood = 0f; // Current market sentiment that fluctuates over time
    private float lastEventTime = -999f; // Time when the last event occurred
    private float eventCooldownPeriod = 10f; // Base cooldown period between events (in seconds)
    private float eventCooldownRandomFactor = 5f; // Random additional cooldown time

    private bool isInited = false;

    private List<MarketEvent> activeEvents = new List<MarketEvent>();

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

            if (Core.Game.State.Bank.Credits < 0 )
            {
                Core.Game.ChangeScene(SceneNames.GameOver);
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
            if(transporter.Transport!=null && transporter.IsActive)
            {
                totalOperatingCost += (float)transporter.Transport.OperatingCost * dt;
            }
        }
        return totalOperatingCost;
    }

    private void UpdatePrices()
    {
        float timeSinceLastEvent = Time.time - lastEventTime;
        float currentProbability = (float)Core.Game.State.Market.EventProbability;

        if (timeSinceLastEvent < eventCooldownPeriod)
        {
            // Probability increases linearly from 10% of base to 100% of base over the cooldown period
            float multiplier = Mathf.Lerp(0.1f, 1.0f, timeSinceLastEvent / eventCooldownPeriod);
            currentProbability *= multiplier;
            Debug.Log(currentProbability);
        }

        // Market-wide factor affecting all materials
        float marketFactor = UnityEngine.Random.Range(-0.1f, 0.1f) * (float)Core.Game.State.Market.Volatility + marketTrend * 0.01f;

        foreach (var material in Core.Game.State.Market.MineralValues)
        {
            // Random price movement based on volatility
            float randomFactor = (float)((random.NextDouble() * 2 - 1) * material.Volatility * 0.1f);

            // Combined price movement
            float priceChange = (float) (material.Value * (randomFactor + marketFactor * material.TrendStrength));

            // Apply random events
            if (Core.Game.State.Market.EnableRandomEvents && UnityEngine.Random.value < currentProbability)
            {
                TriggerRandomEvent();

                lastEventTime = Time.time;

                eventCooldownPeriod = 10f + UnityEngine.Random.Range(0f, eventCooldownRandomFactor);
            }
            activeEvents.ForEach(e =>
            {
                if (e.AffectedMaterials.Contains(material.Mineral.Name))
                {
                    float eventImpact = (float)(e.PriceImpact * Core.Game.State.Market.EventImpactMultiplier);
                    priceChange += (float)material.Value * eventImpact;
                    Debug.Log($"Random event affected {material.Mineral.Name}: {eventImpact:P2} change");
                }
            });

            // Update price with some influence from market mood
            material.CurrentPrice += priceChange + (marketMood * material.TrendStrength * 0.01f * material.Value);

            // Enforce min/max price limits if set
            if (material.MinPrice > 0 && material.CurrentPrice < material.MinPrice)
                material.CurrentPrice = material.MinPrice;
            if (material.MaxPrice > 0 && material.CurrentPrice > material.MaxPrice)
                material.CurrentPrice = material.MaxPrice;
        }
    }

    public MarketEvent TriggerRandomEvent(bool forcePositive = false, bool forceNegative = false)
    {
        if(Core.Game.State.Market.Events.Count > 0)
        {
            // Pick a random event
            MarketEvent selectedEvent = Core.Game.State.Market.Events[UnityEngine.Random.Range(0, Core.Game.State.Market.Events.Count)];

            // Log the event
            Debug.Log($"MARKET EVENT: {selectedEvent.Name} - {selectedEvent.Description}");

            // Trigger price effects
            StartCoroutine(ApplyEventEffect(selectedEvent));

            return selectedEvent;
        }
        return null;
    }

    private IEnumerator ApplyEventEffect(MarketEvent marketEvent)
    {
        activeEvents.Add(marketEvent);

        if (newsBehaviour != null )
        {
            newsBehaviour.ShowEvent(marketEvent);
        }

        // Effect duration
        yield return new WaitForSeconds((float)marketEvent.Duration);

        activeEvents.Remove(marketEvent);

        if(activeEvents.Count == 0 && newsBehaviour != null)
        {
            newsBehaviour.Hide();
        } 

        // Event effect ends (you might want to gradually return to normal)
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
        //Debug.LogWarning($"Material '{mineral.Name}' not found!");
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
