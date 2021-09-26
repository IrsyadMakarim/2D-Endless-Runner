using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorController : MonoBehaviour
{
    [Header("Templates")]
    public List<TerrainTemplateController> terrainTemplates;
    public float TerrainTemplateWidth;

    [Header("Generator Area")]
    public Camera GameCamera;
    public float AreaStartOffset;
    public float AreaEndOffset;

    [Header("Force Early Template")]
    public List<TerrainTemplateController> EarlyTerrainTemplates;

    private const float _debugLineHeight = 10.0f;
    private List<GameObject> _spawnedTerrain;
    private float _lastGeneratedPositionX;
    private float _lastRemovedPositionX;
    //pool list
    private Dictionary<string, List<GameObject>> _pool;

    private float GetHorizontalPositionStart()
    {
        return GameCamera.ViewportToWorldPoint(new Vector2(0f, 0f)).x + AreaStartOffset;
    }

    private float GetHorizontalPositionEnd()
    {
        return GameCamera.ViewportToWorldPoint(new Vector2(1f, 0f)).x + AreaEndOffset;
    }

    private void OnDrawGizmos()
    {
        Vector3 areaStartPosition = transform.position;
        Vector3 areaEndPosition = transform.position;

        areaStartPosition.x = GetHorizontalPositionStart();
        areaEndPosition.x = GetHorizontalPositionEnd();

        Debug.DrawLine(areaStartPosition + Vector3.up * _debugLineHeight / 2,
            areaStartPosition + Vector3.down * _debugLineHeight / 2, Color.red);
        Debug.DrawLine(areaEndPosition + Vector3.up * _debugLineHeight / 2,
            areaEndPosition + Vector3.down * _debugLineHeight / 2, Color.red);
    }
    // Start is called before the first frame update
    private void Start()
    {
        _pool = new Dictionary<string, List<GameObject>>();

        _spawnedTerrain = new List<GameObject>();

        _lastGeneratedPositionX = GetHorizontalPositionStart();
        _lastRemovedPositionX = _lastGeneratedPositionX - TerrainTemplateWidth;

        foreach (TerrainTemplateController terrain in EarlyTerrainTemplates)
        {
            GenerateTerrain(_lastGeneratedPositionX, terrain);
            _lastGeneratedPositionX += TerrainTemplateWidth;
        }

        while (_lastGeneratedPositionX < GetHorizontalPositionEnd())
        {
            GenerateTerrain(_lastGeneratedPositionX);
            _lastGeneratedPositionX += TerrainTemplateWidth;
        }
    }

    //pool function
    private GameObject GenerateFromPool(GameObject item, Transform parent)
    {
        if (_pool.ContainsKey(item.name))
        {
            if (_pool[item.name].Count > 0)
            {
                GameObject newItemFromPool = _pool[item.name][0];
                _pool[item.name].Remove(newItemFromPool);
                newItemFromPool.SetActive(true);
                return newItemFromPool;
            }
        }
        else
        {
            //if item list not defined, create new one
            _pool.Add(item.name, new List<GameObject>());
        }

        //create new one if no item available in pool
        GameObject newItem = Instantiate(item, parent);
        newItem.name = item.name;
        return newItem;
    }

    private void ReturnToPool(GameObject item)
    {
        if (!_pool.ContainsKey(item.name))
        {
            Debug.LogError("INVALID POOL ITEM");
        }

        _pool[item.name].Add(item);
        item.SetActive(false);
    }

    private void GenerateTerrain(float posX, TerrainTemplateController forceTerrain = null)
    {
        GameObject newTerrain = Instantiate(terrainTemplates[Random.Range(0, terrainTemplates.Count)].gameObject, transform);
        newTerrain.transform.position = new Vector2(posX, 0f);
        _spawnedTerrain.Add(newTerrain);
    }

    // Update is called once per frame
    private void Update()
    {
        while (_lastGeneratedPositionX < GetHorizontalPositionEnd())
        {
            GenerateTerrain(_lastGeneratedPositionX);
            _lastGeneratedPositionX += TerrainTemplateWidth;
        }

        while (_lastRemovedPositionX + TerrainTemplateWidth < GetHorizontalPositionStart())
        {
            _lastRemovedPositionX += TerrainTemplateWidth;
            RemoveTerrain(_lastRemovedPositionX);
        }
    }

    private void RemoveTerrain(float posX)
    {
        GameObject terrainToRemove = null;

        //find terrain at posX
        foreach (GameObject item in _spawnedTerrain)
        {
            if (item.transform.position.x == posX)
            {
                terrainToRemove = item;
                break;
            }
        }

        //after found
        if (terrainToRemove != null)
        {
            _spawnedTerrain.Remove(terrainToRemove);
            Destroy(terrainToRemove);
        }
    }
}
