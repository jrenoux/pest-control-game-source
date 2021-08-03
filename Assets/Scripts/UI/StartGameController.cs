using UnityEngine;
using UnityEngine.UI;
public class StartGameController : MonoBehaviour
{
    [SerializeField]
    private GameObject initOverlay;

    public void Start()
    {
        initOverlay.SetActive(true);
    }

    public void Update()
    {

    }
}