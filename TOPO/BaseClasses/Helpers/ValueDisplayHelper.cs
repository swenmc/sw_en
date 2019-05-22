using DATABASE;
using DATABASE.DTO;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses.Helpers
{
    public class ValueDisplayHelper
    {
        Dictionary<string, DataExportTables> allItemsDict;
        Dictionary<string, QuantityLibraryItem> quantityLibrary;
        NumberFormatInfo nfi;
        public ValueDisplayHelper(Dictionary<string, DataExportTables> _allItemsDict, Dictionary<string, QuantityLibraryItem> _quantityLibrary, NumberFormatInfo _nfi)
        {
            allItemsDict = _allItemsDict;
            quantityLibrary = _quantityLibrary;
            nfi = _nfi;
        }

        public string GetStringReport(float value, string identificator)
        {
            DataExportTables item = null;
            allItemsDict.TryGetValue(identificator, out item);
            if (item == null) return value.ToString(nfi); //ak sa to tuna dostane,tak v podstate je nejaka chyba

            QuantityLibraryItem qlItem = null;
            quantityLibrary.TryGetValue(item.UnitIdentificator, out qlItem);
            if (qlItem == null) return value.ToString(nfi); //ak sa to tuna dostane,tak v podstate je nejaka chyba

            if (qlItem.ID == 1) return string.Empty; //Blank
            else return (value * qlItem.ReportUnitFactor).ToString($"F{qlItem.ReportDecimalPlaces}", nfi);
        }
        public string GetStringGUI(float value, string identificator)
        {
            DataExportTables item = null;
            allItemsDict.TryGetValue(identificator, out item);
            if (item == null) return value.ToString(nfi); //ak sa to tuna dostane,tak v podstate je nejaka chyba

            QuantityLibraryItem qlItem = null;
            quantityLibrary.TryGetValue(item.UnitIdentificator, out qlItem);
            if (qlItem == null) return value.ToString(nfi); //ak sa to tuna dostane,tak v podstate je nejaka chyba

            if (qlItem.ID == 1) return string.Empty; //Blank
            else return (value * qlItem.GUIUnitFactor).ToString($"F{qlItem.GUIDecimalPlaces}", nfi);
        }
    }
}
