namespace Noodle.Process
{
    public class InvocationResult
    {
        public string Error { get; set; }
        public string Output { get; set; }

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(ExecutionError); }
        }
        public string ExecutionError { get; set; }
    }
}
