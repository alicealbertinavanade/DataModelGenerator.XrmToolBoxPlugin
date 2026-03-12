namespace DataModelDevOpsExtractor.Model
{
    public enum UploadResultStatus
    {
        Created,
        Existing,
        Error
    }

    public sealed class UploadStatusEntry
    {
        public string Kind { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public UploadResultStatus Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}
