using UnityEngine;

public class Scrap_System : MonoBehaviour
{

    public GameObject Scrap;

    public void ScrapSpawn()
    {
        Instantiate(Scrap);
    }

    private void Start()
    {
        Instantiate(Scrap);
    }
}
