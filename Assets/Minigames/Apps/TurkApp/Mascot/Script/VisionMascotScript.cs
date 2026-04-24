using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;
using static System.Net.Mime.MediaTypeNames;

public class VisionMascotScript : MonoBehaviour
{
    public static VisionMascotScript instance;
    public static bool EnableProgress = true;

    public VideoPlayer CharacterVideo;

    public string MascotName = "";
    public TMP_Text NameText;

    public TMP_Text TextBoxText;
    public GameObject TextBox;

    public GameObject ButtonSetPrefab;
    public GameObject TextFieldPrefab;

    public List<MascotDifficultyDialogueSO> DifficultyChangeMessages;

    public GameObject FocusPanel;

    public AudioSource SpeechAudioSource;

    private bool TextChainActive = false;
    private bool WaitForText = false;
    public bool WaitForInteraction = false;
    public bool SkipWait = false;
    public static bool NewDifficultyUnlocked;

    public bool DialogueActive = false;

    private List<Coroutine> WaitCoroutines = new List<Coroutine>();

    public Animator MessageBoxAnimator;

    public PhoneTutorialScript PhoneTutorial;

    void Awake()
    {
        instance = this;
        MascotClearText();

        NameText.text = MascotName;

        FocusPanel.SetActive(false);

        foreach (MascotDifficultyDialogueSO mascotDialogue in  DifficultyChangeMessages)
        {
            mascotDialogue.ResetData();
        }

        UpdateCharacter();
    }

    public static void SayText(string text)
    {
        if (text.Length == 0) return;

        instance.MascotSayText(text);
    }

    private void MascotSayText(string SayText)
    {
        if(DialogueActive) return;

        TextBox.gameObject.SetActive(true);

        MessageBoxAnimator.Play("Wobble");

        string[] SplitText = SayText.Split("<n>");
        if(SplitText.Length > 0)
        {
            StartCoroutine(MascotSayTextChain(SplitText));
            return;
        }

    }
    private void ShowText(string SayText)
    {
        SpeechAudioSource.Play();

        ChoiceBlock choices = SayText.SplitByChoices();
        if (choices.ChoiceDictionary.Count > 0)
        {
            FocusPanel.SetActive(true);

            WaitForInteraction = true;

            TextBoxText.text = choices.PreText;

            GameObject textboxButtons = Instantiate(ButtonSetPrefab, TextBox.transform);
            textboxButtons.transform.localScale = Vector3.one;
            textboxButtons.transform.localRotation = Quaternion.identity;
            textboxButtons.transform.localPosition = Vector3.zero;

            textboxButtons.GetComponent<MascotButtonScript>().SetChoices(choices);

            LayoutRebuilder.ForceRebuildLayoutImmediate(TextBox.GetComponent<RectTransform>());
            return;
        }

        if (SayText.Contains("<name>"))
        {
            SayText = SayText.Replace("<name>", "").Trim();
            WaitForInteraction = true;

            TextBoxText.text = SayText;

            GameObject textField = Instantiate(TextFieldPrefab, TextBox.transform);
            textField.transform.localScale = Vector3.one;
            textField.transform.localRotation = Quaternion.identity;
            textField.transform.localPosition = Vector3.zero;

            MascotTextFieldScript mascotTextFieldScript = textField.GetComponent<MascotTextFieldScript>();

            int maxCharacters = 6;
            if (DayInfo.CurrentDay != 1) maxCharacters = 8;

            mascotTextFieldScript.SetTextLimit(2, maxCharacters);
            mascotTextFieldScript.OnTextSubmission.AddListener(OnNameSet);

            LayoutRebuilder.ForceRebuildLayoutImmediate(TextBox.GetComponent<RectTransform>());
            return;
        }

        TextBoxText.text = SayText;
        LayoutRebuilder.ForceRebuildLayoutImmediate(TextBox.GetComponent<RectTransform>());

    }

    public IEnumerator MascotSayTextChain(string[] TextChain)
    {
        DialogueActive = true;

        if (TextChain.Length > 1) FocusPanel.SetActive(true);
        TextChainActive = true;
        foreach (string s in TextChain)
        {
            string editedString = s;
            bool showNewTutorial = false;
            if(editedString.Contains("<t>")){
                editedString = editedString.Replace("<t>", "").Trim();
                showNewTutorial = true;
            }

            ShowText(editedString);
            while (WaitForInteraction) { yield return null; }
            if (!SkipWait) yield return new WaitForSeconds(1f);
            WaitForText = true;
            if (!SkipWait) while (WaitForText) { yield return null; }
            if (showNewTutorial) PhoneTutorial.UnlockNewTutorialStep();

            MessageBoxAnimator.Play("Wiggle");

            SkipWait = false;
        }
        TextChainActive = false;
        MascotClearText();

        DialogueActive = false;
    }

    public static void ClearText()
    {
        instance.MascotClearText();
    }

    public void MascotClearText()
    {
        TextBox.gameObject.SetActive(false);
        FocusPanel.SetActive(false);
    }

    public IEnumerator TimerDialogue(TimePassingDialogues timerDialogueData)
    {
        if (!timerDialogueData.AllowRetrigger && timerDialogueData.TriggerOccurances > 0) yield break;

        float timePassed = 0f;

        while (timePassed < timerDialogueData.TimePassed)
        {
            if (!DialogueActive && AppScript.CheckIfActive("Visions")) timePassed += Time.deltaTime;
            yield return null;
        }

        string dialogue = timerDialogueData.SolutionDialogues[timerDialogueData.TriggerOccurances%timerDialogueData.SolutionDialogues.Count];

        if (DialogueActive) yield break;
        MascotSayText(dialogue);

        timerDialogueData.TriggerOccurances++;
    }

    public IEnumerator DelayedOnPuzzleGeneration()
    {
        yield return new WaitForSeconds(0.1f);

        if (DialogueActive) yield break;
        if (!AppScript.CheckIfActive("Visions") || Time.timeScale < 1f) yield break;

        MascotDifficultyDialogueSO currentDifficultyDialogue = DifficultyChangeMessages[TurkPuzzleScript.CurrentDifficutly];

        WaitCoroutines = new List<Coroutine>();
        foreach (TimePassingDialogues tpData in currentDifficultyDialogue.TimeDialogues)
        {
            WaitCoroutines.Add(StartCoroutine(TimerDialogue(tpData)));
        }

        foreach(int triggerValue in currentDifficultyDialogue.SolutionDialogues.Keys)
        {
            int puzzlesCompleted = TurkPuzzleScript.PuzzlesCompleted[TurkPuzzleScript.CurrentDifficutly];

            if (triggerValue <= puzzlesCompleted)
            {
                VisionCompletionMascotText textData = currentDifficultyDialogue.SolutionDialogues[triggerValue];

                if (!textData.Triggered)
                {
                    textData.Triggered = true;
                    MascotSayText(textData.SolutionDialogues);
                    yield break;
                }
            }
        }

        /*
        if (TurkPuzzleScript.PuzzlesCompleted.ContainsKey(TurkPuzzleScript.CurrentDifficutly))
        {
            int puzzlesCompleted = TurkPuzzleScript.PuzzlesCompleted[TurkPuzzleScript.CurrentDifficutly];

            if (currentDifficultyDialogue.SolutionDialogues.ContainsKey(puzzlesCompleted))
            {
                VisionCompletionMascotText textData = currentDifficultyDialogue.SolutionDialogues[puzzlesCompleted];

                if (!textData.Triggered)
                {
                    textData.Triggered = true;
                    MascotSayText(textData.SolutionDialogues);
                    yield break;
                }
            }
        }
        */

        if (TurkPuzzleScript.CurrentDifficutly == TurkPuzzleScript.MaxAvailableDifficutly - 1) NewDifficultyUnlocked = false;
        if (NewDifficultyUnlocked)
        {
            MascotSayText(currentDifficultyDialogue.DifficultyReminder);
            NewDifficultyUnlocked = false;
        }
    }
    public void OnPuzzleGeneration()
    {
        StartCoroutine(DelayedOnPuzzleGeneration());
    }

    public void OnPuzzleEnd()
    {
        foreach(Coroutine c in WaitCoroutines)
        {
            if(c != null) StopCoroutine(c);
        }
    }

    public void OnNameSet(string newName)
    {
        if (newName.ToLower() == "boof")
        {
            ShowText("The ego on that name is a bit too strong. Perhaps you'd pick a different name?<name>");
            return;
        }
        if (newName.ToLower() == "karu")
        {
            ShowText("Hmm, a bit too artsy of a name. Perhaps you'd pick a different name?<name>");
            return;
        }
        if (newName.ToLower() == "sykes")
        {
            ShowText("Way too silly. Perhaps you'd pick a different name?<name>");
            return;
        }
        if (newName.ToLower() == "hat")
        {
            ShowText("DON'T EVEN OFFER ME THAT NAME AS A JOKE. Please, pick another.<name>");
            return;
        }
        if (newName.ToLower() == "aries")
        {
            ShowText("Please don't asssociate me with any outlaws... How about a different name?<name>");
            return;
        }
        if (newName.ToLower() == "aevs")
        {
            ShowText("I'm not small enough for a name like that! Perhaps you'd pick a different name?<name>");
            return;
        }
        if (newName.ToLower() == "kyowin")
        {
            ShowText("I'm a bird, not a cat! Perhaps you'd pick a different name?<name>");
            return;
        }
        if (newName.ToLower() == "flint")
        {
            ShowText("Despite fitting the character limit, that name seems too small! Perhaps you'd pick a different name?<name>");
            return;
        }
        if (newName.ToLower() == "lex")
        {
            ShowText("I'm cute, but not that cute! Perhaps you'd pick a different name?<name>");
            return;
        }
        if (newName.ToLower() == "seven")
        {
            ShowText("That's a number, not a name! Perhaps you'd pick a different name?<name>");
            return;
        }
        if (newName.ToLower() == "trip")
        {
            ShowText("I'm plenty dexterous, I assure you! Perhaps you'd pick a different name?<name>");
            return;
        }
        if (newName.ToLower() == "milo")
        {
            ShowText("A nearby user has already claimed that name! Perhaps you'd pick a different name?<name>");
            return;
        }
        if (newName.ToLower() == "deimos")
        {
            ShowText("Please don't call me a simp! Perhaps you'd pick a different name?<name>");
            return;
        }
        if (newName.ToLower() == "arkae")
        {
            ShowText("That name is too holy for my likeness! Perhaps you'd pick a different name?<name>");
            return;
        }
        if (newName.ToLower() == "saleos")
        {
            ShowText("Perhaps you know more than you let on... But humor me. Might you take a different name?<name>");
            return;
        }
        if (newName.ToLower().Contains("egg"))
        {
            ShowText("I won't be an egg forever. Maybe you should pick something else.<name>");
            return;
        }
        if (newName.ToLower() == "sy")
        {
            ShowText("That name's a bit too out of this world for me. Maybe you should pick something else.<name>");
            return;
        }

        MascotName = newName;
        NameText.text = newName;
        WaitForInteraction = false;
        WaitForText = false;
        SkipWait = true;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!AppScript.CheckIfActive("Visions") || Time.timeScale < 1f) return;

        if (!EnableProgress) return;
        if (!context.started) return;

        if (WaitForInteraction) return;

        if (TextChainActive)
        {
            WaitForText = false;
            return;
        }

        MascotClearText();
    }

    public void OnAdClose()
    {
        if (DialogueActive) return;

        int currentDifficulty = TurkPuzzleScript.CurrentDifficutly;
        if (currentDifficulty >= DifficultyChangeMessages.Count) return;

        MascotDifficultyDialogueSO currentDifficultyDialogue = DifficultyChangeMessages[currentDifficulty];
        if(currentDifficultyDialogue.SpamCloseOccurances > 0)
        {
            MascotSayText(currentDifficultyDialogue.SpamCloseDialogues[currentDifficultyDialogue.SpamCloseOccurances % currentDifficultyDialogue.SpamCloseDialogues.Count]);
        } else
        {
            MascotSayText(currentDifficultyDialogue.SpamCloseDialogues[0]);
        }
        currentDifficultyDialogue.SpamCloseOccurances++;
    }

    public void OnMascotClick()
    {
        if (DialogueActive) return;

        int currentDifficulty = TurkPuzzleScript.CurrentDifficutly;
        if (currentDifficulty >= DifficultyChangeMessages.Count) return;

        MascotDifficultyDialogueSO currentDifficultyDialogue = DifficultyChangeMessages[currentDifficulty];
        MascotSayText(currentDifficultyDialogue.ClickDialogues[currentDifficultyDialogue.ClickOccurrences % currentDifficultyDialogue.ClickDialogues.Count]);
        currentDifficultyDialogue.ClickOccurrences++;
    }

    public static void OnSubmitChoice(string text)
    {
        instance.MascotOnSubmitChoice(text);
    }
    public void MascotOnSubmitChoice(string text)
    {
        WaitForInteraction = false;
        ShowText(text);
    }

    public void UpdateCharacter()
    {
        MascotDifficultyDialogueSO currentDifficultyDialogue = DifficultyChangeMessages[TurkPuzzleScript.CurrentDifficutly];
        SpeechAudioSource.clip = currentDifficultyDialogue.SpeechSound;
        CharacterVideo.clip = currentDifficultyDialogue.CharacterVideo;
    }

    public void OnDifficultyIncrease(int currentDifficulty)
    {
        if (currentDifficulty == TurkPuzzleScript.MaxAvailableDifficutly-1) NewDifficultyUnlocked = false;

        if (DialogueActive) return;

        if (currentDifficulty >= DifficultyChangeMessages.Count) return;

        foreach (Coroutine c in WaitCoroutines)
        {
            if (c != null) StopCoroutine(c);
        }

        MascotDifficultyDialogueSO currentDifficultyDialogue = DifficultyChangeMessages[currentDifficulty];

        UpdateCharacter();

        if (currentDifficultyDialogue.FirstIncrease)
        {
            currentDifficultyDialogue.FirstIncrease = false;
            MascotSayText(currentDifficultyDialogue.FirstDifficultyIncreaseDialogues);
            return;
        }
        MascotSayText(currentDifficultyDialogue.DifficultyIncreaseDialogues[currentDifficultyDialogue.IncreaseOccurrences % currentDifficultyDialogue.DifficultyIncreaseDialogues.Count]);
        currentDifficultyDialogue.IncreaseOccurrences++;
    }

    public void OnDifficultyDecrease(int currentDifficulty)
    {
        if (DialogueActive) return;

        if (currentDifficulty >= DifficultyChangeMessages.Count) return;

        foreach (Coroutine c in WaitCoroutines)
        {
            if (c != null) StopCoroutine(c);
        }

        MascotDifficultyDialogueSO currentDifficultyDialogue = DifficultyChangeMessages[currentDifficulty];

        UpdateCharacter();

        if (currentDifficultyDialogue.FirstDecrease)
        {
            currentDifficultyDialogue.FirstDecrease = false;
            MascotSayText(currentDifficultyDialogue.FirstDifficultyDecreaseDialogues);
            return;
        }
        MascotSayText(currentDifficultyDialogue.DifficultyDecreaseDialogues[currentDifficultyDialogue.DecreaseOccurrences % currentDifficultyDialogue.DifficultyDecreaseDialogues.Count]);
        currentDifficultyDialogue.DecreaseOccurrences++;
    }

    public static void OnNewDifficutlyUnlocked()
    {
        NewDifficultyUnlocked = true;
    }
}
