namespace Concordion.Api.Extension;

[AttributeUsage(AttributeTargets.Class)]
public class ExtensionsAttribute(params Type[] extensionTypes) : Attribute {
    public Type[] ExtensionTypes { get; } = extensionTypes;
}
