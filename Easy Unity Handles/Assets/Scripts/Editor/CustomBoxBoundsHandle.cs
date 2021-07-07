
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UObject = UnityEngine.Object;


namespace EUH
{
    public class CustomHandleBase<T> where T : CustomHandleBase<T>
    {
        protected Color handleColor = Color.yellow;

        
        public T SetColor(Color color)
        {
            return (T)this;
        }
    }

    
    public class CustomBoxBoundsHandle : CustomHandleBase<CustomBoxBoundsHandle>
    {

        UObject uObject;
        BoxBoundsHandle boxBoundsHandle;
        Handles.SizeFunction sizeFunction;//Func<Vector3, float>
        Handles.CapFunction capFunction;
        /// <summary>
        /// if this is true, the handle will assume any axis with a size of zero means that axis is disabled
        /// </summary>
        bool implicitAxisLocking = true;
        static Dictionary<UObject, CustomBoxBoundsHandle> boxBoundsCache = new Dictionary<UObject, CustomBoxBoundsHandle>();
        private PrimitiveBoundsHandle.Axes axes;

        public CustomBoxBoundsHandle(UObject uObject, BoxBoundsHandle boxBoundsHandle)
        {
            this.uObject = uObject;
            this.boxBoundsHandle = boxBoundsHandle;
            
        }

        private void UpdateAxesLocking()
        {
            if (!implicitAxisLocking)
            {
                boxBoundsHandle.axes = axes;
                return;
            }
            else {
                boxBoundsHandle.axes = PrimitiveBoundsHandle.Axes.All;
            }
            if (boxBoundsHandle.size.x == 0)
                boxBoundsHandle.axes &= PrimitiveBoundsHandle.Axes.X;

            if (boxBoundsHandle.size.y == 0)
                //Draw Rectangle
                boxBoundsHandle.axes &= ~PrimitiveBoundsHandle.Axes.Y;


            if (boxBoundsHandle.size.z == 0)
                boxBoundsHandle.axes &= ~PrimitiveBoundsHandle.Axes.Z;
        }

    

        public static void DrawBoxBoundsSizeHandle(UObject target, Transform transform, Action<Vector3> sizeSetter, Func<Vector3> sizeGetter)
        {
            DrawBoxBoundsSizeHandle(target, transform.position, transform.rotation, sizeSetter, sizeGetter);
        }

        public static void DrawBoxBoundsSizeHandle(UObject target, Vector3 position, Quaternion rotation, Action<Vector3> sizeSetter, Func<Vector3> sizeGetter)
        {
            GetCustomBoxBoundsHandle(target, sizeGetter()).DrawSize(position, rotation, sizeSetter, sizeGetter);
        }
       
        public static void SetHandleColor(UObject target, Color color)
        {
            GetCustomBoxBoundsHandle(target, Vector3.one).handleColor = color;
        }
        public static void SetSizeFunction(UObject target, Func<Vector3, float> sizFuncion)
        {
            var handle = GetCustomBoxBoundsHandle(target, Vector3.one);
            handle.sizeFunction = (Vector3 pos) => sizFuncion(pos);
        }
        public static void SetCapFunction(UObject target, Handles.CapFunction capFunc)
        {
            var handle = GetCustomBoxBoundsHandle(target, Vector3.one);
            handle.capFunction = capFunc;
        }

        public void Draw(Transform transform, Action<Vector3> sizeSetter, Func<Vector3> sizeGetter)
        {
            DrawSize(transform.position, transform.rotation, sizeSetter, sizeGetter);
        }

        public void DrawSize(Vector3 position, Quaternion rotation, Action<Vector3> sizeSetter, Func<Vector3> sizeGetter)
        {
            UpdateAxesLocking();
            BoxBoundsHandle boxBounds = this.boxBoundsHandle;
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
            if (sizeFunction != null) boxBounds.midpointHandleSizeFunction = sizeFunction;
            if (capFunction != null) boxBounds.midpointHandleDrawFunction = capFunction;
            using (new Handles.DrawingScope(this.handleColor, matrix))
            {
                EditorGUI.BeginChangeCheck();
                boxBounds.DrawHandle();
                
                if (EditorGUI.EndChangeCheck())
                {
                    sizeSetter(boxBounds.size);
                }
                else
                {
                    boxBounds.size = sizeGetter();
                }
            }
        }
        
        public CustomBoxBoundsHandle SetHandleAxes(PrimitiveBoundsHandle.Axes axes)
        {
            implicitAxisLocking = false;
            this.axes = axes;
            return this;
        }
        public CustomBoxBoundsHandle HideHandleAxes(PrimitiveBoundsHandle.Axes axesToHide)
        {

            implicitAxisLocking = false;
            this.axes &= ~axesToHide;
            return this;
        }
        public CustomBoxBoundsHandle ShowHandleAxes(PrimitiveBoundsHandle.Axes axesToShow)
        {
            implicitAxisLocking = false;
            this.axes |= axesToShow;
            return this;
        }

        public CustomBoxBoundsHandle SetImplictAxisLocking(bool enableImplicitAxisLocking)
        {
            implicitAxisLocking = enableImplicitAxisLocking;
            return this;
        }
        public CustomBoxBoundsHandle SetCap( Handles.CapFunction capFunc) {
            this.capFunction = capFunc;
            return this;
        }
        public CustomBoxBoundsHandle SetSize(Func<Vector3, float> sizeFunc)
        {
            this.sizeFunction = (pos) => sizeFunc(pos);
            return this;
        }


        private static BoxBoundsHandle GetBoxBoundsHandle(UObject target, Vector3 initialSize)
        {
            CustomBoxBoundsHandle boxBounds;
            if (boxBoundsCache.TryGetValue(target, out boxBounds) == false || boxBounds == null)
            {
                boxBounds = new CustomBoxBoundsHandle(target, new BoxBoundsHandle()
                {
                    size = initialSize
                });

                if (boxBoundsCache.ContainsKey(target) == false)
                    boxBoundsCache.Add(target, boxBounds);
                else
                    boxBoundsCache[target] = boxBounds;
            }

            return boxBounds.boxBoundsHandle;
        }
        public static CustomBoxBoundsHandle GetCustomBoxBoundsHandle(UObject target, Vector3 initialSize)
        {
            CustomBoxBoundsHandle boxBounds;
            if (boxBoundsCache.TryGetValue(target, out boxBounds) == false || boxBounds == null)
            {
                boxBounds = new CustomBoxBoundsHandle(target, new BoxBoundsHandle()
                {
                    size = initialSize
                });

                if (boxBoundsCache.ContainsKey(target) == false)
                    boxBoundsCache.Add(target, boxBounds);
                else
                    boxBoundsCache[target] = boxBounds;
            }

            return boxBounds;
        }


    }




}