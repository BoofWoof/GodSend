using UnityEngine;

public class ShopperHoverTriggerScript : MonoBehaviour
{
    public void SendData(UpgradeItemScript data)
    {
        ShopprHoverDescription.Instance.UpdateData(data);
    }

    public void ClearData()
    {
        ShopprHoverDescription.Instance.Clear();
    }
}
