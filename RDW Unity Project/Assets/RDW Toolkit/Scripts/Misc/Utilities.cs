using UnityEngine;
using System.Collections.Generic;


namespace Redirection
{
    public static class Utilities
    {

        public static Vector3 FlattenedPos3D(Vector3 vec, float height = 0)
        {
            return new Vector3(vec.x, height, vec.z);
        }

        public static Vector2 FlattenedPos2D(Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }

        public static Vector3 FlattenedDir3D(Vector3 vec)
        {
            return (new Vector3(vec.x, 0, vec.z)).normalized;
        }

        public static Vector2 FlattenedDir2D(Vector3 vec)
        {
            return new Vector2(vec.x, vec.z).normalized;
        }

        public static Vector3 UnFlatten(Vector2 vec, float height = 0)
        {
            return new Vector3(vec.x, height, vec.y);
        }

        /// <summary>
        /// Gets angle from prevDir to currDir in degrees, assuming the vectors lie in the xz plane (with left handed coordinate system).
        /// </summary>
        /// <param name="currDir"></param>
        /// <param name="prevDir"></param>
        /// <returns></returns>
        public static float GetSignedAngle(Vector3 prevDir, Vector3 currDir)
        {
            return Mathf.Sign(Vector3.Cross(prevDir, currDir).y) * Vector3.Angle(prevDir, currDir);
        }

        public static Vector3 GetRelativePosition(Vector3 pos, Transform origin)
        {
            return Quaternion.Inverse(origin.rotation) * (pos - origin.position);
        }

        public static Vector3 GetRelativeDirection(Vector3 dir, Transform origin)
        {
            return Quaternion.Inverse(origin.rotation) * dir;
        }

        // Based on: http://stackoverflow.com/questions/4780119/2d-euclidean-vector-rotations
        // FORCED LEFT HAND ROTATION AND DEGREES
        public static Vector2 RotateVector(Vector2 fromOrientation, float thetaInDegrees)
        {
            Vector2 ret = Vector2.zero;
            float cos = Mathf.Cos(-thetaInDegrees * Mathf.Deg2Rad);
            float sin = Mathf.Sin(-thetaInDegrees * Mathf.Deg2Rad);
            ret.x = fromOrientation.x * cos - fromOrientation.y * sin;
            ret.y = fromOrientation.x * sin + fromOrientation.y * cos;
            return ret;
        }

        public static bool Approximately(Vector2 v0, Vector2 v1)
        {
            return Mathf.Approximately(v0.x, v1.x) && Mathf.Approximately(v0.y, v1.y);
        }

        // Get nearest distance between currPosReal and obstacles/boundaries
        public static System.Tuple<float, Vector2> GetNearestDistAndPosToObstacleAndTrackingSpace(List<Vector2> space,
            Vector2 currPos)
        {
            var nearestPosList = new List<Vector2>();
            //var space = physicalSpaces[physicalSpaceIndex];
            for (int i = 0; i < space.Count; i++)
            {
                var p = space[i];
                var q = space[(i + 1) % space.Count];
                var nearestPos = GetNearestPos(currPos, new List<Vector2> { p, q });
                nearestPosList.Add(nearestPos);
            }

            //Get min distance
            float dis = float.MaxValue;
            Vector2 pos = Vector2.zero;
            foreach (var p in nearestPosList)
            {
                if ((currPos - p).magnitude < dis)
                {
                    dis = (currPos - p).magnitude;
                    pos = p;
                }
            }

            return new System.Tuple<float, Vector2>(dis, pos);
        }

        //Get Nearest point of the obstacle to the pos, obstacle: vertices of a static obstacle
        public static Vector2 GetNearestPos(Vector2 pos, List<Vector2> obstacle)
        {
            float minDist = float.MaxValue; //record min dist
            var rePos = Vector2.zero;
            for (int i = 0; i < obstacle.Count; i++)
            {
                var p = obstacle[i];
                var q = obstacle[(i + 1) % obstacle.Count];

                //foot of a perpendicular is on the segment
                if (Vector2.Dot(q - p, pos - p) > 0 && Vector2.Dot(p - q, pos - q) > 0)
                {
                    var perp = p + (q - p).normalized * Vector2.Dot(q - p, pos - p) /
                        (q - p).magnitude; //foot of a perpendicular

                    var dist = (pos - perp).magnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        rePos = perp;
                    }
                }
                else //foot of a perpendicular outside the segment, only caculate the distance between vertices
                {
                    var dist = (pos - p).magnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        rePos = p;
                    }

                    dist = (pos - q).magnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        rePos = q;
                    }
                }
            }

            return rePos;
        }
    }
}