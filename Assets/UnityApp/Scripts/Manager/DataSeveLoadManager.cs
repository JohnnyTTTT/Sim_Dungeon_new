using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Johnny.SimDungeon
{
    public class DataSeveLoadManager : MonoBehaviour
    {
        private static ElementsSerializationData saveData = new ElementsSerializationData();
        public static DataSeveLoadManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<DataSeveLoadManager>();
                }
                return s_Instance;
            }
        }
        private static DataSeveLoadManager s_Instance;

        [SerializeField] private string localSavePath = "/SoulGames/Easy Grid Builder Pro 2/EGB Pro 2 Local Saves";
        [SerializeField] private string saveFileName = "/EGB Pro 2 Save";
        [SerializeField] private string saveExtention = ".txt";

        public void Save()
        {
            var cellSaveDataList = new List<LargeElementSaveData>();
            foreach (var cell in ElementManager_LargeCell.Instance.GetAllElements())
            {
                var cellSaveData = new LargeElementSaveData();
                cellSaveData.coord = cell.coord;
                cellSaveData.cellType = cell.cellType;
                cellSaveDataList.Add(cellSaveData);
            }
            saveData.largeElementsSaveData.largeElementsSaveData = cellSaveDataList;

            var edgeSaveDataList = new List<LargeEdgeSaveData>();
            foreach (var edge in ElementManager_Edge.Instance.GetAllElements())
            {
                var edgeSaveData = new LargeEdgeSaveData();
                edgeSaveData.coord = edge.coord;
                edgeSaveData.edgeType = edge.edgeType;
                edgeSaveData.horizontalEdge = edge.horizontalEdge;
                edgeSaveDataList.Add(edgeSaveData);
            }
            saveData.largeEdgesSaveData.largeEdgeSaveDatas = edgeSaveDataList;

            var smallCellSaveDataList = new List<SmallElementSaveData>();
            foreach (var cell in ElementManager_SmallCell.Instance.GetAllElements())
            {
                var cellSaveData = new SmallElementSaveData();
                cellSaveData.coord = cell.coord;
                cellSaveData.smallCellType = cell.cellType;
                smallCellSaveDataList.Add(cellSaveData);
            }
            saveData.smallElementsSaveData.smallElementSaveDatas = smallCellSaveDataList;

            var regionSaveDataList = new List<RegionSaveData>();
            foreach (var region in ElementManager_Region.Instance.regionList)
            {
                var regionSaveData = new RegionSaveData();
                regionSaveData.name = region.name;
                regionSaveData.roomType = region.roomType;
                regionSaveData.roomColor = region.roomColor;
                regionSaveData.containedLargeCells = region.containedLargeCells.ToList();
                regionSaveDataList.Add(regionSaveData);
            }
            saveData.regionsSaveData.regionSaveDatas = regionSaveDataList;
            File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
        }

        public  void Load()
        {
            var saveFile = File.ReadAllText(SaveFileName());
            saveData = JsonUtility.FromJson<ElementsSerializationData>(saveFile);
            ElementManager_LargeCell.Instance.LoadElements(saveData.largeElementsSaveData.largeElementsSaveData);
            ElementManager_Edge.Instance.LoadElements(saveData.largeEdgesSaveData.largeEdgeSaveDatas);
            ElementManager_SmallCell.Instance.LoadElements(saveData.smallElementsSaveData.smallElementSaveDatas);
            ElementManager_Region.Instance.LoadElements(saveData.regionsSaveData.regionSaveDatas);

        }

        public  string SaveFileName()
        {
            return Application.dataPath + localSavePath + saveFileName + saveExtention;
        }
    }
}
