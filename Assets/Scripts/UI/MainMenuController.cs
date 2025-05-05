// Allow to work with scenes
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method for New Game Button
    public void NewGame()
    {
        Debug.Log("Llega aqui");
        SceneManager.LoadScene("Corridor");
    }
}

