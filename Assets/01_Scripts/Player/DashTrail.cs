using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTrail : MonoBehaviour
{
    [Header("Mesh Settings")]
    [Range(0.01f, 0.5f)]
    public float meshRefreshRate = 0.025f;
    public float meshDestroyDelay = 1f;
    public Transform positionToSpawn;

    [Header("Shader Settings")]
    public Material sourceMaterial; 

    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private float trailTimer;
    private bool isTrailActive = false;
    
    private List<TrailSegment> activeSegments = new List<TrailSegment>();
    
    private MaterialPropertyBlock mpb;
    private int alphaPropertyID;

    private class TrailSegment
    {
        public Mesh Mesh { get; set; }
        public Matrix4x4 Matrix { get; set; }
        public float RemainingTime { get; set; }
        public float Alpha { get; set; }

        public void Clear()
        {
            if (Mesh != null) Mesh.Clear();
            RemainingTime = 0;
        }
    }
    
    private Queue<TrailSegment> segmentPool = new Queue<TrailSegment>();
    

    
    void Start()
    {
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        mpb = new MaterialPropertyBlock();
        alphaPropertyID = Shader.PropertyToID("_Alpha");
    }

    public void Activate(bool activate)
    {
        isTrailActive = activate;
    }
    
    public void ActivateForDuration(float duration)
    {
        StartCoroutine(TimedActivationRoutine(duration));
    }

    private IEnumerator TimedActivationRoutine(float duration)
    {
        Activate(true);

        yield return new WaitForSeconds(duration);

        Activate(false);
    }

    void Update()
    {
        if (isTrailActive)
        {
            trailTimer -= Time.deltaTime;
            if (trailTimer <= 0f)
            {
                trailTimer = meshRefreshRate;
                SpawnTrailSegments();
            }
        }
        
        UpdateAndDrawSegments();
    }

    private void SpawnTrailSegments()
    {
        if (skinnedMeshRenderers == null) return;
        
        Matrix4x4 spawnMatrix = Matrix4x4.TRS(positionToSpawn.position, positionToSpawn.rotation, Vector3.one * 1.5f);

        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            TrailSegment segment = segmentPool.Count > 0 ? segmentPool.Dequeue() : new TrailSegment();
            if (segment.Mesh == null) segment.Mesh = new Mesh();

            skinnedMeshRenderers[i].BakeMesh(segment.Mesh, true);
            segment.Matrix = spawnMatrix;
            segment.RemainingTime = meshDestroyDelay;
            segment.Alpha = 1.0f;
            
            activeSegments.Add(segment);
        }
    }

    private void UpdateAndDrawSegments()
    {
        for (int i = activeSegments.Count - 1; i >= 0; i--)
        {
            var segment = activeSegments[i];
            
            segment.RemainingTime -= Time.deltaTime;
            segment.Alpha = segment.RemainingTime / meshDestroyDelay;

            if (segment.Alpha <= 0)
            {
                activeSegments.RemoveAt(i);
                segmentPool.Enqueue(segment);
                continue;
            }

            mpb.SetFloat(alphaPropertyID, segment.Alpha);
            
            Graphics.DrawMesh(
                segment.Mesh,
                segment.Matrix,
                sourceMaterial,
                gameObject.layer, // 트레일이 그려질 레이어
                null,             // Camera (null이면 모든 카메라)
                0,                // Submesh index
                mpb               // MaterialPropertyBlock
            );
        }
    }
}