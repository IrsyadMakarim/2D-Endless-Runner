using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    private int _currentScore = 0;

    // Start is called before the first frame update
    private void Start()
    {
        //reset
        _currentScore = 0;
    }

    public void IncreaseCurrentScore(int increment)
    {
        _currentScore += increment;
    }

    public void FinishScoring()
    {
        if (_currentScore > ScoreData.highScore)
        {
            ScoreData.highScore = _currentScore;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
