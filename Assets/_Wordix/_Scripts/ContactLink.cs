using UnityEngine;
using UnityEngine.EventSystems;

public class ContactLink : MonoBehaviour, IPointerClickHandler
{
    private const string LINK = "https://t.me/defleg";

    private void Awake()
    {
        if (!PlatformBridge.Service.IsExternalLinksAllowed)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlatformBridge.Service.OpenUrl(LINK);
#if UNITY_EDITOR
        Debug.Log("Contact link clicked");
#endif
    }
}
