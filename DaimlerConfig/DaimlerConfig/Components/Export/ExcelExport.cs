using System.ComponentModel;
using DaimlerConfig.Components.Models;
using System.IO;
using ClosedXML.Excel;

namespace DaimlerConfig.Components.Export;

public class ExcelExport
{
    public void Export(string path, Line line,
                      Station[] stations,
                      Tool[] tools,
                      Operation[] operations,
                      StationType[] stationTypes,
                      ToolType[] toolTypes,
                      ToolClass[] toolClasses,
                      DecisionClass[] decisionClasses,
                      GenerationClass[] generationClasses,
                      VerificationClass[] verificationClasses,
                      SavingClass[] savingClasses)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(line.lineName == null ? "unknown" : line.lineName);
        var currentRow = 3;//Starts after head
        
        #region Head
        sheet.Cells("A1").Value = "Line Name: " + line.lineName;
        #endregion
        
        #region Columns
        sheet.Cells("A2").Value = "Station Name";
        sheet.Cells("B2").Value = "Station Description";
        sheet.Cells("C2").Value = "Station Type";
        sheet.Cells("D2").Value = "Tools ID";
        sheet.Cells("E2").Value = "Tools Shortname";
        sheet.Cells("F2").Value = "Tools Description";
        sheet.Cells("G2").Value = "Tools Class";
        sheet.Cells("H2").Value = "Tools Type";
        sheet.Cells("I2").Value = "Operations ID";
        sheet.Cells("J2").Value = "Operations Sequenz-Group";
        sheet.Cells("K2").Value = "Operations Sequenz";
        sheet.Cells("L2").Value = "Operations Shortname";
        sheet.Cells("M2").Value = "Operations Description";
        sheet.Cells("N2").Value = "Operations Decision";
        sheet.Cells("O2").Value = "Decision Class";
        sheet.Cells("P2").Value = "Generation Class";
        sheet.Cells("Q2").Value = "Verification Class";
        sheet.Cells("R2").Value = "Saving Class";
        sheet.Cells("S2").Value = "Q-Gate relevant";
        sheet.Cells("T2").Value = "Always perform";
        sheet.Cells("U2").Value = "Parallel?";
        sheet.Cells("V2").Value = "IP-Address Device";
        sheet.Cells("W2").Value = "PLC-Name";
        sheet.Cells("X2").Value = "DBNo Send";
        sheet.Cells("Y2").Value = "DBNo Receive";
        sheet.Cells("Z2").Value = "PreCheck Byte";
        sheet.Cells("AA2").Value = "Address in send-DB";
        sheet.Cells("AB2").Value = "Address in receive-DB";
        #endregion
        
        #region Stations-Tools-Operations
        foreach (var station in stations) 
        {
            sheet.Cells("A"+currentRow).Value = station.assemblystation;
            sheet.Cells("B"+currentRow).Value = station.stationName;
            sheet.Cells("C"+currentRow).Value = stationTypes.FirstOrDefault(st => st.stationTypeID == station.stationTypeID)?.stationTypeName ?? "";
            
            foreach (var operation in operations)
            {
                currentRow++;
                var tool = operation.toolID.HasValue ? tools.FirstOrDefault(t => t.toolID == operation.toolID.Value) : null;
                
                sheet.Cells("D"+currentRow).Value = operation.toolID?.ToString() ?? "";
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
                
                sheet.Cells("I"+currentRow).Value = operation.operationID;
                sheet.Cells("J"+currentRow).Value = operation.operationSequenceGroup;
                sheet.Cells("K"+currentRow).Value = operation.operationSequence;
                sheet.Cells("L"+currentRow).Value = operation.operationShortname;
                sheet.Cells("M"+currentRow).Value = operation.operationDescription;
                sheet.Cells("N"+currentRow).Value = operation.operationDecisionCriteria;
                sheet.Cells("O"+currentRow).Value = decisionClasses.FirstOrDefault(dc => dc.decisionClassID == operation.decisionClassID)?.decisionClassName ?? "";
                sheet.Cells("P"+currentRow).Value = generationClasses.FirstOrDefault(gc => gc.generationClassID == operation.generationClassID)?.generationClassName ?? "";
                sheet.Cells("Q"+currentRow).Value = verificationClasses.FirstOrDefault(vc => vc.verificationClassID == operation.verificationClassID)?.verificationClassName ?? "";
                sheet.Cells("R"+currentRow).Value = savingClasses.FirstOrDefault(sc => sc.savingClassID == operation.savingClassID)?.savingClassName ?? "";
                sheet.Cells("S"+currentRow).Value = operation.qGateID;
                sheet.Cells("T"+currentRow).Value = operation.alwaysPerform;
                sheet.Cells("U"+currentRow).Value = operation.parallel;
                sheet.Cells("V"+currentRow).Value = tool?.ipAddressDevice ?? "";
                sheet.Cells("W"+currentRow).Value = tool?.plcName ?? "";
                sheet.Cells("X"+currentRow).Value = tool?.dbNoSend ?? "";
                sheet.Cells("Y"+currentRow).Value = tool?.dbNoReceive ?? "";
                sheet.Cells("Z"+currentRow).Value = tool?.preCheckByte.ToString() ?? "";
                sheet.Cells("AA"+currentRow).Value = tool?.addressSendDB ?? "";
                sheet.Cells("AB"+currentRow).Value = tool?.addressReceiveDB ?? "";
            }
        }
        #endregion
        workbook.SaveAs(path);
    }
}