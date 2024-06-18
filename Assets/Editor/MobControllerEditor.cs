using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MobController), true)] // 이제 모든 MobController 파생 클래스에 대해 이 에디터를 사용할 수 있습니다.
public class MobControllerEditor : Editor
{
    private float _handleSize = 30f; // Scene view에서의 핸들 크기를 더 작게 조정
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // 기존 인스펙터 GUI 요소를 그린다.

        MobController mob = (MobController)target;
        if (GUILayout.Button("Initialize Patrol Points"))
        {
            Vector3 center = mob.transform.position;
            Vector3 pointA = center + new Vector3(-350f, 0f, 0f);
            Vector3 pointB = center + new Vector3(350f, 0f, 0f);
        
            mob.SetPatrolPoints(pointA, pointB);
        
            EditorUtility.SetDirty(mob);
        }
    }

    protected void OnSceneGUI()
    {
        MobController mob = (MobController)target;
        EditorGUI.BeginChangeCheck();

        Vector3 pointAWorld = mob.GetPatrolPointA();
        Vector3 pointBWorld = mob.GetPatrolPointB();

        // 현재 상태에 따라 범위를 다르게 그리기
        IMobState currentState = mob.GetCurrentState();
        if (currentState is PatrolState)
        {
            DrawDetectionRange(mob);
            DrawAttackRange(mob);
        }
        else if (currentState is ChaseState) {
            DrawChasableRange(mob);
            DrawAttackRange(mob);
        }
        else if (currentState is AttackState)
        {
            DrawAttackRange(mob);
        }
        
        Handles.color = Color.red;
        var fmh_36_59_638505562907255502 = Quaternion.identity; pointAWorld = Handles.FreeMoveHandle(pointAWorld, _handleSize, Vector3.zero, Handles.SphereHandleCap);
        Handles.Label(pointAWorld, "Patrol Point A");

        Handles.color = Color.blue;
        var fmh_40_59_638505562907279895 = Quaternion.identity; pointBWorld = Handles.FreeMoveHandle(pointBWorld, _handleSize, Vector3.zero, Handles.SphereHandleCap);
        Handles.Label(pointBWorld, "Patrol Point B");

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(mob, "Change Patrol Points");
            mob.SetPatrolPoints(pointAWorld, pointBWorld);
            EditorUtility.SetDirty(mob);
        }

        Handles.DrawLine(pointAWorld, pointBWorld);
    }
    
    private void DrawDetectionRange(MobController mob)
    {
        Handles.color = Color.yellow;
        Handles.DrawWireArc(mob.transform.position, Vector3.forward, Vector3.up, 360, mob.GetDetectionRange());
    }

    private void DrawChasableRange(MobController mob)
    {
        Handles.color = Color.yellow;
        Handles.DrawWireArc(mob.transform.position, Vector3.forward, Vector3.up, 360, mob.GetChasableRange());
    }

    private void DrawAttackRange(MobController mob)
    {
        Handles.color = Color.red;
        Handles.DrawWireArc(mob.transform.position, Vector3.forward, Vector3.up, 360, mob.GetAttackRange());
    }
}
