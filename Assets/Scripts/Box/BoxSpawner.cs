using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject boxPrefab;

    [Header("Settings")]
    [SerializeField] private float boxesPerRow = 5;
    [SerializeField] private float rows = 3;
    [SerializeField] private float columnSpacing = 1.2f;
    [SerializeField] private float rowSpacing = 1.2f;

    List<GameObject> boxes = new List<GameObject>();

    /* Clears the entire grid of boxes */
    public void clearBoxes(){
        foreach (GameObject box in boxes){
            if (box != null){
                Destroy(box);
            }
        }
        boxes.Clear();
    }

    /* brings down all the current boxes and spawns a new row */
    public void spawnNewRow(){
        // bring down all the current boxes
        foreach (GameObject box in boxes){
            if (box != null){
                Vector3 oldPosition = box.transform.position;
                box.transform.position -= new Vector3(0, rowSpacing, 0);
            }
        }

        // spawn a new row
        for(int i = 0; i < boxesPerRow; i++){
            Vector3 spawnerPosition = transform.position;
            Vector2 spawnPosition = new Vector2(i * columnSpacing, 0) + (Vector2)spawnerPosition;
            spawnBox(spawnPosition);
        }
    }

    public void spawnBoxes(){
        for(int i = 0; i < rows; i++){
            for(int j = 0; j < boxesPerRow; j++){
                Vector3 spawnerPosition = transform.position;
                Vector2 spawnPosition = new Vector2(j * columnSpacing, -i * rowSpacing) + (Vector2)spawnerPosition;
                spawnBox(spawnPosition);
            }
        }
    }

    private void spawnBox(Vector2 position){
        GameObject box = Instantiate(boxPrefab, position, Quaternion.identity, transform);
        BoxComponent boxComponent = box.GetComponent<BoxComponent>();
        boxComponent.setSpawner(this);
        boxComponent.onBoxDamaged.AddListener(GameManager.Instance.onBoxDamaged);
        boxes.Add(box);
    }

    public List<GameObject> getBoxes(){
        return boxes;
    }
}
