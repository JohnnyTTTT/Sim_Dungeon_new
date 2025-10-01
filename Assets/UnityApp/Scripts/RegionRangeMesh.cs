
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// �����������Ŀ�������������Ҫ�� using ���

namespace Johnny.SimDungeon
{
    public class RegionRangeMesh : MonoBehaviour
    {
        private static readonly Vector2[] s_CellLocalUVs = new[]
        {
        new Vector2(0, 0), // v0: ����
        new Vector2(0, 1), // v1: ����
        new Vector2(1, 1), // v2: ����
        new Vector2(1, 0)  // v3: ����
        };

        [SerializeField] private MeshFilter m_MeshFilter;

        private void BuildRegionMeshAtlas(HashSet<Vector2Int> regionCells, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs0)
        {
            var atlasUnit = AtlasUtility.GetAtlasUnitSize();
            var cellSize = 2;

            foreach (var cellCoord in regionCells)
            {
                var currentVertexCount = vertices.Count;
                var worldX = cellCoord.x * cellSize;
                var worldY = cellCoord.y * cellSize;

                // --- Atlas ӳ����� ---
                // 1. ��ȡ�� Cell ��Ӧ�� Atlas ����
                var atlasIndex = AtlasUtility.GetAtlasIndex(cellCoord, regionCells);

                // 2. ������ת��Ϊʵ�ʵ� UV ��ʼ���� (���½�)
                // ��� Vector2 �Ѿ��� [0, 1] ��Χ�ڵ� UV ���꣬���� (0.285, 0.571)
                var atlasUVStart = AtlasUtility.GetAtlasUVStart(atlasIndex);

                //Vector2 atlasUVStart = RegionAtlasMapper.GetAtlasUVStart(atlasIndex);
                //Debug.Log(atlasIndex);
                // 3. ���� 4 �����㣬�������� UV �� Vertex
                for (int i = 0; i < 4; i++)
                {
                    var localUV = s_CellLocalUVs[i];

                    // ����λ�� (����������������ͼ������)
                    var vX = worldX + localUV.x * cellSize;
                    var vY = worldY + localUV.y * cellSize;

                    // ���� UV = Atlas ��ʼ�� + �ֲ� UV * Atlas ��Ԫ�ߴ�
                    var finalU = atlasUVStart.x + localUV.x * atlasUnit;
                    var finalV = atlasUVStart.y + localUV.y * atlasUnit;

                    vertices.Add(new Vector3(vX, 0f, vY));
                    uvs0.Add(new Vector2(finalU, finalV));
                    //uvs0.Add(new Vector2(0f, 1f));
                }

                // 4. ��������� (�������ֲ���)
                triangles.Add(currentVertexCount + 0);
                triangles.Add(currentVertexCount + 1);
                triangles.Add(currentVertexCount + 2);

                triangles.Add(currentVertexCount + 0);
                triangles.Add(currentVertexCount + 2);
                triangles.Add(currentVertexCount + 3);
            }
        }

        public void UpdateRegionMap(Region region)
        {
            if (m_MeshFilter.sharedMesh == null)
            {
                m_MeshFilter.sharedMesh = new Mesh();
            }

            var regionCells = region.containedLargeCells.Select(x => x.coord).ToHashSet();

            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs0 = new List<Vector2>();

            BuildRegionMeshAtlas(regionCells, vertices, triangles, uvs0);

            m_MeshFilter.sharedMesh.Clear();
            m_MeshFilter.sharedMesh.vertices = vertices.ToArray();
            m_MeshFilter.sharedMesh.triangles = triangles.ToArray();

            m_MeshFilter.sharedMesh.uv = uvs0.ToArray();
            //m_MeshFilter.sharedMesh.uv2 = uvs1.ToArray();  // UV2 ����״̬
            //foreach (var item in uvs0)
            //{
            //    Debug.Log(item);
            //}
            m_MeshFilter.sharedMesh.RecalculateNormals();
            m_MeshFilter.sharedMesh.RecalculateBounds();
        }

        public void Dispose()
        {
            if (m_MeshFilter.sharedMesh == null)
            {
                if (Application.isPlaying)
                {
                    Destroy(m_MeshFilter.sharedMesh);
                }
                else
                {
                    DestroyImmediate(m_MeshFilter.sharedMesh);
                }
            }
        }
    }
}