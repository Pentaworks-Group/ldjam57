using Assets.Scripts.Base;
using Assets.Scripts.Scenes.GameScene;
using TMPro;
using UnityEngine;

public class MoneyBehaviour : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI cashLabel;

    private void Update()
    {
        updateCashLabel();

        calcDiggingCosts(Time.deltaTime);
    }

    private void updateCashLabel()
    {
        if (cashLabel != null)
        {
            cashLabel.text = Core.Game.State.Bank.Credits.ToString();
        }
    }

    private float calcDiggingCosts(float dt)
    {
        foreach (var digger in Core.Game.State.ActiveDiggers)
        {
            if(digger.IsMining)
            {

            }
        }
    }
}
