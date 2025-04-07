using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    public class MouseEventBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent PointerEntered = new UnityEvent();
        public UnityEvent PointerExited = new UnityEvent();

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.PointerEntered.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.PointerExited.Invoke();
        }
    }
}
