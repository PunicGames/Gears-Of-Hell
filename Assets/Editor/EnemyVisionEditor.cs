using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyVision))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyVision fov = (EnemyVision)target;
        Vector3 viewAngle01 = DirectionFromAngle(fov.parent.eulerAngles.y, -fov.angle *0.5f);
        Vector3 viewAngle02 = DirectionFromAngle(fov.parent.eulerAngles.y, fov.angle *0.5f);

        Handles.color = Color.yellow;
        Handles.DrawWireArc(fov.transform.position, fov.transform.up, Quaternion.AngleAxis(fov.angle*.5f, fov.transform.up) *fov.parent.forward, -fov.angle, fov.perceptionRadius);
        Handles.color = Color.red;
        Handles.DrawWireArc(fov.transform.position, fov.transform.up, Quaternion.AngleAxis(fov.angle * .5f, fov.transform.up) * fov.parent.forward, -fov.angle, fov.spotRadius);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.perceptionRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.perceptionRadius);
        Handles.color = Color.red;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.spotRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.spotRadius);

        if (fov.playerInReach)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.playerRef.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}