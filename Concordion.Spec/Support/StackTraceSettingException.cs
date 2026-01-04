using System.Text;

namespace Concordion.Spec.Support;

internal class StackTraceSettingException : Exception {
    public List<string> StackTraceElements { get; set; }

    public override string StackTrace {
        get {
            var builder = new StringBuilder();
            foreach (var element in StackTraceElements) {
                builder.AppendLine(element);
            }

            return builder.ToString();
        }
    }

    public StackTraceSettingException()
    {
        StackTraceElements = [];
    }

    public StackTraceSettingException(string message) : base(message)
    {
        StackTraceElements = [];
    }

    public StackTraceSettingException(string message, Exception inner)
        : base(message, inner)
    {
        StackTraceElements = [];
    }
}
