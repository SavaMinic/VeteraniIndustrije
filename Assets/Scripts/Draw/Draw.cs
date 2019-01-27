using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NDraw
{
    public static partial class Draw
    {
        internal readonly struct ColorIndex
        {
            public readonly int i;
            public readonly Color c;

            public ColorIndex(int i, Color c)
            {
                this.i = i;
                this.c = c;
            }
        }

        internal static List<Vector3> screenPoints = new List<Vector3>();
        internal static List<Vector3> worldPoints = new List<Vector3>();
        internal static List<Vector3> screenTrisPoints = new List<Vector3>();

        internal static List<ColorIndex> screenColorIndices = new List<ColorIndex>();
        internal static List<ColorIndex> worldColorIndices = new List<ColorIndex>();
        internal static List<ColorIndex> screenTrisColorIndices = new List<ColorIndex>();

        internal static void Clear()
        {
            screenPoints.Clear();
            screenColorIndices.Clear();

            worldPoints.Clear();
            worldColorIndices.Clear();

            screenTrisPoints.Clear();
            screenTrisColorIndices.Clear();
        }

        public static partial class Screen
        {
            public static void SetColor(Color color)
            {
                if (!Drawer.Exists) return;

                ColorIndex ci = new ColorIndex(screenPoints.Count, color);
                screenColorIndices.Add(ci);
            }

            public static void SetFillColor(Color color)
            {
                if (!Drawer.Exists) return;

                ColorIndex ci = new ColorIndex(screenTrisPoints.Count, color);
                screenTrisColorIndices.Add(ci);
            }

            public static void Line(int p1x, int p1y, int p2x, int p2y)
            {
                if (!Drawer.Exists) return;

                screenPoints.Add(new Vector2(p1x, p1y));
                screenPoints.Add(new Vector2(p2x, p2y));
            }

            public static void Line(Vector2 p1, Vector2 p2)
            {
                if (!Drawer.Exists) return;

                screenPoints.Add(p1);
                screenPoints.Add(p2);
            }

            public static void Rect(Rect rect)
            {
                if (!Drawer.Exists) return;

                Rect(rect.x, rect.y, rect.width, rect.height);
            }

            public static void Rect(float x, float y, float width, float height)
            {
                if (!Drawer.Exists) return;

                screenPoints.Add(new Vector2(x, y));
                screenPoints.Add(new Vector2(x + width, y));

                screenPoints.Add(new Vector2(x + width, y));
                screenPoints.Add(new Vector2(x + width, y + height));

                screenPoints.Add(new Vector2(x + width, y + height));
                screenPoints.Add(new Vector2(x, y + height));

                screenPoints.Add(new Vector2(x, y + height));
                screenPoints.Add(new Vector2(x, y));
            }

            public static void Circle(float centerX, float centerY, float pixelRadius)
            {
                if (!Drawer.Exists) return;

                Vector2 size = new Vector2(pixelRadius, pixelRadius);
                Vector2 center = new Vector2(centerX, centerY);

                Ellipse(center, size);
            }

            public static void Ellipse(Vector2 center, Vector2 size)
            {
                if (!Drawer.Exists) return;

                float radX = size.x;
                float radY = size.y;

                Vector2 ci = new Vector2(
                    center.x + (1 * radX),
                    center.y);

                Vector2 ci0 = ci;

                for (float theta = 0.0f; theta < (2 * Mathf.PI); theta += 0.1f)
                {
                    screenPoints.Add(ci);

                    ci = new Vector2(center.x + (Mathf.Cos(theta) * radX), center.y + (Mathf.Sin(theta) * radY));

                    screenPoints.Add(ci);
                }

                // close
                screenPoints.Add(ci);
                screenPoints.Add(ci0);

                //screenPoints.Add(ci);
            }

            public static void MultiLine(Vector2[] points)
            {
                if (!Drawer.Exists) return;

                if (points.Length < 2) return;

                for (int i = 0; i < points.Length - 1; i++)
                {
                    screenPoints.Add(points[i]);
                    screenPoints.Add(points[i + 1]);
                }
            }

            public static void Grid(int xLineNum, int yLineNum, Rect rect)
            {
                if (!Drawer.Exists) return;

                float add = rect.height / yLineNum;
                for (int i = 0; i <= yLineNum; i++)
                {
                    float y = rect.yMax - i * add;
                    screenPoints.Add(new Vector2(rect.x, y));
                    screenPoints.Add(new Vector2(rect.xMax, y));
                }

                add = rect.width / yLineNum;
                for (int i = 0; i <= yLineNum; i++)
                {
                    float x = rect.x + i * add;
                    screenPoints.Add(new Vector2(x, rect.y));
                    screenPoints.Add(new Vector2(x, rect.yMax));
                }
            }

            // TODO: Add scaled/moving grid

            // FILLED

            public static void FillRect(Rect rect)
            {
                if (!Drawer.Exists) return;

                FillRect(rect.x, rect.y, rect.width, rect.height);
            }

            public static void FillRect(float x, float y, float width, float height)
            {
                if (!Drawer.Exists) return;

                Vector3 p0 = new Vector3(x, y);
                Vector3 p1 = new Vector3(x + width, y);
                Vector3 p2 = new Vector3(x, y + height);
                Vector3 p3 = new Vector3(x + width, y + height);

                screenTrisPoints.Add(p0);
                screenTrisPoints.Add(p1);
                screenTrisPoints.Add(p2);

                screenTrisPoints.Add(p1);
                screenTrisPoints.Add(p3);
                screenTrisPoints.Add(p2);
            }
        }

        public static partial class World
        {
            public static void SetColor(Color color)
            {
                if (!Drawer.Exists) return;

                ColorIndex ci = new ColorIndex(worldPoints.Count, color);
                worldColorIndices.Add(ci);
            }

            public static void Line(Vector3 p1, Vector3 p2)
            {
                if (!Drawer.Exists) return;

                worldPoints.Add(p1);
                worldPoints.Add(p2);
            }

            public static void Cube(Vector3 center, Vector3 size, Vector3 forward, Vector3 up)
            {
                if (!Drawer.Exists) return;

                forward = forward.normalized;
                up = Vector3.ProjectOnPlane(up, forward).normalized;
                Vector3 right = Vector3.Cross(forward, up);

                Vector3 frw = forward * size.z * 0.5f;
                Vector3 rgt = right * size.x * 0.5f;
                Vector3 upw = up * size.y * 0.5f;

                // vertical lines
                worldPoints.Add(center - frw - rgt - upw);
                worldPoints.Add(center - frw - rgt + upw);

                worldPoints.Add(center - frw + rgt - upw);
                worldPoints.Add(center - frw + rgt + upw);

                worldPoints.Add(center + frw - rgt - upw);
                worldPoints.Add(center + frw - rgt + upw);

                worldPoints.Add(center + frw + rgt - upw);
                worldPoints.Add(center + frw + rgt + upw);

                // horizontal lines
                worldPoints.Add(center - frw - rgt - upw);
                worldPoints.Add(center - frw + rgt - upw);

                worldPoints.Add(center - frw - rgt + upw);
                worldPoints.Add(center - frw + rgt + upw);

                worldPoints.Add(center + frw - rgt - upw);
                worldPoints.Add(center + frw + rgt - upw);

                worldPoints.Add(center + frw - rgt + upw);
                worldPoints.Add(center + frw + rgt + upw);

                // forward lines
                worldPoints.Add(center - frw - rgt - upw);
                worldPoints.Add(center + frw - rgt - upw);

                worldPoints.Add(center - frw + rgt - upw);
                worldPoints.Add(center + frw + rgt - upw);

                worldPoints.Add(center - frw - rgt + upw);
                worldPoints.Add(center + frw - rgt + upw);

                worldPoints.Add(center - frw + rgt + upw);
                worldPoints.Add(center + frw + rgt + upw);
            }

            public static void Circle(Vector3 center, float radius, Vector3 normal)
            {
                if (!Drawer.Exists) return;

                normal = normal.normalized;
                Vector3 forward = normal == Vector3.up ?
                    Vector3.ProjectOnPlane(Vector3.forward, normal).normalized :
                    Vector3.ProjectOnPlane(Vector3.up, normal);

                //Vector3 right = Vector3.Cross(normal, forward);

                Vector3 ci = center + forward * radius;
                Vector3 c0 = ci;

                for (float theta = 0.0f; theta < (2 * Mathf.PI); theta += 0.01f)
                {
                    //Vector3 ci = center + forward * Mathf.Cos(theta) * radius + right * Mathf.Sin(theta) * radius;

                    worldPoints.Add(ci);

                    Vector3 angleDir = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, normal) * forward;
                    ci = center + angleDir.normalized * radius;

                    worldPoints.Add(ci);

                    //if (theta != 0)
                    //GL.Vertex(ci);
                }

                worldPoints.Add(ci);
                worldPoints.Add(c0);
            }
        }
    }
}