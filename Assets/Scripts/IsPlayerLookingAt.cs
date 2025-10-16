using UnityEngine;

namespace Gameplay
{
    public class IsPlayerLookingAt : MonoBehaviour
    {
        public static Camera Cam;
        public static Plane[] CameraArea;
        public Camera SelectedCamera;
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
                //check for walls
                Ray ray = new Ray(lookingObject.transform.position, (Cam.transform.position - lookingObject.transform.position));
                if(Physics.Raycast(ray, out RaycastHit hit, 20))
                {
                    if (hit.collider != null && hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
