using System;
using HarmonyLib;

namespace Server
{
    public static class RPC
    {
        public static void RPC_SendGenData(ZRpc rpc, ZPackage pkg)
        {
            WorldGenOptions.GenOptions.log.LogInfo("Received gen data.");
            WorldGenData data = new WorldGenData();
            try
            {
                data.ReadGenData(ref pkg);
            }
            catch
            {
                WorldGenOptions.GenOptions.log.LogWarning("Server sent incomplete gen data. Server is likely out of date.");
            }
            finally
            {
                WorldGenOptions.GenOptions.hasGenData = true;
                WorldGenOptions.GenOptions.usingData = data;
            }
        }
    }

    [HarmonyPatch(typeof(ZNet))]
    [HarmonyPatch(nameof(ZNet.SendPeerInfo))]
    public static class SendRequest_Patch
    {
        public static void Prefix(ZNet __instance, ZRpc rpc)
        {
            if(__instance.IsServer())
            {
                ZPackage pkg = new ZPackage();
                WorldGenOptions.GenOptions.savedData.WriteGenData(ref pkg);
                WorldGenOptions.GenOptions.log.LogInfo("Calling RPC request.");
                rpc.Invoke("SendGenData", pkg);
            }
        }
    }

    [HarmonyPatch(typeof(ZNet))]
    [HarmonyPatch(nameof(ZNet.OnNewConnection))]
    public static class RegisterRPC_GenData
    {
        public static void Prefix(ZNetPeer peer)
        {
            try
            {
                peer.m_rpc.Register<ZPackage>("SendGenData", new Action<ZRpc, ZPackage>(RPC.RPC_SendGenData));
            }
            catch (Exception e)
            {
                WorldGenOptions.GenOptions.log.LogError(e);
            }
        }
    }
}