using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TileBoard board;
    public CanvasGroup gameOver;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    private int score;

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        SetScore(0);
        highScoreText.text = LoadHighScore().ToString();
        gameOver.alpha = 0f;
        
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
        gameOver.interactable = false;
        

    }

    public void GameOver()
    {
        board.enabled = false;
        gameOver.interactable = true;
        StartCoroutine(Fade(gameOver, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        yield return new WaitForSeconds(delay);
        float elapsed = 0f;
        float from = canvasGroup.alpha;
        float duration = 0.5f;
        while(elapsed<duration)
        {
            gameOver.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();
        SaveHighScore();
    }

    private void SaveHighScore()
    {
        int highscore = LoadHighScore();
        if (score > highscore)
        {
            PlayerPrefs.SetInt("highscore", score);
        }
    }

    private int LoadHighScore()
    {
        return PlayerPrefs.GetInt("highscore", 0);
    }
    
}
