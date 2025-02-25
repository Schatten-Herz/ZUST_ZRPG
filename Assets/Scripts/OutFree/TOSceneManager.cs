using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TOSceneManager : MonoBehaviour
{
    public void JumpToTheaterBattle()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TheaOutsideBattle");
    }
}
