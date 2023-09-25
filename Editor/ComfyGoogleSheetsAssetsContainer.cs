using System;
using System.Collections.Generic;
using Depra.SerializedReference.Dropdown.Runtime;
using Plugins.ComfyGoogleSheets.Editor.SerializationModes;
using Plugins.ComfyGoogleSheets.Editor.SerializationModes.Implemented;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.ComfyGoogleSheets.Editor
{
    [CreateAssetMenu(menuName = "ComfyGoogleSheets/Assets Container", fileName = "new ComfyGoogleSheetsAssetsContainer")]
    public class ComfyGoogleSheetsAssetsContainer : ScriptableObject
    {
        public GoogleSheetsCredits credits;

        //[ShowIf("@credits != null"), ValueDropdown("@credits.TableTitles")]
        public string selectedTable;

        [SerializeReference] [SubtypeMenu]
        public BaseTableRowSerializationMode serializationMode = new AttributeSerializationMode();

        public List<Object> assets;

        [ContextMenu(nameof(PushDataToTable))]
        public void PushDataToTable() => SerializeAssetsToTable(assets);

        [ContextMenu(nameof(PullDataFromTableToAssets))]
        public void PullDataFromTableToAssets()
        {
            if (!EditorUtility.DisplayDialog("Pull Data", "Are you sure you want to pull data from the table?", "Yes", "No"))
            {
                return;
            }

            DeserializeAssetsFromTable(assets);
        }

        [ContextMenu(nameof(ClearRemoteTable))]
        public void ClearRemoteTable()
        {
            // Unity confirmation window
            if (!EditorUtility.DisplayDialog("Clear Table", "Are you sure you want to clear the table?", "Yes", "No"))
            {
                return;
            }

            credits.GetTable(selectedTable).ClearTable();
        }

        private void SerializeAssetsToTable(IEnumerable<Object> selectedAssets)
        {
            if (!credits.HasTable(selectedTable)) throw new Exception("Table not found");

            var table = credits.GetTable(selectedTable);

            foreach (var selectedAsset in selectedAssets)
            {
                if (!serializationMode.CanSerializeToRow(selectedAsset)) continue;

                var row = serializationMode.SerializeToRow(selectedAsset);

                table.WriteRow(row);
            }

            table.PushTable();

            Debug.Log("Assets serialized!");
        }

        private void DeserializeAssetsFromTable(IEnumerable<Object> selectedAssets)
        {
            if (!credits.HasTable(selectedTable)) throw new Exception("Table not found");

            var table = credits.GetTable(selectedTable);

            table.PullTable();


            foreach (var selectedAsset in selectedAssets)
            {
                if (!serializationMode.CanSerializeToRow(selectedAsset)) continue;

                var row = table.ReadRow(selectedAsset.name);

                serializationMode.DeserializeFromRow(selectedAsset, row);
            }

            AssetDatabase.Refresh();

            Debug.Log("Assets Deserialized!");
        }

        [ContextMenu(nameof(OpenTableInBrowser))]
        public void OpenTableInBrowser()
        {
            var url = $"https://docs.google.com/spreadsheets/d/{credits.GetTable(selectedTable, false).spreadsheetId}/edit#gid=0";

            Application.OpenURL(url);
        }
    }
}