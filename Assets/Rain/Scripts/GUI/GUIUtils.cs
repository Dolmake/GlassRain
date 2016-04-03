/*-------------------------------------------------
Created by: Daniel Peribáñez Sepúlveda
January 2013
peribanez.daniel@gmail.com

/**/

using UnityEngine;
using System.Collections;

namespace GlassGUI
{
    public static class GUIUtils
    {

        public static Rect GetRectangle_MOUSE_ORBIT(float top, float bottom, float width, float height, ref bool mouseOver)
        {
            bool over = false;
            Rect res = GetRectangleCheckingMouse(top, bottom, width, height, out over);
            mouseOver |= over;
            return res;
        }

        public static Rect GetRectangleCheckingMouse(float top, float bottom, float width, float height, out bool mouseOver)
        {
            Rect res = new Rect(top, bottom, width, height);
            mouseOver = res.Contains(new Vector3(Input.mousePosition.x, Camera.main.pixelHeight - Input.mousePosition.y, Input.mousePosition.z));
            return res;
        }
        public static Rect GetRectangle(int rows, int cols, int row, int col, int size, ref bool mouseOver)
        {
            float w = Camera.main.pixelWidth;
            float h = Camera.main.pixelHeight;

            float rowH = h / (float)rows;
            float colW = w / (float)cols;


            Rect rect = GetRectangle_MOUSE_ORBIT(col * colW, row * rowH, colW, rowH, ref mouseOver);
            Vector2 center = rect.center;
            rect.width = size;
            rect.height = size;
            rect.center = center;
            return rect;
        }

        public static Rect GetRectangle(int rows, int cols, int row, int col, float percent)
        {
            float min = Mathf.Min(Camera.main.pixelWidth, Camera.main.pixelHeight);
            int size = (int)(min * percent);
            return GetRectangle(rows, cols, row, col, size);
        }

    }
}
