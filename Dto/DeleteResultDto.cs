namespace SmartMenuManagerApp.Dto
{
    public class DeleteResultDto
    {
        public string Status { get; set; } // "success" or "error"
        public string Message { get; set; } // A message related to the operation
        public object Data { get; set; } // Optional data related to the operation (e.g., deleted category ID)
        public string Error { get; set; } // Optional error message if something goes wrong
    }
}
