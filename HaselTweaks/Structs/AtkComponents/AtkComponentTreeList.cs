using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.STD;

namespace HaselTweaks.Structs;

[StructLayout(LayoutKind.Explicit, Size = 0x228)]
public unsafe partial struct AtkComponentTreeList
{
    [FieldOffset(0)] public FFXIVClientStructs.FFXIV.Component.GUI.AtkComponentTreeList Base;

    [FieldOffset(0x1A8)] public StdVector<Pointer<AtkComponentTreeListItem>> Items;

    [FieldOffset(0x21C)] public byte LayoutRefreshPending;

    [VirtualFunction(31)]
    public readonly partial void SelectItem(uint index, bool dispatchEvent);

    [VirtualFunction(32)]
    public readonly partial void DeselectItem();

    [MemberFunction("E8 ?? ?? ?? ?? 44 39 BD ?? ?? ?? ??")]
    public readonly partial void LoadAtkValues(
        uint atkValuesCount,
        AtkValue* atkValues,
        uint uintValuesOffset,
        uint stringValuesOffset,
        uint uintValuesCountPerItem,
        uint stringValuesCountPerItem,
        uint itemCount,
        nint a9);

    // this collapses all other groups too, pretty handy
    [MemberFunction("E8 ?? ?? ?? ?? 44 88 AB ?? ?? ?? ?? 80 4F 42 10")]
    public readonly partial void ExpandGroup(AtkComponentTreeListItem* group, bool a3 = false);

    [MemberFunction("E8 ?? ?? ?? ?? 44 38 60 45")]
    public readonly partial AtkComponentTreeListItem* GetItem(uint index);

    // only creates it, doesn't add it to the list
    [MemberFunction("E8 ?? ?? ?? ?? 48 8B D8 45 85 ED")]
    public readonly partial AtkComponentTreeListItem* CreateItem();
}

[StructLayout(LayoutKind.Explicit, Size = 0x48)]
public unsafe struct AtkComponentTreeListItem
{
    [FieldOffset(0)] public StdVector<uint> UIntValues; // first one is always AtkComponentTreeListItemType
    [FieldOffset(0x18)] public StdVector<Pointer<byte>> StringValues; // first one is always the displayed text
    [FieldOffset(0x30)] public AtkComponentListItemRenderer* Renderer;

    [FieldOffset(0x42)] public byte Flags; // for groups: 1 = expanded
    [FieldOffset(0x43)] public byte Flags2;
}

public enum AtkComponentTreeListItemType : uint
{
    Leaf = 0,
    LastLeafInGroup = 1,
    CollapsibleGroupHeader = 2, // see AddonMJICraftScheduleSetting
    GroupHeader = 4, // always expanded, see AddonTelepotTown
}
