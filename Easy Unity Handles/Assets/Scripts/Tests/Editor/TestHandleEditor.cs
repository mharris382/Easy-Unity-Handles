using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EUH;
[CustomEditor(typeof(TestHandle))]
#if ODIN_INSPECTOR
public class TestHandleEditor : OdinEditor
#else
public class TestHandleEditor : Editor
#endif
{
    private void OnSceneGUI()
    {
        var testHandle = target as TestHandle;
              

        CustomBoxBoundsHandle.GetCustomBoxBoundsHandle(testHandle, testHandle.boundsSize)
           .SetCap(Handles.DotHandleCap)
           .SetSize(pos => HandleUtility.GetHandleSize(pos) * 0.5f)
           .SetColor(Color.red)
           .Draw(testHandle.transform, newSize => testHandle.boundsSize = newSize, () => testHandle.boundsSize);



    }
}


