using PixelCrushers;
using System;
using System.Collections.Generic;

public class ActiveCharacterSaver : Saver
{
    [Serializable]
    public class ActiveCharacterSave
    {
        public List<string> DeactivatedCharacters = new List<string>();
    }

    public override string RecordData()
    {
        ActiveCharacterSave newSaveData = new ActiveCharacterSave();

        foreach (CharacterActivationScript caScript in CharacterActivationScript.GetAllActivationScripts())
        {
            if (!caScript.CharacterRoot.activeSelf)
            {
                newSaveData.DeactivatedCharacters.Add(caScript.TakenName);
            }
        }

        return SaveSystem.Serialize(newSaveData);
    }

    public override void ApplyData(string s)
    {
        ActiveCharacterSave saveData = SaveSystem.Deserialize<ActiveCharacterSave>(s);

        if (saveData == null) return;

        foreach (string deactiveName in saveData.DeactivatedCharacters)
        {
            CharacterActivationScript.DisableCharacter(deactiveName);
        }
    }
}
