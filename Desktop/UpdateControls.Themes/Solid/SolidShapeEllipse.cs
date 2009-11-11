/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2008 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace UpdateControls.Themes.Solid
{
    public class SolidShapeEllipse : SolidShape
    {
        public static SolidShape Instance = new SolidShapeEllipse();

        private SolidShapeEllipse()
        {
        }

        public override bool Normal(PointF bounds, PointF p, out PointF3D n, out float z, out float alpha)
        {
            // z = 1 - x^2 - y^2
            // Where -1 <= x <= 1 and -1 <= y <= 1
            // 0 <= z <= 1

            // dz/dx = -2x
            // dz/dy = -2y

            if ((p.X >= 0.0f) && (p.X <= bounds.X) &&
                (p.Y >= 0.0f) && (p.Y <= bounds.Y) &&
                (bounds.X > 2.0f && bounds.Y > 2.0f))
            {
                // Convert to scaled units.
                // dx/dpx = 2/bx
                // dy/dpy = 2/by
                float cx = bounds.X * 0.5f;
                float cy = bounds.Y * 0.5f;
                float x = p.X / cx - 1.0f;
                float y = p.Y / cy - 1.0f;

                z = 1.0f - x * x - y * y;
                if (z > 0)
                {
                    // Calculate the normal.
                    // n || (-dz/dpx, -dz/dpy, 1)
                    // dz/dpx = dz/dx dx/dpx
                    // dz/dpy = dz/dy dy/dpy
                    n = new PointF3D(
                        2.0f * x / cx,
                        2.0f * y / cy,
                        1.0f);

                    // Approximate the distance from the edge.
                    float dx = p.X - cx;
                    float dy = p.Y - cy;
                    float d = (float)Math.Sqrt(cx * cy) - (float)Math.Sqrt(cy * dx * dx / cx + cx * dy * dy / cy);
                    if (d > 1.0f)
                        alpha = 1.0f;
                    else if (d < 0.0f)
                        alpha = 0.0f;
                    else
                        alpha = d;
                    return true;
                }
            }

            n = PointF3D.Zero;
            z = 0.0f;
            alpha = 0.0f;
            return false;
        }
    }
}