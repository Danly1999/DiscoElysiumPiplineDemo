using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CopyTexture))]
public class CopyTextureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 绘制默认的 Inspector 窗口内容

        CopyTexture script = (CopyTexture)target;

        // 在 Inspector 窗口中显示一个按钮
        if (GUILayout.Button("Create RT"))
        {
            // 当按钮被点击时执行的代码
            script.Render();
        }
        if (GUILayout.Button("Create PNG"))
        {
            // 当按钮被点击时执行的代码
            script.Copy();
        }
    }
}
