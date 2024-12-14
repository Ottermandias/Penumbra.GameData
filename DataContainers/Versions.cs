namespace Penumbra.GameData.DataContainers;

public static class Version
{
    // Groups
    public const int GlobalOffset          = 0;
    public const int UsesExtractTextOffset = 0;
    public const int UsesTitleCaseOffset   = 0 + UsesExtractTextOffset;

    // Misc
    public const int DictActionOffset = 9;
    public const int DictAction       = GlobalOffset + DictActionOffset;

    public const int DictStainOffset = 5;
    public const int DictStain       = UsesExtractTextOffset + DictStainOffset;

    public const int HumanModelOffset = 5;
    public const int HumanModelList   = GlobalOffset + HumanModelOffset;

    public const int IdentificationListModelsOffset = 9;
    public const int IdentificationListModels       = GlobalOffset + IdentificationListModelsOffset;

    // Names
    public const int DictBNpcNamesOffset = 5;
    public const int DictBNpcNames       = GlobalOffset + DictBNpcNamesOffset;

    public const int DictCompanionOffset = 9;
    public const int DictCompanion       = UsesTitleCaseOffset + DictCompanionOffset;

    public const int DictBNpcOffset = 9;
    public const int DictBNpc       = UsesTitleCaseOffset + DictBNpcOffset;

    public const int DictWorldOffset = 9;
    public const int DictWorld       = UsesExtractTextOffset + DictWorldOffset;

    public const int DictMountOffset = 9;
    public const int DictMount       = UsesTitleCaseOffset + DictMountOffset;

    public const int DictOrnamentOffset = 9;
    public const int DictOrnament       = UsesTitleCaseOffset + DictOrnamentOffset;

    public const int DictENpcOffset = 9;
    public const int DictENpc       = UsesTitleCaseOffset + DictENpcOffset;

    public const int NameDictsOffset =
        DictCompanionOffset + DictWorldOffset + DictMountOffset + DictOrnamentOffset + DictBNpcOffset + DictENpcOffset - 54;

    public const int DictEmoteOffset = 9;
    public const int DictEmote       = UsesExtractTextOffset + DictEmoteOffset;

    public const int DictModelCharaOffset = -1;
    public const int DictModelChara       = NameDictsOffset + DictBNpcNamesOffset + UsesTitleCaseOffset + DictModelCharaOffset;

    // Items
    public const int DictBonusItemsOffset = 3;
    public const int DictBonusItems       = UsesExtractTextOffset + DictBonusItemsOffset;

    public const int ItemsByTypeOffset = 2;
    public const int ItemsByType       = DictBonusItemsOffset + UsesExtractTextOffset + ItemsByTypeOffset;

    public const int ItemsPrimaryModelOffset = 1;
    public const int ItemsPrimaryModel       = ItemsByType + ItemsPrimaryModelOffset;

    public const int ItemsSecondaryModelOffset = 1;
    public const int ItemsSecondaryModel       = ItemsByType + ItemsSecondaryModelOffset;

    public const int ItemsTertiaryModelOffset = 1;
    public const int ItemsTertiaryModel       = ItemsByType + ItemsTertiaryModelOffset;

    public const int IdentificationListEquipmentOffset = 7;
    public const int IdentificationListEquipment       = ItemsByType + IdentificationListEquipmentOffset;

    public const int IdentificationListWeaponsOffset = ItemsByType + 5;
    public const int IdentificationListWeapons       = ItemsByType + IdentificationListWeaponsOffset;

    public const int RestrictedItemsOffset = 4;
    public const int RestrictedItems       = GlobalOffset + RestrictedItemsOffset;
}
