using UnityEngine;

public class Brazier : MonoBehaviour
{
    [SerializeField] private GameObject flameGO; // Flame prefab or child object
    private bool isLit = false;

    public void LightUp()
    {
        if (!isLit && flameGO != null)
        {
            flameGO.SetActive(true);
            isLit = true;
        }
    }
}
