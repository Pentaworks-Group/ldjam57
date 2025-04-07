using Assets.Scripts.Core.Model;
using Assets.Scripts.Scenes.GameScene;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewsBehaviour : MonoBehaviour
{
    [SerializeField]
    GameObject popupObject;
    [SerializeField]
    TextMeshProUGUI titleLabel;
    [SerializeField]
    TextMeshProUGUI descriptionLabel;

    public void ShowEvent(MarketEvent marketEvent)
    {
        popupObject.SetActive(true);

        titleLabel.SetText(marketEvent.Name);
        descriptionLabel.SetText(marketEvent.Description);
    }

    public void Hide() {
        popupObject.SetActive(false);
    }
}
