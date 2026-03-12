using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] GridLayoutGroup grid;
    [SerializeField] GameObject imagePrefab;
    [SerializeField] List<Sprite> cardImages;

    int rows, columns;
    [SerializeField] TMP_InputField rowsInput, columnsInput;

    #region Set Grid Layout with the correct input
    public void SetGridLayout()
    {
        if (!CorrectGridLayout()) return;

        grid.enabled = true;

        // Destroy previous children safely
        for (int i = grid.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(grid.transform.GetChild(i).gameObject);

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        columns = int.Parse(columnsInput.text);
        rows = int.Parse(rowsInput.text);
        grid.constraintCount = columns;

        int totalCards = rows * columns;       // total number of cards
        int neededPairs = totalCards / 2;      // how many unique sprites we need

        if (neededPairs > cardImages.Count)
        {
            Debug.LogError("Not enough sprites for this grid size!");
            return;
        }

        // Pick the needed sprites and duplicate each one
        List<Sprite> spritesToAssign = new List<Sprite>();
        for (int i = 0; i < neededPairs; i++)
        {
            spritesToAssign.Add(cardImages[i]);
            spritesToAssign.Add(cardImages[i]);
        }

        // Shuffle the list so pairs are in random positions
        ShuffleList(spritesToAssign);

        // Instantiate prefabs and assign the sprites
        for (int i = 0; i < totalCards; i++)
        {
            GameObject obj = Instantiate(imagePrefab, grid.transform);

            if (obj.transform.childCount > 0)
            {
                Image childImage = obj.transform.GetChild(0).GetComponent<Image>();
                if (childImage != null)
                    childImage.sprite = spritesToAssign[i];
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(grid.GetComponent<RectTransform>());
        grid.enabled = false;
    }

    bool CorrectGridLayout()
    {
        columns = int.Parse(columnsInput.text);
        rows = int.Parse(rowsInput.text);

        if (columns == 1 && rows == 1) return false;
        if (columns > 6 || rows > 6|| columns == 0 || rows == 0) return false;

        int totalCards = rows * columns;
        if (totalCards % 2 != 0) return false; // must be divisible by 2

        return true;
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
    #endregion
}
