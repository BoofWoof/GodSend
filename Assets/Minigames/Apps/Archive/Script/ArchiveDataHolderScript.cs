using PixelCrushers;
using System;
using System.Collections.Generic;

public class ArchiveDataHolderScript : Saver
{
    public List<ArchiveDataSO> ArchiveDatas;

    public int Priority;

    public bool Submitted = false;
    public bool AutomaticallySubmitAtStart = false;

    public string UnlockAnnouncement;
    
    [Serializable]
    public class ArchiveDiscoveredSaveData
    {
        public bool Saved = false;
    }

    public override void ApplyData(string s)
    {
        ArchiveDiscoveredSaveData saveData = SaveSystem.Deserialize<ArchiveDiscoveredSaveData>(s);

        if (saveData == null) return;

        if (saveData.Saved) SubmitArchiveDataWithAnnouncement(false);
    }

    public override string RecordData()
    {
        ArchiveDiscoveredSaveData newSaveData = new ArchiveDiscoveredSaveData()
        {
            Saved = Submitted
        };
        return SaveSystem.Serialize(newSaveData);
    }

    override public void Start()
    {
        base.Start();
        if (AutomaticallySubmitAtStart) SubmitArchiveData();
    }

    public void SubmitArchiveData()
    {
        SubmitArchiveDataWithAnnouncement(true);
    }

    public void SubmitArchiveDataWithAnnouncement(bool sendAnnouncement = true)
    {
        if (Submitted) return;
        Submitted = true;
        ArchiveScript.AddArchiveStatic(ArchiveDatas, Priority);

        if (UnlockAnnouncement.Length > 0 && sendAnnouncement)
        {
            AppScript targetApp = AppScript.AppsDict["Archives"];
            string appName = targetApp.AppName;
            string previewText = $"<b>New Data Available:</b>\nHead to <b>{appName}</b> to check it out!";

            AppNotificationScript.SetNotification(new AppNotificationScript.NotificationInfo
            {
                SourceApp = targetApp,
                PreviewImage = targetApp.AssociatedIcon,
                PreviewText = previewText,
                AdditionalActions = null
            });
            //AnnouncementScript.StartAnnouncement(UnlockAnnouncement);
        }

        if (!AutomaticallySubmitAtStart) ArchiveScript.instance.ShowNotification();
    }
}
