using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    [Header("Quiz Data")]
    public List<QuizQuestion> questions;
    private int currentQuestionIndex = 0;

    [Header("UI Elements")]
    public Image signImage;
    public TextMeshProUGUI questionText;

    public Button optionAButton;
    public Button optionBButton;
    public Button optionCButton;

    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI feedbackText;
    public Button submitButton;

    [Header("ML API")]
    public PerformanceAPI performanceAPI;

    // Performance tracking
    private int totalQuestions;
    private int correctAnswers = 0;
    private int attempts = 0;
    private float totalResponseTime = 0f;
    private float questionStartTime;

    private int selectedOption = -1;

    void Start()
    {
        totalQuestions = questions.Count;
        ShowQuestion();
    }

    // ---------------- SHOW QUESTION ----------------
    void ShowQuestion()
    {
        QuizQuestion q = questions[currentQuestionIndex];

        signImage.sprite = q.signImage;
        questionText.text = q.question;

        optionAButton.GetComponentInChildren<TextMeshProUGUI>().text = q.options[0];
        optionBButton.GetComponentInChildren<TextMeshProUGUI>().text = q.options[1];
        optionCButton.GetComponentInChildren<TextMeshProUGUI>().text = q.options[2];

        selectedOption = -1;
        feedbackText.text = "";

        questionStartTime = Time.time;
    }

    // ---------------- INTERNAL OPTION SELECT ----------------
    void SelectOption(int index)
    {
        selectedOption = index;
        feedbackText.text = "Option selected";
    }

    // -------- BUTTON WRAPPERS (VISIBLE IN INSPECTOR) --------
    public void SelectOptionA() { SelectOption(0); }
    public void SelectOptionB() { SelectOption(1); }
    public void SelectOptionC() { SelectOption(2); }

    // ---------------- SUBMIT ANSWER ----------------
    public void SubmitAnswer()
    {
        if (selectedOption == -1)
        {
            feedbackText.text = "Please select an option!";
            return;
        }

        float responseTime = Time.time - questionStartTime;
        totalResponseTime += responseTime;

        QuizQuestion q = questions[currentQuestionIndex];

        if (selectedOption == q.correctAnswerIndex)
        {
            correctAnswers++;
            feedbackText.text = "Correct ✔";
        }
        else
        {
            attempts++;
            feedbackText.text = "Wrong ❌";
        }

        UpdateAccuracy();

        currentQuestionIndex++;

        if (currentQuestionIndex < totalQuestions)
        {
            Invoke(nameof(ShowQuestion), 1.2f);
        }
        else
        {
            EndQuiz();
        }
    }

    // ---------------- ACCURACY ----------------
    void UpdateAccuracy()
    {
        float accuracy = (float)correctAnswers / totalQuestions * 100f;
        accuracyText.text = "Accuracy: " + accuracy.ToString("F0") + "%";
    }

    // ---------------- END QUIZ ----------------
    void EndQuiz()
    {
        float finalAccuracy = (float)correctAnswers / totalQuestions * 100f;
        float avgResponseTime = totalResponseTime / totalQuestions;

        feedbackText.text = "Analyzing your performance...";

        if (performanceAPI != null)
        {
            StartCoroutine(
                performanceAPI.SendData(
                    finalAccuracy,
                    avgResponseTime,
                    attempts,
                    ShowPersonalizedFeedback
                )
            );
        }
        else
        {
            feedbackText.text = "ML system not connected";
        }
    }

    // -------- ML-BASED PERSONALIZED FEEDBACK --------
    void ShowPersonalizedFeedback(string performance)
    {
        switch (performance)
        {
            case "Excellent":
                feedbackText.text = "🌟 Excellent! You have mastered this sign.";
                break;

            case "Good":
                feedbackText.text = "👍 Good job! Keep practicing.";
                break;

            case "Average":
                feedbackText.text = "🙂 Average performance. Practice more.";
                break;

            case "Poor":
                feedbackText.text = "⚠️ Needs improvement. Try again.";
                break;

            default:
                feedbackText.text = "Performance: " + performance;
                break;
        }
    }
}
