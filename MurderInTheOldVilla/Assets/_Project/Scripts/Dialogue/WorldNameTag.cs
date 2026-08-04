using UnityEngine;

namespace MurderVilla.Dialogue
{
    public sealed class WorldNameTag : MonoBehaviour
    {
        private void LateUpdate()
        {
            Camera activeCamera = Camera.main;
            if (activeCamera == null)
                return;

            transform.rotation = activeCamera.transform.rotation;
        }
    }
}
