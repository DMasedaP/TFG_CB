using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tree treePrefab;
    [SerializeField] private Transform treeParent;
    [SerializeField] private GridBuildingSystem gridSystem;

    [Header("Cantidad")]
    [SerializeField] private int maxTrees = 60;
    [SerializeField] private int initialTreesToSpawn = 25;
    [SerializeField] private bool registerExistingTrees = true;

    [Header("Tamaño en Grid")]
    [SerializeField] private int treeWidth = 1;
    [SerializeField] private int treeHeight = 1;

    [Header("Tiempo de generación")]
    [SerializeField] private float minSpawnDelay = 15f;
    [SerializeField] private float maxSpawnDelay = 35f;
    [SerializeField] private int treesPerSpawn = 1;

    [Header("Grid")]
    [SerializeField] private int minGridX = 0;
    [SerializeField] private int maxGridX = 40;
    [SerializeField] private int minGridY = 0;
    [SerializeField] private int maxGridY = 40;

    [Header("Transformación")]
    [SerializeField] private Vector2 scaleRange = new Vector2(0.9f, 1.2f);
    [SerializeField] private bool randomYRotation = true;

    [Header("Intentos")]
    [SerializeField] private int maxSpawnAttempts = 40;

    private readonly List<Tree> activeTrees = new List<Tree>();

    private void Start()
    {
        if (treePrefab == null)
        {
            Debug.LogError("TreeSpawner: falta asignar el prefab del árbol.");
            enabled = false;
            return;
        }

        if (gridSystem == null)
        {
            Debug.LogError("TreeSpawner: falta asignar el GridBuildingSystem.");
            enabled = false;
            return;
        }

        if (registerExistingTrees)
            RegisterExistingTrees();

        SpawnInitialTrees();

        StartCoroutine(SpawnRoutine());
    }

    private void RegisterExistingTrees()
    {
        Tree[] existingTrees = FindObjectsOfType<Tree>();

        foreach (Tree tree in existingTrees)
        {
            if (tree == null)
                continue;

            if (tree == treePrefab)
                continue;

            gridSystem.GetXY(tree.transform.position, out int x, out int y);

            if (!gridSystem.CanPlaceBuilding(x, y, treeWidth, treeHeight))
            {
                Debug.LogWarning("Hay un árbol existente colocado sobre una celda ya ocupada: " + tree.name);
                continue;
            }

            gridSystem.SetOccupied(x, y, treeWidth, treeHeight, true);
            tree.RegisterInGrid(gridSystem, x, y, treeWidth, treeHeight);

            if (!activeTrees.Contains(tree))
                activeTrees.Add(tree);
        }
    }

    private void SpawnInitialTrees()
    {
        int spawned = 0;
        int safetyAttempts = initialTreesToSpawn * maxSpawnAttempts;

        while (spawned < initialTreesToSpawn && GetActiveTreeCount() < maxTrees && safetyAttempts > 0)
        {
            safetyAttempts--;

            if (TrySpawnTree())
                spawned++;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            CleanDestroyedTrees();

            if (GetActiveTreeCount() >= maxTrees)
                continue;

            for (int i = 0; i < treesPerSpawn; i++)
            {
                if (GetActiveTreeCount() >= maxTrees)
                    break;

                TrySpawnTree();
            }
        }
    }

    private bool TrySpawnTree()
    {
        CleanDestroyedTrees();

        if (GetActiveTreeCount() >= maxTrees)
            return false;

        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            int x = Random.Range(minGridX, maxGridX + 1);
            int y = Random.Range(minGridY, maxGridY + 1);

            if (!gridSystem.CanPlaceBuilding(x, y, treeWidth, treeHeight))
                continue;

            SpawnTreeAtGridPosition(x, y);
            return true;
        }

        return false;
    }

    private void SpawnTreeAtGridPosition(int x, int y)
    {
        Vector3 position = gridSystem.GetCellCenterWorld(x, y);

        Quaternion rotation = Quaternion.identity;

        if (randomYRotation)
            rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        Tree newTree = Instantiate(treePrefab, position, rotation, treeParent);
        Debug.LogError($"@@ Spawneo arbol en {position}");

        float randomScale = Random.Range(scaleRange.x, scaleRange.y);
        newTree.transform.localScale = Vector3.one * randomScale;

        gridSystem.SetOccupied(x, y, treeWidth, treeHeight, true);
        newTree.RegisterInGrid(gridSystem, x, y, treeWidth, treeHeight);

        activeTrees.Add(newTree);
    }

    private int GetActiveTreeCount()
    {
        CleanDestroyedTrees();
        return activeTrees.Count;
    }

    private void CleanDestroyedTrees()
    {
        activeTrees.RemoveAll(tree => tree == null);
    }
}