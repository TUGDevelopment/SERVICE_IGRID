using static BLL.Enums.EnumAttribute;

namespace BLL.Enums
{

    public enum PackagingTypeMapping
    {
        [Mapping(PackagingTypeKey = "C", PackagingType = "Cardboard", BomItemMulti = "NEW MULTI CARDBOARD", BomItemNonMulti = "NEW CARDBOARD")]
        Cardboard,

        [Mapping(PackagingTypeKey = "D", PackagingType = "Displayer", BomItemMulti = "NEW MULTI DISPLAYER", BomItemNonMulti = "NEW DISPLAYER")]
        Displayer,

        [Mapping(PackagingTypeKey = "F", PackagingType = "Carton", BomItemMulti = "NEW MULTI CARTON", BomItemNonMulti = "NEW CARTON")]
        Carton,

        [Mapping(PackagingTypeKey = "G", PackagingType = "Tray", BomItemMulti = "NEW MULTI TRAY", BomItemNonMulti = "NEW TRAY")]
        Tray,

        [Mapping(PackagingTypeKey = "H", PackagingType = "Sleeve Box", BomItemMulti = "NEW MULTI SLEEVE BOX", BomItemNonMulti = "NEW SLEEVE BOX")]
        SleeveBox,

        [Mapping(PackagingTypeKey = "J", PackagingType = "Sticker", BomItemMulti = "NEW MULTI STICKER", BomItemNonMulti = "NEW STICKER")]
        Sticker,

        [Mapping(PackagingTypeKey = "K", PackagingType = "Label", BomItemMulti = "NEW MULTI LABEL", BomItemNonMulti = "NEW LABEL")]
        Label,

        [Mapping(PackagingTypeKey = "L", PackagingType = "Leaflet", BomItemMulti = "NEW MULTI LEAFLET", BomItemNonMulti = "NEW LEAFLET")]
        Leaflet,

        [Mapping(PackagingTypeKey = "M", PackagingType = "Plastic", BomItemMulti = "NEW MULTI PLASTIC", BomItemNonMulti = "NEW PLASTIC")]
        Plastic,

        [Mapping(PackagingTypeKey = "N", PackagingType = "Inner Corrugated", BomItemMulti = "NEW MULTI INNER CORRUGAT", BomItemNonMulti = "NEW INNER CORRUGAT")]
        InnerCorrugated,

        [Mapping(PackagingTypeKey = "P", PackagingType = "Insert Paper", BomItemMulti = "NEW MULTI INSERT PAPER", BomItemNonMulti = "NEW INSERT PAPER")]
        InsertPaper,

        [Mapping(PackagingTypeKey = "R", PackagingType = "Inner non Corrugated", BomItemMulti = "NEW MULTI INNER NON CORR", BomItemNonMulti = "NEW INNER NON CORR")]
        InnernonCorrugated,

    }
}
