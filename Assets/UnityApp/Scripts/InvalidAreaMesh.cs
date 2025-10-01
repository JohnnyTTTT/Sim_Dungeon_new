using DungeonArchitect.Flow.Domains.Tilemap;
using System.Collections.Generic;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class InvalidAreaMesh : MonoBehaviour
    {
        [SerializeField] private MeshFilter m_MeshFilter;

        public void UpdateMesh()
        {
            if (m_MeshFilter.sharedMesh == null)
            {
                m_MeshFilter.sharedMesh = new Mesh();
            }

            var verts = new List<Vector3>();
            var tris = new List<int>();
            var cellSize = 2;
     
            var width = DungeonController.Instance.largeTilemapSize.x;
            var hight = DungeonController.Instance.largeTilemapSize.y;
            var index = 0;

            for (int y = 0; y < hight; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var coord = new Vector2Int(x, y);
                    var cell = ElementManager_LargeCell.Instance.GetElement(coord);

                    if (cell.Data.CellType == FlowTilemapCellType.Floor) continue;

                    float px = x * cellSize;
                    float py = y * cellSize;

                    // 一个格子的4顶点 (放在 y=0 平面上)
                    verts.Add(new Vector3(px, 0, py));
                    verts.Add(new Vector3(px + cellSize, 0, py));
                    verts.Add(new Vector3(px + cellSize, 0, py + cellSize));
                    verts.Add(new Vector3(px, 0, py + cellSize));

                    // 两个三角形
                    tris.Add(index + 0);
                    tris.Add(index + 2);
                    tris.Add(index + 1);

                    tris.Add(index + 0);
                    tris.Add(index + 3);
                    tris.Add(index + 2);

                    index += 4;
                }
            }

            m_MeshFilter.sharedMesh.Clear();
            m_MeshFilter.sharedMesh.SetVertices(verts);
            m_MeshFilter.sharedMesh.SetTriangles(tris, 0);
            m_MeshFilter.sharedMesh.RecalculateNormals();
            m_MeshFilter.sharedMesh.RecalculateBounds();
        }

        public void Clear()
        {
            if (m_MeshFilter.sharedMesh != null)
            {
                m_MeshFilter.sharedMesh.Clear();
            }
        }
    }
}
