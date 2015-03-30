using UnityEngine;

namespace NeonShooter.Utils
{
    public static class GameObjectHelper
    {
        public static void SetParentDontMessUpCoords(GameObject childToBe, GameObject parentToBe)
        {
            var oldTranslation = childToBe.transform.localPosition;
            var oldRotation = childToBe.transform.localRotation;
            var oldScale = childToBe.transform.localScale;

            childToBe.transform.parent = parentToBe.transform;

            childToBe.transform.localPosition = oldTranslation;
            childToBe.transform.localRotation = oldRotation;
            childToBe.transform.localScale = oldScale;
        }
    }
}
