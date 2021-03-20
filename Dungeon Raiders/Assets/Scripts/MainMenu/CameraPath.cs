using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraPath : MonoBehaviour
{
    public List<CameraPathSegment> segments;

    [Header("Transition settings")]

    public float totalDuration;

    public AnimationCurve progressCurve;

    public float positionSmoothing = 60;
    public float rotationSmoothing = 10;

    [Header("Debugging")]
    [Range(0,1)]
    public float ProgressPreview;

    public Vector3 InitialPosition => segments != null && segments[0] != null ? 
        segments[0].startNode.Position : this.transform.position;
    public Vector3 FinalPosition => segments != null && segments[segments.Count-1] != null ? 
        segments[segments.Count - 1].endNode.Position : this.transform.position;
    public Quaternion InitialRotation => segments != null && segments[0] != null ?
        segments[0].startNode.Rotation : this.transform.rotation;
    public Quaternion FinalRotation => segments != null && segments[segments.Count - 1] != null ?
        segments[segments.Count - 1].endNode.Rotation : this.transform.rotation;
    public float InitialFOV => segments != null && segments[0] != null ?
        segments[0].startNode.FOV : 60;
    public float FinalFOV => segments != null && segments[segments.Count - 1] != null ?
        segments[segments.Count - 1].endNode.FOV : 60;

    public Vector3 GetNormalizedPosition(float progress)
    {
        return GetSegment(progress).GetPosition(progress);
    }
    public CameraPathSegment GetSegment(float progress)
    {
        return segments.FirstOrDefault(s => s.weightStart <= progress && s.weightEnd >= progress);
    }
    public float TotalLength
    {
        get
        {
            var totalLength = 0f;
            foreach (var segment in segments)
                totalLength += segment.Length;
            return totalLength;
        }
    }

    public void UpdateSegments(List<CameraPathNode> nodes)
    {
        var totalLength = 0f;

        segments = new List<CameraPathSegment>();
        var segmentsCount = nodes.Count - 1;
        for (int i = 0; i < segmentsCount; i++)
        {
            var segment = new CameraPathSegment();

            segment.startNode = nodes[i];
            segment.endNode = nodes[i + 1];

            totalLength += segment.Length;

            segments.Add(segment);
        }

        var tempLength = totalLength;
        for (int i = segmentsCount-1; i > -1; i--)
        {
            segments[i].weightEnd = i == segmentsCount - 1 ? 1 : tempLength / totalLength;
            tempLength -= segments[i].Length;
            segments[i].weightStart = i == 0 ? 0 : tempLength / totalLength;
        }
    }

    private void OnDrawGizmosSelected()
    {
        //var activeSegment = GetSegment(ProgressPreview);
        //for (int i = 0; i < nodes.Count-1; i++)
        //{
        //    if (nodes[i] == activeSegment.startNode)
        //        Gizmos.color = Color.cyan;
        //    else
        //        Gizmos.color = Color.magenta;

        //    Gizmos.DrawWireSphere(nodes[i].Position, 0.5f);
        //    Gizmos.DrawWireSphere(nodes[i + 1].Position, 0.5f);

        //    Gizmos.DrawLine(nodes[i].Position, nodes[i+1].Position);
        //}

        Gizmos.color = Color.magenta;
        for (int i = 0; i < segments.Count; i++)
        {
            Gizmos.DrawLine(segments[i].startNode.Position, segments[i].endNode.Position);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetNormalizedPosition(ProgressPreview), 0.5f);
    }
}

[System.Serializable]
public class CameraPathSegment
{
    public CameraPathNode startNode;
    public float weightStart;

    public CameraPathNode endNode;
    public float weightEnd;

    public float WeightDelta => weightEnd - weightStart;

    public Vector3 EulerDelta => endNode.transform.eulerAngles - startNode.transform.eulerAngles;

    public float FOVDelta => endNode.FOV - startNode.FOV;

    public float Length => (endNode.Position - startNode.Position).magnitude;

    public Vector3 Direction => (endNode.Position - startNode.Position).normalized;

    public Vector3 GetPosition(float progress)
    {
        progress = Mathf.Clamp(progress, weightStart, weightEnd);

        var portion = (progress - weightStart) / WeightDelta;

        return startNode.Position + Direction * Length * portion;
    }

    public float GetFOV(float progress)
    {
        progress = Mathf.Clamp(progress, weightStart, weightEnd);

        var portion = (progress - weightStart) / WeightDelta;

        return startNode.FOV + FOVDelta * portion;
    }

    public Vector3 GetRotation(float progress)
    {
        progress = Mathf.Clamp(progress, weightStart, weightEnd);

        var portion = (progress - weightStart) / WeightDelta;

        return startNode.transform.eulerAngles + EulerDelta * portion;
    }
}
