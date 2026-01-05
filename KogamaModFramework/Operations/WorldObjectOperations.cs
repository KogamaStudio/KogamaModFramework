using Il2CppSystem;
using Il2Cpp;
using Il2CppMV.WorldObject;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KogamaModFramework.Conversion;
using Il2CppMV.Common;
using System.Collections;

namespace KogamaModFramework.Operations;

public static class WorldObjectOperations
{
    private static int RootGroupId => MVGameControllerBase.WOCM?.RootGroup?.Id ?? -1;

    public static void AddLink(int outputWoId, int inputWoId)
    {
        var outputWo = GetObject(outputWoId);
        var inputWo = GetObject(inputWoId);

        if (outputWo == null || inputWo == null) return;

        Link link = new Link();
        link.outputWOID = outputWoId;
        link.inputWOID = inputWoId;

        MVGameControllerBase.OperationRequests.AddLink(link);
    }

    public static void RemoveLink(int outputWoId, int inputWoId)
    {
        var outputWo = GetObject(outputWoId);
        if (outputWo == null) return;

        foreach (var link in outputWo.OutputLinkRefs)
        {
            if (link.outputWOID == outputWoId && link.inputWOID == inputWoId)
            {
                MVGameControllerBase.OperationRequests.RemoveLink(link.id);
                return;
            }
        }
    }

    public static List<int> GetAllInputLinks(int woId)
    {
        var wo = GetObject(woId) as MVWorldObjectClient;
        if (wo == null) return new List<int>();

        var result = new List<int>();
        foreach (var link in wo.InputLinkRefs)
        {
            result.Add(link.outputWOID);
        }
        return result;
    }

    public static List<int> GetAllOutputLinks(int woId)
    {
        var wo = GetObject(woId) as MVWorldObjectClient;
        if (wo == null) return new List<int>();

        var result = new List<int>();
        foreach (var link in wo.OutputLinkRefs)
        {
            result.Add(link.inputWOID);
        }
        return result;
    }

    public static List<int> GetAllWorldObjectIds()
    {
        var wocm = MVGameControllerBase.WOCM;
        if (wocm == null) return new List<int>();

        var ids = new Il2CppSystem.Collections.Generic.HashSet<int>();
        wocm.GetAllWoIds(RootGroupId, ids);

        var result = new List<int>();

        foreach (var id in ids)
        {
            result.Add(id);
        }

        return result;
    }

    public static MVWorldObject GetObject(int woId)
    {
        var wo = MVGameControllerBase.WOCM?.GetWorldObjectClient(woId);
        return wo;
    }

    public static void SetPosition(int woId, Vector3 position)
    {
        var wo = GetObject(woId) as MVWorldObjectClient;
        if (wo == null) return;

        wo.Position = position;
        MVGameControllerBase.OperationRequests.TransferOwnership(woId, 0, wo.Transform);
    }

    public static void SetRotation(int woId, Quaternion rotation)
    {
        var wo = GetObject(woId) as MVWorldObjectClient;
        if (wo == null) return;

        wo.Rotation = rotation;
        MVGameControllerBase.OperationRequests.TransferOwnership(woId, 0, wo.Transform);
    }

    public static IEnumerator AddItemToWorld(int itemId, Vector3 position, Quaternion rotation, System.Action<int> callback)
    {
        int pre = MVGameControllerBase.WOCM.worldObjects.Count;
        MVGameControllerBase.OperationRequests.AddItemToWorld(itemId, RootGroupId, position, rotation, true, true, false);

        float timeout = Time.time + 2.0f;
        while (MVGameControllerBase.WOCM.worldObjects.Count == pre && Time.time < timeout)
            yield return null;

        int nid = -1;
        foreach (int k in MVGameControllerBase.WOCM.worldObjects.Keys) if (k > nid) nid = k;
        callback(nid);
    }
}

