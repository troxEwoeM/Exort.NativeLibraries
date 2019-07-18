namespace Exort.NativeLibraries.Base.Results
{
    public class LoadMethodResult<T>
    {
        public bool Success { get; }
        public string Error { get; }
        public T Delegate { get; }

        public LoadMethodResult(bool success, T @delegate, string error = null)
        {
            this.Success = success;
            this.Error = error;
            this.Delegate = @delegate;
        }
    }
}