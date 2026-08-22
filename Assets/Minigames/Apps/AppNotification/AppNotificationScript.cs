using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AppNotificationScript : MonoBehaviour
{
    public class NotificationInfo
    {
        public AppScript SourceApp;
        public Sprite PreviewImage;
        public string PreviewText;
        public UnityEvent AdditionalActions;
    }

    public static AppNotificationScript Instance;

    public Animator ScrolldownAnimator;

    private List<NotificationInfo> Notifications = new();
    private NotificationInfo CurrentNotification;

    public Image NotificationImage;
    public TMP_Text NotificationText;

    private bool NotificationIgnored;
    private bool NotificationShowing;

    public void Awake()
    {
        Instance = this;
    }

    public static void SetNotification(NotificationInfo notificationInfo)
    {
        Instance._SetNotification(notificationInfo);
    }

    public void _SetNotification(NotificationInfo notificationInfo)
    {
        foreach(NotificationInfo notification in Notifications)
        {
            if (notification.SourceApp == notificationInfo.SourceApp) return;
        }

        Notifications.Add(notificationInfo);
        StartCoroutine(DisplayNotifications());
    }

    public IEnumerator DisplayNotifications()
    {
        if (NotificationShowing) yield break;
        NotificationShowing = true;

        while (Notifications.Count > 0)
        {
            NotificationIgnored = true;

            CurrentNotification = Notifications[0];

            NotificationImage.sprite = CurrentNotification.PreviewImage;
            NotificationText.text = CurrentNotification.PreviewText;

            ScrolldownAnimator.Play("DipDown");

            float timePassed = 0f;
            while (timePassed < 10f && NotificationIgnored)
            {
                if(PhonePositionScript.raised) timePassed += Time.deltaTime;
                yield return null;
            }

            ScrolldownAnimator.Play("DipUp");

            Notifications.RemoveAt(0);
        }

        NotificationShowing = false;
    }

    public void OnNotificationClick()
    {
        if (CurrentNotification.SourceApp != null) AppScript.Swap(CurrentNotification.SourceApp);
        ScrolldownAnimator.Play("DipDown", 0, 1f);
        NotificationIgnored = false;
        CurrentNotification.AdditionalActions?.Invoke();
    }
}
