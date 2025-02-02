﻿using System;
using System.Collections.Generic;
using System.Linq;
using Depra.SerializedReference.Dropdown.Runtime;
using Plugins.ComfyGoogleSheets.Runtime;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Plugins.ComfyGoogleSheets.Editor.SerializationModes.Implemented
{
    [Serializable]
    [SubtypeMenuAlias(nameof(ExternalAssetsSerializationMode))]
    public class ExternalAssetsSerializationMode : BaseTableRowSerializationMode
    {
        protected override bool CanSerializeToRowInternal(Object asset)
        {
            return GetSerializers().Any(serializer => serializer.GetSerializationType == asset.GetType());
        }

        protected override TableRow SerializeToRowInternal(Object asset)
        {
            var serializer = GetSerializers()
                .FirstOrDefault(serializer => serializer.GetSerializationType == asset.GetType());

            return serializer?.SerializeToTableRow(asset);
        }

        protected override void DeserializeFromRowInternal(Object asset, TableRow row)
        {
            var serializer = GetSerializers()
                .FirstOrDefault(serializer => serializer.GetSerializationType == asset.GetType());

            serializer?.DeserializeFromTableRow(row, asset);
        }

        private IEnumerable<IExternalAssetTableRowConverter> GetSerializers()
        {
            return TypeCache.GetTypesDerivedFrom<IExternalAssetTableRowConverter>()
                .Select(sType => Activator.CreateInstance(sType) as IExternalAssetTableRowConverter);
        }
    }
}