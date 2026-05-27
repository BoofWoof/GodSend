using UnityEngine;

public class ShopperHoverTriggerScript : MonoBehaviour
{
    public Minigame AssociatedMinigame;

    public void SendData(UpgradeItemScript data)
    {
        if (!ShopprHoverDescription.Instances.ContainsKey(AssociatedMinigame)) return;
        ShopprHoverDescription.Instances[AssociatedMinigame].UpdateData(data);
    }

    public void ClearData()
    {
        if (!ShopprHoverDescription.Instances.ContainsKey(AssociatedMinigame)) return;
        ShopprHoverDescription.Instances[AssociatedMinigame].Clear();
    }
}
