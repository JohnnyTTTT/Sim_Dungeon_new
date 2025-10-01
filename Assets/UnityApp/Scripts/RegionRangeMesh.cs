
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// 请根据您的项目需求添加其他必要的 using 语句

namespace Johnny.SimDungeon
{
    public class RegionRangeMesh : MonoBehaviour
    {
        private static readonly Vector2[] s_CellLocalUVs = new[]
        {
        new Vector2(0, 0), // v0: 左下
        new Vector2(0, 1), // v1: 左上
        new Vector2(1, 1), // v2: 右上
        new Vector2(1, 0)  // v3: 右下
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

                // --- Atlas 映射核心 ---
                // 1. 获取该 Cell 对应的 Atlas 索引
                var atlasIndex = AtlasUtility.GetAtlasIndex(cellCoord, regionCells);

                // 2. 将索引转换为实际的 UV 起始坐标 (左下角)
                // 这个 Vector2 已经是 [0, 1] 范围内的 UV 坐标，例如 (0.285, 0.571)
                var atlasUVStart = AtlasUtility.GetAtlasUVStart(atlasIndex);

                //Vector2 atlasUVStart = RegionAtlasMapper.GetAtlasUVStart(atlasIndex);
                //Debug.Log(atlasIndex);
                // 3. 遍历 4 个顶点，计算最终 UV 和 Vertex
                for (int i = 0; i < 4; i++)
                {
                    var localUV = s_CellLocalUVs[i];

                    // 顶点位置 (无收缩，收缩由贴图本身负责)
                    var vX = worldX + localUV.x * cellSize;
                    var vY = worldY + localUV.y * cellSize;

                    // 最终 UV = Atlas 起始点 + 局部 UV * Atlas 单元尺寸
                    var finalU = atlasUVStart.x + localUV.x * atlasUnit;
                    var finalV = atlasUVStart.y + localUV.y * atlasUnit;

                    vertices.Add(new Vector3(vX, 0f, vY));
                    uvs0.Add(new Vector2(finalU, finalV));
                    //uvs0.Add(new Vector2(0f, 1f));
                }

                // 4. 添加三角形 (索引保持不变)
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
            //m_MeshFilter.sharedMesh.uv2 = uvs1.ToArray();  // UV2 传递状态
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