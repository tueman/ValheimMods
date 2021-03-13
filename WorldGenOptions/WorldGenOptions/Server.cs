using System;
using HarmonyLib;

namespace Server
{
    public static class RPC
    {
        public static void RPC_RequestGenData(long sender)
        {
            WorldGenOptions.GenOptions.log.LogInfo("Received request for gen dat from " + sender);
            ZPackage pkg = new ZPackage();
            WorldGenOptions.GenOptions.savedData.WriteGenData(ref pkg);
            ZRoutedRpc.instance.InvokeRoutedRPC(sender, "SendGenData", new object[] { pkg });
        }

        public static void RPC_SendGenData(long sender, ZPackage pkg)
        {
            WorldGenOptions.GenOptions.log.LogInfo("Received gen data from " + sender);
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
                WorldGenOptions.GenOptions.usingData = data;
            }
        }
    }

    [HarmonyPatch(typeof(WorldGenerator))]
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(World) })]
    public static class RegisterRPC_GenData
    {
        public static void Prefix(ref World world)
        {
            if(world.m_menu)
            {
                return;
            }
            try
            {
                ZRoutedRpc.instance.Register("RequestGenData", new Action<long>(RPC.RPC_RequestGenData));
                ZRoutedRpc.instance.Register("SendGenData", new Action<long, ZPackage>(RPC.RPC_SendGenData));
            }
            catch (Exception e)
            {
                WorldGenOptions.GenOptions.log.LogError(e);
            }
        }
    }
}