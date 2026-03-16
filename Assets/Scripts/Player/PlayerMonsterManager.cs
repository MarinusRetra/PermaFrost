using UnityEngine;

namespace Gameplay
{
    public class PlayerMonsterManager : MonoBehaviour
    {
        public bool HasFoundTicket = false;
        public bool InSideroom = false;

        //IsPlayerLookingAt
        private static Camera _cam;
        private static Plane[] _cameraArea;
        public Camera SelectedCamera;
        void Awake()
        {
            _cam = SelectedCamera;
        }

        public static bool IsPlayerLookingAtObj(Collider lookingObject, bool lookThroughWalls = false)
        {
            //Check if object is in camera reach
            Bounds bounds = lookingObject.bounds;
            _cameraArea = GeometryUtility.CalculateFrustumPlanes(_cam);
            if (GeometryUtility.TestPlanesAABB(_cameraArea, bounds))
            {
                //check for walls
                if (lookThroughWalls)
                {
                    return true;
                }
                Ray ray = new Ray(lookingObject.transform.position, (_cam.transform.position - lookingObject.transform.position));
                if (Physics.Raycast(ray, out RaycastHit hit, 20))
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

        public bool OverridePlayerSounds = false;
        //Noise

        /// <summary>
        /// Fill in a location for them to go there
        /// </summary>
        /// <param name="location"></param>
        public static void MakeNoise(Vector3 location)
        {
            var allEars = FindObjectsByType<AllEars>(FindObjectsSortMode.None);
            for (int i = 0; i < allEars.Length; i++)
            {
                allEars[i].Aggro(location);
            }
        }

        /// <summary>
        /// Defaults to player
        /// </summary>
        public static void MakeNoise()
        {
            if (PlrRefs.inst.PlayerMonsterManager.OverridePlayerSounds) { return; }
            var allEars = FindObjectsByType<AllEars>(FindObjectsSortMode.None);
            for (int i = 0; i < allEars.Length; i++)
            {
                allEars[i].Aggro(PlrRefs.inst.transform.position);
            }
        }

        public void GrabTicket()
        {
            HasFoundTicket = true;
        }

        public void ResetTicket()
        {
            HasFoundTicket = false;
        }
    }
}
