/* 
 * written by Ninoslav Kjireski 05/2019
 * parts of this project were provided by Joseph Hocking 2017
 * and the Unity-Chan Asset Package 05/2019 from the Unity Asset Store.
 * Written for DTT as an application test
 * released under MIT license (https://opensource.org/licenses/MIT)
 */

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    void OnGenNewMazeClicked()
    {
        SceneManager.LoadScene("gameTemp");
    }
}