namespace ExternalStore.Domain
{
    public abstract record ContextBase<TResult> : ContextBase
    {
        public TResult? Result { get; set; }
    }
    public abstract record ContextBase
    {
        private string? _error;
        public bool IsError { get; private set; }
        public string? ErrorDescription { get; set; }
        public string? UserError { get; private set; }
        public string? Error
        {
            get => _error;
            set
            {
                _error = value;
                IsError = true;
            }
        }
        public void SetErrors(string error, string userError, string? description = null)
        {
            Error += error + "\n";
            UserError += userError + "\n";
            ErrorDescription += description + "\n";
        }
    }
}
