using Assets.Scripts.Base;
using TMPro;
using UnityEngine;

public class MoneyBehaviour : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI cashLabel;

    private bool isInited = false;

    private void Awake()
    {
        Core.Game.ExecuteAfterInstantation(() => isInited = true);
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
}
