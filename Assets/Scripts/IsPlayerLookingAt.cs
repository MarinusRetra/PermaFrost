using UnityEngine;

namespace Gameplay
{
    public class IsPlayerLookingAt : MonoBehaviour
    {
        public static Camera Cam;
        public static Plane[] CameraArea;
        public Camera SelectedCamera;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Cam = SelectedCamera;
        }

        public static bool IsPlayerLookingAtObj(Collider lookingObject)
        {
            Bounds bounds = lookingObject.bounds;
            CameraArea = GeometryUtility.CalculateFrustumPlanes(Cam);
            if (GeometryUtility.TestPlanesAABB(CameraArea, bounds))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
