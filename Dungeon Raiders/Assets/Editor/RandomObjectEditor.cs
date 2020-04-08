using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomObject))]
public class RandomObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var randomObject = (RandomObject)target;

        int removalIndex;

        #region Защита от багов


        if (randomObject.types == null)
            randomObject.types = new List<GameObject>();
        if (randomObject.chances == null)
            randomObject.chances = new List<float>();
        if (randomObject.randomLines == null)
            randomObject.randomLines = new List<bool>();

        int countTypes = randomObject.types.Count;
        if (randomObject.chances.Count != countTypes)
            randomObject.chances = new List<float>(countTypes);
        if (randomObject.randomLines.Count != countTypes)
        {
            randomObject.randomLines = new List<bool>();
            foreach (var item in randomObject.types)
                randomObject.randomLines.Add(false);
        }

        if (randomObject.affects == null)
            randomObject.affects = new List<RandomObject>();
        if (randomObject.exclude == null)
            randomObject.exclude = new List<bool>();


        if (randomObject.decrease == null)
            randomObject.decrease = new List<bool>();
        if (randomObject.decreaseRate == null)
            randomObject.decreaseRate = new List<float>();

        if (randomObject.increase == null)
            randomObject.increase = new List<bool>();
        if (randomObject.increaseRate == null)
            randomObject.increaseRate = new List<float>();
        #endregion

        #region Отрисовка таблицы типов и их вероятностей

        EditorGUILayout.LabelField("Types");

        removalIndex = -1;

        EditorGUI.indentLevel += 1;

        if (randomObject.types.Count > 0)
        {
            for (int i = 0; i < randomObject.types.Count; i++)
            {
                GUILayout.BeginHorizontal();

                randomObject.types[i] = (GameObject)EditorGUILayout.ObjectField(
                    randomObject.types[i], typeof(GameObject), false);

                var chance = randomObject.chances[i] * 100;
                chance = EditorGUILayout.Slider(chance, 0, 100, GUILayout.MaxWidth(164));
                randomObject.chances[i] = chance / 100;

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                randomObject.randomLines[i] = EditorGUILayout.Toggle("Random line",
                    randomObject.randomLines[i]);

                //GUILayout.Label("");

                if (GUILayout.Button("Remove"))
                {
                    removalIndex = i;
                }

                GUILayout.EndHorizontal();

                GUILayout.Space(8);
            }
        }
        else
        {
            EditorGUILayout.LabelField("No data");
        }

        EditorGUI.indentLevel -= 1;

        // Удаление указанного объекта из списка
        if (removalIndex != -1)
        {
            randomObject.types.RemoveAt(removalIndex);
            randomObject.chances.RemoveAt(removalIndex);
            randomObject.randomLines.RemoveAt(removalIndex);
        }

        // Кнопка для добавления ячейки
        if (GUILayout.Button("Add") && randomObject.types.Count < 7)
        {
            randomObject.types.Add(null);
            randomObject.chances.Add(0.1f);
        }

        #endregion

        #region Отрисовка таблицы зависимых объектов

        EditorGUILayout.LabelField("Affects others");

        removalIndex = -1;

        EditorGUI.indentLevel += 1;

        var list = randomObject.affects;

        if (list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {

                RandomObject otherObject;
                otherObject = (RandomObject)EditorGUILayout.ObjectField(
                    randomObject.affects[i], typeof(RandomObject), true);
                if (otherObject != randomObject)
                    randomObject.affects[i] = otherObject;
                else
                    randomObject.affects[i] = null;


                // Галочка "Исключает полностью"
                GUILayout.BeginHorizontal();

                randomObject.exclude[i] = EditorGUILayout.Toggle(
                    randomObject.exclude[i], GUILayout.Width(28));
                GUILayout.Label("Exclude", GUILayout.Width(56));

                if (randomObject.exclude[i])
                {
                    randomObject.decrease[i] = false;
                    randomObject.increase[i] = false;
                }

                GUILayout.EndHorizontal();

                // Галочка "Снижает вероятность"
                GUILayout.BeginHorizontal();

                randomObject.decrease[i] = EditorGUILayout.Toggle(
                    randomObject.decrease[i], GUILayout.Width(28));
                GUILayout.Label("Decrease", GUILayout.Width(56));

                if (randomObject.decrease[i])
                {
                    randomObject.exclude[i] = false;
                    randomObject.increase[i] = false;
                }

                var decRate = randomObject.decreaseRate[i] * 100;
                decRate = EditorGUILayout.Slider(decRate, 0, 100, GUILayout.MaxWidth(164));
                randomObject.decreaseRate[i] = decRate / 100;

                GUILayout.EndHorizontal();

                // Галочка "Повышает вероятность"
                GUILayout.BeginHorizontal();

                randomObject.increase[i] = EditorGUILayout.Toggle(
                    randomObject.increase[i], GUILayout.Width(28));
                GUILayout.Label("Increase", GUILayout.Width(56));

                if (randomObject.increase[i])
                {
                    randomObject.decrease[i] = false;
                    randomObject.exclude[i] = false;
                }

                var incRate = randomObject.increaseRate[i] * 100;
                decRate = EditorGUILayout.Slider(decRate, 0, 100, GUILayout.MaxWidth(164));
                randomObject.increaseRate[i] = incRate / 100;

                GUILayout.EndHorizontal();

                //Кнопка "Убрать из списка"
                GUILayout.BeginHorizontal();

                GUILayout.Label("");

                if (GUILayout.Button("Remove"))
                {
                    removalIndex = i;
                }

                GUILayout.EndHorizontal();

                GUILayout.Space(8);
            }
        }
        else
        {
            EditorGUILayout.LabelField("None");
        }

        EditorGUI.indentLevel -= 1;

        // Удаление указанного объекта из списка
        if (removalIndex != -1)
        {
            randomObject.affects.RemoveAt(removalIndex);
            randomObject.exclude.RemoveAt(removalIndex);
            randomObject.decrease.RemoveAt(removalIndex);
            randomObject.decreaseRate.RemoveAt(removalIndex);
            randomObject.increase.RemoveAt(removalIndex);
            randomObject.increaseRate.RemoveAt(removalIndex);
        }

        // Кнопка для добавления ячейки
        if (GUILayout.Button("Add"))
        {
            randomObject.affects.Add(null);
            randomObject.exclude.Add(true);
            randomObject.decrease.Add(false);
            randomObject.decreaseRate.Add(0.15f);
            randomObject.increase.Add(false);
            randomObject.increaseRate.Add(0.15f);
        }

        #endregion

        if (GUILayout.Button("Generate"))
            randomObject.Generate();

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
