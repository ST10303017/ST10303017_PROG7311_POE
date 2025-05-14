// File: Models/ErrorViewModel.cs
namespace ST10303017_PROG7311_POE.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}