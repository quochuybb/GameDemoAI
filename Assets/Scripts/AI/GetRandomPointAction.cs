using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Get Random Point Around Agent", 
    category: "Action/Navigation", 
    story: "Find random point around [agentObject] within [radius] and save to [outputTarget]",
    id: "get_random_point_around_agent")]
public partial class GetRandomPointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> agentObject;
    [SerializeReference] public BlackboardVariable<float> radius;
    [SerializeReference] public BlackboardVariable<Vector3> outputTarget;

    private List<Vector3> visitedPoints = new List<Vector3>();
    private int maxHistory = 15; // Nhớ 15 điểm gần nhất

    protected override Status OnStart()
    {
        if (agentObject == null || agentObject.Value == null)
        {
            return Status.Failure;
        }

        Vector3 origin = agentObject.Value.transform.position;
        Vector3 bestPoint = origin;
        float bestScore = -1f;
        bool foundValidPoint = false;

        // Sinh ra 20 điểm ngẫu nhiên để chọn ra điểm tốt nhất
        for (int i = 0; i < 20; i++)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius.Value;
            Vector3 candidatePoint = origin + randomDirection;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidatePoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                float score = CalculateExplorationScore(hit.position);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestPoint = hit.position;
                    foundValidPoint = true;
                }
            }
        }

        if (foundValidPoint)
        {
            outputTarget.Value = bestPoint;
            
            // Lưu lại điểm này vào lịch sử
            visitedPoints.Add(bestPoint);
            if (visitedPoints.Count > maxHistory)
            {
                visitedPoints.RemoveAt(0); // Xóa điểm cũ nhất để có thể quay lại
            }
            
            return Status.Success;
        }
        return Status.Failure;
    }

    private float CalculateExplorationScore(Vector3 candidate)
    {
        // Nếu chưa đi đâu cả, điểm nào cũng là điểm tốt nhất
        if (visitedPoints.Count == 0) return 1f;

        float minDistanceToVisited = float.MaxValue;
        foreach (var point in visitedPoints)
        {
            float dist = Vector3.Distance(candidate, point);
            if (dist < minDistanceToVisited)
            {
                minDistanceToVisited = dist;
            }
        }
        
        // Càng xa các điểm cũ thì điểm (score) càng cao
        return minDistanceToVisited;
    }
}