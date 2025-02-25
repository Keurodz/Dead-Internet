using UnityEngine;
using UnityEngine.SceneManagement;

public class SignalProcessor : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty!");
            return;
        }

        print("Tried To load this shit");
        SceneManager.LoadScene(1);
    }
}
