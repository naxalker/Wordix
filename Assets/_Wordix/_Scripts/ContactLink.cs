using UnityEngine;
using UnityEngine.EventSystems;
using YG;

public class ContactLink : MonoBehaviour, IPointerClickHandler
{
    private const string LINK = "https://t.me/defleg";

    private void Awake()
    {
        if (YG2.platform == "CrazyGames")
        {
            gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        YG2.OnURL(LINK);
#if UNITY_EDITOR
        Debug.Log("Contact link clicked");
#endif
    }
}
