using System.ComponentModel;
using DConfig.Components.Models;
using System.IO;
using ClosedXML.Excel;


namespace DConfig.Components.Export;


public class ExcelExport
{

    public void Export(Stream stream, Line line,
                      Station[] stations,
                      Tool[] tools,
                      StationType[] stationTypes,
                      ToolType[] toolTypes,
                      ToolClass[] toolClasses)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(line.lineName == null ? "unknown" : line.lineName);
        var currentRow = 3;//Starts after head
        
        #region Head
        sheet.Cells("A1").Value = "Line name: " + line.lineName;
        sheet.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#A6A6A6");
        sheet.Row(1).Style.Font.Bold = true;
        #endregion
        
        #region Columns Head
        sheet.Row(2).Style.Fill.BackgroundColor = XLColor.FromHtml("#00566A");
        sheet.Row(2).Style.Font.Bold = true;
        sheet.Row(2).Style.Font.FontColor = XLColor.White;
        sheet.Cells("A2").Value = "Station Name";
        sheet.Cells("B2").Value = "Station Description";
        sheet.Cells("C2").Value = "Station Type";
        sheet.Cells("D2").Value = "Tools ID";
        sheet.Cells("E2").Value = "Tools Shortname";
        sheet.Cells("F2").Value = "Tools Description";
        sheet.Cells("G2").Value = "Tools Class";
        sheet.Cells("H2").Value = "Tools Type";
        sheet.Cells("I2").Value = "IP-Address Device";
        sheet.Cells("J2").Value = "PLC-Name";
        sheet.Cells("K2").Value = "DBNo Send";
        sheet.Cells("L2").Value = "DBNo Receive";
        sheet.Cells("M2").Value = "PreCheck Byte";
        sheet.Cells("N2").Value = "Address in send-DB";
        sheet.Cells("O2").Value = "Address in receive-DB";
        #endregion
        
        #region Stations-Tools
        foreach (var station in stations) 
        {
            sheet.Cells("A"+currentRow).Value = station.assemblystation;
            sheet.Cells("B"+currentRow).Value = station.stationName;
            sheet.Cells("C"+currentRow).Value = stationTypes.FirstOrDefault(st => st.stationTypeID == station.stationTypeID)?.stationTypeName ?? "";
            sheet.Row(currentRow).Style.Fill.BackgroundColor = XLColor.FromHtml("#99CCFF");
            
            foreach (var tool in tools)
            {
                if (tool.stationID != station.stationID) continue;
                currentRow++;
                sheet.Row(currentRow).Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9");
                sheet.Cells("D"+currentRow).Value = tool.toolID?.ToString() ?? "";
                sheet.Cells("E"+currentRow).Value = tool?.toolShortname ?? "";
                sheet.Cells("F"+currentRow).Value = tool?.toolDescription ?? "";
                
                var toolClass = tool?.toolClassID.HasValue == true 
                    ? toolClasses.FirstOrDefault(tc => tc.toolClassID == tool.toolClassID.Value) 
                    : null;
                sheet.Cells("G"+currentRow).Value = toolClass?.toolClassName ?? "";
                
                var toolType = tool?.toolTypeID.HasValue == true 
                    ? toolTypes.FirstOrDefault(tt => tt.toolTypeID == tool.toolTypeID.Value) 
                    : null;
                sheet.Cells("H"+currentRow).Value = toolType?.toolTypeName ?? "";
                sheet.Cells("I"+currentRow).Value = tool?.ipAddressDevice ?? "";
                sheet.Cells("J"+currentRow).Value = tool?.plcName ?? "";
                sheet.Cells("K"+currentRow).Value = tool?.dbNoSend ?? "";
                sheet.Cells("L"+currentRow).Value = tool?.dbNoReceive ?? "";
                sheet.Cells("M"+currentRow).Value = tool?.preCheckByte.ToString() ?? "";
                sheet.Cells("N"+currentRow).Value = tool?.addressSendDB ?? "";
                sheet.Cells("O"+currentRow).Value = tool?.addressReceiveDB ?? "";
            }
            currentRow++;
        }
        #endregion
        
        sheet.Columns().AdjustToContents();
        var usedRange = sheet.RangeUsed();
        int lastColumn = usedRange.LastColumnUsed().ColumnNumber();
        int lastRow = usedRange.LastRowUsed().RowNumber();

        for (int row = 1; row <= lastRow; row++)
        {
            for (int column = 2; column <= lastColumn; column++)
            {
                sheet.Cell(row, column).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            }
        }
        
        workbook.SaveAs(stream);
        stream.Position = 0;
    }
}