using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.Events;
using static BasicPrayerGroupSO;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal.Internal;

public class PrayerResponse
{
    public string AssociatedPrayerSetID;

    public int AssociatedIdx;
    public string Prayer;
    public string Author;
    public bool GoodPrayer;
    public bool SpecialPrayer;
    
    public Response AssociatedResponse;

    public SpecialPrayerData AssociatedSpecialPrayerData;
    public SpecialPrayerSetSO AssociatedSpecialPrayerSet;

    public Transform TopicTarget;

    public BasicPrayerGroupSO BasicPrayerSource = null;
}


public class PrayerScript : MonoBehaviour
{
    public UnityEvent OnGoodPrayer;
    public UnityEvent OnBadPrayer;
    public UnityEvent OnPrayer;

    public static PrayerScript instance;

    public PrayerManagerSO GoodPrayerManager;
    public PrayerManagerSO BadPrayerManager;

    public Dictionary<int, PrayerResponse> CurrentResponse;

    public float RamAngyLevel = 0;
    public float AngerRate = 1f;
    public float AngerReduction = 50f;
    public float AngerThreshold = 180f;

    public float ResumeAngerLevel = 0f;

    private int GoodIdx;

    public List<MessageSendScript> SubmissionButtons;
    public TMP_Text AngyDebug;

    public delegate void PrayerSubmittedCallback(bool GoodPrayer);
    public static event PrayerSubmittedCallback PrayerSubmitted;

    public static int GoodPrayerCount = 0;
    public static int BadPrayerCount = 0;
    public static int TotalPrayerCount = 0;

    public float WaitForNextPrayerSec = 2f;

    public SplineMessageScript FireworkLauncher;
    public PrayerFireworkTextScript PrayerFireworkTextScript;

    //Dialogue Options
    public static bool StoryMode = false;
    private Response[] Responses;

    [Header("Judgement Variables")]
    public bool JudgementActive = false;
    public bool JudgementFocus = false;

    public UnityEvent OnJudgementActivate;

    private bool _SpecialPrayerActive = false;

    [Header("Wrong and Right")]
    public Material NeonGlowMaterial;
    [ColorUsage(false, true)] public Color CorrectColor = Color.green;
    [ColorUsage(false, true)] public Color WrongColor = Color.red;
    public AudioSource GoodPrayerAudio;
    public AudioSource BadPrayerAudio;

    private bool AutoSaveCheckActive = false;
    private float FirstSavePeriod = 30f;
    private float LaterSavePeriods = 60 * 5f;

    public void Awake()
    {
        instance = this;

        GoodPrayerManager.PrepareLists();
        BadPrayerManager.PrepareLists();

        GenerateNewPrayers();

        PrayerSubmitted = null;

        GoodPrayerCount = 0;
        BadPrayerCount = 0;
        TotalPrayerCount = 0;

        StoryMode = false;
    }

    public void OnEnable()
    {
        SPrayerSubmissionScript.OnNewForcedPrayers += OnNewForcedPrayer;
        GameStateMonitor.OnEventChange += OnGameEventStateChange;
    }

    public void OnDisable()
    {
        SPrayerSubmissionScript.OnNewForcedPrayers -= OnNewForcedPrayer;
        GameStateMonitor.OnEventChange -= OnGameEventStateChange;
    }

    public void ActivateJudgement()
    {
        if (JudgementActive) return;
        Debug.Log("Activating Judgement");
        JudgementActive = true;

        RamAngyLevel = ResumeAngerLevel;
        ResumeAngerLevel = 0f;

        JudgementFocus = false;
        BalconyEventsScript.instance.StartSystem();

        GenerateNewPrayers();

        OnJudgementActivate?.Invoke();

        StartCoroutine(AutosaveCheck());
    }

    public void DeactivateJudgement()
    {
        if (!JudgementActive) return;
        Debug.Log("Deactivating Judgement");
        JudgementActive = false;
        JudgementFocus = false;
        BalconyEventsScript.instance.StopSystem();
    }

    public void IncreaseAngerThreshold(float AdditionalThreshold)
    {
        AngerThreshold += AdditionalThreshold;
    }

    public float GetAngerLevel()
    {
        return (RamAngyLevel / AngerThreshold);
    }

    private void Update()
    {
        if (!JudgementActive) return;
        RamAngyLevel += Time.deltaTime * AngerRate;
        AngyDebug.text = "RamAngyLevel: " + GetAngerLevel().ToString("F2");

        if(!JudgementFocus && GetAngerLevel() > 0.8f)
        {
            CharacterSpeechScript.BroadcastGestureParameter("MacroAries", "SitForward");
            JudgementFocus = true;
        }

        if (JudgementFocus && GetAngerLevel() < 0.4f)
        {
            CharacterSpeechScript.BroadcastGestureParameter("MacroAries", "SitBack");
            JudgementFocus = false;
        }
        if (RamAngyLevel > AngerThreshold)
        {
            RamAngyLevel = AngerThreshold;
            if(!GameStateMonitor.isEventActive()) ActivateGameOver();
        }
    }
    public void SubmitAnswer(int answerIdx)
    {
        if (StoryMode)
        {
            StartCoroutine(SubmitResponse(answerIdx));
        } else
        {
            StartCoroutine(SubmitPrayer(answerIdx));
        }
    }

    #region Conversation
    public IEnumerator SubmitResponse(int answerIdx)
    {
        GameStateMonitor.ActivePrayer = true;
        DisableButtons();
        SubmissionButtons[answerIdx].SetAuthorName("");

        FireworkLauncher.ActivateMessage();
        string answerText = CurrentResponse[answerIdx].AssociatedResponse.formattedText.text;
        PrayerFireworkTextScript.ActivateFirework(answerText);

        yield return new WaitForSeconds(WaitForNextPrayerSec + 3f);
        (DialogueManager.dialogueUI as AbstractDialogueUI).OnClick(CurrentResponse[answerIdx].AssociatedResponse);
    }
    public void OnConversationResponseMenu(Response[] responses)
    {
        if (!ConversationManagerScript.isMacroConvo) return;

        GameStateMonitor.ActivePrayer = false;

        BalconyEventsScript.instance.StartSystem();

        Debug.Log("Creating choices: " + responses.Length.ToString());

        Responses = responses;

        for (int i = 0; i < 3; i++)
        {
            SubmissionButtons[i].RestartMessage();
            SubmissionButtons[i].SetAuthorName("");
        }

        List<PrayerResponse> prayerResponses = new List<PrayerResponse>();
        foreach (Response response in responses)
        {
            PrayerResponse newResponse = new PrayerResponse
            {
                AssociatedIdx = -1,
                Prayer = response.formattedText.text,
                Author = "SozaPM",
                GoodPrayer = true,
                SpecialPrayer = false,
                AssociatedResponse = response
            };

            prayerResponses.Add(newResponse);
        }
        CreateVariableLengthOptions(prayerResponses);
    }

    public void CreateVariableLengthOptions(List<PrayerResponse> responses)
    {
        Debug.Log($"Prayer Count: {responses.Count}");

        if (responses.Count == 1)
        {
            SubmissionButtons[0].gameObject.SetActive(false);
            SubmissionButtons[0].SetButtonText("");
            SubmissionButtons[0].SubmissionButton.interactable = false;
            SubmissionButtons[0].SetNormalResponse();
            SubmissionButtons[0].SetAuthorName("");

            SubmissionButtons[1].gameObject.SetActive(true);
            SubmissionButtons[1].SetButtonText(responses[0].Prayer);
            SubmissionButtons[1].SubmissionButton.interactable = true;
            if (responses[0].SpecialPrayer) SubmissionButtons[1].SetSpecialResponse(responses[0].GoodPrayer);
            else SubmissionButtons[1].SetNormalResponse();
            SubmissionButtons[1].SetAuthorName(responses[0].Author);
            responses[0].AssociatedIdx = 1;

            SubmissionButtons[2].gameObject.SetActive(false);
            SubmissionButtons[2].SetButtonText("");
            SubmissionButtons[2].SubmissionButton.interactable = false;
            SubmissionButtons[2].SetNormalResponse();
            SubmissionButtons[2].SetAuthorName("");
        }
        else if (responses.Count == 2)
        {
            SubmissionButtons[0].gameObject.SetActive(true);
            SubmissionButtons[0].SetButtonText(responses[0].Prayer);
            SubmissionButtons[0].SubmissionButton.interactable = true;
            if (responses[0].SpecialPrayer) SubmissionButtons[0].SetSpecialResponse(responses[0].GoodPrayer);
            else SubmissionButtons[0].SetNormalResponse();
            SubmissionButtons[0].SetAuthorName(responses[0].Author);
            responses[0].AssociatedIdx = 0;

            SubmissionButtons[1].gameObject.SetActive(false);
            SubmissionButtons[1].SetButtonText("");
            SubmissionButtons[1].SubmissionButton.interactable = false;
            SubmissionButtons[1].SetNormalResponse();
            SubmissionButtons[1].SetAuthorName("");

            SubmissionButtons[2].gameObject.SetActive(true);
            SubmissionButtons[2].SetButtonText(responses[1].Prayer);
            SubmissionButtons[2].SubmissionButton.interactable = true;
            if (responses[1].SpecialPrayer) SubmissionButtons[2].SetSpecialResponse(responses[1].GoodPrayer);
            else SubmissionButtons[2].SetNormalResponse();
            SubmissionButtons[2].SetAuthorName(responses[1].Author);
            responses[1].AssociatedIdx = 2;
        }
        else if (responses.Count == 3)
        {
            SubmissionButtons[0].gameObject.SetActive(true);
            SubmissionButtons[0].SetButtonText(responses[0].Prayer);
            SubmissionButtons[0].SubmissionButton.interactable = true;
            if (responses[0].SpecialPrayer) SubmissionButtons[0].SetSpecialResponse(responses[0].GoodPrayer);
            else SubmissionButtons[0].SetNormalResponse();
            SubmissionButtons[0].SetAuthorName(responses[0].Author);
            responses[0].AssociatedIdx = 0;

            SubmissionButtons[1].gameObject.SetActive(true);
            SubmissionButtons[1].SetButtonText(responses[1].Prayer);
            SubmissionButtons[1].SubmissionButton.interactable = true;
            if (responses[1].SpecialPrayer) SubmissionButtons[1].SetSpecialResponse(responses[1].GoodPrayer);
            else SubmissionButtons[1].SetNormalResponse();
            SubmissionButtons[1].SetAuthorName(responses[1].Author);
            responses[1].AssociatedIdx = 1;

            SubmissionButtons[2].gameObject.SetActive(true);
            SubmissionButtons[2].SetButtonText(responses[2].Prayer);
            SubmissionButtons[2].SubmissionButton.interactable = true;
            if (responses[2].SpecialPrayer) SubmissionButtons[2].SetSpecialResponse(responses[2].GoodPrayer);
            else SubmissionButtons[2].SetNormalResponse();
            SubmissionButtons[2].SetAuthorName(responses[2].Author);
            responses[2].AssociatedIdx = 2;
        }
        CurrentResponse = new Dictionary<int, PrayerResponse>();
        foreach (PrayerResponse response in responses)
        {
            CurrentResponse.Add(response.AssociatedIdx, response);
        }
    }

    public void OnConversationEnd(Transform actor)
    {
        if (!ConversationManagerScript.isMacroConvo) return;
        StoryMode = false;
        GenerateNewPrayers();
        MusicSelectorScript.SetOverworldSong(1);
    }
    #endregion

    #region MiniGame
    IEnumerator SubmitPrayer(int answerIdx)
    {
        GameStateMonitor.ActivePrayer = true;

        bool isSpecialPrayer = CurrentResponse[answerIdx].SpecialPrayer;

        FireworkLauncher.ActivateMessage();
        string answerText = CurrentResponse[answerIdx].Prayer;
        PrayerFireworkTextScript.ActivateFirework(answerText);

        SubmissionButtons[answerIdx].SetButtonText("");
        SubmissionButtons[answerIdx].SetAuthorName("");
        DisableButtons();


        if (isSpecialPrayer)
        {
            CurrentResponse[answerIdx].AssociatedSpecialPrayerData.StartLoadResponseChain();
        }

        yield return new WaitForSeconds(5f);

        OnPrayer?.Invoke();

        TotalPrayerCount++;
        SteamManager.UpdateIntStat("Prayers:Total", TotalPrayerCount, false);
        if (CurrentResponse[answerIdx].GoodPrayer)
        {
            GoodPrayerCount++;
            SteamManager.UpdateIntStat("Prayers:Good", GoodPrayerCount);
            OnGoodPrayer?.Invoke();
            PrayerSubmitted.Invoke(true);
        } else
        {
            BadPrayerCount++;
            SteamManager.UpdateIntStat("Prayers:Bad", BadPrayerCount);
            OnBadPrayer?.Invoke();
            PrayerSubmitted.Invoke(false);
        }


        if (isSpecialPrayer)
        {
            SPrayerSubmissionScript.SentPrayerIDs.Add(CurrentResponse[answerIdx].AssociatedPrayerSetID);

            RamAngyLevel -= AngerReduction;
            if (RamAngyLevel < 0) RamAngyLevel = 0;

            if (CurrentResponse[answerIdx].GoodPrayer)
            {
                int ariesLike = DialogueLua.GetVariable("AriesLike").asInt;
                DialogueLua.SetVariable("AriesLike", ariesLike + 1);
                Debug.Log($"Aries Like {ariesLike + 1}.");
            } else
            {
                int ariesSpite = DialogueLua.GetVariable("AriesSpite").asInt;
                DialogueLua.SetVariable("AriesSpite", ariesSpite + 1);
                Debug.Log($"Aries Spite {ariesSpite + 1}.");
            }

            yield return instance.StartCoroutine(CurrentResponse[answerIdx].AssociatedSpecialPrayerData.WaitLoadResponseChain());

            if (CurrentResponse[answerIdx].AssociatedSpecialPrayerData.SpecialResponseChainVL != null && CurrentResponse[answerIdx].AssociatedSpecialPrayerData.SpecialResponseChainVL.Count > 0)
            {
                if (CurrentResponse[answerIdx].AssociatedSpecialPrayerSet != null) SPrayerSubmissionScript.WaitingForcedPrayers.Remove(CurrentResponse[answerIdx].AssociatedSpecialPrayerSet);
                else SPrayerSubmissionScript.WaitingSpecialPrayers.Remove(CurrentResponse[answerIdx].AssociatedSpecialPrayerData);

                CharacterSpeechScript.BroadcastSpeechAttempt("MacroAries", CurrentResponse[answerIdx].AssociatedSpecialPrayerData.SpecialResponseChainVL);

                LookScript.ExternalDistractionPoint = CurrentResponse[answerIdx].TopicTarget;

                yield return new WaitForSeconds(CurrentResponse[answerIdx].AssociatedSpecialPrayerData.GetChainTime());

                LookScript.ExternalDistractionPoint = null;
            }

        } else if (GoodIdx == answerIdx)
        {

            RamAngyLevel -= AngerReduction;
            if (RamAngyLevel < 0) RamAngyLevel = 0;

            yield return StartCoroutine(SendBasicPrayerVoiceLine(CurrentResponse[answerIdx].BasicPrayerSource.GetResponse()));

            StartCoroutine(CorrectAnswerMaterialPing(CorrectColor));
            GoodPrayerAudio?.Play();
        }
        else
        {
            RamAngyLevel += AngerReduction * 2f / 3f;
            if (RamAngyLevel > AngerThreshold && !GameStateMonitor.isSpeakingSourceActive())
            {
                GameStateMonitor.ActivePrayer = false;
                yield break;
            }

            yield return StartCoroutine(SendBasicPrayerVoiceLine(CurrentResponse[answerIdx].BasicPrayerSource.GetResponse()));

            StartCoroutine(CorrectAnswerMaterialPing(WrongColor));
            BadPrayerAudio?.Play();
        }


        GameStateMonitor.ActivePrayer = false;
        GenerateNewPrayers();
    }

    private void ActivateGameOver()
    {
        OverworldBehavior.BroadcastBehaviors("A", "Tap");
    }

    private void DisableButtons()
    {
        //DisableButtons
        for (int i = 0; i < 3; i++)
        {
            SubmissionButtons[i].SetButtonText("");
            SubmissionButtons[i].SetAuthorName("");
            SubmissionButtons[i].SetNormalResponse();
            SubmissionButtons[i].SubmissionButton.interactable = false;
        }
    }

    public void OnNewForcedPrayer()
    {
        if (GameStateMonitor.isEventActive()) return;
        GenerateNewPrayers();
    }
    public void OnGameEventStateChange(bool EventActive)
    {
        if (_SpecialPrayerActive) GenerateNewPrayers();
    }

    public void ForcePrayerReferesh()
    {
        GenerateNewPrayers();
    }

    private void GenerateNewPrayers()
    {
        if (StoryMode) return;
        if (GameStateMonitor.ActivePrayer) return;

        _SpecialPrayerActive = false;

        Debug.Log("GENERATING PRAYERS");
        Debug.Log(SPrayerSubmissionScript.WaitingForcedPrayers.Count);
        Debug.Log(GameStateMonitor.isEventActive());

        if (SPrayerSubmissionScript.WaitingForcedPrayers.Count > 0 && !GameStateMonitor.isEventActive())
        {
            SpecialPrayerSetSO targetPrayer = SPrayerSubmissionScript.WaitingForcedPrayers[0];

            List<PrayerResponse> prayerResponsesSpecial = new List<PrayerResponse>();
            foreach(SpecialPrayerData prayerData in targetPrayer.PrayerOptions)
            {
                Debug.Log("SETTING PRAYER 2");
                Debug.Log(prayerData.TopicTarget);
                PrayerResponse newResponse = new PrayerResponse
                {
                    AssociatedPrayerSetID = prayerData.ParentID,
                    AssociatedIdx = -1,
                    Prayer = prayerData.Option,
                    Author = prayerData.AuthorName,
                    GoodPrayer = prayerData.GoodPrayer,
                    SpecialPrayer = true,
                    AssociatedSpecialPrayerData = prayerData,
                    AssociatedSpecialPrayerSet = targetPrayer,
                    TopicTarget = prayerData.TopicTarget
                };

                prayerResponsesSpecial.Add(newResponse);
            }
            _SpecialPrayerActive = true;
            CreateVariableLengthOptions(prayerResponsesSpecial);

            RestartMessages();
            return;
        }

        List<PrayerResponse> prayerResponses = new List<PrayerResponse>();
        GoodIdx = Random.Range(0, 3);
        int badCount = 0;
        List<int> selectedSpecials = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            string[] split;
            if (GoodIdx == i)
            {
                PrayerData prayerDataGood = GoodPrayerManager.SamplePrayer();
                PrayerResponse newGoodResponse = new PrayerResponse
                {
                    AssociatedIdx = -1,
                    Prayer = prayerDataGood.Prayer,
                    Author = prayerDataGood.Author,
                    GoodPrayer = true,
                    SpecialPrayer = false,
                    BasicPrayerSource = prayerDataGood.SourcePrayer
                };

                prayerResponses.Add(newGoodResponse);
                continue;
            }

            int SpecialPrayerCount = SPrayerSubmissionScript.WaitingSpecialPrayers.Count;
            if (SPrayerSubmissionScript.WaitingSpecialPrayers.Count > 0)
            {
                Debug.Log("Testing for special prayers.");
                if (Random.value < 0.2f && !GameStateMonitor.isEventActive())
                    {
                    Debug.Log("Adding special prayer to mix.");

                    int selectedPrayerIdx = Random.Range(0, SpecialPrayerCount);
                    SpecialPrayerData selectedPrayer = SPrayerSubmissionScript.WaitingSpecialPrayers[selectedPrayerIdx];

                    if (!selectedSpecials.Contains(selectedPrayerIdx))
                    {
                        _SpecialPrayerActive = true;
                        selectedSpecials.Add(selectedPrayerIdx);
                        PrayerResponse newResponse = new PrayerResponse
                        {
                            AssociatedPrayerSetID = selectedPrayer.ParentID,
                            AssociatedIdx = -1,
                            Prayer = selectedPrayer.Option,
                            Author = selectedPrayer.AuthorName,
                            GoodPrayer = selectedPrayer.GoodPrayer,
                            SpecialPrayer = true,
                            AssociatedSpecialPrayerData = selectedPrayer,
                            TopicTarget = selectedPrayer.TopicTarget
                        };

                        prayerResponses.Add(newResponse);
                        continue;
                    }
                }
            }

            PrayerData prayerDataBad = BadPrayerManager.SamplePrayer();
            PrayerResponse newBadResponse = new PrayerResponse
            {
                AssociatedIdx = -1,
                Prayer = prayerDataBad.Prayer,
                Author = prayerDataBad.Author,
                GoodPrayer = false,
                SpecialPrayer = false,
                BasicPrayerSource = prayerDataBad.SourcePrayer
            };

            prayerResponses.Add(newBadResponse);
            badCount++;
        }
        CreateVariableLengthOptions(prayerResponses);

        RestartMessages();
    }

    public void RestartMessages()
    {
        for (int i = 0; i < 3; i++)
        {
            SubmissionButtons[i].RestartMessage();
        }
    }
    #endregion

    public IEnumerator SendBasicPrayerVoiceLine(string voiceFilePath)
    {
        if (string.IsNullOrEmpty(voiceFilePath)) yield break;

        voiceFilePath = voiceFilePath.CleanResourcePath();

        Debug.Log($"Loading prayer from {voiceFilePath}.");

        ResourceRequest request = Resources.LoadAsync<VoiceLineSO>(voiceFilePath); // Replace GameObject with your asset type

        yield return request;

        if (request.asset == null) yield break;

        Debug.Log($"Asset loaded {request.asset.name}.");

        VoiceLineSO voiceLine = request.asset as VoiceLineSO;

        yield return new WaitForSeconds(0.3f);

        if (JudgementActive)
        {
            if (!GameStateMonitor.isSpeakingSourceActive())
            {
                CharacterSpeechScript.BroadcastSpeechAttempt("MacroAries", voiceLine);
            }
            else
            {
                CharacterSpeechScript.BroadcastSpeechAttempt("MacroAries", voiceLine, false);
            }
        }

        yield return new WaitForSeconds(voiceLine.AudioData.length + 0.4f);
    }

    public IEnumerator CorrectAnswerMaterialPing(Color targetPingColor)
    {
        Color defaultColor = NeonGlowMaterial.GetColor("_EmissionColor");

        NeonGlowMaterial.SetColor("_EmissionColor", targetPingColor);
        NeonGlowMaterial.SetColor("_BaseColor", targetPingColor);

        float timePassed = 0;
        float pingPeriod = 4f;
        while(timePassed < pingPeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / pingPeriod;
            Color progressColor = Color.Lerp(targetPingColor, defaultColor, progress);
            NeonGlowMaterial.SetColor("_EmissionColor", progressColor);
            NeonGlowMaterial.SetColor("_BaseColor", progressColor);
            
            yield return null;
        }

        NeonGlowMaterial.SetColor("_EmissionColor", defaultColor);
        NeonGlowMaterial.SetColor("_BaseColor", defaultColor);
    }

    public IEnumerator AutosaveCheck()
    {
        if (AutoSaveCheckActive) yield break;
        AutoSaveCheckActive = true;

        float timePassed = 0f;
        while(timePassed < FirstSavePeriod)
        {
            timePassed += Time.deltaTime;

            if (!JudgementActive || BusySignScript.GlobalCurrentMode != BusySignScript.SignMode.Leisure)
            {
                AutoSaveCheckActive = false;
                yield break;
            }

            yield return null;
        }

        SaveMenuScript.AutoSave();

        while (true)
        {
            timePassed = 0f;
            while (timePassed < LaterSavePeriods)
            {
                timePassed += Time.deltaTime;

                if (!JudgementActive || BusySignScript.GlobalCurrentMode != BusySignScript.SignMode.Leisure)
                {
                    AutoSaveCheckActive = false;
                    yield break;
                }

                yield return null;
            }
        }
    }
}
