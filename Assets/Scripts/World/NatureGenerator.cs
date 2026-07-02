using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NatureGenerator : MonoBehaviour
{
    private const int MaxBatchSize = 1023;

    private class InstancedBatch
    {
        public Mesh mesh;
        public Matrix4x4[] matrices;
        public int count;
    }

    [Header("Modelos")]
    [SerializeField] private Material sharedMaterial;
    [SerializeField] private List<Mesh> models = new List<Mesh>();

    [Header("Mapa")]
    [SerializeField] private Vector2 mapSize = new Vector2(100f, 100f);
    [SerializeField] private Vector3 mapCenter = Vector3.zero;
    [SerializeField] private float fixedY = 0f;
    [SerializeField] private float borderMargin = 3f;

    [Header("Generación")]
    [SerializeField] private int totalInstances = 1000;
    [SerializeField] private bool useRandomSeed = false;
    [SerializeField] private int seed = 12345;
    [SerializeField] private Vector2 scaleRange = new Vector2(0.8f, 1.3f);
    [SerializeField] private float minDistanceBetweenObjects = 1.5f;
    [SerializeField] private int maxTriesPerInstance = 20;

    [Header("Suelo")]
    [SerializeField] private bool raycastToGround = false;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float raycastHeight = 50f;

    [Header("Render")]
    [SerializeField] private ShadowCastingMode castShadows = ShadowCastingMode.On;
    [SerializeField] private bool receiveShadows = true;
    [SerializeField] private int renderLayer = 0;

    private readonly List<InstancedBatch> batches = new List<InstancedBatch>();

    private void Awake()
    {
        if (sharedMaterial == null)
        {
            Debug.LogError("NatureGenerator: falta asignar el material compartido.");
            enabled = false;
            return;
        }

        if (models == null || models.Count == 0)
        {
            Debug.LogError("NatureGenerator: no hay meshes asignadas en Models.");
            enabled = false;
            return;
        }

        sharedMaterial.enableInstancing = true;

        GenerateInstances();
    }

    private void LateUpdate()
    {
        DrawInstances();
    }

    private void GenerateInstances()
    {
        batches.Clear();

        System.Random random = new System.Random(useRandomSeed ? System.Environment.TickCount : seed);

        Dictionary<Mesh, List<Matrix4x4>> matricesByMesh = new Dictionary<Mesh, List<Matrix4x4>>();
        List<Vector2> usedPositions = new List<Vector2>();

        int placed = 0;
        int failed = 0;

        for (int i = 0; i < totalInstances; i++)
        {
            bool placedThisInstance = false;

            for (int attempt = 0; attempt < maxTriesPerInstance; attempt++)
            {
                Vector3 position = GetRandomPosition(random);
                Vector2 position2D = new Vector2(position.x, position.z);

                if (minDistanceBetweenObjects > 0f && !IsFarEnough(position2D, usedPositions))
                    continue;

                if (raycastToGround && !TrySnapToGround(ref position))
                    continue;

                Mesh mesh = GetRandomMesh(random);

                if (mesh == null)
                    continue;

                float scale = RandomRange(random, scaleRange.x, scaleRange.y);
                float randomYRotation = RandomRange(random, 0f, 360f);

                Matrix4x4 matrix = Matrix4x4.TRS(
                    position,
                    Quaternion.Euler(0f, randomYRotation, 0f),
                    Vector3.one * scale
                );

                if (!matricesByMesh.TryGetValue(mesh, out List<Matrix4x4> matrices))
                {
                    matrices = new List<Matrix4x4>();
                    matricesByMesh.Add(mesh, matrices);
                }

                matrices.Add(matrix);
                usedPositions.Add(position2D);

                placed++;
                placedThisInstance = true;
                break;
            }

            if (!placedThisInstance)
                failed++;
        }

        CreateBatches(matricesByMesh);

        Debug.Log($"Atrezzo generado: {placed} instancias. Fallidas: {failed}. Batches: {batches.Count}.");
    }

    private void DrawInstances()
    {
        for (int i = 0; i < batches.Count; i++)
        {
            InstancedBatch batch = batches[i];

            Graphics.DrawMeshInstanced(
                batch.mesh,
                0,
                sharedMaterial,
                batch.matrices,
                batch.count,
                null,
                castShadows,
                receiveShadows,
                renderLayer,
                null,
                LightProbeUsage.Off
            );
        }
    }

    private void CreateBatches(Dictionary<Mesh, List<Matrix4x4>> matricesByMesh)
    {
        foreach (KeyValuePair<Mesh, List<Matrix4x4>> pair in matricesByMesh)
        {
            Mesh mesh = pair.Key;
            List<Matrix4x4> matrices = pair.Value;

            int index = 0;

            while (index < matrices.Count)
            {
                int count = Mathf.Min(MaxBatchSize, matrices.Count - index);
                Matrix4x4[] batchMatrices = new Matrix4x4[count];

                matrices.CopyTo(index, batchMatrices, 0, count);

                batches.Add(new InstancedBatch
                {
                    mesh = mesh,
                    matrices = batchMatrices,
                    count = count
                });

                index += count;
            }
        }
    }

    private Vector3 GetRandomPosition(System.Random random)
    {
        float halfX = Mathf.Max(0f, mapSize.x * 0.5f - borderMargin);
        float halfZ = Mathf.Max(0f, mapSize.y * 0.5f - borderMargin);

        float x = RandomRange(random, -halfX, halfX);
        float z = RandomRange(random, -halfZ, halfZ);

        return new Vector3(
            mapCenter.x + x,
            fixedY,
            mapCenter.z + z
        );
    }

    private Mesh GetRandomMesh(System.Random random)
    {
        int index = random.Next(0, models.Count);
        return models[index];
    }

    private bool TrySnapToGround(ref Vector3 position)
    {
        Vector3 origin = position + Vector3.up * raycastHeight;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, raycastHeight * 2f, groundMask))
        {
            position = hit.point;
            return true;
        }

        return false;
    }

    private bool IsFarEnough(Vector2 position, List<Vector2> usedPositions)
    {
        float minSqrDistance = minDistanceBetweenObjects * minDistanceBetweenObjects;

        for (int i = 0; i < usedPositions.Count; i++)
        {
            if ((position - usedPositions[i]).sqrMagnitude < minSqrDistance)
                return false;
        }

        return true;
    }

    private float RandomRange(System.Random random, float min, float max)
    {
        return Mathf.Lerp(min, max, (float)random.NextDouble());
    }
}