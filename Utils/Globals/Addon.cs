using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace HaselTweaks.Utils.Globals;

public static unsafe class Addon
{
    public static AtkUnitBase* GetAddon(string name, int index = 1)
    {
        var ptr = (nint)AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName(name, index);
        return ptr != 0 && (*(byte*)(ptr + 0x189) & 1) == 1 // IsAddonReady
            ? (AtkUnitBase*)ptr
            : null;
    }

    public static bool TryGetAddon(string name, int index, out AtkUnitBase* addon)
        => (addon = GetAddon(name, index)) != null;

    public static bool TryGetAddon(string name, out AtkUnitBase* addon)
        => TryGetAddon(name, 1, out addon);

    public static bool TryGetAddon<T>(string name, int index, out T* addon) where T : unmanaged
        => (addon = (T*)GetAddon(name, index)) != null;

    public static bool TryGetAddon<T>(string name, out T* addon) where T : unmanaged
        => TryGetAddon(name, 1, out addon);

    public static AtkUnitBase* GetAddon(ushort id)
        => RaptureAtkModule.Instance()->AtkModule.IsAddonReady(id)
            ? AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonById(id)
            : null;

    public static bool TryGetAddon(ushort id, out AtkUnitBase* addon)
        => (addon = GetAddon(id)) != null;

    public static AtkUnitBase* GetAddon(uint id)
        => GetAddon((ushort)id);

    public static bool TryGetAddon(uint id, out AtkUnitBase* addon)
        => TryGetAddon((ushort)id, out addon);

    public static AtkUnitBase* GetAddon(AgentInterface* agent)
        => agent != null && agent->IsAgentActive() ? GetAddon((ushort)agent->GetAddonID()) : null;

    public static bool TryGetAddon(AgentInterface* agent, out AtkUnitBase* addon)
        => (addon = GetAddon(agent)) != null;

    public static AtkUnitBase* GetAddon(AgentId id)
        => GetAddon(GetAgent<AgentInterface>(id));

    public static bool TryGetAddon(AgentId id, out AtkUnitBase* addon)
        => (addon = GetAddon(id)) != null;

    public static T* GetAddon<T>(ushort id) where T : unmanaged
        => (T*)GetAddon(id);

    public static T* GetAddon<T>(uint id) where T : unmanaged
        => GetAddon<T>((ushort)id);

    public static T* GetAddon<T>(AgentInterface* agent) where T : unmanaged
        => agent != null && agent->IsAgentActive() ? GetAddon<T>(agent->GetAddonID()) : null;

    public static bool TryGetAddon<T>(AgentInterface* agent, out T* addon) where T : unmanaged
        => (addon = GetAddon<T>(agent)) != null;

    public static bool TryGetAddon<T>(AgentId id, out T* addon) where T : unmanaged
        => TryGetAddon(GetAgent<AgentInterface>(id), out addon);
}
