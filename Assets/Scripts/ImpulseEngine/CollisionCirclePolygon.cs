/*
    Copyright (c) 2013 Randy Gaul http://RandyGaul.net

    This software is provided 'as-is', without any express or implied
    warranty. In no event will the authors be held liable for any damages
    arising from the use of this software.

    Permission is granted to anyone to use this software for any purpose,
    including commercial applications, and to alter it and redistribute it
    freely, subject to the following restrictions:
      1. The origin of this software must not be misrepresented; you must not
         claim that you wrote the original software. If you use this software
         in a product, an acknowledgment in the product documentation would be
         appreciated but is not required.
      2. Altered source versions must be plainly marked as such, and must not be
         misrepresented as being the original software.
      3. This notice may not be removed or altered from any source distribution.
      
    Port to Java by Philip Diffenderfer http://magnos.org - Port to C# Unity by Leroy Thompson http://leroy.ron@gmail.com
*/
public partial class ImpulseEngine
{
    public class CollisionCirclePolygon : CollisionCallback
    {
        public static readonly CollisionCirclePolygon instance = new CollisionCirclePolygon();

        public override void HandleCollision(Manifold m, Body a, Body b)
        {
            Circle A = (Circle)a.shape;
            Polygon B = (Polygon)b.shape;

            m.contactCount = 0;

            // Transform circle center to Polygon model space
            // Vec2 center = a->position;
            // center = B->u.Transpose( ) * (center - b->position);
            Vec2 center = B.u.Transpose().Muli(a.position.Sub(b.position));

            // Find edge with minimum penetration
            // Exact concept as using support points in Polygon vs Polygon
            float separation = -float.MaxValue;
            int faceNormal = 0;
            for (int i = 0; i < B.vertexCount; ++i)
            {
                // real s = Dot( B->m_normals[i], center - B->m_vertices[i] );
                float s = Vec2.Dot(B.normals[i], center.Sub(B.vertices[i]));

                if (s > A.radius)
                {
                    return;
                }

                if (s > separation)
                {
                    separation = s;
                    faceNormal = i;
                }
            }

            // Grab face's vertices
            Vec2 v1 = B.vertices[faceNormal];
            int i2 = faceNormal + 1 < B.vertexCount ? faceNormal + 1 : 0;
            Vec2 v2 = B.vertices[i2];

            // Check to see if center is within polygon
            if (separation < ImpulseMath.EPSILON)
            {
                // m->contact_count = 1;
                // m->normal = -(B->u * B->m_normals[faceNormal]);
                // m->contacts[0] = m->normal * A->radius + a->position;
                // m->penetration = A->radius;

                m.contactCount = 1;
                B.u.Mul(B.normals[faceNormal], m.normal).Negi();
                m.contacts[0].Set(m.normal).Muli(A.radius).Addi(a.position);
                m.penetration = A.radius;
                return;
            }

            // Determine which voronoi region of the edge center of circle lies within
            // real dot1 = Dot( center - v1, v2 - v1 );
            // real dot2 = Dot( center - v2, v1 - v2 );
            // m->penetration = A->radius - separation;
            float dot1 = Vec2.Dot(center.Sub(v1), v2.Sub(v1));
            float dot2 = Vec2.Dot(center.Sub(v2), v1.Sub(v2));
            m.penetration = A.radius - separation;

            // Closest to v1
            if (dot1 <= 0.0f)
            {
                if (Vec2.DistanceSq(center, v1) > A.radius * A.radius)
                {
                    return;
                }

                // m->contact_count = 1;
                // Vec2 n = v1 - center;
                // n = B->u * n;
                // n.Normalize( );
                // m->normal = n;
                // v1 = B->u * v1 + b->position;
                // m->contacts[0] = v1;

                m.contactCount = 1;
                B.u.Muli(m.normal.Set(v1).Subi(center)).Normalize();
                B.u.Mul(v1, m.contacts[0]).Addi(b.position);
            }

            // Closest to v2
            else if (dot2 <= 0.0f)
            {
                if (Vec2.DistanceSq(center, v2) > A.radius * A.radius)
                {
                    return;
                }

                // m->contact_count = 1;
                // Vec2 n = v2 - center;
                // v2 = B->u * v2 + b->position;
                // m->contacts[0] = v2;
                // n = B->u * n;
                // n.Normalize( );
                // m->normal = n;

                m.contactCount = 1;
                B.u.Muli(m.normal.Set(v2).Subi(center)).Normalize();
                B.u.Mul(v2, m.contacts[0]).Addi(b.position);
            }

            // Closest to face
            else
            {
                Vec2 n = B.normals[faceNormal];

                if (Vec2.Dot(center.Sub(v1), n) > A.radius)
                {
                    return;
                }

                // n = B->u * n;
                // m->normal = -n;
                // m->contacts[0] = m->normal * A->radius + a->position;
                // m->contact_count = 1;

                m.contactCount = 1;
                B.u.Mul(n, m.normal).Negi();
                m.contacts[0].Set(a.position).Addsi(m.normal, A.radius);
            }
        }
    }
}