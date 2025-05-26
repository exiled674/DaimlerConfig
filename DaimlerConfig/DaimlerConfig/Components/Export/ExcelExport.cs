using DaimlerConfig.Components.Models;
using System.IO;
using OfficeOpenXml;
    
namespace DaimlerConfig.Components.Export;

public class ExcelExport
{
    public void Export(string path,Line line,
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
        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add(line.lineName);
        var currentRow = 3;//Starts after head
        
        #region Head
        sheet.Cells[1, 1].Value = "Line Name: " + line.lineName;
        #endregion
        
        #region Columns
        sheet.Cells[2,1].Value = "Station Name";
        sheet.Cells[2,2].Value = "Station Description";
        sheet.Cells[2,3].Value = "Station Type";
        sheet.Cells[2,4].Value = "Tools ID";
        sheet.Cells[2,5].Value = "Tools Shortname";
        sheet.Cells[2,6].Value = "Tools Description";
        sheet.Cells[2,7].Value = "Tools Class";
        sheet.Cells[2,8].Value = "Tools Type";
        sheet.Cells[2,9].Value = "Operations ID";
        sheet.Cells[2,10].Value = "Operations Sequenz-Group";
        sheet.Cells[2,11].Value = "Operations Sequenz";
        sheet.Cells[2,12].Value = "Operations Shortname";
        sheet.Cells[2,13].Value = "Operations Description";
        sheet.Cells[2,14].Value = "Operations Decision";
        sheet.Cells[2,15].Value = "Decision Class";
        sheet.Cells[2,16].Value = "Generation Class";
        sheet.Cells[2,17].Value = "Verification Class";
        sheet.Cells[2,18].Value = "Saving Class";
        sheet.Cells[2,19].Value = "Q-Gate relevant";
        sheet.Cells[2,20].Value = "Always perform";
        sheet.Cells[2,21].Value = "Parallel?";
        sheet.Cells[2,22].Value = "IP-Address Device";
        sheet.Cells[2,23].Value = "PLC-Name";
        sheet.Cells[2,24].Value = "DBNo Send";
        sheet.Cells[2,25].Value = "DBNo Receive";
        sheet.Cells[2,26].Value = "PreCheck Byte";
        sheet.Cells[2,27].Value = "Address in send-DB";
        sheet.Cells[2,28].Value = "Address in receive-DB";
        #endregion
        
        #region Stations-Tools-Operations
        foreach (var station in stations) 
        {
            sheet.Cells[currentRow, 1].Value = station.assemblystation;
            sheet.Cells[currentRow, 2].Value = station.stationName;
            sheet.Cells[currentRow, 3].Value = stationTypes[station.stationTypeID].stationTypeName;
            foreach (var operation in operations)
            {
                currentRow++;
                sheet.Cells[currentRow, 4].Value = operation.toolID == null ? "" : operation.toolID.Value;
                sheet.Cells[currentRow, 5].Value = operation.toolID == null ? "" : tools[operation.toolID.Value].toolShortname;
                sheet.Cells[currentRow, 6].Value = operation.toolID == null ? "" : tools[operation.toolID.Value].toolDescription;
                // IDE warning is incorrect here – null checks are properly handled underneath
                sheet.Cells[currentRow, 7].Value = operation.toolID == null ? "" : tools[operation.toolID.Value].toolClassID == null ? "" : toolClasses[tools[operation.toolID.Value].toolClassID.Value].toolClassName;
                sheet.Cells[currentRow, 8].Value = operation.toolID == null ? "" : tools[operation.toolID.Value].toolTypeID == null ? "" : toolTypes[tools[operation.toolID.Value].toolTypeID.Value].toolTypeName;
                
                sheet.Cells[currentRow, 9].Value = operation.operationID;
                sheet.Cells[currentRow, 10].Value = operation.operationSequenceGroup;
                sheet.Cells[currentRow, 11].Value = operation.operationSequence;
                sheet.Cells[currentRow, 12].Value = operation.operationShortname;
                sheet.Cells[currentRow, 13].Value = operation.operationDescription;
                sheet.Cells[currentRow, 14].Value = operation.operationDecisionCriteria;
                sheet.Cells[currentRow, 15].Value = decisionClasses[operation.decisionClassID].decisionClassName;
                sheet.Cells[currentRow, 16].Value = generationClasses[operation.generationClassID].generationClassName;
                sheet.Cells[currentRow, 17].Value = verificationClasses[operation.verificationClassID].verificationClassName;
                sheet.Cells[currentRow, 18].Value = savingClasses[operation.savingClassID].savingClassName;
                sheet.Cells[currentRow, 19].Value = operation.qGateID;
                sheet.Cells[currentRow, 20].Value = operation.alwaysPerform;
                sheet.Cells[currentRow, 21].Value = operation.parallel;
                sheet.Cells[currentRow, 22].Value = operation.toolID == null ? "" : tools[operation.toolID.Value].ipAddressDevice;
                sheet.Cells[currentRow, 23].Value = operation.toolID == null ? "" : tools[operation.toolID.Value].plcName;
                sheet.Cells[currentRow, 24].Value = operation.toolID == null ? "" : tools[operation.toolID.Value].dbNoSend;
                sheet.Cells[currentRow, 25].Value = operation.toolID == null ? "" : tools[operation.toolID.Value].dbNoReceive;
                sheet.Cells[currentRow, 26].Value = operation.toolID == null ? "" : tools[operation.toolID.Value].preCheckByte;
                sheet.Cells[currentRow, 26].Value = operation.toolID == null ? "" : tools[operation.toolID.Value].addressSendDB;
                sheet.Cells[currentRow, 27].Value = operation.toolID == null ? "" : tools[operation.toolID.Value].addressReceiveDB;
            }
        }
        #endregion
    }
}