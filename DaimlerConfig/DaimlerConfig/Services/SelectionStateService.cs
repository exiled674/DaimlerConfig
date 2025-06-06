using Microsoft.Maui.Storage;

namespace DaimlerConfig.Services
{
    public class SelectionStateService
    {
        private const string SELECTED_STATION_KEY = "SelectedStation";
        private const string SELECTED_TOOL_KEY = "SelectedTool";
        private const string SELECTED_OPERATION_KEY = "SelectedOperation";
        private const string SELECTION_TIMESTAMP_KEY = "SelectionTimestamp";

        public class SelectionState
        {
            public int? StationID { get; set; }
            public int? ToolID { get; set; }
            public int? OperationID { get; set; }
            public DateTime Timestamp { get; set; }
        }

        public void SaveSelection(int? stationID = null, int? toolID = null, int? operationID = null)
        {
            var currentState = GetSavedSelection();

            // Logik für die hierarchische Auswahl
            if (stationID.HasValue)
            {
                currentState.StationID = stationID.Value;
                // Wenn neue Station ausgewählt wird, Tool und Operation nur zurücksetzen wenn keine neuen Werte übergeben werden
                if (!toolID.HasValue)
                    currentState.ToolID = null;
                if (!operationID.HasValue)
                    currentState.OperationID = null;
            }

            if (toolID.HasValue && toolID.Value > 0) // Nur echte Tool-IDs speichern
            {
                currentState.ToolID = toolID.Value;
                // Wenn neues Tool ausgewählt wird, Operation nur zurücksetzen wenn kein neuer Wert übergeben wird
                if (!operationID.HasValue)
                    currentState.OperationID = null;
            }

            if (operationID.HasValue && operationID.Value > 0) // Nur echte Operation-IDs speichern
            {
                currentState.OperationID = operationID.Value;
            }

            currentState.Timestamp = DateTime.Now;

            // Speichern
            if (currentState.StationID.HasValue)
                Preferences.Default.Set(SELECTED_STATION_KEY, currentState.StationID.Value);
            else
                Preferences.Default.Remove(SELECTED_STATION_KEY);

            if (currentState.ToolID.HasValue)
                Preferences.Default.Set(SELECTED_TOOL_KEY, currentState.ToolID.Value);
            else
                Preferences.Default.Remove(SELECTED_TOOL_KEY);

            if (currentState.OperationID.HasValue)
                Preferences.Default.Set(SELECTED_OPERATION_KEY, currentState.OperationID.Value);
            else
                Preferences.Default.Remove(SELECTED_OPERATION_KEY);

            Preferences.Default.Set(SELECTION_TIMESTAMP_KEY, currentState.Timestamp.ToBinary());
        }

        public SelectionState GetSavedSelection()
        {
            var timestampBinary = Preferences.Default.Get(SELECTION_TIMESTAMP_KEY, 0L);
            var timestamp = timestampBinary != 0 ? DateTime.FromBinary(timestampBinary) : DateTime.MinValue;

            var stationID = Preferences.Default.Get(SELECTED_STATION_KEY, -1);
            var toolID = Preferences.Default.Get(SELECTED_TOOL_KEY, -1);
            var operationID = Preferences.Default.Get(SELECTED_OPERATION_KEY, -1);

            return new SelectionState
            {
                StationID = stationID != -1 ? stationID : null,
                ToolID = toolID != -1 ? toolID : null,
                OperationID = operationID != -1 ? operationID : null,
                Timestamp = timestamp
            };
        }

        public void ClearSelection()
        {
            Preferences.Default.Remove(SELECTED_STATION_KEY);
            Preferences.Default.Remove(SELECTED_TOOL_KEY);
            Preferences.Default.Remove(SELECTED_OPERATION_KEY);
            Preferences.Default.Remove(SELECTION_TIMESTAMP_KEY);
        }

        public bool HasValidSelection()
        {
            var state = GetSavedSelection();
            return state.StationID.HasValue;
        }

        // Hilfsmethoden für spezifische Checks
        public bool HasStationSelection() => GetSavedSelection().StationID.HasValue;
        public bool HasToolSelection() => GetSavedSelection().ToolID.HasValue;
        public bool HasOperationSelection() => GetSavedSelection().OperationID.HasValue;
    }
}