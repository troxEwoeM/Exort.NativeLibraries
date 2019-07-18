namespace Exort.NativeLibraries.Base.Results
{
    public class LoadLibraryResult
    {
        public bool Success { get; }
        public string Error { get; }

        internal LoadLibraryResult(bool success, string error = null)
        {
            this.Success = success;
            this.Error = error;
        }
    }
}