// Yutaka ReiRoku
// Place this script in an "Editor" folder
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

public class CsvToListImporterWindow : EditorWindow
{
    private TextAsset csvFile; // csv File
    private ScriptableObject targetDatabase; // SO asset

    [MenuItem("Tools/Import CSV to List")]
    public static void ShowWindow()
    {
        GetWindow<CsvToListImporterWindow>("Import CSV to List");
    }

    void OnGUI()
    {
        GUILayout.Label("Import CSV to ScriptableObject List", EditorStyles.boldLabel);

        csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV File", csvFile, typeof(TextAsset), false);

        targetDatabase = (ScriptableObject)EditorGUILayout.ObjectField("Target Database (ScriptableObject)", targetDatabase, typeof(ScriptableObject), false);

        if (GUILayout.Button("Import Data to List"))
        {
            if (csvFile != null && targetDatabase != null)
            {
                ImportData();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Require csv file and SO asset.", "OK");
            }
        }
    }

    private void ImportData()
    {
        string itemsList = "allItems"; // Change "allItems" if named it differently in SO

        FieldInfo listField = targetDatabase.GetType().GetField(itemsList); 
        if (listField == null)
        {
            Debug.LogError("Could not find the " + itemsList + " list in the ScriptableObject!");
            return;
        }

        System.Type itemType = listField.FieldType.GetGenericArguments()[0];

        FieldInfo[] itemFields = itemType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        var list = listField.GetValue(targetDatabase) as System.Collections.IList;
        list.Clear();

        string[] lines = csvFile.text.Split('\n');
        string[] headers = lines[0].Trim().Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Trim().Split(',');

            object newItem = System.Activator.CreateInstance(itemType);

            for (int j = 0; j < headers.Length; j++)
            {
                string header = headers[j].Trim();
                FieldInfo field = itemFields.FirstOrDefault(f => f.Name == header);
                if (field != null)
                {
                    object convertedValue = System.Convert.ChangeType(values[j], field.FieldType);
                    field.SetValue(newItem, convertedValue);
                }
            }

            list.Add(newItem);  
        }

        EditorUtility.SetDirty(targetDatabase);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Complete", $"Successfully imported {list.Count} items into '{targetDatabase.name}'.", "OK");
    }
}

/*csv horizontal file

for instance:

itemID, itemName, description, price
1, Sword,"A basic sword.",100
2, Shield,"A wooden shield.",75
3, Potion,"Restores 50 HP.",25*/