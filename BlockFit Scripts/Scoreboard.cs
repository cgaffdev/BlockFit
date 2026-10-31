using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Scoreboard : MonoBehaviour
{

    public static Scoreboard instance;

    [SerializeField] TMPro.TextMeshPro text;
    int currentScore = 0;
    [SerializeField] int pointsPerClear = 10;
    [SerializeField] float multiplier = 0.15f;
    [SerializeField] float pointRate = 0.15f;


    void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        if(text==null){
            Debug.LogError("Scoreboard text is not assigned");
        }else {
            text.text = "000";
        }
    }

    public void UpdateScoreboard(int score){
        // Get pointsPerClear on 1 clear, every beyond gain multiplier in addition. EX: *1, *1.15, *1.30 . . .
        // Ex: 100, 215, 345,...
        int pointsAdded = pointsPerClear;
        for(int i = 1; i < score; i++){
            pointsAdded += (int)(pointsPerClear * Mathf.Pow(1 + multiplier, i));
        }
        
        currentScore += pointsAdded;

        if(text!=null){
            StartCoroutine(AnimateScore(currentScore));
        }
    }

    IEnumerator AnimateScore(int score){
        int currentScore = int.Parse(text.text);
        while(currentScore < score){
            currentScore++;
            text.text = currentScore.ToString("D3"); // Makes certain that digits are 3 wide, e.g. 001 instead of 1
            yield return new WaitForSeconds(pointRate);
        }
    }
}
