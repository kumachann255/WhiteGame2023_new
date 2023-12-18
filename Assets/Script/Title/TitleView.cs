using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TitleView : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    
    public Action OnStartButtonClicked { get; set; }

    private void OnDestroy()
    {
        OnStartButtonClicked = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        _startButton.onClick.AddListener(() => OnStartButtonClicked?.Invoke());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
