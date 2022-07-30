using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        #region Parameters
        [SerializeField]
        private float _waypointGizmoRadius = 0.2f;
        #endregion

        ////////////////////////////////////////////////

        #region EngineMethods
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                
                Gizmos.DrawSphere(GetWaypoint(i), _waypointGizmoRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }
        #endregion

        #region PublicMethods
        public int GetNextIndex(int i)
        {
            return transform.childCount == i + 1 ? 0 : i + 1;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
        #endregion
    }
}
