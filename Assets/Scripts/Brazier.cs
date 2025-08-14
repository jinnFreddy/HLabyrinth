using UnityEngine;

public class Brazier : MonoBehaviour
{
    [SerializeField] private GameObject[] flames; // Flame prefab or child object

    public void LightUp()
    {
        foreach (GameObject go in flames)
        {
            if (flames != null)
            {
                go.SetActive(true);
            }
        }
    }
}
