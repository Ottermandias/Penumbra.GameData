using Lumina.Excel;
using Lumina.Excel.Sheets;
using Luna;
using Penumbra.GameData.Data;
using Penumbra.GameData.Enums;

namespace Penumbra.GameData.DataContainers.Bases;

/// <summary> Helper functions to create gender restricted item collections. </summary>
public static class GenderRestrictedItems
{
    /// <summary> Go through all items and check if there are any restricted items that are not known yet. </summary>
    /// <param name="dict"> The dictionary to add to. </param>
    /// <param name="items"> The sheet of all items. </param>
    /// <param name="log"> A logger. </param>
    /// <param name="restriction"> The desired restriction, 1 for male, 2 for female. </param>
    internal static void AddUnknownItems(Dictionary<uint, uint> dict, ExcelSheet<Item> items, Logger log, byte restriction)
    {
        var unhandled = 0;
        foreach (var item in items.Where(i => i.EquipRestriction == restriction && i.EquipSlotCategory.RowId > 0))
        {
            // Skip Scion Chronocler's Ringbands and Scion Thaumaturge's Moccasins as they are not actually restricted.
            if (item.RowId is 13700 or 13699)
                continue;

            var value = (uint)item.ModelMain | ((uint)((EquipSlot)item.EquipSlotCategory.RowId).ToSlot() << 24);
            if (dict.ContainsKey(value) || KnownItems.Any(restriction == 2 ? i => i.MaleId == item.RowId : i => i.FemaleId == item.RowId))
                continue;

            ++unhandled;
            AddEmperor(item);

            log.Warning(
                $"{item.RowId:D5} {item.Name.ExtractTextExtended()} is restricted to {(restriction == 2 ? "male" : "female")} characters but is not known, redirected to Emperor. {item.EquipSlotCategory.RowId}");
        }

        if (unhandled > 0)
            log.Warning($"{unhandled} Items were restricted to {(restriction == 2 ? "male" : "female")} characters but unhandled.");

        return;

        // Add a redirection to emperors gear for unknown items.
        void AddEmperor(Item item)
        {
            var slot = ((EquipSlot)item.EquipSlotCategory.RowId).ToSlot();
            var emperor = ((uint)slot << 24)
              | slot switch
                {
                    EquipSlot.Head    => 279u,
                    EquipSlot.Body    => 279u,
                    EquipSlot.Hands   => 279u,
                    EquipSlot.Legs    => 279u,
                    EquipSlot.Feet    => 279u,
                    EquipSlot.Ears    => 053u,
                    EquipSlot.Neck    => 053u,
                    EquipSlot.Wrists  => 053u,
                    EquipSlot.RFinger => 053u,
                    EquipSlot.LFinger => 053u,
                    _                 => 0u,
                };
            if (emperor == 0)
                return;

            dict.TryAdd((uint)item.ModelMain | ((uint)slot << 24), emperor);
        }
    }

    /// <summary> Add a known item restricted to female characters to the dictionary. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static void AddItemFemale(Dictionary<uint, uint> dict, RestrictedItemPair pair, ExcelSheet<Item> items, Logger log)
        => AddItem(dict, pair, items, log, 2);

    /// <summary> Add a known item restricted to male characters to the dictionary. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static void AddItemMale(Dictionary<uint, uint> dict, RestrictedItemPair pair, ExcelSheet<Item> items, Logger log)
        => AddItem(dict, pair, items, log, 1);

    /// <summary> Add a known item to the dictionary. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void AddItem(Dictionary<uint, uint> dict, RestrictedItemPair pair, ExcelSheet<Item> items, Logger log, byte direction)
    {
        if ((pair.Add & direction) == 0)
            return;

        // Get the direction.
        var (source, target, restriction) = direction == 1 ? (pair.MaleId, pair.FemaleId, 2) : (pair.FemaleId, pair.MaleId, 3);
        if (!items.TryGetRow(source, out var sourceRow) || !items.TryGetRow(target, out var targetRow))
        {
            log.Warning($"Could not add item pair [{pair.MaleId}, {pair.FemaleId}] to restricted items.");
            return;
        }

        if (sourceRow.EquipRestriction != restriction)
        {
            log.Warning($"{sourceRow.Name.ExtractTextExtended()} is not restricted anymore.");
            return;
        }

        var sourceSlot = ((EquipSlot)sourceRow.EquipSlotCategory.RowId).ToSlot();
        var targetSlot = ((EquipSlot)targetRow.EquipSlotCategory.RowId).ToSlot();
        if (!sourceSlot.IsAccessory() && !sourceSlot.IsEquipment())
        {
            log.Warning($"{sourceRow.Name.ExtractTextExtended()} is not equippable to a known slot.");
            return;
        }

        if (sourceSlot != targetSlot)
        {
            log.Warning($"{sourceRow.Name.ExtractTextExtended()} and {targetRow.Name.ExtractTextExtended()} are not compatible.");
            return;
        }

        var sourceId = (uint)sourceRow.ModelMain | ((uint)sourceSlot << 24);
        var targetId = (uint)targetRow.ModelMain | ((uint)targetSlot << 24);
        if (sourceId != targetId)
            dict.TryAdd(sourceId, targetId);
    }

    /// <summary> A pair of items to redirect and the directions to redirect to. </summary>
    /// <param name="MaleId"></param>
    /// <param name="FemaleId"></param>
    /// <param name="Add"></param>
    internal readonly record struct RestrictedItemPair(uint MaleId, uint FemaleId, byte Add);

    // @formatter:off
    /// <summary> All currently existing and known gender restricted items. </summary>
    internal static readonly RestrictedItemPair[] KnownItems =
    [
        new(02967, 02970, 3), // Lord's Yukata (Blue)                       <-> Lady's Yukata (Red)
        new(02968, 02971, 3), // Lord's Yukata (Green)                      <-> Lady's Yukata (Blue)
        new(02969, 02972, 3), // Lord's Yukata (Grey)                       <-> Lady's Yukata (Black)
        new(02973, 02978, 3), // Red Summer Top                             <-> Red Summer Halter
        new(02974, 02979, 3), // Green Summer Top                           <-> Green Summer Halter
        new(02975, 02980, 3), // Blue Summer Top                            <-> Blue Summer Halter
        new(02976, 02981, 3), // Solar Summer Top                           <-> Solar Summer Halter
        new(02977, 02982, 3), // Lunar Summer Top                           <-> Lunar Summer Halter
        new(02996, 02997, 3), // Hempen Undershirt                          <-> Hempen Camise
        new(03280, 03283, 3), // Lord's Drawers (Black)                     <-> Lady's Knickers (Black)
        new(03281, 03284, 3), // Lord's Drawers (White)                     <-> Lady's Knickers (White)
        new(03282, 03285, 3), // Lord's Drawers (Gold)                      <-> Lady's Knickers (Gold)
        new(03286, 03291, 3), // Red Summer Trunks                          <-> Red Summer Tanga
        new(03287, 03292, 3), // Green Summer Trunks                        <-> Green Summer Tanga
        new(03288, 03293, 3), // Blue Summer Trunks                         <-> Blue Summer Tanga
        new(03289, 03294, 3), // Solar Summer Trunks                        <-> Solar Summer Tanga
        new(03290, 03295, 3), // Lunar Summer Trunks                        <-> Lunar Summer Tanga
        new(03307, 03308, 3), // Hempen Underpants                          <-> Hempen Pantalettes
        new(03748, 03749, 3), // Lord's Clogs                               <-> Lady's Clogs
        new(06045, 06041, 3), // Bohemian's Coat                            <-> Guardian Corps Coat
        new(06046, 06042, 3), // Bohemian's Gloves                          <-> Guardian Corps Gauntlets
        new(06047, 06043, 3), // Bohemian's Trousers                        <-> Guardian Corps Skirt
        new(06048, 06044, 3), // Bohemian's Boots                           <-> Guardian Corps Boots
        new(06094, 06098, 3), // Summer Evening Top                         <-> Summer Morning Halter
        new(06095, 06099, 3), // Summer Evening Trunks                      <-> Summer Morning Tanga
        new(06096, 06100, 3), // Striped Summer Top                         <-> Striped Summer Halter
        new(06097, 06101, 3), // Striped Summer Trunks                      <-> Striped Summer Tanga
        new(06102, 06104, 3), // Black Summer Top                           <-> Black Summer Halter
        new(06103, 06105, 3), // Black Summer Trunks                        <-> Black Summer Tanga
        new(08532, 08535, 3), // Lord's Yukata (Blackflame)                 <-> Lady's Yukata (Redfly)
        new(08533, 08536, 3), // Lord's Yukata (Whiteflame)                 <-> Lady's Yukata (Bluefly)
        new(08534, 08537, 3), // Lord's Yukata (Blueflame)                  <-> Lady's Yukata (Pinkfly)
        new(08542, 08549, 3), // Ti Leaf Lei                                <-> Coronal Summer Halter
        new(08543, 08550, 3), // Red Summer Maro                            <-> Red Summer Pareo
        new(08544, 08551, 3), // South Seas Talisman                        <-> Sea Breeze Summer Halter
        new(08545, 08552, 3), // Blue Summer Maro                           <-> Sea Breeze Summer Pareo
        new(08546, 08553, 3), // Coeurl Talisman                            <-> Coeurl Beach Halter
        new(08547, 08554, 3), // Coeurl Beach Maro                          <-> Coeurl Beach Pareo
        new(08548, 08555, 3), // Coeurl Beach Briefs                        <-> Coeurl Beach Tanga
        new(10316, 10317, 3), // Southern Seas Vest                         <-> Southern Seas Swimsuit
        new(10318, 10319, 3), // Southern Seas Trunks                       <-> Southern Seas Tanga
        new(10320, 10321, 3), // Striped Southern Seas Vest                 <-> Striped Southern Seas Swimsuit
        new(13300, 13639, 3), // Lord's Suikan                              <-> Lady's Suikan
        new(13724, 13725, 3), // Little Lord's Clogs                        <-> Little Lady's Clogs
        new(15922, 15925, 3), // Moonfire Vest                              <-> Moonfire Halter
        new(15923, 15926, 3), // Moonfire Trunks                            <-> Moonfire Tanga
        new(15924, 15927, 3), // Moonfire Caligae                           <-> Moonfire Sandals
        new(16106, 16111, 3), // Makai Mauler's Facemask                    <-> Makai Manhandler's Facemask
        new(16107, 16112, 3), // Makai Mauler's Oilskin                     <-> Makai Manhandler's Jerkin
        new(16108, 16113, 3), // Makai Mauler's Fingerless Gloves           <-> Makai Manhandler's Fingerless Gloves
        new(16109, 16114, 3), // Makai Mauler's Leggings                    <-> Makai Manhandler's Quartertights
        new(16110, 16115, 3), // Makai Mauler's Boots                       <-> Makai Manhandler's Longboots
        new(16116, 16121, 3), // Makai Marksman's Eyepatch                  <-> Makai Markswoman's Ribbon
        new(16117, 16122, 3), // Makai Marksman's Battlegarb                <-> Makai Markswoman's Battledress
        new(16118, 16123, 3), // Makai Marksman's Fingerless Gloves         <-> Makai Markswoman's Fingerless Gloves
        new(16119, 16124, 3), // Makai Marksman's Slops                     <-> Makai Markswoman's Quartertights
        new(16120, 16125, 3), // Makai Marksman's Boots                     <-> Makai Markswoman's Longboots
        new(16126, 16131, 3), // Makai Sun Guide's Circlet                  <-> Makai Moon Guide's Circlet
        new(16127, 16132, 3), // Makai Sun Guide's Oilskin                  <-> Makai Moon Guide's Gown
        new(16128, 16133, 3), // Makai Sun Guide's Fingerless Gloves        <-> Makai Moon Guide's Fingerless Gloves
        new(16129, 16134, 3), // Makai Sun Guide's Slops                    <-> Makai Moon Guide's Quartertights
        new(16130, 16135, 3), // Makai Sun Guide's Boots                    <-> Makai Moon Guide's Longboots
        new(16136, 16141, 3), // Makai Priest's Coronet                     <-> Makai Priestess's Headdress
        new(16137, 16142, 3), // Makai Priest's Doublet Robe                <-> Makai Priestess's Jerkin
        new(16138, 16143, 3), // Makai Priest's Fingerless Gloves           <-> Makai Priestess's Fingerless Gloves
        new(16139, 16144, 3), // Makai Priest's Slops                       <-> Makai Priestess's Skirt
        new(16140, 16145, 3), // Makai Priest's Boots                       <-> Makai Priestess's Longboots
        new(17204, 17209, 3), // Common Makai Mauler's Facemask             <-> Common Makai Manhandler's Facemask
        new(17205, 17210, 3), // Common Makai Mauler's Oilskin              <-> Common Makai Manhandler's Jerkin
        new(17206, 17211, 3), // Common Makai Mauler's Fingerless Gloves    <-> Common Makai Manhandler's Fingerless Glove
        new(17207, 17212, 3), // Common Makai Mauler's Leggings             <-> Common Makai Manhandler's Quartertights
        new(17208, 17213, 3), // Common Makai Mauler's Boots                <-> Common Makai Manhandler's Longboots
        new(17214, 17219, 3), // Common Makai Marksman's Eyepatch           <-> Common Makai Markswoman's Ribbon
        new(17215, 17220, 3), // Common Makai Marksman's Battlegarb         <-> Common Makai Markswoman's Battledress
        new(17216, 17221, 3), // Common Makai Marksman's Fingerless Gloves  <-> Common Makai Markswoman's Fingerless Glove
        new(17217, 17222, 3), // Common Makai Marksman's Slops              <-> Common Makai Markswoman's Quartertights
        new(17218, 17223, 3), // Common Makai Marksman's Boots              <-> Common Makai Markswoman's Longboots
        new(17224, 17229, 3), // Common Makai Sun Guide's Circlet           <-> Common Makai Moon Guide's Circlet
        new(17225, 17230, 3), // Common Makai Sun Guide's Oilskin           <-> Common Makai Moon Guide's Gown
        new(17226, 17231, 3), // Common Makai Sun Guide's Fingerless Gloves <-> Common Makai Moon Guide's Fingerless Glove
        new(17227, 17232, 3), // Common Makai Sun Guide's Slops             <-> Common Makai Moon Guide's Quartertights
        new(17228, 17233, 3), // Common Makai Sun Guide's Boots             <-> Common Makai Moon Guide's Longboots
        new(17234, 17239, 3), // Common Makai Priest's Coronet              <-> Common Makai Priestess's Headdress
        new(17235, 17240, 3), // Common Makai Priest's Doublet Robe         <-> Common Makai Priestess's Jerkin
        new(17236, 17241, 3), // Common Makai Priest's Fingerless Gloves    <-> Common Makai Priestess's Fingerless Gloves
        new(17237, 17242, 3), // Common Makai Priest's Slops                <-> Common Makai Priestess's Skirt
        new(17238, 17243, 3), // Common Makai Priest's Boots                <-> Common Makai Priestess's Longboots
        new(24599, 24602, 3), // Far Eastern Schoolboy's Hat                <-> Far Eastern Schoolgirl's Hair Ribbon
        new(24600, 24603, 3), // Far Eastern Schoolboy's Hakama             <-> Far Eastern Schoolgirl's Hakama
        new(24601, 24604, 3), // Far Eastern Schoolboy's Zori               <-> Far Eastern Schoolgirl's Boots
        new(37442, 37447, 3), // Makai Vanguard's Monocle                   <-> Makai Vanbreaker's Ribbon
        new(37443, 37448, 3), // Makai Vanguard's Battlegarb                <-> Makai Vanbreaker's Battledress
        new(37444, 37449, 3), // Makai Vanguard's Fingerless Gloves         <-> Makai Vanbreaker's Fingerless Gloves
        new(37445, 37450, 3), // Makai Vanguard's Leggings                  <-> Makai Vanbreaker's Quartertights
        new(37446, 37451, 3), // Makai Vanguard's Boots                     <-> Makai Vanbreaker's Longboots
        new(37452, 37457, 3), // Makai Harbinger's Facemask                 <-> Makai Harrower's Facemask
        new(37453, 37458, 3), // Makai Harbinger's Battlegarb               <-> Makai Harrower's Jerkin
        new(37454, 37459, 3), // Makai Harbinger's Fingerless Gloves        <-> Makai Harrower's Fingerless Gloves
        new(37455, 37460, 3), // Makai Harbinger's Leggings                 <-> Makai Harrower's Quartertights
        new(37456, 37461, 3), // Makai Harbinger's Boots                    <-> Makai Harrower's Longboots
        new(37462, 37467, 3), // Common Makai Vanguard's Monocle            <-> Common Makai Vanbreaker's Ribbon
        new(37463, 37468, 3), // Common Makai Vanguard's Battlegarb         <-> Common Makai Vanbreaker's Battledress
        new(37464, 37469, 3), // Common Makai Vanguard's Fingerless Gloves  <-> Common Makai Vanbreaker's Fingerless Gloves
        new(37465, 37470, 3), // Common Makai Vanguard's Leggings           <-> Common Makai Vanbreaker's Quartertights
        new(37466, 37471, 3), // Common Makai Vanguard's Boots              <-> Common Makai Vanbreaker's Longboots
        new(37472, 37477, 3), // Common Makai Harbinger's Facemask          <-> Common Makai Harrower's Facemask
        new(37473, 37478, 3), // Common Makai Harbinger's Battlegarb        <-> Common Makai Harrower's Jerkin
        new(37474, 37479, 3), // Common Makai Harbinger's Fingerless Gloves <-> Common Makai Harrower's Fingerless Gloves
        new(37475, 37480, 3), // Common Makai Harbinger's Leggings          <-> Common Makai Harrower's Quartertights
        new(37476, 37481, 3), // Common Makai Harbinger's Boots             <-> Common Makai Harrower's Longboots
        new(13323, 13322, 3), // Scion Thief's Tunic                        <-> Scion Conjurer's Dalmatica
        new(13693, 10034, 1), // Scion Thief's Halfgloves                    -> The Emperor's New Gloves
        new(13694, 13691, 3), // Scion Thief's Gaskins                      <-> Scion Conjurer's Chausses
        new(13695, 13692, 3), // Scion Thief's Armored Caligae              <-> Scion Conjurer's Pattens
        new(13326, 30063, 3), // Scion Thaumaturge's Robe                   <-> Scion Sorceress's Headdress
        new(13696, 30062, 3), // Scion Thaumaturge's Monocle                <-> Scion Sorceress's Robe
        new(13697, 30064, 3), // Scion Thaumaturge's Gauntlets              <-> Scion Sorceress's Shadowtalons
        new(13698, 10035, 1), // Scion Thaumaturge's Gaskins                 -> The Emperor's New Breeches
        new(13699, 30065, 2), // Scion Thaumaturge's Moccasins              <-  Scion Sorceress's High Boots
        new(13327, 15942, 3), // Scion Chronocler's Cowl                    <-> Scion Healer's Robe
        new(13701, 15943, 1), // Scion Chronocler's Tights                   -> Scion Healer's Halftights
        new(13702, 15944, 3), // Scion Chronocler's Caligae                 <-> Scion Healer's Highboots
        new(14861, 13324, 3), // Head Engineer's Goggles                    <-> Scion Striker's Visor
        new(14862, 13325, 3), // Head Engineer's Attire                     <-> Scion Striker's Attire
        new(15938, 33751, 3), // Scion Rogue's Jacket                       <-> Oracle Top
        new(15939, 10034, 1), // Scion Rogue's Armguards                     -> The Emperor's New Gloves
        new(15940, 33752, 3), // Scion Rogue's Gaskins                      <-> Oracle Leggings
        new(15941, 33753, 3), // Scion Rogue's Boots                        <-> Oracle Pantalettes
        new(16042, 16046, 3), // Abes Jacket                                <-> High Summoner's Dress
        new(16043, 16047, 3), // Abes Gloves                                <-> High Summoner's Armlets
        new(16044, 10035, 1), // Abes Halfslops                              -> The Emperor's New Breeches
        new(16045, 16048, 3), // Abes Boots                                 <-> High Summoner's Boots
        new(17473, 28553, 3), // Lord Commander's Coat                      <-> Majestic Dress
        new(17474, 28554, 3), // Lord Commander's Gloves                    <-> Majestic Wristdresses
        new(10036, 28555, 2), // Emperor's New Boots                        <-  Majestic Boots
        new(21021, 21026, 3), // Werewolf Feet                              <-> Werewolf Legs
        new(22452, 20633, 3), // Cracked Manderville Monocle                <-> Blackbosom Hat
        new(22453, 20634, 3), // Torn Manderville Coatee                    <-> Blackbosom Dress
        new(22454, 20635, 3), // Singed Manderville Gloves                  <-> Blackbosom Dress Gloves
        new(22455, 10035, 1), // Stained Manderville Bottoms                 -> The Emperor's New Breeches
        new(22456, 20636, 3), // Scuffed Manderville Gaiters                <-> Blackbosom Boots
        new(23013, 21302, 3), // Doman Liege's Dogi                         <-> Scion Liberator's Jacket
        new(23014, 21303, 3), // Doman Liege's Kote                         <-> Scion Liberator's Fingerless Gloves
        new(23015, 21304, 3), // Doman Liege's Kyakui                       <-> Scion Liberator's Pantalettes
        new(23016, 21305, 3), // Doman Liege's Kyahan                       <-> Scion Liberator's Sabatons
        new(09293, 21306, 2), // The Emperor's New Earrings                 <-  Scion Liberator's Earrings
        new(24158, 23008, 1), // Leal Samurai's Kasa                         -> Eastern Socialite's Hat
        new(24159, 23009, 1), // Leal Samurai's Dogi                         -> Eastern Socialite's Cheongsam
        new(24160, 23010, 1), // Leal Samurai's Tekko                        -> Eastern Socialite's Gloves
        new(24161, 23011, 1), // Leal Samurai's Tsutsu-hakama                -> Eastern Socialite's Skirt
        new(24162, 23012, 1), // Leal Samurai's Geta                         -> Eastern Socialite's Boots
        new(02966, 13321, 2), // Reindeer Suit                              <-  Antecedent's Attire
        new(15479, 36843, 2), // Swine Body                                 <-  Lyse's Leadership Attire
        new(21941, 24999, 2), // Ala Mhigan Gown                            <-  Gown of Light
        new(30757, 25000, 2), // Southern Seas Skirt                        <-  Skirt of Light
        new(36821, 27933, 2), // Archfiend Helm                             <-  Scion Hearer's Hood
        new(36822, 27934, 2), // Archfiend Armor                            <-  Scion Hearer's Coat
        new(36825, 27935, 2), // Archfiend Sabatons                         <-  Scion Hearer's Shoes
        new(32393, 39302, 2), // Edenmete Gown of Casting                   <-  Gaia's Attire
    ];
    // @formatter:on
}
