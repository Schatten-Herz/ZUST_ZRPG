using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    public void JumpToTheaterBattle()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TheaterBattle");
    }

    public void JumpToTheaOutSideBattle()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TheaOutsideBattle");
    }
    
    public void JumpToTheaOutSideFree()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TheaOutsideFree");
    }

    public void JumpToTheaterSecond()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TheaterSecond");
    }
}
