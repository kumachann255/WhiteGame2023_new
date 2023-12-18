using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlePresenter : MonoBehaviour
{
    [SerializeField] TitleView _view;

    private void Start()
    {
        _view.OnStartButtonClicked += _TransitionToGameScene;
    }

    private void _TransitionToGameScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
