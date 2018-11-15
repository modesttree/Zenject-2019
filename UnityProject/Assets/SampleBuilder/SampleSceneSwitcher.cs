using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zenject
{
    public class SampleSceneSwitcher : MonoBehaviour
    {
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                SceneManager.LoadScene("Asteroids");
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                SceneManager.LoadScene("SpaceFighter");
            }
        }
    }
}
