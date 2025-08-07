using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class DialogueTreeEditor : EditorWindow
{
    DialogueTreeSO dialogueTree;
    Vector2 offset;
    Vector2 drag;

    DialogueTreeSO.DialogueNode selectedNode = null;
    DialogueTreeSO.DialogueOption selectedOption = null;

    const float nodeWidth = 250f;
    const float nodeHeight = 150f;

    private bool isLinking = false;
    private DialogueTreeSO.DialogueNode linkingFromNode = null;
    private DialogueTreeSO.DialogueOption linkingFromOption = null;

    private Rect linkingButtonRect;
    private Dictionary<DialogueTreeSO.DialogueOption, Rect> optionButtonRects = new();

    private float zoom = 1.0f;
    private Vector2 panOffset = Vector2.zero;

    [MenuItem("Window/Dialogue Tree Editor")]
    public static void OpenWindow()
    {
        DialogueTreeEditor window = GetWindow<DialogueTreeEditor>();
        window.titleContent = new GUIContent("Dialogue Tree Editor");
    }

    private void OnGUI()
    {
        DrawToolbar();

        if (dialogueTree == null)
        {
            EditorGUILayout.HelpBox("Please load a DialogueTree asset.", MessageType.Info);
            if (GUILayout.Button("Load DialogueTree"))
            {
                string path = EditorUtility.OpenFilePanel("Select DialogueTree", "Assets", "asset");
                if (!string.IsNullOrEmpty(path))
                {
                    path = "Assets" + path.Substring(Application.dataPath.Length);
                    dialogueTree = AssetDatabase.LoadAssetAtPath<DialogueTreeSO>(path);
                }
            }
            return;
        }

        ProcessEvents(Event.current);

        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        if (Event.current.type == EventType.MouseDown)
        {
            bool clickedOnNode = false;

            foreach (var node in dialogueTree.nodes)
            {
                float dynamicHeight = CalculateNodeHeight(node);
                Rect nodeRect = new Rect(node.position.x, node.position.y, nodeWidth, dynamicHeight);

                if (nodeRect.Contains(Event.current.mousePosition))
                {
                    clickedOnNode = true;
                    break;
                }
            }

            if (!clickedOnNode || Event.current.button == 1) 
            {
                isLinking = false;
                linkingFromNode = null;
                linkingFromOption = null;
                Event.current.Use(); 
            }
        }

        if (GUI.changed) Repaint();
    }

    public static void OpenWithAsset(DialogueTreeSO tree)
    {
        var window = GetWindow<DialogueTreeEditor>();
        window.titleContent = new GUIContent("Dialogue Tree Editor");
        window.dialogueTree = tree;
    }

    void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(
                new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset,
                new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(
                new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset,
                new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("New Node", EditorStyles.toolbarButton)) AddNode(new Vector2(position.width / 2, position.height / 2));
        if (GUILayout.Button("Save", EditorStyles.toolbarButton)) { EditorUtility.SetDirty(dialogueTree); AssetDatabase.SaveAssets(); }
        if (GUILayout.Button("Recenter View", EditorStyles.toolbarButton)) RecenterView();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Load DialogueTree", EditorStyles.toolbarButton))
        {
            var path = EditorUtility.OpenFilePanel("Select DialogueTree", "Assets", "asset");
            if (!string.IsNullOrEmpty(path))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
                dialogueTree = AssetDatabase.LoadAssetAtPath<DialogueTreeSO>(path);
                selectedNode = null;
                selectedOption = null;
            }
        }
        GUILayout.EndHorizontal();
    }

    void RecenterView()
    {
        if (dialogueTree == null || dialogueTree.nodes == null || dialogueTree.nodes.Count == 0)
            return;

        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        foreach (var node in dialogueTree.nodes)
        {
            float dynamicHeight = CalculateNodeHeight(node);
            minX = Mathf.Min(minX, node.position.x);
            minY = Mathf.Min(minY, node.position.y);
            maxX = Mathf.Max(maxX, node.position.x + nodeWidth);
            maxY = Mathf.Max(maxY, node.position.y + dynamicHeight);
        }

        Vector2 center = new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f);
        Vector2 windowCenter = new Vector2(position.width / 2f, position.height / 2f);
        Vector2 offsetToCenter = windowCenter - center;

        foreach (var node in dialogueTree.nodes)
        {
            node.position += offsetToCenter;
        }

        offset = Vector2.zero; 
        Repaint();
    }

    void AddNode(Vector2 pos)
    {
        if (dialogueTree == null) return;

        DialogueTreeSO.DialogueNode node = new DialogueTreeSO.DialogueNode();
        node.id = System.Guid.NewGuid().ToString();
        node.text = "New Dialogue Node";
        node.position = pos;
        dialogueTree.nodes.Add(node);

        EditorUtility.SetDirty(dialogueTree);
    }

    void DrawNodes()
    {
        if (dialogueTree.nodes == null) return;

        for (int i = 0; i < dialogueTree.nodes.Count; i++)
        {
            DrawNode(dialogueTree.nodes[i]);
        }
    }

    void DrawNode(DialogueTreeSO.DialogueNode node)
    {
        float height = CalculateNodeHeight(node);
        Rect nodeRect = new Rect(node.position.x, node.position.y, nodeWidth, height);

        if (node.isStartNode || node.isActionNode)
        {
            Color fill = node.isStartNode && node.isActionNode ? new(0.3f, 0.9f, 0.9f, 0.2f) :
                         node.isStartNode ? new(0.3f, 0.9f, 0.3f, 0.2f) :
                                                              new(0.3f, 0.3f, 0.9f, 0.2f);
            Color outline = fill * 0.5f; outline.a = 0.4f;
            Handles.DrawSolidRectangleWithOutline(nodeRect, fill, outline);
        }
        else GUI.Box(nodeRect, "", EditorStyles.helpBox);

        Rect closeRect = new Rect(nodeRect.xMax - 22, nodeRect.y + 2, 20, 18);
        if (GUI.Button(closeRect, "X"))
        {
            dialogueTree.nodes.Remove(node);
            EditorUtility.SetDirty(dialogueTree);
            GUIUtility.ExitGUI();
        }

        GUILayout.BeginArea(nodeRect);
        EditorGUILayout.BeginHorizontal();

        bool newStart = GUILayout.Toggle(node.isStartNode, "Start", GUILayout.Width(60));
        node.isActionNode = GUILayout.Toggle(node.isActionNode, "Action", GUILayout.Width(65));

        if (newStart != node.isStartNode)
        {
            foreach (var n in dialogueTree.nodes) n.isStartNode = false;
            node.isStartNode = true;
            EditorUtility.SetDirty(dialogueTree);
        }

        EditorGUILayout.EndHorizontal();

        if (node.isActionNode)
        {
            node.onTriggerAction ??= new UnityEvent();
            SerializedObject so = new(dialogueTree);
            int idx = dialogueTree.nodes.IndexOf(node);
            if (idx >= 0 && idx < so.FindProperty("nodes").arraySize)
            {
                var eventProp = so.FindProperty("nodes").GetArrayElementAtIndex(idx).FindPropertyRelative("onTriggerAction");
                EditorGUILayout.PropertyField(eventProp, new GUIContent("Action Event"));
                so.ApplyModifiedProperties();
            }

            GUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Link to next node:");
            if (GUILayout.Button("Link", GUILayout.Width(50)) && node.ActionOption != null)
            {
                isLinking = true;
                linkingFromNode = node;
                linkingFromOption = node.ActionOption;
                node.ActionOption.nextNodeId = null;
            }

            linkingButtonRect = new Rect(GUILayoutUtility.GetLastRect().position + nodeRect.position, GUILayoutUtility.GetLastRect().size);

            Rect btnRect = GUILayoutUtility.GetLastRect();
            optionButtonRects[node.ActionOption] = new Rect(btnRect.position + nodeRect.position, btnRect.size);

            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            var style = EditorStyles.textArea;
            float minHeight = Mathf.Max(style.CalcHeight(new GUIContent(node.text), nodeWidth - 20), 60f);
            node.text = EditorGUILayout.TextArea(node.text, style, GUILayout.Height(minHeight));
            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(dialogueTree);

            EditorGUILayout.Space();
            GUILayout.Label("Options:");
            node.options ??= new();

            for (int i = 0; i < node.options.Count; i++)
            {
                var opt = node.options[i];
                if (opt.isHiddenForAction) continue;

                EditorGUILayout.BeginHorizontal();
                opt.optionText = EditorGUILayout.TextField(opt.optionText);

                if (GUILayout.Button("x", GUILayout.Width(20)))
                {
                    node.options.RemoveAt(i--);
                    EditorUtility.SetDirty(dialogueTree);
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                if (GUILayout.Button("Link", GUILayout.Width(50)))
                {
                    isLinking = true;
                    linkingFromNode = node;
                    linkingFromOption = opt;
                    opt.nextNodeId = null;
                }

                Rect btnRect = GUILayoutUtility.GetLastRect();
                optionButtonRects[opt] = new Rect(btnRect.position + nodeRect.position, btnRect.size);
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Option"))
            {
                node.options.Add(new DialogueTreeSO.DialogueOption { optionText = "New Option", nextNodeId = "" });
                EditorUtility.SetDirty(dialogueTree);
            }
        }

        GUILayout.EndArea();
        ProcessNodeEvents(nodeRect, node);
    }

    float CalculateNodeHeight(DialogueTreeSO.DialogueNode node)
    {
        float height = 0f;
        height += EditorGUIUtility.singleLineHeight * 1.2f;

        GUIStyle textAreaStyle = EditorStyles.textArea;
        float textHeight = textAreaStyle.CalcHeight(new GUIContent(node.text), nodeWidth - 20);
        textHeight = Mathf.Max(textHeight, 60f);
        height += textHeight;

        height += EditorGUIUtility.singleLineHeight; 

        int optionCount = node.options != null ? node.options.Count : 0;

        if (!node.isActionNode)
        {
            height += optionCount * (EditorGUIUtility.singleLineHeight + 4f);

            height += EditorGUIUtility.singleLineHeight;
        }
        else
        {
            SerializedObject so = new SerializedObject(dialogueTree);
            SerializedProperty nodesProp = so.FindProperty("nodes");

            int nodeIndex = dialogueTree.nodes.IndexOf(node);
            if (nodeIndex >= 0)
            {
                SerializedProperty nodeProp = nodesProp.GetArrayElementAtIndex(nodeIndex);
                SerializedProperty unityEventProp = nodeProp.FindPropertyRelative("onTriggerAction");

                height += EditorGUI.GetPropertyHeight(unityEventProp, true);
            }

            height -= EditorGUIUtility.singleLineHeight + 30f;
        }

        return height;
    }


    void ProcessNodeEvents(Rect nodeRect, DialogueTreeSO.DialogueNode node)
    {
        Event e = Event.current;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (nodeRect.Contains(e.mousePosition))
                {
                    selectedNode = node;
                    GUI.FocusControl(null);
                    e.Use();
                }
                else if (selectedNode == node)
                {
                    selectedNode = null;
                    e.Use();
                }

                if (isLinking && nodeRect.Contains(e.mousePosition))
                {
                    if (linkingFromOption != null && linkingFromNode != null)
                    {
                        linkingFromOption.nextNodeId = node.id;
                        isLinking = false;
                        linkingFromOption = null;
                        linkingFromNode = null;
                        EditorUtility.SetDirty(dialogueTree);
                        e.Use();
                        return;
                    }
                }

                if (isLinking && e.button == 1)
                {
                    isLinking = false;
                    linkingFromOption = null;
                    linkingFromNode = null;
                    e.Use();
                }

                Vector2 globalOffset = new Vector2(node.position.x, node.position.y);
                linkingButtonRect.position += globalOffset;
                break;

            case EventType.MouseDrag:
                if (selectedNode == node && e.button == 0)
                {
                    node.position += e.delta;
                    EditorUtility.SetDirty(dialogueTree);
                    e.Use();
                }
                break;
        }
    }

    void DrawConnections()
    {
        if (dialogueTree.nodes == null) return;

        foreach (var node in dialogueTree.nodes)
        {
            if (!node.isActionNode && node.options != null)
            {
                int optionIndex = 0;

                foreach (var option in node.options)
                {
                    if (option.isHiddenForAction)
                        continue;

                    var targetNode = dialogueTree.GetNodeById(option.nextNodeId);
                    if (targetNode != null)
                    {
                        Vector2 startPos;
                        if (optionButtonRects.TryGetValue(option, out Rect rect))
                        {
                            startPos = new Vector2(rect.xMax, rect.center.y);
                        }
                        else
                        {
                            float fallbackY = 80f + optionIndex * 24f + 10f;
                            startPos = new Vector2(
                                node.position.x + nodeWidth,
                                node.position.y + fallbackY
                            );
                        }

                        Vector2 endPos = new Vector2(
                            targetNode.position.x,
                            targetNode.position.y + 40f
                        );

                        DrawBezier(startPos, endPos);
                    }

                    optionIndex++;
                }
            }

            if (node.isActionNode)
            {
                var option = node.ActionOption;
                var targetNode = dialogueTree.GetNodeById(option.nextNodeId);
                if (targetNode != null)
                {
                    if (optionButtonRects.TryGetValue(option, out Rect rect))
                    {
                        Vector2 startPos = new Vector2(rect.xMax, rect.center.y);
                        Vector2 endPos = new Vector2(targetNode.position.x, targetNode.position.y + 40f);
                        DrawBezier(startPos, endPos);
                    }
                    else
                    {
                        Vector2 fallbackStart = new Vector2(node.position.x + nodeWidth, node.position.y + 90f);
                        Vector2 endPos = new Vector2(targetNode.position.x, targetNode.position.y + 40f);
                        DrawBezier(fallbackStart, endPos);
                    }
                }
            }
        }

        if (isLinking && linkingFromNode != null && linkingFromOption != null)
        {
            if (optionButtonRects.TryGetValue(linkingFromOption, out Rect rect))
            {
                Vector2 startPos = new Vector2(rect.xMax, rect.center.y);
                Vector2 endPos = Event.current.mousePosition;
                DrawBezier(startPos, endPos);
                Repaint();
            }
        }
    }

    void DrawBezier(Vector2 start, Vector2 end)
    {
        Vector2 startTangent = start + Vector2.right * 50f;
        Vector2 endTangent = end + Vector2.left * 50f;
        Handles.DrawBezier(start, end, startTangent, endTangent, Color.white, null, 3f);
    }

    void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        if (e.type == EventType.MouseDrag && e.button == 2)
        {
            drag = e.delta;
            offset += drag;

            for (int i = 0; i < dialogueTree.nodes.Count; i++)
            {
                dialogueTree.nodes[i].position += drag;
            }

            e.Use();
        }
    }
}