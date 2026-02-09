using UnityEngine;

[System.Serializable]
public class QuizQuestion
{
    public Sprite signImage;
    public string question;
    public string[] options = new string[3];
    public int correctAnswerIndex;
}
